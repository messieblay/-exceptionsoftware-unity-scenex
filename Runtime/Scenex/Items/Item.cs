using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [System.Serializable]
    public abstract class Item : ScriptableObject
    {
        [SerializeField] public int priority;
        [SerializeField] public string description = string.Empty;
    }
}
