using RPGProject.Combat;
using RPGProject.GameResources;
using RPGProject.Movement;
using UnityEngine;

public class AnimationEventCaller : MonoBehaviour
{
    //Refactor - will this be necessary outside of the Prototype models;

    Fighter fighter = null;
    CombatMover mover = null;
    Health health = null;

    public void InitalizeAnimationCaller(GameObject _parent)
    {
        fighter = _parent.GetComponent<Fighter>();
        mover = _parent.GetComponent<CombatMover>();
        health = _parent.GetComponent<Health>();
    }

    void FootL()
    {
        mover.FootL();
    }

    void FootR()
    {
        mover.FootR();
    }

    void Hit()
    {
        fighter.Hit();
    }

    void Shoot()
    {
        fighter.Shoot();
    }

    void OnAnimDeath()
    {
        health.OnAnimDeath();
    }
}
