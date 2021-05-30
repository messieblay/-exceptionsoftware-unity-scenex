using System.Collections.Generic;
using UnityEngine;
namespace ExceptionSoftware.ExScenes
{
    public class Schema : ScriptableObject
    {
        public SceneInfo mainScene = null;
        public List<SceneInfo> scenes = new List<SceneInfo>();
        public bool waitForInput = true;

        [System.NonSerialized] public List<SceneInfo> loadedScenes = new List<SceneInfo>();

    }
}
