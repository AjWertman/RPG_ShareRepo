using RPGProject.Core;
using System.Collections;
using UnityEngine;

namespace RPGProject.Control
{
    public class Checkpoint : MonoBehaviour, IRaycastable
    {
        [SerializeField] float activationDistance = 5f;

        FastTravelPoint fastTravelPoint = null;
        PlayerController player = null;

        Fader fader = null;

        private void Awake()
        {
            fastTravelPoint = GetComponent<FastTravelPoint>();
        }

        private void Start()
        {
            fader = FindObjectOfType<Fader>();
            player = FindObjectOfType<PlayerController>();
        }

        public void FastTravel(FastTravelPoint _fastTravelPoint)
        {
            player.ForceDeactivateCheckpointMenu();

            Transform teleportLocation = _fastTravelPoint.GetTeleportLocation();
            StartCoroutine(Teleport(teleportLocation));
        }

        private IEnumerator Teleport(Transform _teleportLocation)
        {
            CharacterController characterController = player.GetPlayerMover().GetCharacterController();

            yield return fader.FadeOut(Color.black, 1f);
            
            characterController.enabled = false;
            transform.position = _teleportLocation.position;

            yield return fader.FadeIn(.5f);

            characterController.enabled = true;
        }

        public bool HandleRaycast(PlayerController _playerController)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= activationDistance)
            {
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

        public void WhatToDoOnClick(PlayerController _playerController)
        {
            _playerController.SetLastCheckpoint(this);
            player.ActivateCheckpointMenu(this);
        }
    }
}

