using UnityEngine;

public class Checkpoint : MonoBehaviour, IRaycastable
{
    [SerializeField] float activationDistance = 5f;

    FastTravelPoint fastTravelPoint = null;
    PlayerController player = null;

    private void Awake()
    {
        fastTravelPoint = GetComponent<FastTravelPoint>();
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public void FastTravel(FastTravelPoint fastTravelPoint)
    {
        player.ForceDeactivateCheckpointMenu();
        StartCoroutine(player.Teleport(fastTravelPoint.GetTeleportLocation()));
    }

    public bool HandleRaycast(PlayerController playerController)
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= activationDistance)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerController.SetLastCheckpoint(this);
            }
            return true;
        }

        return false;
    }

    public FastTravelPoint GetFastTravelPoint()
    {
        return fastTravelPoint;
    }

    public string WhatToActivate()
    {
        return "Stop at Checkpoint";
    }

    public void WhatToDoOnClick(PlayerController playerController)
    {
        player.ActivateCheckpointMenu(this);
    }
}
