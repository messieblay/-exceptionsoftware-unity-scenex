using System.Collections;

namespace ExceptionSoftware.ExScenes
{
    public interface iSceneLoading
    {
        IEnumerator Loading();
        IEnumerator UnLoading();

        int Priority();


    }
}
