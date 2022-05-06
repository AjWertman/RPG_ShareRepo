using UnityEngine;

public class EnemyController : MonoBehaviour, ISaveable, IOverworld
{
    [SerializeField] GameObject owMeshObject = null;   
    [SerializeField] float chaseDistance = 10f;

    Animator animator = null;
    BattleZoneTrigger enemyTrigger = null;
    UnitSoundFX unitSoundFX = null;
    Mover mover = null;

    GameObject player = null;

    bool shouldBeDisabled = false;
    bool isBattling = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        mover = GetComponent<Mover>();
        enemyTrigger = GetComponentInChildren<BattleZoneTrigger>();
        unitSoundFX = GetComponent<UnitSoundFX>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mover.SetAnimator(animator, false);

        enemyTrigger.updateShouldBeDisabled += SetShouldBeDisabled;
    }

    private void Update()
    {
        if (shouldBeDisabled) return;
        if (isBattling) return;
        if(GetDistanceToPlayer() <= chaseDistance)
        {
            Chase();
        }
    }

    private void Chase()
    {
        mover.MoveTo(player.transform.position);
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    public void BattleStartBehavior()
    {
        isBattling = true;
        owMeshObject.SetActive(false);
    }

    public void BattleEndBehavior()
    {
        isBattling = false;
        if(!shouldBeDisabled)
        {
            owMeshObject.SetActive(true);
        }
        else
        {
            DisableEnemy();
        }
    }

    public void SetShouldBeDisabled(bool _shouldBeDisabled)
    {
        shouldBeDisabled = _shouldBeDisabled;
    }

    public void DisableEnemy()
    {
        owMeshObject.SetActive(false);
        enemyTrigger.gameObject.SetActive(false);
    }

    public void FootR()
    {
        CreateFootStepSound();
    }

    public void FootL()
    {
        CreateFootStepSound();
    }

    private void CreateFootStepSound()
    {
        unitSoundFX.CreateSoundFX(unitSoundFX.GetFootStepSound());
    }

    public object CaptureState()
    {
        return shouldBeDisabled;
    }

    public void RestoreState(object state)
    {
        SetShouldBeDisabled((bool)state);

        if (shouldBeDisabled)
        {
            DisableEnemy();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}
