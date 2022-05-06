using UnityEngine;

public class AnimationEventCaller : MonoBehaviour
{
    [SerializeField] bool isBattleUnit = true;
    [SerializeField] bool isPlayer = false;

    Fighter fighter = null;
    Mover mover = null;
    Health health = null;

    PlayerController playerController = null;
    EnemyController enemyController = null;

    private void Awake()
    {
        if (isBattleUnit)
        {
            mover = GetComponentInParent<Mover>();
            fighter = GetComponentInParent<Fighter>();
            health = GetComponentInParent<Health>();
        }
        else
        {
            if (isPlayer)
            {
                playerController = GetComponentInParent<PlayerController>();
            }
            else
            {
                enemyController = GetComponentInParent<EnemyController>();
            }

        }
    }

    void Hit()
    {
        fighter.Hit();
    }

    void FootL()
    {
        if (isBattleUnit)
        {
            mover.FootL();
        }
        else
        {
            if (isPlayer)
            {
                playerController.FootL();
            }
            else
            {
                enemyController.FootL();
            }

        }

    }

    void FootR()
    {
        if (isBattleUnit)
        {
            mover.FootR();
        }
        else
        {
            if (isPlayer)
            {
                playerController.FootR();
            }
            else
            {
                enemyController.FootR();
            }
        }
    }

    void StartJump()
    {
        mover.StartJump();
    }

    void OnAnimDeath()
    {
        health.OnAnimDeath();
    }
}
