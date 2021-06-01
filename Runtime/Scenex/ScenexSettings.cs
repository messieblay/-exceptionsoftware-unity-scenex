using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class ScenexSettings : ScriptableObject
    {
        [SerializeField] public bool useEmptySceneToLoad = true;
        [SerializeField] public float delayBetweenLoading = .1f;
        [SerializeField] public UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects;

        [SerializeField] public List<SceneInfo> scenes = new List<SceneInfo>();
        [SerializeField] public List<Group> groups = new List<Group>();

    }
}
