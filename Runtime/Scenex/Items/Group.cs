using System.Collections.Generic;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class Group : Layout
    {
        [SerializeField] public bool waitForInput = true;
        [SerializeField] public List<SubGroup> childs = new List<SubGroup>();
    }
}
