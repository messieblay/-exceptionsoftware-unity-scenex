using System.Collections.Generic;
using System.Linq;

namespace ExceptionSoftware.ExScenes
{

    static class TableGenerator
    {
        static int IDCounter;
        static bool _dataloaded = false;
        static ScenexSettings _db;
        static List<TableElement> roots;

        static TableElement _folderScenes, _folderLoading, _folderGroup;

        public static List<TableElement> GenerateTree()
        {
            IDCounter = 0;
            TableElement root = new TableElement("Root", -1, IDCounter);

            _db = ScenexUtilityEditor.Settings;
            roots = new List<TableElement>();
            roots.Add(root);


            CreateScenes(root);
            CreateLoadingScenes(root);
            CreateGroups(root);

            return roots;
        }

        static void CreateScenes(TableElement root)
        {
            _folderScenes = CreateFolder(root, "Scenes", 7000);
            foreach (var scene in _db.scenes)
            {
                CreateScene(_folderScenes, scene);
            }
        }
        static void CreateLoadingScenes(TableElement root)
        {
            _folderLoading = CreateFolder(root, "Loading screens", 8000);
            foreach (var scene in _db.loadingScreens)
            {
                CreateLoadingScreen(_folderLoading, scene);
            }
        }
        static void CreateGroups(TableElement root)
        {
            _folderGroup = CreateFolder(root, "Groups", 9000);
            foreach (var scene in _db.groups)
            {
                CreateGroup(_folderGroup, scene);
            }
        }

        static TableElement CreateFolder(TableElement parent, string name, int id)
        {
            var item = new TableElement(name, parent.depth + 1, id/*++IDCounter*/);
            item.item = null;
            roots.Add(item);
            return item;
        }
        public static void CreateScene(TableElement parent, SceneInfo scene)
        {
            if (parent.children != null && parent.children.Cast<TableElement>().Any(s => s.item as SceneInfo == scene)) return;
            var item = new TableElement(scene.name, parent.depth + 1, ++IDCounter);
            item.item = scene;

            roots.Add(item);
        }
        public static void CreateLoadingScreen(TableElement parent, SceneInfo scene)
        {
            var item = new TableElement(scene.name, parent.depth + 1, ++IDCounter);
            item.item = scene;

            roots.Add(item);
        }

        static void CreateGroup(TableElement parent, Group group)
        {
            var item = new TableElement(group.name, parent.depth + 1, group.GetInstanceID()/*++IDCounter*/);
            item.item = group;

            roots.Add(item);

            foreach (var scene in group.scenes)
            {
                CreateScene(item, scene);
            }

            foreach (var subgroup in group.childs)
            {
                CreateSubGroup(item, subgroup);
            }
        }

        static void CreateSubGroup(TableElement parent, SubGroup subgroup)
        {
            var item = new TableElement(subgroup.name, parent.depth + 1, subgroup.GetInstanceID()/*++IDCounter*/);
            item.item = subgroup;
            roots.Add(item);

            if (subgroup.loadingScreen != null)
            {
                CreateLoadingScreen(item, subgroup.loadingScreen);
            }

            foreach (var scene in subgroup.scenes)
            {
                CreateScene(item, scene);
            }

        }


    }
}
