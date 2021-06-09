using System.Collections;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    public class ScenexEntryPoint : MonoBehaviour
    {
        public SceneInfo sceneInfo;
        public virtual void OnSceneStart(SceneInfo sceneInfo)
        {
            this.sceneInfo = sceneInfo;
        }

        public virtual IEnumerator OnLoaded() { yield return null; }
        public virtual IEnumerator OnUnLoading() { yield return null; }

        public virtual IEnumerator OnWaitForInput() { yield return null; }

        public virtual IEnumerator OnLoadingFinished() { yield return null; }

    }
}
