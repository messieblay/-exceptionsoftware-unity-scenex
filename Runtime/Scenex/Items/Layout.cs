using System.Collections.Generic;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class Layout : Item
    {
        public string ID => name.Replace(" ", string.Empty).ToLower();
        [SerializeField] public List<SceneInfo> scenes = new List<SceneInfo>();

        [System.NonSerialized] public List<SceneInfo> loadedScenes = new List<SceneInfo>();

    }
}
