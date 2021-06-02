using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace ExceptionSoftware.ExScenes
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;
        public static readonly GUIContent[] s_PlayIcons;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };

            s_PlayIcons = new GUIContent[]
            {
                EditorGUIUtility.TrIconContent("PlayButton", "Play"),
                EditorGUIUtility.IconContent( "d_PreMatQuad" ),
                EditorGUIUtility.IconContent("PlayButton On")
            };
        }
    }

    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        static SceneSwitchLeftButton()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            DoPlayButtons(ScenexUtilityEditor.PlayEditor, ScenexUtilityEditor.StopEditor);
        }

        static void DoPlayButtons(System.Action startAction, System.Action stopAction)
        {
            GUI.changed = false;
            {
                int num = EditorApplication.isPlaying ? 1 : 0;
                bool edit = GUILayout.Toggle(EditorApplication.isPlaying, ToolbarStyles.s_PlayIcons[num], EditorApplication.isPlaying ? (GUIStyle)"AppCommandLeftOn" : (GUIStyle)"AppCommandLeft", GUILayout.Height(60));

                if (GUI.changed)
                {
                    if (edit)
                    {
                        if (startAction != null) startAction();
                    }
                    else
                    {
                        if (stopAction != null) stopAction();
                    }
                }
            }
            GUI.changed = false;
        }
    }

    static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (sceneToOpen == null || EditorApplication.isPlaying || EditorApplication.isPaused || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // need to get scene via search because the path to the scene
                // file contains the package version so it'll change over time
                string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
                if (guids.Length == 0)
                {
                    Debug.LogWarning("Couldn't find scene file");
                }
                else
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath);
                    EditorApplication.isPlaying = true;
                }
            }
            sceneToOpen = null;
        }
    }
}
