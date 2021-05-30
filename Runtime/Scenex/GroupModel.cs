using System.Collections.Generic;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class GroupModel
    {
        [SerializeField] public string id;
        [SerializeField] public bool waitForInput = true;
        [SerializeField] public List<SceneInfo> scenes = new List<SceneInfo>();
    }
}
