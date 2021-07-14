using System.Collections;

namespace ExceptionSoftware.ExScenes
{
    public interface iSceneLoading
    {
        IEnumerator OnUnLoading();

        IEnumerator OnLoaded();
        IEnumerator OnBeforeWaitForInput();
        IEnumerator OnLoadingFinished();



    }
}
