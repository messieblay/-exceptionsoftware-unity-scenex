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

        public System.Func<IEnumerator> onPreLoading = null;
        public System.Func<IEnumerator> onPostLoading = null;

        public System.Func<IEnumerator> onWaitForInput = null;

        public System.Func<IEnumerator> onFadeIn = null;
        public System.Func<IEnumerator> onFadeOut = null;

        public System.Func<IEnumerator> onFadeInLoading = null;
        public System.Func<IEnumerator> onFadeOutLoading = null;

        public System.Action onLoadingProgressStarts = null;
        public System.Action onLoadingProgressEnds = null;
        public System.Action onAllScenesLoaded = null;


        #region Group
        [System.NonSerialized] public Group currentGroup = null;
        [System.NonSerialized] public SubGroup currentSubGroup = null;

        public void LoadScene(string groupToLoad)
        {
            StartCoroutine(LoadScenes(groupToLoad));
        }

        public IEnumerator LoadScenes(string groupToLoad)
        {
            string[] split = groupToLoad.Split('_');
            Debug.Log($"{split[0]}-{split[1]}");
            Group group = null;
            SubGroup subgroup = null;

            group = ScenexUtility.Settings.groups.Find(s => s.ID == split[0].ToLower());
            if (group == null)
            {
                Debug.Log($"Group {split[0]} not found");
                yield break;
            }


            subgroup = group.childs.Find(s => s.ID == split[1].ToLower());
            if (subgroup == null)
            {
                Debug.Log($"SubGroup {split[1]} not found");
                yield break;
            }

            yield return LoadScenes(group, subgroup);
        }
        IEnumerator LoadScenes(Group group, SubGroup subgroup)
        {
            ScenexSettings scenexSettings = ScenexUtility.Settings;

            Scene empty;
            currentGroup = group;
            currentSubGroup = subgroup;

            List<SceneInfo> listScenesToLoad = new List<SceneInfo>();
            listScenesToLoad.AddRange(currentGroup.scenes);
            listScenesToLoad.AddRange(currentSubGroup.scenes);

            listScenesToLoad = listScenesToLoad.OrderBy(s => s.priority).ToList();



            yield return onPreLoading.Call();

            onLoadingProgressStarts.Call();
            yield return onFadeIn.Call();

            empty = SceneManager.CreateScene("Empty", new CreateSceneParameters());
            SceneManager.SetActiveScene(empty);
            yield return new WaitForSeconds(1);

            yield return UnloadAllScenes();
            Debug.Log("All current scenes unloaded");

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
                yield return onFadeOutLoading.Call();
            }

            yield return ScenexUtility.CollectRoutine();

            yield return LoadAllScenes();

            //yield return SceneManager.UnloadSceneAsync(empty, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            onAllScenesLoaded.Call();

            if (group.waitForInput)
            {
                yield return null;
                yield return onWaitForInput.Call();
                yield return null;
                yield return new WaitForSeconds(scenexSettings.delayAfterWaitInput);
            }

            //Set MAIN scene
            SceneInfo mainScene = listScenesToLoad.Find(s => s.isMainScene);
            if (mainScene != null)
                SceneManager.SetActiveScene(mainScene.sceneObject);


            if (currentSubGroup.loadingScreen)
            {
                yield return new WaitForSeconds(3);
                yield return onFadeInLoading.Call();
                yield return SceneManager.UnloadSceneAsync(currentSubGroup.loadingScreen.buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }

            yield return onFadeOut.Call();

            onLoadingProgressEnds.Call();
            yield return null;

            yield return onPostLoading.Call();



            ScenexUtility.Log("Created current operation");

            yield return null;


            IEnumerator UnloadAllScenes()
            {
                //Unload all Scenes
                for (int i = SceneManager.sceneCount - 1; -1 < i; i--)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene == empty)
                    {
                        Debug.Log($"{scene.name} skipped");
                        continue;
                    }
                    yield return SceneManager.UnloadSceneAsync(scene.buildIndex, scenexSettings.unloadSceneOptions);

                    yield return new WaitForSeconds(scenexSettings.delayBetweenUnLoading);
                }
            }

            IEnumerator LoadAllScenes()
            {


                /*
                 * CARGADO ESCENAS DEPENDIENTES DEL ESQUEMA
                 */
                for (int i = 0; i < listScenesToLoad.Count; i++)
                {

                    listScenesToLoad[i].asyncOperation = SceneManager.LoadSceneAsync(listScenesToLoad[i].buildIndex, LoadSceneMode.Additive);
                    listScenesToLoad[i].asyncOperation.allowSceneActivation = false;

                    while (listScenesToLoad[i].asyncOperation.progress < 0.9f)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    listScenesToLoad[i].sceneObject = SceneManager.GetSceneByBuildIndex(listScenesToLoad[i].buildIndex);
                    yield return new WaitForSeconds(scenexSettings.delayBetweenLoading);

                    yield return null;
                }



                //Activado de escenas
                for (int i = 0; i < listScenesToLoad.Count; i++)
                {

                    listScenesToLoad[i].asyncOperation.allowSceneActivation = true;
                    yield return null;
                }




            }
        }


        #endregion

    }
}
