using UnityEngine;

public class FastTravelPoint : MonoBehaviour
{
    [SerializeField] Transform teleportLocation = null;
    [SerializeField] string pointName = "Default Point";

    public Transform GetTeleportLocation()
    {
        return teleportLocation;
    }

    public string GetName()
    {
        return pointName;
    }
}
