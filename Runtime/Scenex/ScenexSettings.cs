using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class ScenexSettings : ScriptableObject
    {
        [SerializeField] public bool useEmptySceneToLoad = true;

        [Header("Times")]
        [SerializeField] public float delayBetweenUnLoading = .1f;
        [SerializeField] public float delayBetweenLoading = .1f;
        [SerializeField] public float delayAfterWaitInput = .1f;

        [Header("Unloading")]
        [SerializeField] public UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects;

        [Header("Fade")]
        [SerializeField] public bool useDefaultFade = true;
        [SerializeField] public float fadeTime = .3f;
        [SerializeField] public Color fadeColor = Color.black;
        [SerializeField] public AnimationCurve faceCurve = AnimationCurve.Linear(0, 0, 1, 1);


        [Header("Objects")]
        [SerializeField] public List<SceneInfo> scenes = new List<SceneInfo>();
        [SerializeField] public List<SceneInfo> loadingScreens = new List<SceneInfo>();
        [SerializeField] public List<Group> groups = new List<Group>();

    }
}
