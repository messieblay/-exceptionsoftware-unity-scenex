using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class ScenexSettings : ScriptableObject
    {
        [SerializeField] public bool useEmptySceneToLoad = true;

        [Header("Loading")]
        [SerializeField] public float delayBetweenLoading = .1f;

        [SerializeField] public float delayBetweenSceneActivation = .1f;

        [Header("Wait for input")]
        [SerializeField] public float delayBeforeWaitInput = .1f;
        [SerializeField] public float delayAfterWaitInput = .1f;

        [Header("Unloading")]
        [SerializeField] public UnloadSceneOptions unloadSceneOptions = UnloadSceneOptions.UnloadAllEmbeddedSceneObjects;
        [SerializeField] public float delayBetweenUnLoading = .1f;

        [Header("Fade")]
        [SerializeField] public bool useDefaultFade = true;
        [SerializeField] public float fadeTime = .5f;
        [SerializeField] public Color fadeColor = Color.black;
        [SerializeField] public AnimationCurve faceCurve = AnimationCurve.Linear(0, 0, 1, 1);


        [Header("Loggin")]
        [SerializeField] public bool useUnityConsoleLog = true;

        [Header("Objects")]
        [SerializeField] public List<SceneInfo> scenes = new List<SceneInfo>();
        [SerializeField] public List<SceneInfo> loadingScreens = new List<SceneInfo>();
        [SerializeField] public List<Group> groups = new List<Group>();
    }
}
