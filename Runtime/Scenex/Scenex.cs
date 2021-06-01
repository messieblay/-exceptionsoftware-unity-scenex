using System.Collections;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    public static class Scenex
    {
        static ScenexController _controller = null;
        public static void Load(System.Enum group)
        {
            Load(group.ToString());
        }
        public static IEnumerator LoadRoutine(System.Enum group)
        {
            yield return LoadRoutine(group.ToString());
        }

        public static void Load(string groupToLoad)
        {
            if (_controller == null)
            {
                _controller = GameObject.FindObjectOfType<ScenexController>();
            }

            if (_controller == null)
            {
                _controller = Instancex.Create<ScenexController>("Scenex controller");
            }

            _controller.LoadScene(groupToLoad);
        }

        public static IEnumerator LoadRoutine(string groupToLoad)
        {
            if (_controller == null)
            {
                _controller = GameObject.FindObjectOfType<ScenexController>();
            }

            if (_controller == null)
            {
                _controller = Instancex.Create<ScenexController>("Scenex controller");
            }

            yield return _controller.LoadScenes(groupToLoad);
        }

    }
}
