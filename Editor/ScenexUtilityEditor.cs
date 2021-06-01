using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [InitializeOnLoad]
    public static class ScenexUtilityEditor
    {
        static ScenexSettings _settings = null;
        public static ScenexSettings Settings => LoadOrCreate();

        public const string SCENES_PATH = ExConstants.GAME_PATH + "Scenes/";
        public const string SCENES_PATH_RESOURCES = SCENES_PATH + "Resources/";
        public const string SCENES_PATH_SCENES = SCENES_PATH_RESOURCES + "Scenes/";
        public const string SCENES_PATH_LAYOUT = SCENES_PATH_RESOURCES + "Layouts/";
        public const string SCENES_PATH_GROUPS = SCENES_PATH_RESOURCES + "Groups/";
        public const string SCENES_MENU_ITEM = "Tools/Scenes/";

        public const string VARIABLES_SETTINGS_FILENAME = "ExScenesSettings";


        public static System.Action onDataChanged = null;

        static ScenexUtilityEditor()
        {
            LoadOrCreate();
            AttachToPlayMode();
        }

        internal static ScenexSettings LoadOrCreate()
        {
            if (!System.IO.Directory.Exists(SCENES_PATH))
                System.IO.Directory.CreateDirectory(SCENES_PATH);

            if (!System.IO.Directory.Exists(SCENES_PATH_RESOURCES))
                System.IO.Directory.CreateDirectory(SCENES_PATH_RESOURCES);

            if (!System.IO.Directory.Exists(SCENES_PATH_SCENES))
                System.IO.Directory.CreateDirectory(SCENES_PATH_SCENES);

            if (!System.IO.Directory.Exists(SCENES_PATH_LAYOUT))
                System.IO.Directory.CreateDirectory(SCENES_PATH_LAYOUT);

            if (!System.IO.Directory.Exists(SCENES_PATH_GROUPS))
                System.IO.Directory.CreateDirectory(SCENES_PATH_GROUPS);

            if (_settings == null)
            {
                _settings = ExAssets.FindAssetsByType<ScenexSettings>().First();
            }

            if (_settings == null)
            {
                _settings = ExAssets.CreateAsset<ScenexSettings>(SCENES_PATH_RESOURCES, VARIABLES_SETTINGS_FILENAME);
            }


            if (_settings != null)
            {
                var scenes = ExAssets.FindAssetsByType<SceneInfo>().OrderBy(s => s.priority);
                _settings.scenes = scenes.Where(s => !s.isLoadingScreen).ToList();
                _settings.loadingScreens = scenes.Where(s => s.isLoadingScreen).ToList();
                _settings.groups = ExAssets.FindAssetsByType<Group>().OrderBy(s => s.priority).ToList();
                ValidateSceneInfoNames();

                EditorUtility.SetDirty(_settings);
            }
            return _settings;
        }

        [MenuItem(SCENES_MENU_ITEM + "Select Asset", priority = 3000)]
        static void SelectAsset()
        {
            LoadOrCreate();
            Selection.activeObject = _settings;
        }
        public static void ValidateSceneInfoNames() => _settings.scenes.ForEach(s => ValidateName(s));
        static void ValidateName(SceneInfo scene)
        {
            if (scene.name != scene.sceneAsset.name)
            {
                ExAssets.RenameAsset(scene, scene.sceneAsset.name);
            }
        }
        public static void AddSceneTo(SceneInfo scene, Layout item)
        {
            if (item.scenes.Contains(scene)) return;
            item.scenes.Add(scene);
            EditorUtility.SetDirty(item);
            onDataChanged.TryInvoke();
        }
        public static void AddLoadingToSubgroup(SceneInfo scene, SubGroup item)
        {
            if (item.loadingScreen == scene) return;
            item.loadingScreen = scene;
            EditorUtility.SetDirty(item);
            onDataChanged.TryInvoke();
        }

        public static void RemoveSceneFrom(SceneInfo scene, Layout item)
        {
            if (!item.scenes.Contains(scene)) return;
            item.scenes.Remove(scene);
            EditorUtility.SetDirty(item);
            onDataChanged.TryInvoke();
        }

        public static void CreateScene(SceneAsset scene)
        {
            if (_settings.scenes.Exists(s => s.sceneAsset == scene))
            {
                Logx.Log("Scenex", $"Rejected import: Already exist in bbdd");
                return;
            }

            SceneInfo info = ExAssets.CreateAsset<SceneInfo>(SCENES_PATH_SCENES, scene.name);
            info.sceneAsset = scene;

            _settings.scenes.Add(info);
            EditorUtility.SetDirty(info);
            EditorUtility.SetDirty(_settings);
            onDataChanged.TryInvoke();
        }

        public static void SetSceneAsLoading(params SceneInfo[] scenesinfo)
        {
            if (scenesinfo == null) return;
            foreach (var scene in scenesinfo)
            {
                if (scene == null) continue;

                if (IsSceneAssigned(scene))
                {
                    //Debug.Log()
                    continue;
                }

                scene.isLoadingScreen = true;

                _settings.scenes.Remove(scene);
                _settings.loadingScreens.Add(scene);

                EditorUtility.SetDirty(scene);
            }
        }
        public static void SetSceneAsNormal(params SceneInfo[] scenesinfo)
        {
            if (scenesinfo == null) return;
            foreach (var scene in scenesinfo)
            {
                if (scene == null) continue;

                foreach (var sg in _settings.groups.SelectMany(s => s.childs).Where(s => s.loadingScreen == scene))
                {
                    sg.loadingScreen = null;
                    EditorUtility.SetDirty(sg);
                }

                scene.isLoadingScreen = false;

                _settings.loadingScreens.Remove(scene);
                _settings.scenes.Add(scene);

                EditorUtility.SetDirty(scene);
            }
        }

        static bool IsSceneAssigned(SceneInfo scene)
        {
            foreach (var g in _settings.groups)
            {
                if (g.scenes.Contains(scene))
                {
                    return true;
                }

                if (g.childs.SelectMany(s => s.scenes).Contains(scene)) return true;

            }
            return false;
        }
        public static void AddSubGroup(Group parent, SubGroup sub, bool fireDataChange = true)
        {
            if (parent == null || sub == null) return;
            if (sub.parent != null)
            {
                EditorUtility.SetDirty(sub.parent);
                sub.parent.childs.Remove(sub);
            }
            sub.parent = parent;
            parent.childs.Add(sub);

            EditorUtility.SetDirty(sub);
            EditorUtility.SetDirty(sub.parent);
            EditorUtility.SetDirty(_settings);
            if (fireDataChange) onDataChanged.TryInvoke();
        }

        public static void CreateGroupSub(params Group[] groups)
        {
            if (groups == null) return;

            foreach (var group in groups)
            {
                if (group == null) return;
                var subgroup = ExAssets.CreateAsset<SubGroup>(SCENES_PATH_GROUPS, "New subgroup");
                AddSubGroup(group, subgroup);
            }

            EditorUtility.SetDirty(_settings);
            onDataChanged.TryInvoke();
        }
        public static void CreateGroup()
        {
            _settings.groups.Add(ExAssets.CreateAsset<Group>(SCENES_PATH_GROUPS, "New group"));
            onDataChanged.TryInvoke();
        }

        public static void RemoveSubGroup(params SubGroup[] subs)
        {
            if (subs == null) return;

            foreach (var sub in subs)
            {
                if (subs == null) continue;

                sub.parent.childs.Remove(sub);
                EditorUtility.SetDirty(sub.parent);

                sub.parent = null;
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(sub));
            }

            EditorUtility.SetDirty(_settings);
            onDataChanged.TryInvoke();
        }

        public static void RemoveGroup(params Group[] groups)
        {
            if (groups == null) return;
            foreach (var group in groups)
            {
                if (group == null) continue;

                _settings.groups.Remove(group);

                foreach (var sub in group.childs)
                {
                    RemoveSubGroup(sub);
                }
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(group));
                EditorUtility.SetDirty(_settings);
            }

            onDataChanged.TryInvoke();
        }
        public static void RemoveLoadingScene(SubGroup subgroup, bool fireDataChange = true)
        {
            subgroup.loadingScreen = null;
            EditorUtility.SetDirty(subgroup);
            if (fireDataChange) onDataChanged.TryInvoke();
        }
        public static void RemoveSceneFromParent(SceneInfo scene, Item parent, bool fireDataChange = true)
        {
            if (parent == null)
            {
                string sceneName = scene.name;
                //Se quiere borrar escena de toda la bases de datos
                foreach (var g in _settings.groups)
                {
                    if (g.scenes.Contains(scene))
                    {
                        g.scenes.Remove(scene);
                        EditorUtility.SetDirty(g);
                    }

                    foreach (var sg in g.childs)
                    {

                        if (sg.scenes.Contains(scene))
                        {
                            sg.scenes.Remove(scene);
                            EditorUtility.SetDirty(sg);
                        }
                    }
                }

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(scene));
                Debug.Log($"Scene {sceneName} removed");
            }
            else
            {
                Layout layout = parent as Layout;
                layout.scenes.Remove(scene);
                EditorUtility.SetDirty(layout);
                if (fireDataChange) onDataChanged.TryInvoke();
            }
        }
        public static void SetPriority<T>(ref List<T> list) where T : Item
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].priority = i;
            }
        }
        public static void SetScenesPriorityByCurrentOrder()
        {
            for (int i = 0; i < _settings.scenes.Count; i++)
            {
                _settings.scenes[i].priority = i;

                EditorUtility.SetDirty(_settings.scenes[i]);
            }

            for (int i = 0; i < _settings.groups.Count; i++)
            {
                _settings.groups[i].priority = i;

                for (int u = 0; u < _settings.groups[i].childs.Count; u++)
                {
                    _settings.groups[i].childs[u].priority = u;

                    EditorUtility.SetDirty(_settings.groups[i].childs[u]);
                }

                EditorUtility.SetDirty(_settings.groups[i]);
            }


            EditorUtility.SetDirty(_settings);
        }

        public static void CreateScenexEnumToLoadScenes()
        {
            CodeFactory.CodeFactory.CreateScripts(new CodeFactory.EnumTemplate(SCENES_PATH)
            {
                className = "ScenexEnum",
                enums = CodeFactory.CodeFactory.GenerateEnumContent(_settings.groups.SelectMany(s => s.childs).Select(s => s.parent.name + "_" + s.name).ToList())
            });
            Debug.Log("ScenexEnum created");
        }
        /// <summary>
        /// Activa el Play mode y guarda cual es la escena actual. Cuando se quite el play recupera esa escena
        /// </summary>
        public static void PlayEditor()
        {
            EditorPrefs.SetString("SceneListUtilityEditor_lastOpenedScene", EditorSceneManager.GetActiveScene().path);
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(ScenexUtility.Settings.scenes[0].path, OpenSceneMode.Single);
                EditorPrefs.SetBool("SceneListUtilityEditor_returnToScene", true);
                AttachToPlayMode();
                EditorApplication.isPlaying = true;
            }
        }


        public static void StopEditor()
        {
            EditorApplication.isPlaying = false;
        }

        static void AttachToPlayMode()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }


        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode) return;
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode && EditorPrefs.GetBool("SceneListUtilityEditor_returnToScene"))
            {
                EditorPrefs.SetBool("SceneListUtilityEditor_returnToScene", false);
                EditorApplication.OpenScene(EditorPrefs.GetString("SceneListUtilityEditor_lastOpenedScene"));
            }
        }


        #region Load All

        public static ScenexSettings LoadScenes()
        {
            SceneType type;
            string parentName;
            string chunkname;
            int priority;
            ScenexSettings list = ExAssets.FindAssetsByType<ScenexSettings>().First();

            List<SceneAsset> scenes = ExAssets.FindAssetsByType<SceneAsset>();
            List<SceneInfo> scenesInfo = new List<SceneInfo>();
            List<SceneInfo> newscenesInfo = new List<SceneInfo>();

            GetAllSceneAsset(list.scenes.ToList(), ref scenesInfo);

            foreach (SceneAsset scene in scenes)
            {
                string path = AssetDatabase.GetAssetPath(scene);
                //if (path.StartsWith(PathUtilityEditor.pathScenes))
                {
                    GetInfoOfScene(scene, out type, out parentName, out chunkname, out priority);
                    SceneInfo info = GetSceneAssetInfo(scene);
                    //info.type = type;
                    info.priority = priority;
                    info.sceneAsset = scene;
                    info.sceneName = scene.name;
                    //info.ParentName = parentName;
                    //info.ChunkName = chunkname;
                    info.path = path;
                    //info.parent = null;
                    //info.childs.Clear();
                    //System.Enum.TryParse<SceneListEnum>(scene.name, true, out info.Scene);
                    EditorUtility.SetDirty(info);
                    newscenesInfo.Add(info);


                    if (info.name != scene.name)
                    {
                        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(info), scene.name);
                    }
                }
            }

            foreach (var toDelete in ExAssets.FindAssetsByType<SceneInfo>().Except(newscenesInfo))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(toDelete));
            }


            //newscenesInfo = newscenesInfo.OrderBy(s => s.type).ThenBy(s => s.priority).ToList();
            for (int i = newscenesInfo.Count - 1; -1 < i; i--)
            {
                //if (newscenesInfo[i].type == SceneType.STATIC || newscenesInfo[i].type == SceneType.SUBSCENE)
                //{
                //    string parentNane = GetParentName(newscenesInfo[i].SceneName);
                //    var parentScene = newscenesInfo.Find(s => s.ParentName == newscenesInfo[i].ParentName && s.type == SceneType.LOGIC);
                //    if (parentScene != null)
                //    {
                //        newscenesInfo[i].parent = parentScene;
                //        parentScene.childs.Add(newscenesInfo[i]);
                //        newscenesInfo.RemoveAt(i);
                //    }
                //}
            }

            list.scenes = newscenesInfo;
            ExAssets.SaveAsset(list);
            AssetDatabase.Refresh();

            return list;

            SceneInfo GetSceneAssetInfo(SceneAsset asset)
            {
                SceneInfo assetinfo = scenesInfo.Find(s => s.sceneAsset == asset);
                if (assetinfo != null)
                {
                    return assetinfo;
                }

                assetinfo = ScriptableObject.CreateInstance<SceneInfo>();
                //string path = AssetDatabase.GenerateUniqueAssetPath(SCENES_ASSET_PATH + asset.name + ".asset");
                //UnityEditor.AssetDatabase.CreateAsset(assetinfo, path);

                return assetinfo;
            }


            void GetAllSceneAsset(List<SceneInfo> sceneAssetInfos, ref List<SceneInfo> finalLines)
            {
                foreach (var info in sceneAssetInfos)
                {
                    finalLines.Add(info);
                    //GetAllSceneAsset(info.childs, ref finalLines);
                }
            }
        }

        static void GetInfoOfScene(SceneAsset scene, out SceneType type, out string parentName, out string chunkname, out int priority)
        {

            chunkname = string.Empty;
            type = SceneType.General;
            priority = 0;

            string name = scene.name;
            string[] splits = name.Split('_');
            parentName = splits.First();

            if (!int.TryParse(splits.Last(), out priority))
            {
                priority = int.MaxValue;
            }

            if (name.ToLower().StartsWith("first"))
            {
                type = SceneType.First;
                return;
            }

            if (name.ToLower().StartsWith("empty"))
            {
                type = SceneType.Empty;
                return;
            }

            if (name.ToLower().StartsWith("loading"))
            {
                type = SceneType.Loading;
                return;
            }

            if (name.ToLower().StartsWith("shell"))
            {
                //type = SceneType.Shell;
                return;
            }

            if (name.ToLower().StartsWith("audio"))
            {
                //type = SceneType.Audio;
                return;
            }


            if (name.Contains("_BK"))
            {
                //type = SceneType.Background;
                parentName = name.Substring(0, name.IndexOf("_BK"));
                return;
            }

            if (name.Contains("_UI"))
            {
                type = SceneType.UI;
                return;
            }

            if (name.Contains("_WC_"))
            {
                type = SceneType.SUBSCENE;
                chunkname = name.Split('_').Last();
                return;
            }


            if (name.EndsWith("_WL"))
            {
                type = SceneType.MASTER;
                return;
            }


            if (name.EndsWith("_WS"))
            {
                type = SceneType.SUBSCENE;
                return;
            }

        }

        public static string GetParentName(string sceneName)
        {
            return sceneName.Trim().Split('_').First();
        }
        #endregion


        public static void PublishToBuildSettings()
        {
            _settings.scenes?.ClearNulls();
            _settings.loadingScreens?.ClearNulls();

            List<SceneInfo> scenesToBuild = _settings.scenes.ToList();
            if (_settings.loadingScreens != null && _settings.loadingScreens.Count > 0)
            {
                scenesToBuild.AddRange(_settings.loadingScreens);
            }

            // Find valid Scene paths and make a list of EditorBuildSettingsScene
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();


            for (int x = 0; x < scenesToBuild.Count; x++)
            {
                var sceneInfo = scenesToBuild[x];


                sceneInfo.buildIndex = x;
                sceneInfo.name = sceneInfo.sceneName = sceneInfo.sceneAsset.name;

                EditorUtility.SetDirty(sceneInfo);
                sceneInfo.path = AssetDatabase.GetAssetPath(sceneInfo.sceneAsset);
                if (!string.IsNullOrEmpty(sceneInfo.path))
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(sceneInfo.path, true));
            }

            // Set the Build Settings window Scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();



            Debug.Log($"{_settings.scenes.Count} Scenes published in BuildSettings");
        }
    }
}
