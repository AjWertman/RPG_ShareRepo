using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour, ISaveable
{
    [SerializeField] Transform retreatTransform = null;
    [SerializeField] bool isMover = true;
    [SerializeField] bool isBattleUnit = false;

    Animator animator = null;
    NavMeshAgent navMeshAgent = null;
    SoundFXManager unitSoundFX = null;

    Vector3 startPosition = Vector3.zero;
    Quaternion startRotation = Quaternion.identity;

    float startStoppingDistance = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetUnitSoundFX(SoundFXManager _unitSoundFX)
    {
        unitSoundFX = _unitSoundFX;
    }

    public void SetStartingTransforms()
    {
        startStoppingDistance = navMeshAgent.stoppingDistance;
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    public void MoveTo(Vector3 destination)
    {
        if (!isMover) return;
        navMeshAgent.SetDestination(destination);
    }

    public IEnumerator JumpToPos(Vector3 jumpPosition, Quaternion _startingRotation, bool isAttack)
    {
        bool hasCreatedJumpSound = false;

        animator.CrossFade("Jump", .1f);

        navMeshAgent.updateRotation = false;
        UpdateStoppingDistance(!isAttack);

        if (!hasCreatedJumpSound)
        {
            hasCreatedJumpSound = true;
            //unitSoundFX.CreateSoundFX(unitSoundFX.GetJumpSound());
        }

        MoveTo(jumpPosition);

        yield return new WaitForSeconds(.75f);

       
        animator.CrossFade("Idle", .1f);

        yield return new WaitForSeconds(.5f);

        transform.localRotation = _startingRotation;
    
        if (navMeshAgent == null) yield break;

        UpdateStoppingDistance(false);
        navMeshAgent.updateRotation = true;
        hasCreatedJumpSound = false;
    }

    public IEnumerator Retreat()
    {
        yield return JumpToPos(retreatTransform.position, startRotation, false);
    }

    public IEnumerator ReturnToStart()
    {
        yield return JumpToPos(startPosition, startRotation, false);
    }

    public void UpdateStoppingDistance(bool isZero)
    {
        if (isZero)
        {
            navMeshAgent.stoppingDistance = 0;
        }
        else
        {
            navMeshAgent.stoppingDistance = startStoppingDistance;
        }
    }

    public void FootR()
    {
        //unitSoundFX.CreateSoundFX(unitSoundFX.GetFootStepSound());
    }

    public void FootL()
    {
        //unitSoundFX.CreateSoundFX(unitSoundFX.GetFootStepSound());
    }

    public void StartJump()
    {
        //unitSoundFX.CreateSoundFX(unitSoundFX.GetJumpSound());
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }

    public Quaternion GetStartRotation()
    {
        return startRotation;
    }

    public object CaptureState()
    {
        return new SerializableVector3(transform.position);
    }

    public void RestoreState(object state)
    {
        SerializableVector3 position = (SerializableVector3)state;

        this.enabled = false;

        transform.position = position.ToVector3();

        this.enabled = true;
    }
}
