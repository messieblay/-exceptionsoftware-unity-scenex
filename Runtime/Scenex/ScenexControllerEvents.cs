using System.Collections;

namespace ExceptionSoftware.ExScenes
{
    public class ScenexControllerEvents
    {
        public System.Func<IEnumerator> onLoadingBegin = null;
        public System.Func<IEnumerator> onLoadingEnd = null;

        public System.Func<IEnumerator> onWaitForInput = null;

        public System.Func<IEnumerator> onFadeInFromGame = null;
        public System.Func<IEnumerator> onFadeOutToGame = null;

        public System.Func<IEnumerator> onFadeInToLoading = null;
        public System.Func<IEnumerator> onFadeOutFromLoading = null;

        public System.Action onLoadingProgressBegin = null;
        public System.Action onLoadingProgressEnd = null;

        public System.Action onLoadingScreenBegin = null;
        public System.Action onLoadingScreenEnd = null;

        public System.Action onWaitForInputBegin = null;
        public System.Action onWaitForInputEnd = null;

        public System.Action onMainSceneActived = null;

        public System.Action onAllScenesUnLoaded = null;
        public System.Action onAllScenesLoaded = null;


    }
}
