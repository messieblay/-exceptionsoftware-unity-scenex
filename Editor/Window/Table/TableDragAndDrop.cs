using ExceptionSoftware.TreeViewTemplate;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace ExceptionSoftware.ExScenes
{
    public partial class Table
    {
        // Dragging

        enum DragMode
        {
            Scene,
            Loading,
            Group,
            SubGroup,
            ReorderScene,
            ReorderLoading,
            ReorderGroup,
            ReorderSubGroup,
            SceneToGroup,
            SceneToSubGroup,
            SubGroupToGroup,
            GroupToSubGroup,
            GroupToGroup,
            SubGroupToSubGroup,
            LoadingToSubgroup,
            None
        }

        DragMode _dragModeBegin = DragMode.None;
        DragMode _dragModeEnding = DragMode.None;
        TableElement parentDraggedEleemnts;
        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            TableElement parent = null;
            _dragModeBegin = DragMode.None;
            parentDraggedEleemnts = null;
            foreach (var id in args.draggedItemIDs)
            {
                var element = treeModel.Find(id);
                if (element == null) continue;

                if (element.IsScene || element.IsLoadingScene || element.IsLayout)
                {
                    if (parent == null)
                    {
                        parent = element.parent as TableElement;
                    }
                    else
                    {
                        if (parent != element.parent as TableElement) return false;
                    }
                }
            }
            if (parent.IsFolderScenes)
            {
                _dragModeBegin = DragMode.Scene;
            }
            if (parent.IsFolderLoading)
            {
                _dragModeBegin = DragMode.Loading;
            }
            else if (parent.IsGroup)
            {
                _dragModeBegin = DragMode.Group;
            }
            else if (parent.IsSubGroup)
            {
                _dragModeBegin = DragMode.SubGroup;
            }

            parentDraggedEleemnts = parent;
            return true;
        }



        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
            if (draggedRows == null)
                return DragAndDropVisualMode.None;


            if (args.performDrop)
            {
                OnPerformDrag(args);
            }

            if (args.parentItem == null) return DragAndDropVisualMode.Rejected;
            TableElement parentElement = treeModel.Find(args.parentItem.id);

            if (parentElement == parentDraggedEleemnts)
            {
                switch (_dragModeBegin)
                {
                    case DragMode.Scene:
                        _dragModeEnding = DragMode.ReorderScene;
                        return DragAndDropVisualMode.Move;
                    case DragMode.Loading:
                        _dragModeEnding = DragMode.ReorderLoading;
                        return DragAndDropVisualMode.Move;
                    case DragMode.Group:
                        _dragModeEnding = DragMode.ReorderGroup;
                        return DragAndDropVisualMode.Move;
                    case DragMode.SubGroup:
                        _dragModeEnding = DragMode.ReorderSubGroup;
                        return DragAndDropVisualMode.Move;
                }
            }
            else
            {
                switch (_dragModeBegin)
                {
                    case DragMode.Scene:
                        if (parentElement.IsGroup)
                        {
                            _dragModeEnding = DragMode.SceneToGroup;
                            return DragAndDropVisualMode.Copy;
                        }
                        if (parentElement.IsSubGroup)
                        {
                            _dragModeEnding = DragMode.SceneToSubGroup;
                            return DragAndDropVisualMode.Copy;
                        }
                        break;
                    case DragMode.Loading:
                        if (parentElement.IsSubGroup)
                        {
                            _dragModeEnding = DragMode.LoadingToSubgroup;
                            return DragAndDropVisualMode.Copy;
                        }
                        break;
                    case DragMode.Group:
                        if (parentElement.IsGroup)
                        {
                            _dragModeEnding = DragMode.GroupToGroup;
                            return DragAndDropVisualMode.Copy;
                        }
                        if (parentElement.IsSubGroup)
                        {
                            _dragModeEnding = DragMode.GroupToSubGroup;
                            return DragAndDropVisualMode.Copy;
                        }
                        break;
                    case DragMode.SubGroup:
                        if (parentElement.IsGroup)
                        {
                            _dragModeEnding = DragMode.SubGroupToGroup;
                            return DragAndDropVisualMode.Copy;
                        }
                        if (parentElement.IsSubGroup)
                        {
                            _dragModeEnding = DragMode.SubGroupToSubGroup;
                            return DragAndDropVisualMode.Copy;
                        }
                        break;
                }
            }

            return DragAndDropVisualMode.Rejected;
        }

        void OnPerformDrag(DragAndDropArgs args)
        {
            var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
            TableElement parentElement = treeModel.Find(args.parentItem.id);
            //T parentData = ((TreeViewItem<T>)args.parentItem).data;
            switch (_dragModeEnding)
            {
                case DragMode.ReorderScene:
                case DragMode.ReorderGroup:
                case DragMode.ReorderLoading:
                case DragMode.ReorderSubGroup:
                    {
                        var parent = treeModel.Find(args.parentItem.id);
                        var selecteds = treeModel.Find(draggedRows.Select(s => s.id).ToList()).Cast<TreeElement>().ToList();
                        treeModel.MoveElements(parent, args.insertAtIndex, selecteds);

                        SaveReorder();
                    }
                    break;

                case DragMode.SceneToSubGroup:
                case DragMode.SceneToGroup:
                case DragMode.GroupToSubGroup:
                case DragMode.GroupToGroup:
                case DragMode.SubGroupToGroup:
                case DragMode.SubGroupToSubGroup:
                    foreach (TreeViewItem item in draggedRows)
                    {
                        ScenexUtilityEditor.AddSceneTo(treeModel.Find(item.id).item as SceneInfo, parentElement.item as Layout);
                    }

                    Reload();
                    break;

                case DragMode.LoadingToSubgroup:
                    foreach (TreeViewItem item in draggedRows)
                    {
                        ScenexUtilityEditor.AddLoadingToSubgroup(treeModel.Find(item.id).item as SceneInfo, parentElement.item as SubGroup);
                    }

                    break;
            }
        }


        void SaveReorder()
        {
            //Scenes Rerorder
            var sceneFolder = treeModel.root.children[0];
            var neworder = sceneFolder.children.Cast<TableElement>().Select(s => s.item as SceneInfo).ToList();
            ScenexUtilityEditor.SetPriority(ref neworder);
            ScenexUtilityEditor.Settings.scenes.ClearAddRange(neworder);

            //loading Rerorder
            var lopadingFolder = treeModel.root.children[1];
            var loadings = lopadingFolder.children.Cast<TableElement>().Select(s => s.item as SceneInfo).ToList();
            ScenexUtilityEditor.SetPriority(ref loadings);
            ScenexUtilityEditor.Settings.loadingScreens.ClearAddRange(loadings);

            //Groups Rerorder
            var sceneGroups = treeModel.root.children[2];
            var groups = sceneGroups.children.Cast<TableElement>().Select(s => s.item as Group).ToList();
            ScenexUtilityEditor.SetPriority(ref groups);
            ScenexUtilityEditor.Settings.groups = groups;

            foreach (var group in sceneGroups.children.Cast<TableElement>())
            {
                if (!group.hasChildren) continue;

                //Scenas de grupo
                var groupscenes = group.children.Cast<TableElement>().Where(s => s.ItemType == TableElement.Type.Scene).ToList();
                group.AsLayout.scenes = groupscenes.Select(s => s.item as SceneInfo).ToList();

                //Reordenar subgrupos
                var groupsubgroups = group.children.Cast<TableElement>().Where(s => s.ItemType == TableElement.Type.SubGroup).ToList();
                var subgroups = groupsubgroups.Select(s => s.item as SubGroup).ToList();
                ScenexUtilityEditor.SetPriority(ref subgroups);
                (group.item as Group).childs = subgroups;

                EditorUtility.SetDirty(group.AsLayout);

                foreach (var subgroup in groupsubgroups)
                {
                    if (!subgroup.hasChildren) continue;

                    //Scenas de grupo
                    var subgroupscenes = subgroup.children.Cast<TableElement>().Where(s => s.ItemType == TableElement.Type.Scene).ToList();
                    subgroup.AsLayout.scenes = subgroupscenes.Select(s => s.item as SceneInfo).ToList();
                    EditorUtility.SetDirty(subgroup.AsLayout);
                }
            }
            ScenexUtilityEditor.SetScenesPriorityByCurrentOrder();
            EditorUtility.SetDirty(ScenexUtilityEditor.Settings);
        }
    }
}
