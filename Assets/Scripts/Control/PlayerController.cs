using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, ISaveable, IOverworld
{
    [Header("Core")]
    [SerializeField] GameObject owMeshObject = null;
    [SerializeField] GameObject cameraObject = null;
    [SerializeField] GameObject camLookObject = null;
    [SerializeField] Checkpoint lastCheckpoint = null;

    List<Unit> playerTeam = new List<Unit>();

    [Header("Controls")]
    [SerializeField] KeyCode menuKeyCode = KeyCode.Escape;
    [SerializeField] Texture2D cursorTexture = null;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 100f;
    [SerializeField] float turnSmoothness = .1f;
    [SerializeField] float gravity =20f;
    float turnSmoothVelocity;

    Animator animator = null;
    CharacterController characterController = null;
    PlayerConversant playerConversant = null;
    Vector3 direction = Vector3.zero;
    SoundFXManager unitSoundFX = null;

    FollowCamera followCamera = null;

    PlayerTeam playerTeamInfo = null;
    UICanvas uiCanvas = null;

    BattleZoneTrigger contestedBattleZoneTrigger = null;
    bool isInBattleTrigger = false;

    bool hasStartedBattle = false;
    bool isBattling = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerConversant = GetComponent<PlayerConversant>();
        playerTeamInfo = FindObjectOfType<PlayerTeam>();
        unitSoundFX = GetComponent<SoundFXManager>();
    }

    private void Start()
    {
        playerTeam = playerTeamInfo.GetPlayerTeam();

        followCamera = FindObjectOfType<FollowCamera>();

        uiCanvas = FindObjectOfType<UICanvas>();

        //Cursor.SetCursor(cursorTexture, new Vector2(0, 0), CursorMode.ForceSoftware);
    }

    private void Update()
    {
        if (!isBattling)
        {
            Move();

            if (InteractWithUI()) return;           
            if (HandleMouseRaycast()) return;
          
            SetCursor(false);
        }
        else
        {
            SetCursor(true);
        }
    }

    private void Move()
    {
        if (!uiCanvas.IsAnyMenuActive() && !uiCanvas.IsTutorialActive())
        {
            direction = new Vector3(Input.GetAxisRaw("Horizontal"), -0, Input.GetAxisRaw("Vertical")).normalized;
            float magnitude = (direction.x * direction.x + direction.z * direction.z);
            if (magnitude >= .01f)
            {
                float aimAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, aimAngle, ref turnSmoothVelocity, turnSmoothness);
                transform.rotation = Quaternion.Euler(0, angle, 0);

                Vector3 moveDirection = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

                characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
                animator.SetBool("isRunning", true);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        characterController.Move(Vector3.down.normalized * gravity * Time.deltaTime);
    }
    
    private bool InteractWithUI()
    {
        if (uiCanvas.IsTutorialActive())
        {
            followCamera.SetCanRotate(false);
            SetCursor(true);
            return true;
        }

        if (playerConversant.IsChatting())
        {
            followCamera.SetCanRotate(false);
            SetCursor(true);
            return true;
        }

        if (uiCanvas.IsCheckPointMenuActive())
        {
            followCamera.SetCanRotate(false);
            SetCursor(true);
            return true;
        }

        if (Input.GetKeyDown(menuKeyCode) || Input.GetKeyDown(KeyCode.Escape))
        {
            ActivateCoreMenu();
        }

        if (uiCanvas.IsCoreMenuActive())
        {
            followCamera.SetCanRotate(false);
            SetCursor(true);
            return true;
        }
        
        followCamera.SetCanRotate(true);
        return false;
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
                    uiCanvas.ActivateActivateUI(raycastable.WhatToActivate());

                    if (Input.GetMouseButton(0))
                    {
                        uiCanvas.DeactivateActivateUI();
                        raycastable.WhatToDoOnClick(this);
                    }
                    return true;
                }
            }
        }
        uiCanvas.DeactivateActivateUI();
        return false;
    }

    private RaycastHit[] RaycastAllSorted(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray);
        float[] distances = new float[hits.Length];

        for (int i = 0; i < hits.Length; i++)
        {
            distances[i] = hits[i].distance;
        }

        Array.Sort(distances, hits);

        return hits;
    }

    private void ActivateCoreMenu()
    {
        if (!uiCanvas.AreAnyCoreMenusActive())
        {
            uiCanvas.ActivateCoreMenu(true);
        }
        else
        {
            uiCanvas.ActivateCoreMenu(false);
        }
    }

    public void ActivateCheckpointMenu(Checkpoint checkpoint)
    {
        uiCanvas.ActivateCheckpointMenu(checkpoint);
    }

    public void ForceDeactivateCheckpointMenu()
    {
        uiCanvas.ForceDeactivateCheckpointMenu();
    }

    public Transform GetCamLookTransform()
    {
        return camLookObject.transform;
    }

    public IEnumerator Teleport(Transform teleportLocation)
    {
        yield return FindObjectOfType<Fader>().FadeOut(Color.white, 1f);
        yield return MoveCharacter(teleportLocation);
        yield return new WaitForSeconds(1f);
        yield return FindObjectOfType<Fader>().FadeIn(.5f);
    }

    private IEnumerator MoveCharacter(Transform teleportLocation)
    {
        characterController.enabled = false;
        transform.position = teleportLocation.position;
        characterController.enabled = true;
        yield return null;
    }

    public void ReturnToLastCheckpoint()
    {
        uiCanvas.ForceDeactivateCoreMenu();
        StartCoroutine(Teleport(lastCheckpoint.GetFastTravelPoint().GetTeleportLocation()));
    }

    public void SetLastCheckpoint(Checkpoint newCheckpoint)
    {
        if (lastCheckpoint == newCheckpoint) return;
        lastCheckpoint = newCheckpoint;
    }

    public void BattleStartBehavior()
    {
        isBattling = true;
        animator.SetBool("isRunning", false);
        cameraObject.SetActive(false);
        owMeshObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        uiCanvas.DeactivateAllMenus();
    }

    public void BattleEndBehavior()
    {
        isBattling = false;
        hasStartedBattle = false; 
        cameraObject.SetActive(true);
        owMeshObject.SetActive(true);
    }

    private void SetCursor(bool shouldActivate)
    {
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

    //private void UpdateAnimator()
    //{
    //    Vector3 velocity = GetComponent<Rigidbody>().velocity;
    //    Vector3 localVelocity = transform.InverseTransformDirection(velocity);
    //    float forwardSpeed = localVelocity.z;

    //    animator.SetFloat("forwardSpeed", forwardSpeed);
    //}

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
                isInBattleTrigger = true;
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
                isInBattleTrigger = false;
            }
        }
    }

    private void FootStepBehavior()
    {
        //unitSoundFX.CreateSoundFX(unitSoundFX.GetFootStepSound());
        if (contestedBattleZoneTrigger != null)
        {
            contestedBattleZoneTrigger.BattleCheck();
        }
    }

    public void FootR()
    {
        FootStepBehavior();
    }

    public void FootL()
    {
        FootStepBehavior();
    }

    public object CaptureState()
    {
        return new SerializableVector3(transform.position);
    }

    public void RestoreState(object state)
    {
        SerializableVector3 position = (SerializableVector3)state;

        GetComponent<CharacterController>().enabled = false;

        transform.position = position.ToVector3();

        GetComponent<CharacterController>().enabled = true;
    }
}
