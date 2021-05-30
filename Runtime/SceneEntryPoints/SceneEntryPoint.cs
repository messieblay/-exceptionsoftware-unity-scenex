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

    }
}
