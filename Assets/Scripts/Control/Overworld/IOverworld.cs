namespace RPGProject.Control
{
    /// <summary>
    /// The interface for overworld entitys that determines what happens on battle start/end.
    /// </summary>
    public interface IOverworld
    {
        void BattleStartBehavior();
        void BattleEndBehavior();
    }
}