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
            Group,
            SubGroup,
            ReorderScene,
            ReorderGroup,
            ReorderSubGroup,
            SceneToGroup,
            SceneToSubGroup,
            SubGroupToGroup,
            GroupToSubGroup,
            GroupToGroup,
            SubGroupToSubGroup,
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

                if (!element.IsScene) return false;

                if (parent == null)
                {
                    parent = element.parent as TableElement;
                }
                else
                {
                    if (parent != element.parent as TableElement) return false;
                }
            }
            if (parent.IsFolder)
            {
                _dragModeBegin = DragMode.Scene;
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
            //Debug.Log(args.dragAndDropPosition);

            if (parentElement == parentDraggedEleemnts)
            {
                switch (_dragModeBegin)
                {
                    case DragMode.Scene:
                        _dragModeEnding = DragMode.ReorderScene;
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
                    {
                        var parent = treeModel.Find(args.parentItem.id);
                        var selecteds = treeModel.Find(draggedRows.Select(s => s.id).ToList()).Cast<TreeElement>().ToList();
                        treeModel.MoveElements(parent, args.insertAtIndex, selecteds);
                    }
                    break;
                case DragMode.ReorderGroup:
                    {
                        var parent = treeModel.Find(args.parentItem.id);
                        var selecteds = treeModel.Find(draggedRows.Select(s => s.id).ToList()).Cast<TreeElement>().ToList();
                        treeModel.MoveElements(parent, args.insertAtIndex, selecteds);
                    }
                    break;
                case DragMode.ReorderSubGroup:
                    {
                        var parent = treeModel.Find(args.parentItem.id);
                        var selecteds = treeModel.Find(draggedRows.Select(s => s.id).ToList()).Cast<TreeElement>().ToList();
                        treeModel.MoveElements(parent, args.insertAtIndex, selecteds);
                        for (int i = 0; i < parent.children.Count; i++)
                        {
                            var item = (parent.children[i] as TableElement).item;
                            EditorUtility.SetDirty(item);
                        }

                        parent.children.Cast<TableElement>().Select(s => s.item).ToList();
                        if (parent.item != null) EditorUtility.SetDirty(parent.item);

                        //EditorUtility.SetDirty(_d)
                        //ScenexUtilityEditor.SetScenesPriorityByCurrentOrder();
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
            }
        }
    }
}
