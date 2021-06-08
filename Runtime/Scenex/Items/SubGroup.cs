
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class SubGroup : Layout
    {
        [SerializeField] public Group parent = null;
        [SerializeField] public SceneInfo loadingScreen = null;
        //[SerializeField] public string ID = null;
        [SerializeField] public bool waitForInput = true;
        [SerializeField] public LoadMode loadingMode = LoadMode.UnloadPrevious;

        public enum LoadMode
        {
            UnloadPrevious, Additive
        }
    }


}
