﻿using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ExceptionSoftware.ExScenes
{
    [InitializeOnLoad]
    public static class ScenexUtility
    {
        static ScenexSettings _settings = null;
        public static ScenexSettings Settings => LoadAsset();
        internal static ScenexSettings LoadAsset()
        {
            if (_settings == null)
            {
                _settings = ExAssets.FindAssetsByType<ScenexSettings>().FirstOrDefault();
            }

            if (_settings == null)
            {
                _settings = Resources.FindObjectsOfTypeAll<ScenexSettings>().FirstOrDefault();
            }

            return _settings;
        }




        #region Collect
        public static void Collect()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        public static void CollectGarbage() => System.GC.Collect();

        public static IEnumerator CollectRoutine()
        {
            System.GC.Collect();
            yield return null;
            var asyncop = Resources.UnloadUnusedAssets();
            yield return new WaitUntil(() => asyncop.isDone);
            yield return null;
        }
        #endregion


        public static void Log(string msg)
        {
#if EXLOGS
            Logx.Log("Scenex", msg, Settings.useUnityConsoleLog ? UnityLogType.Log : UnityLogType.None);
#else
            if (_settings.useUnityConsoleLog)
                Debug.Log("[Scenex] "+msg);
#endif
        }


        public static void LogError(string msg)
        {
#if EXLOGS
            Logx.Log("Scenex", msg, Settings.useUnityConsoleLog ? UnityLogType.Error : UnityLogType.None);
#else
            if (_settings.useUnityConsoleLog)
                Debug.LogError("[Scenex] " + msg);
#endif
        }

        public static void Call(this System.Action action) { if (action != null) action(); }
        public static IEnumerator Call(this System.Func<IEnumerator> function)
        {
            if (function != null)
            {
                foreach (var i in function.GetInvocationList())
                {
                    yield return ((System.Func<IEnumerator>)i).Invoke();
                }
            }
        }
    }
}
