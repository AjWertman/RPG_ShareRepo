public interface IRaycastable
{
    bool HandleRaycast(PlayerController playerController);
    string WhatToActivate();
    void WhatToDoOnClick(PlayerController playerController);
}
