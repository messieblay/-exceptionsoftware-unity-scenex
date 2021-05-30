using System.Collections.Generic;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class Layout : Item
    {
        [SerializeField] public List<SceneInfo> scenes = new List<SceneInfo>();
    }
}
