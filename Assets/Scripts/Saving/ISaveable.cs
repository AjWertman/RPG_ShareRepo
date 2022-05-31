namespace RPGProject.Saving
{
    public interface ISaveable
    {
        object CaptureState();
        void RestoreState(object _state);
    }
}
