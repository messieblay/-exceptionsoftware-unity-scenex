using System.Collections;

namespace ExceptionSoftware.ExScenes
{
    public interface iSceneLoading
    {
        IEnumerator OnUnLoading();

        IEnumerator OnLoaded();
        IEnumerator OnActivated();
        IEnumerator OnBeforeWaitForInput();
        IEnumerator OnLoadingFinished();



    }
}
