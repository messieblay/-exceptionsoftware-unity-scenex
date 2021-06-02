using System.Collections.Generic;
using UnityEditor;

namespace ExceptionSoftware.ExScenes
{
    public partial class Table
    {

        protected override void ContextClickedItem(int id)
        {
            //GetSelection

            bool CheckIfSelectionIsSameType()
            {
                IList<int> ids = GetSelection();
                for (int i = 1; i < ids.Count; i++)
                {
                    if (treeModel.Find(ids[i - 1]).ItemType != treeModel.Find(ids[i]).ItemType) return false;
                }
                return true;
            }


            if (!CheckIfSelectionIsSameType())
            {
                return;
            }


            GenericMenu menu;
            TableElement itemClicked = treeModel.Find(id);
            if (itemClicked.IsFolderGroups)
            {
                menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TrTextContent("Create group"), false, ScenexUtilityEditor.CreateGroup);
                menu.ShowAsContext();
            }

            if (itemClicked.IsSubGroup)
            {
                menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TrTextContent("Open scenes"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<SubGroup>(GetSelection()).ToArray();
                    ScenexUtilityEditor.OpenSubGroup(groupsSelected);
                });
                menu.AddSeparator("");
                menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, delegate ()
                {
                    var subgroupsSelected = FindScenexItems<SubGroup>(GetSelection()).ToArray();
                    ScenexUtilityEditor.RemoveSubGroup(subgroupsSelected);
                });

                menu.ShowAsContext();
            }


            if (itemClicked.IsGroup)
            {
                menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TrTextContent("Open scenes"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<Group>(GetSelection()).ToArray();
                    ScenexUtilityEditor.OpenGroup(groupsSelected);
                });
                menu.AddSeparator("");
                menu.AddItem(EditorGUIUtility.TrTextContent("Create SubGroup"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<Group>(GetSelection()).ToArray();
                    ScenexUtilityEditor.CreateGroupSub(groupsSelected);
                });

                menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<Group>(GetSelection()).ToArray();
                    ScenexUtilityEditor.RemoveGroup(groupsSelected);
                });

                menu.ShowAsContext();
            }

            if (itemClicked.IsScene)
            {
                menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TrTextContent("Open scene"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<SceneInfo>(GetSelection()).ToArray();
                    ScenexUtilityEditor.OpenScene(groupsSelected);
                });
                menu.AddSeparator("");

                menu.AddItem(EditorGUIUtility.TrTextContent("Set as Loading screen"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<SceneInfo>(GetSelection()).ToArray();
                    ScenexUtilityEditor.SetSceneAsLoading(groupsSelected);
                    ScenexUtilityEditor.onDataChanged.TryInvoke();
                });

                menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, delegate ()
                {
                    TableElement itemToRemove = null;
                    var selection = GetSelection();

                    foreach (var idSelected in selection)
                    {
                        itemToRemove = treeModel.Find(idSelected);
                        if (itemToRemove != null && itemToRemove.IsScene)
                        {

                            TableElement parent = treeModel.Find(itemToRemove.parent.id);
                            ScenexUtilityEditor.RemoveSceneFromParent(itemToRemove.item as SceneInfo, parent.item, false);
                            ScenexUtilityEditor.Log($"Removed {idSelected}: {itemToRemove.name}");
                        }
                    }

                    ScenexUtilityEditor.onDataChanged.TryInvoke();
                });
                menu.ShowAsContext();
            }
            if (itemClicked.IsLoadingScene)
            {
                menu = new GenericMenu();
                menu.AddItem(EditorGUIUtility.TrTextContent("Open scene"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<SceneInfo>(GetSelection()).ToArray();
                    ScenexUtilityEditor.OpenScene(groupsSelected);
                });
                menu.AddSeparator("");

                menu.AddItem(EditorGUIUtility.TrTextContent("Set as Scene"), false, delegate ()
                {
                    var groupsSelected = FindScenexItems<SceneInfo>(GetSelection()).ToArray();
                    ScenexUtilityEditor.SetSceneAsNormal(groupsSelected);
                    ScenexUtilityEditor.onDataChanged.TryInvoke();
                });

                menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, delegate ()
                {
                    TableElement itemToRemove = null;
                    var selection = GetSelection();

                    foreach (var idSelected in selection)
                    {
                        itemToRemove = treeModel.Find(idSelected);
                        if (itemToRemove != null && itemToRemove.IsLoadingScene)
                        {

                            TableElement parent = treeModel.Find(itemToRemove.parent.id);
                            ScenexUtilityEditor.RemoveLoadingScene(parent.item as SubGroup, false);
                        }
                    }

                    ScenexUtilityEditor.onDataChanged.TryInvoke();
                });
                menu.ShowAsContext();
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            TableElement element;
            element = treeModel.Find(id);

            if (element.item != null && (element.IsScene || element.IsLoadingScene))
            {
                Selection.activeObject = (element.item as SceneInfo).sceneAsset;
            }
        }

        List<T> FindScenexItems<T>(IList<int> ids) where T : Item
        {
            List<T> list = new List<T>();
            TableElement element;
            foreach (var id in ids)
            {
                element = treeModel.Find(id);
                if (element == null) continue;
                if (element.item is T)
                {
                    list.Add(element.item as T);
                }
            }
            return list;
        }
    }
}
