using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExceptionSoftware.ExScenes
{
    [RequireComponent(typeof(DontDestroy))]
    public class ScenexController : MonoBehaviour
    {
        public ScenexControllerEvents events = new ScenexControllerEvents();

        [System.NonSerialized] public Group currentGroup = null;
        [System.NonSerialized] public SubGroup currentSubGroup = null;
        [System.NonSerialized] public List<SceneInfo> listScenesToLoad;

        ScenexSettings _scenexSettings;
        public void LoadScene(string groupToLoad, ScenexControllerEvents events)
        {
            StartCoroutine(LoadScenes(groupToLoad, events));
        }

        public IEnumerator LoadScenes(string groupToLoad, ScenexControllerEvents events)
        {
            if (events != null)
            {
                this.events = events;
            }
            ScenexUtility.Log("--------------------------");

            _scenexSettings = ScenexUtility.Settings;
            if (_scenexSettings == null)
            {
                ScenexUtility.LogError("Scene loading canceled: Scenex settings not found");
                yield break;
            }

            string[] split = groupToLoad.Split('_');
            Group group = null;
            SubGroup subgroup = null;

            group = ScenexUtility.Settings.groups.Find(s => s.ID == split[0].ToLower());
            if (group == null)
            {
                ScenexUtility.LogError($"Scene loading canceled: Group {split[0]} not found");
                yield break;
            }


            subgroup = group.childs.Find(s => s.ID == split[1].ToLower());
            if (subgroup == null)
            {
                ScenexUtility.LogError($"Scene loading canceled: SubGroup {split[1]} not found");
                yield break;
            }

            //Set group and subgroup
            currentGroup = group;
            currentSubGroup = subgroup;

            //Generate scenes to load list;
            listScenesToLoad = new List<SceneInfo>();
            listScenesToLoad.AddRange(currentGroup.scenes);
            listScenesToLoad.AddRange(currentSubGroup.scenes);
            listScenesToLoad = listScenesToLoad.OrderBy(s => s.priority).ToList();

            if (listScenesToLoad.Count == 0)
            {
                ScenexUtility.LogError($"Scene loading canceled: No scenes to load");
                yield break;
            }

            yield return LoadScenes(group, subgroup);
        }
        IEnumerator LoadScenes(Group group, SubGroup subgroup)
        {
            TryLoadDefaultFade();

            Scene empty;

            ScenexUtility.Log("Loading STARTED");
            yield return events.onLoadingBegin.Call();

            events.onLoadingProgressBegin.Call();
            yield return FadeInFromGame();

            BeginProgressBar();

            empty = SceneManager.CreateScene("Empty", new CreateSceneParameters());
            SceneManager.SetActiveScene(empty);
            yield return new WaitForSeconds(1);

            yield return UnloadAllScenes();

            if (currentSubGroup.loadingScreen)
            {
                yield return SceneManager.LoadSceneAsync(currentSubGroup.loadingScreen.buildIndex, LoadSceneMode.Single);
                currentSubGroup.loadingScreen.asyncOperation = SceneManager.LoadSceneAsync(currentSubGroup.loadingScreen.buildIndex, LoadSceneMode.Single);
                currentSubGroup.loadingScreen.asyncOperation.allowSceneActivation = false;

                while (currentSubGroup.loadingScreen.asyncOperation.progress < 0.9f)
                {
                    yield return new WaitForEndOfFrame();
                }

                currentSubGroup.loadingScreen.sceneObject = SceneManager.GetSceneByBuildIndex(currentSubGroup.loadingScreen.buildIndex);
                currentSubGroup.loadingScreen.asyncOperation.allowSceneActivation = true;
                SceneManager.SetActiveScene(currentSubGroup.loadingScreen.sceneObject);

                events.onLoadingScreenBegin.Call();
                yield return FadeOutToLoading();
            }

            yield return ScenexUtility.CollectRoutine();

            yield return LoadAllScenes();



            //Set MAIN scene
            SceneInfo mainScene = listScenesToLoad.Find(s => s.isMainScene);
            if (mainScene != null)
            {
                SceneManager.SetActiveScene(mainScene.sceneObject);
                ScenexUtility.Log($"Main scene {mainScene.name} activated");
                events.onMainSceneActived.Call();
            }

            EndProgressBar();

            //Wait For Input if wants to
            if (group.waitForInput)
            {
                ScenexUtility.Log("Waiting for input");
                events.onWaitForInputBegin.Call();
                yield return new WaitForSeconds(_scenexSettings.delayAfterWaitInput);
                yield return null;
                yield return events.onWaitForInput.Call();
                yield return null;
                yield return new WaitForSeconds(_scenexSettings.delayAfterWaitInput);
                events.onWaitForInputEnd.Call();
            }



            if (currentSubGroup.loadingScreen)
            {
                events.onLoadingScreenEnd.Call();
                yield return new WaitForSeconds(3);
                yield return FadeInFromLoading();
                yield return SceneManager.UnloadSceneAsync(currentSubGroup.loadingScreen.buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }

            yield return FadeOutToGame();

            events.onLoadingProgressEnd.Call();
            yield return null;

            ScenexUtility.Log("Loading FINISHED");
            yield return events.onLoadingEnd.Call();
            ScenexUtility.Log("--------------------------");


            TryDesactiveFadeInOut();

            yield return null;


            IEnumerator UnloadAllScenes()
            {
                //Unload all Scenes
                for (int i = SceneManager.sceneCount - 1; -1 < i; i--)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene == empty)
                    {
                        ScenexUtility.Log($"{scene.name} skipped");
                        continue;
                    }
                    yield return SceneManager.UnloadSceneAsync(scene.buildIndex, _scenexSettings.unloadSceneOptions);

                    IncProgressBar();

                    yield return new WaitForSeconds(_scenexSettings.delayBetweenUnLoading);

                }

                ScenexUtility.Log("All scenes unloaded");
                events.onAllScenesLoaded.Call();
            }

            IEnumerator LoadAllScenes()
            {
                for (int i = 0; i < listScenesToLoad.Count; i++)
                {

                    listScenesToLoad[i].asyncOperation = SceneManager.LoadSceneAsync(listScenesToLoad[i].buildIndex, LoadSceneMode.Additive);
                    listScenesToLoad[i].asyncOperation.allowSceneActivation = false;

                    while (listScenesToLoad[i].asyncOperation.progress < 0.9f)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    listScenesToLoad[i].sceneObject = SceneManager.GetSceneByBuildIndex(listScenesToLoad[i].buildIndex);

                    IncProgressBar();

                    yield return new WaitForSeconds(_scenexSettings.delayBetweenLoading);

                    yield return null;
                }

                ScenexUtility.Log("All scenes loaded");


                //Activado de escenas
                for (int i = 0; i < listScenesToLoad.Count; i++)
                {

                    listScenesToLoad[i].asyncOperation.allowSceneActivation = true;
                    yield return null;

                    yield return new WaitForSeconds(_scenexSettings.delayBetweenSceneActivation);

                    if (events.onSceneActivated != null)
                    {
                        yield return events.onSceneActivated(listScenesToLoad[i]);
                    }

                }

                ScenexUtility.Log("All scenes activated");

                events.onAllScenesLoaded.Call();
            }
        }


        #region DefaultFade
        FadeInOut _defaultFade = null;

        void TryLoadDefaultFade()
        {
            if (_scenexSettings.useDefaultFade && _defaultFade == null)
            {
                var prefab = Resources.Load<FadeInOut>("Scenex/Canvas Fade");
                _defaultFade = GameObject.Instantiate<FadeInOut>(prefab);
            }
            if (_defaultFade)
            {
                _defaultFade.LoadDefaultData(_scenexSettings.fadeColor, _scenexSettings.fadeTime, _scenexSettings.faceCurve);
                _defaultFade.gameObject.SetActive(true);
            }
        }
        void TryDesactiveFadeInOut()
        {
            if (_scenexSettings.useDefaultFade && _defaultFade != null)
            {
                _defaultFade.gameObject.SetActive(false);
            }
        }

        IEnumerator FadeInFromGame()
        {
            if (_scenexSettings.useDefaultFade && _defaultFade)
            {
                yield return _defaultFade.FadeIn();
            }
            else
            {
                yield return events.onFadeInFromGame();
            }
        }
        IEnumerator FadeOutToGame()
        {
            if (_scenexSettings.useDefaultFade && _defaultFade)
            {
                yield return _defaultFade.FadeOut();
            }
            else
            {
                yield return events.onFadeOutToGame();
            }
        }
        IEnumerator FadeInFromLoading()
        {
            if (_scenexSettings.useDefaultFade && _defaultFade)
            {
                yield return _defaultFade.FadeIn();
            }
            else
            {
                yield return events.onFadeInToLoading();
            }
        }
        IEnumerator FadeOutToLoading()
        {
            if (_scenexSettings.useDefaultFade && _defaultFade)
            {
                yield return _defaultFade.FadeOut();
            }
            else
            {
                yield return events.onFadeOutFromLoading();
            }
        }

        #endregion

        #region Progress
        float _progressMaxActions = 0;
        float _progressCurrentActions = 0;
        void BeginProgressBar()
        {
            _progressMaxActions = SceneManager.sceneCount + listScenesToLoad.Count;
            if (events.onLoadingProgressChanged != null) events.onLoadingProgressChanged(0);
        }
        void IncProgressBar()
        {
            if (events.onLoadingProgressChanged != null) events.onLoadingProgressChanged(_progressCurrentActions / _progressMaxActions);
        }
        void EndProgressBar()
        {
            if (events.onLoadingProgressChanged != null) events.onLoadingProgressChanged(1);
        }
        #endregion

    }
}
