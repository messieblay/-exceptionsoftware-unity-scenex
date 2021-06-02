using System.Collections;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    public static class Scenex
    {
        static ScenexController _controller = null;
        public static void Load(System.Enum group, ScenexControllerEvents events = null)
        {
            Load(group.ToString(), events);
        }
        public static IEnumerator LoadRoutine(System.Enum group, ScenexControllerEvents events = null)
        {
            yield return LoadRoutine(group.ToString(), events);
        }

        public static void Load(string groupToLoad, ScenexControllerEvents events = null)
        {
            if (_controller == null)
            {
                _controller = GameObject.FindObjectOfType<ScenexController>();
            }

            if (_controller == null)
            {
                _controller = Instancex.Create<ScenexController>("Scenex controller");
            }

            _controller.LoadScene(groupToLoad, events);
        }

        public static IEnumerator LoadRoutine(string groupToLoad, ScenexControllerEvents events = null)
        {
            if (_controller == null)
            {
                _controller = GameObject.FindObjectOfType<ScenexController>();
            }

            if (_controller == null)
            {
                _controller = Instancex.Create<ScenexController>("Scenex controller");
            }

            yield return _controller.LoadScenes(groupToLoad, events);
        }

    }
}
