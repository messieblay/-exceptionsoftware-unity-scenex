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

        public System.Action onLoadingProgressStarts = null;
        public System.Action onLoadingProgressEnds = null;
        public System.Action onAllScenesLoaded = null;


        IEnumerator LoadScenes()
        {

            yield return onPreLoading();

            //Loading tasks


            yield return onPostLoading();
        }

        #region Loading Full Group

        public void OpenScenes(Group Group, SceneInfo mainScene = null)
        {
            //if (_loadingRoutine != null) return;
            //if (Group.mainScene == null && mainScene != null) Group.mainScene = mainScene;
            //if (Group.mainScene == null)
            //{
            //    Debug.LogError("Impedido cargado de escenas. Escena principal null");
            //}
            //_loadingRoutine = StartCoroutine(LoadScenesRoutine(Group));
        }

        OperationInfo currentOperation;
        public IEnumerator LoadScenesRoutine(Group Group)
        {
            onLoadingProgressStarts.Call();

            //InputServicex.DesActiveIS(111);
            //_actionDone = 0;
            //_actionsMax = (Group.scenes.Count + 1) * 3;
            currentOperation = new OperationInfo();
            //Group.currentLoadingScreen = Group.loadingScreen ?? _sceneList.defaultLoadginScreen;

            //OnChangeProgress();

            yield return onFadeIn.Call();

            ScenexUtility.Log("################ StartLoading");
            yield return null;

            /*
             * Descargar todas las escenas
             */
            ScenexUtility.Log("\tUnloadPreviousScenesRoutine");
            //yield return UnloadPreviousScenesRoutine();
            yield return null;

            /*
            * Cargar la escena vacia
            */
            ScenexUtility.Log("Empty");
            //SceneManager.LoadScene(_sceneList.Empty.SceneName);
            yield return null;

            //yield return OpenLoadingScreen(Group);
            yield return null;

            //yield return LoadScenes(Group);
            yield return null;

            ScenexUtility.Log($" -- pre  START WAITING ALL SCENES LOADED-- ");
            bool canpass = false;

            ScenexUtility.Log($" -- START WAITING ALL SCENES LOADED-- ");
            while (!canpass)
            {
                canpass = true;
                try
                {
                    for (int i = 0; i < Group.loadedScenes.Count; i++)
                    {
                        if (Group.loadedScenes[i] == null) continue;
                        if (Group.loadedScenes[i].Status != SceneStatus.Loaded)
                        {
                            canpass = false;
                            break;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    ScenexUtility.Log($" -- FALLO -- {e.Message}");
                    canpass = false;
                }
                //ScenexUtility.Log($" ---- wait {!canpass} ");
                yield return null;
            }
            ScenexUtility.Log($" -- ALL SCENE MUST BE LOADED -- ");

            yield return null;
            ScenexUtility.Log("#ShowLoadedScenes");
            //yield return ShowLoadedScenes();
            yield return null;


            ScenexUtility.Log("#CallShowSceneEntryPoint");
            for (int i = 0; i < Group.loadedScenes.Count; i++)
            {
                //CallShowSceneEntryPoint(Group.loadedScenes[i]);
                //IncProgress();
                yield return null;
            }

            //yield return LoadIScenesRoutine();

            //Toda la carga realizada
            onAllScenesLoaded.Call();

            if (Group.waitForInput)
            {
                yield return null;
                yield return onWaitForInput.Call();
                yield return null;
            }

            //InputServicex.ActiveIS(111, 2);

            currentOperation.InputReceived = true;
            //IncProgress();

            onLoadingProgressEnds.Call();
            yield return null;

            yield return onPostLoading.Call();

            yield return onFadeOut.Call();

            //_loadingRoutine = null;
        }

        #endregion
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


            //UNLOAD SCENES
            empty = SceneManager.CreateScene("Empty", new CreateSceneParameters());

            SceneManager.SetActiveScene(empty);

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
            }


            Debug.Log("All current scenes unloaded");



            yield return new WaitForSeconds(5);

            {
                /*
                * Ahora que unity tiene todo vaciado sabe distinguir cuales son los assets que sobran. Limpiamos
                */
                ScenexUtility.Log("Assets UNLOAD");
                yield return ScenexUtility.CollectRoutine();
                ScenexUtility.Log("Assets UNLOADED");
            }


            List<SceneInfo> listScenesToLoad = new List<SceneInfo>();
            listScenesToLoad.AddRange(currentGroup.scenes);
            listScenesToLoad.AddRange(currentSubGroup.scenes);

            listScenesToLoad = listScenesToLoad.OrderBy(s => s.priority).ToList();


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

                yield return null;
            }


            //Show scenes
            for (int i = 0; i < listScenesToLoad.Count; i++)
            {

                listScenesToLoad[i].asyncOperation.allowSceneActivation = true;
                yield return null;
            }

            SceneInfo mainScene = listScenesToLoad.Find(s => s.isMainScene);
            if (mainScene != null)
                SceneManager.SetActiveScene(mainScene.sceneObject);

            yield return SceneManager.UnloadSceneAsync(empty, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

            ScenexUtility.Log("--End Start LoadingScenes");

            ScenexUtility.Log("Created current operation");

            yield return null;
        }


        #endregion

    }
}
