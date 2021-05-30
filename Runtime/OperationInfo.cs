using System.Collections.Generic;

namespace ExceptionSoftware.ExScenes
{
    public class OperationInfo
    {
        public bool InputReceived = false;
        public float scenesProgress = 0;
        List<SceneInfo> currentScenes;

        public void CopyLoadedScenes(List<SceneInfo> sceneData)
        {
            currentScenes = sceneData.CloneList();
        }

        public bool GetScenesProgress
        {
            get
            {
                scenesProgress = 0;
                if (currentScenes != null)
                {
                    foreach (SceneInfo sceneData in currentScenes)
                    {
                        if (0.9f < (float)sceneData.asyncOperation.progress)
                            return false;
                    }
                    return true;
                }
                return false;
            }
        }

        public bool GetScenesProgressIsDone
        {
            get
            {
                scenesProgress = 0;
                if (currentScenes != null)
                {
                    foreach (SceneInfo sceneData in currentScenes)
                    {
                        if (!sceneData.asyncOperation.isDone)
                            return false;
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
