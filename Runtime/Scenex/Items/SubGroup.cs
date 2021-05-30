
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public class SubGroup : Layout
    {
        [SerializeField] public Group parent = null;
        [SerializeField] public bool waitForInput = true;
    }
}
