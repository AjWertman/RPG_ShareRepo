using RPGProject.Core;
using RPGProject.Movement;
using System;
using UnityEngine;

namespace RPGProject.Control
{
    public class PlayerController : MonoBehaviour, IOverworld
    {
        [SerializeField] PlayerKey[] playerTeam = null;

        [SerializeField] GameObject playerMesh = null;
        [SerializeField] GameObject camLookObject = null;

        [SerializeField] KeyCode menuKeyCode = KeyCode.Escape;
        [SerializeField] Texture2D cursorTexture = null;

        Animator animator = null;
        FollowCamera followCamera = null;
        PlayerConversant playerConversant = null;
        PlayerMover playerMover = null;
        PlayerTeam playerTeamManager = null;
        UICanvas uiCanvas = null;

        BattleZoneTrigger contestedBattleZoneTrigger = null;
        Checkpoint lastCheckpoint = null;

        bool hasStartedBattle = false;
        bool isBattling = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            playerConversant = GetComponent<PlayerConversant>();

            playerMover = GetComponent<PlayerMover>();
        }

        private void Start()
        {
            followCamera = FindObjectOfType<FollowCamera>();
            uiCanvas = FindObjectOfType<UICanvas>();

            playerTeamManager = FindObjectOfType<PlayerTeam>();
            playerTeamManager.PopulateTeamInfos(playerTeam);

            //Cursor.SetCursor(cursorTexture, new Vector2(0, 0), CursorMode.ForceSoftware);
        }

        private void Update()
        {
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

            followCamera.SetCanRotate(!isInteracting);
            playerMover.SetCanMove(!isInteracting);
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

        private void OnTriggerEnter(Collider other)
        {
            BattleZoneTrigger battleZoneTrigger = other.GetComponent<BattleZoneTrigger>();
            if (battleZoneTrigger != null)
            {
                bool isEnemyTrigger = battleZoneTrigger.IsEnemyTrigger();
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
                bool isEnemyTrigger = battleZoneTrigger.IsEnemyTrigger();
                if (!isEnemyTrigger)
                {
                    contestedBattleZoneTrigger = null;
                }
            }
        }

        public void BattleStartBehavior()
        {
            isBattling = true;
            animator.enabled = false;
            playerMesh.SetActive(false);
            followCamera.gameObject.SetActive(false);
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

        //Refactor - Create sound effect
        private void FootStepBehavior()
        {
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

        public PlayerMover GetPlayerMover()
        {
            return playerMover;
        }

        public Transform GetCamLookTransform()
        {
            return camLookObject.transform;
        }

        public void EquipWeapon(bool _isSword)
        {
            EquipmentManager equipmentManager = FindObjectOfType<EquipmentManager>();

            print(equipmentManager == null);
            equipmentManager.EquipWeapon(_isSword);
            //Equip abilities
        }

    }
}