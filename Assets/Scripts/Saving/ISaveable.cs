namespace RPGProject.Saving
{
    /// <summary>
    /// Interface used to handle individual saving and loading.
    /// </summary>
    public interface ISaveable
    {
        object CaptureState();
        void RestoreState(object _state);
    }
}
