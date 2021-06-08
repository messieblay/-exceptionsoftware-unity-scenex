using System.Collections;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    public class SceneEntryPoint : MonoBehaviour
    {
        public SceneInfo sceneInfo;
        public virtual void OnSceneStart(SceneInfo sceneInfo)
        {
            this.sceneInfo = sceneInfo;
        }

        public virtual IEnumerator OnLoaded() { yield return null; }
        public virtual IEnumerator OnUnLoading() { yield return null; }
    }
}
