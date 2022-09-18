namespace RPGProject.Control
{
    /// <summary>
    /// Interface on how the player will interact with objects
    /// during raycasts. Will not raycast if the attached collider
    /// does not have this interface.
    /// </summary>
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController _playerController); //If the player can or cannot raycast.
        string WhatToActivate(); //The string to signify what the player would activate.
        void WhatToDoOnClick(PlayerController _playerController);
    }
}