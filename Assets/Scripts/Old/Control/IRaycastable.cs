public interface IRaycastable
{
    bool HandleRaycast(PlayerController _playerController);
    string WhatToActivate();
    void WhatToDoOnClick(PlayerController _playerController);
}
