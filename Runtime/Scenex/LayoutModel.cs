using System.Collections.Generic;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class LayoutModel
    {
        [SerializeField, HideInInspector] public string id;
        [SerializeField] public List<SceneInfo> scenes = new List<SceneInfo>();
        [SerializeField] public List<GroupModel> groups = new List<GroupModel>();

    }
}
