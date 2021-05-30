namespace ExceptionSoftware.ExScenes
{
    public enum SceneStatus
    {
        Unload,
        Loading,
        Loaded,
        Showing
    }

    public enum SceneEvents
    {
        OnUnload,
        OnLoaded,
        OnShowing,
        OnOpened,
        OnClosed
    }
}
