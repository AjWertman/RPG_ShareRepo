using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour, ISaveable
{
    [SerializeField] Transform retreatTransform = null;
    [SerializeField] bool isMover = true;

    Animator animator = null;
    NavMeshAgent navMeshAgent = null;
    UnitSoundFX unitSoundFX = null;

    Vector3 startPosition = Vector3.zero;
    Quaternion startRotation = Quaternion.identity;

    float startStoppingDistance = 0;

    bool isBattleUnit = true;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SetAnimator(Animator _animator, bool _isBattleUnit)
    {
        isBattleUnit = _isBattleUnit;
        animator = _animator;
    }

    public void SetUnitSoundFX(UnitSoundFX _unitSoundFX)
    {
        unitSoundFX = _unitSoundFX;
    }

    public void SetStartingTransforms()
    {
        startStoppingDistance = navMeshAgent.stoppingDistance;
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }

    private void Update()
    {
        if(animator != null)
        {
            UpdateAnimator(isBattleUnit);
        }
    }

    public void MoveTo(Vector3 destination)
    {
        if (!isMover) return;
        navMeshAgent.SetDestination(destination);
    }

    public IEnumerator JumpToPos(Vector3 jumpPosition, Quaternion _startingRotation, bool isForward)
    {
        string animTrigger = "";
        bool hasCreatedJumpSound = false;

        if (isForward)
        {
            animTrigger = "isAdvancing";
        }
        else
        {
            animTrigger = "isJumping";
        }

        navMeshAgent.updateRotation = false;
        UpdateStoppingDistance(true);
        animator.SetBool(animTrigger, true);

        if (!hasCreatedJumpSound)
        {
            hasCreatedJumpSound = true;
            unitSoundFX.CreateSoundFX(unitSoundFX.GetJumpSound());
        }

        MoveTo(jumpPosition);

        yield return new WaitForSeconds(1f);

       
        animator.SetBool(animTrigger, false);

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

    public IEnumerator ReturnToStart(bool isForward)
    {
        yield return JumpToPos(startPosition, startRotation, isForward);
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

    private void UpdateAnimator(bool isBattleUnit)
    {
        Vector3 velocity = navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        float forwardSpeed = localVelocity.z;
        if (isBattleUnit)
        {
            if (forwardSpeed > .08f)
            {
                animator.SetBool("isAdvancing", true);
            }
            else
            {
                animator.SetBool("isAdvancing", false);
            }
        }
        else
        {
            //animator.SetFloat("forwardSpeed", forwardSpeed);
        }
    }

    public void FootR()
    {
        unitSoundFX.CreateSoundFX(unitSoundFX.GetFootStepSound());
    }

    public void FootL()
    {
        unitSoundFX.CreateSoundFX(unitSoundFX.GetFootStepSound());
    }

    public void StartJump()
    {
        unitSoundFX.CreateSoundFX(unitSoundFX.GetJumpSound());
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
