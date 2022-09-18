using RPGProject.Control.Combat;
using RPGProject.Core;
using RPGProject.Inventories;
using RPGProject.Movement;
using RPGProject.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class PlayerController : MonoBehaviour, IOverworld
    {
        [SerializeField] List<CharacterKey> playerTeam = new List<CharacterKey>();

        [SerializeField] GameObject playerMesh = null;
        [SerializeField] GameObject camLookObject = null;

        [SerializeField] KeyCode menuKeyCode = KeyCode.Escape;
        [SerializeField] Texture2D cursorTexture = null;

        [SerializeField] AudioClip footstepsClip = null;

        Animator animator = null;
        FollowCamera followCamera = null;
        PlayerConversant playerConversant = null;
        PlayerMover playerMover = null;
        SoundFXManager soundFXManager = null;
        UICanvas uiCanvas = null;

        Inventory playerInventory = null;
        PlayerTeamManager playerTeamManager = null;

        BattleZoneTrigger contestedBattleZoneTrigger = null;
        Checkpoint lastCheckpoint = null;

        bool hasStartedBattle = false;
        bool isBattling = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerConversant = GetComponent<PlayerConversant>();
            playerMover = GetComponent<PlayerMover>();

            playerInventory = GetComponentInChildren<Inventory>();
        }

        private void Start()
        {
            followCamera = FindObjectOfType<FollowCamera>();
            uiCanvas = FindObjectOfType<UICanvas>(false);
            soundFXManager = FindObjectOfType<SoundFXManager>();

            playerTeamManager = FindObjectOfType<PlayerTeamManager>();
            playerTeamManager.PopulateTeamInfos(playerTeam);

            //Cursor.SetCursor(cursorTexture, new Vector2(0, 0), CursorMode.ForceSoftware);
        }

        private void Update()
        {
            int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            if (currentSceneIndex == 0 || currentSceneIndex == 1) return;

            if (!isBattling)
            {
                playerMover.Move();

                if (InteractWithUI()) return; 
                
                if (HandleMouseRaycast()) return;

                ActivateCursor(false);
            }
            else
            {
                ActivateCursor(true);
            }
        }

        private bool InteractWithUI()
        {
            //Refactor - turn back on when ready
            return false;

            bool isInteracting = false;

            //if (uiCanvas.IsTutorialActive()) isInteracting = true;
            if (playerConversant.IsChatting()) isInteracting = true;
            if (uiCanvas.IsAnyMenuActive()) isInteracting = true;

            if (Input.GetKeyDown(menuKeyCode) || Input.GetKeyDown(KeyCode.Escape))
            {
                ActivateCoreMenu();
                isInteracting = true;
                playerConversant.Quit();
            }

            if(followCamera != null)
            {
                followCamera.SetCanRotate(!isInteracting);
            }

            playerMover.canMove = !isInteracting;
            ActivateCursor(isInteracting);
            return isInteracting;
        }

        private bool HandleMouseRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = RaycastAllSorted(ray);

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.gameObject.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        uiCanvas.ActivateActivateUIPrompt(raycastable.WhatToActivate());

                        if (Input.GetMouseButton(0))
                        {
                            uiCanvas.DeactivateActivateUIPrompt();
                            raycastable.WhatToDoOnClick(this);
                        }
                        return true;
                    }
                }
            }

            uiCanvas.DeactivateActivateUIPrompt();
            return false;
        }

        private void ActivateCoreMenu()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1) return;

            if (!uiCanvas.IsAnyPlayerMenuActive())
            {
                uiCanvas.ActivatePlayerMenu();
            }
            else
            {
                uiCanvas.DeactivatePlayerMenu();
            }
        }

        public void ActivateCheckpointMenu(Checkpoint _checkpoint)
        {
            uiCanvas.ActivateCheckpointMenu(_checkpoint);
        }

        public void ForceDeactivateCheckpointMenu()
        {
            uiCanvas.DeactivateCheckpointMenu();
        }

        public void BattleStartBehavior()
        {
            isBattling = true;
            animator.enabled = false;
            playerMesh.SetActive(false);

            if(followCamera != null)
            {
                followCamera.gameObject.SetActive(false);
            }

            ActivateCursor(true);

            uiCanvas.DeactivateAllUI();
        }

        public void BattleEndBehavior()
        {
            isBattling = false;
            hasStartedBattle = false;
            animator.enabled = true;  
            followCamera.gameObject.SetActive(true);
            playerMesh.SetActive(true);
        }

        public void ReturnToLastCheckpoint()
        {
            uiCanvas.DeactivatePlayerMenu();
            lastCheckpoint.FastTravel(lastCheckpoint.GetFastTravelPoint());
        }

        public void SetLastCheckpoint(Checkpoint _newCheckpoint)
        {
            if (lastCheckpoint == _newCheckpoint) return;
            lastCheckpoint = _newCheckpoint;
        }

        private void ActivateCursor(bool shouldActivate)
        {
            if (Cursor.visible == shouldActivate) return;
            if (shouldActivate)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void FootStepBehavior()
        {
            soundFXManager.CreateSoundFX(footstepsClip, transform, .2f);
            if (contestedBattleZoneTrigger != null)
            {
                contestedBattleZoneTrigger.BattleCheck();
            }
        }

        //Animation Event
        public void FootR()
        {
            FootStepBehavior();
        }

        //Animation Event
        public void FootL()
        {
            FootStepBehavior();
        }

        private void OnTriggerEnter(Collider other)
        {
            BattleZoneTrigger battleZoneTrigger = other.GetComponent<BattleZoneTrigger>();
            if (battleZoneTrigger != null)
            {
                bool isEnemyTrigger = battleZoneTrigger.isEnemyTrigger;
                if (isEnemyTrigger)
                {
                    if (!hasStartedBattle)
                    {
                        hasStartedBattle = true;
                        StartCoroutine(battleZoneTrigger.StartBattle());
                    }
                }
                else
                {
                    contestedBattleZoneTrigger = battleZoneTrigger;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            BattleZoneTrigger battleZoneTrigger = other.GetComponent<BattleZoneTrigger>();
            if (battleZoneTrigger != null)
            {
                bool isEnemyTrigger = battleZoneTrigger.isEnemyTrigger;
                if (!isEnemyTrigger)
                {
                    contestedBattleZoneTrigger = null;
                }
            }
        }

        private RaycastHit[] RaycastAllSorted(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        public Inventory GetInventory()
        {
            return playerInventory;
        }

        public PlayerMover GetPlayerMover()
        {
            return playerMover;
        }
        
        public Transform GetCamLookTransform()
        {
            return camLookObject.transform;
        }

        public void AddTeammate(CharacterKey _playerKey)
        { 
            playerTeamManager.AddTeammate(_playerKey);
        }

        public void RemoveTeammate(CharacterKey _playerKey)
        {
            playerTeamManager.RemoveTeammate(_playerKey);
        }
    }
}