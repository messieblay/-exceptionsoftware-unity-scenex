using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class SceneInfo : Item
    {

        [SerializeField] public int canvasSortOrder = 0;
        [SerializeField] public bool autoActive = false;
        [SerializeField] public bool isMainScene = false;
        [SerializeField, HideInInspector] public bool isLoadingScreen = false;


#if UNITY_EDITOR
        [SerializeField] public UnityEditor.SceneAsset sceneAsset = null;
#endif

        [SerializeField] public string sceneName;
        [SerializeField] public int buildIndex;
        [SerializeField] public string path = string.Empty;


        public override string ToString()
        {
            return $"{sceneName}";
        }

        #region non serialized
        [System.NonSerialized] public Scene sceneObject;
        [System.NonSerialized] public SceneStatus status;

        [System.NonSerialized] public AsyncOperation asyncOperation;

        public void ConfigureAsUnloadScene() => Status = SceneStatus.Unload;

        public SceneStatus Status
        {
            get => status;
            set => status = value;
        }
        #endregion


    }
}
