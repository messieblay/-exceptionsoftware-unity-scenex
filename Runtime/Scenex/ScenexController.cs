using System.Collections;
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

        #region Loading Full Schema

        public void OpenScenes(Schema schema, SceneInfo mainScene = null)
        {
            //if (_loadingRoutine != null) return;
            if (schema.mainScene == null && mainScene != null) schema.mainScene = mainScene;
            if (schema.mainScene == null)
            {
                Debug.LogError("Impedido cargado de escenas. Escena principal null");
            }
            //_loadingRoutine = StartCoroutine(LoadScenesRoutine(schema));
        }

        OperationInfo currentOperation;
        public IEnumerator LoadScenesRoutine(Schema schema)
        {
            onLoadingProgressStarts.Call();

            //InputServicex.DesActiveIS(111);
            //_actionDone = 0;
            //_actionsMax = (schema.scenes.Count + 1) * 3;
            currentOperation = new OperationInfo();
            //schema.currentLoadingScreen = schema.loadingScreen ?? _sceneList.defaultLoadginScreen;

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

            //yield return OpenLoadingScreen(schema);
            yield return null;

            yield return LoadScenes(schema);
            yield return null;

            ScenexUtility.Log($" -- pre  START WAITING ALL SCENES LOADED-- ");
            bool canpass = false;

            ScenexUtility.Log($" -- START WAITING ALL SCENES LOADED-- ");
            while (!canpass)
            {
                canpass = true;
                try
                {
                    for (int i = 0; i < schema.loadedScenes.Count; i++)
                    {
                        if (schema.loadedScenes[i] == null) continue;
                        if (schema.loadedScenes[i].Status != SceneStatus.Loaded)
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
            yield return ShowLoadedScenes();
            yield return null;


            ScenexUtility.Log("#CallShowSceneEntryPoint");
            for (int i = 0; i < schema.loadedScenes.Count; i++)
            {
                //CallShowSceneEntryPoint(schema.loadedScenes[i]);
                //IncProgress();
                yield return null;
            }

            //yield return LoadIScenesRoutine();

            //Toda la carga realizada
            onAllScenesLoaded.Call();

            if (schema.waitForInput)
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
        #region Schema
        [System.NonSerialized] public Schema currentSchema = null;
        IEnumerator LoadScenes(Schema schema)
        {

            currentSchema = schema;

            {
                /*
                * Ahora que unity tiene todo vaciado sabe distinguir cuales son los assets que sobran. Limpiamos
                */
                ScenexUtility.Log("Assets UNLOAD");
                yield return ScenexUtility.CollectRoutine();
                ScenexUtility.Log("Assets UNLOADED");
            }



            /*
             * INICIO DE CARGADO
             * CARGADO DE MAIN SCENE
             */
            LoadScene(currentSchema.mainScene);

            /*
             * CARGADO DE CHUNKS
             */
            //for (int i = 0; i < currentSchema.mainScene.childs.Count; i++)
            //{
            //    LoadScene(currentSchema.mainScene.childs[i]);

            //    yield return null;
            //}

            /*
             * CARGADO ESCENAS DEPENDIENTES DEL ESQUEMA
             */
            for (int i = 0; i < currentSchema.scenes.Count; i++)
            {
                LoadScene(currentSchema.scenes[i]);

                yield return null;
            }
            ScenexUtility.Log("--End Start LoadingScenes");

            try
            {
                currentOperation.CopyLoadedScenes(currentSchema.loadedScenes);
            }
            catch (System.Exception e)
            {
                ScenexUtility.Log("[FAIL] Creating current operation " + e.ToString());
            }

            ScenexUtility.Log("Created current operation");

            yield return null;
        }


        #endregion

        public IEnumerator ShowLoadedScenes()
        {
            for (int i = 0; i < currentSchema.loadedScenes.Count; i++)
            {
                ScenexUtility.Log($"\t Activate {currentSchema.loadedScenes[i].sceneName}");
                currentSchema.loadedScenes[i].asyncOperation.allowSceneActivation = true;
                yield return null;
            }

            for (int i = 0; i < currentSchema.loadedScenes.Count; i++)
            {
                //while (!currentSchema.loadedScenes[i].SceneObject.isLoaded)
                //{
                //    yield return null;
                //}

                try
                {
                    ScenexUtility.Log($"\t Showing {currentSchema.loadedScenes[i].sceneName}");
                    currentSchema.loadedScenes[i].Status = SceneStatus.Showing;
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }

                //try
                //{
                //    CallSceneEntryPoint(currentSchema.loadedScenes[i]);
                //}
                //catch (System.Exception ex)
                //{
                //    Debug.LogException(ex);
                //}
                //IncProgress();
                yield return new WaitForSeconds(.1f);
            }

            ScenexUtility.Log($"- loadingCoroutineFinished");
            yield return null;
            yield return null;
            //ActiveSchemaMainScene();
            yield return null;
            yield return new WaitForSeconds(2);


            //void ActiveSchemaMainScene() => SceneManager.SetActiveScene(currentSchema.loadedScenes.First().SceneObject);
        }


        #region Load 1 Scene

        public void LoadScene(SceneInfo info)
        {
            StartCoroutine(LoadSceneRoutine(info));
        }

        internal IEnumerator LoadSceneRoutine(SceneInfo info)
        {
            ScenexUtility.Log($"\tBEGIN {info.sceneName}");
            currentSchema.loadedScenes.Add(info);
            info.asyncOperation = SceneManager.LoadSceneAsync(info.sceneName, LoadSceneMode.Additive);
            info.asyncOperation.allowSceneActivation = false;

            info.Status = SceneStatus.Loading;

            while (info.asyncOperation.progress < 0.9f)
            {
                yield return new WaitForEndOfFrame();
            }

            info.sceneObject = SceneManager.GetSceneByName(info.sceneName);
            info.Status = SceneStatus.Loaded;
            //IncProgress();
            ScenexUtility.Log($"\tEND {info.sceneName}");
        }

        #endregion


        //void Call(System.Action action) { if (action != null) action(); }
        //IEnumerator Call(System.Func<IEnumerator> function) { if (function != null) yield return function(); }
    }
}
