using System.Collections.Generic;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    public class ScenexSettings : ScriptableObject
    {
        public bool useEmptySceneToLoad = true;
        public float delayBetweenLoading = .1f;

        public List<SceneInfo> scenes = new List<SceneInfo>();
        public List<Group> groups = new List<Group>();
        public List<SubGroup> subgroups = new List<SubGroup>();

    }
}
