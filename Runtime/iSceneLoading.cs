using System.Collections;

namespace ExceptionSoftware.ExScenes
{
    public interface iSceneLoading
    {
        IEnumerator OnLoaded();
        IEnumerator OnUnLoading();



    }
}
