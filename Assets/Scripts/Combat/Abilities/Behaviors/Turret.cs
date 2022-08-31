using RPGProject.Combat.Grid;
using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// The behavior of a turret placeable on a grid. Will attack any combatants that venture into its attack range.
    /// It also gets a turn of its own and will attack a random target.
    /// </summary>
    public class Turret : AbilityBehavior, CombatTarget
    {
        [SerializeField] Fighter testTarget = null;
        //Had to place the TurretGun into an empty parent object to have better look at functionality
        [SerializeField] Transform gunTransform = null;
        [SerializeField] Transform bulletLaunchTransform = null;
        [SerializeField] Transform lookTransform = null;

        public AbilityObjectKey projectileKey = AbilityObjectKey.Missile;

        Fighter fighter = null;
        Projectile bulletProjectile = null;
        public GridBlock myBlock = null;

        public List<GridBlock> attackRadius = new List<GridBlock>();
        public List<Fighter> fightersInRange = new List<Fighter>();

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
        }

        /// <summary>
        /// Shoots a target with a projectile.
        /// If it is this turrets turn, it will shoot a random target.
        /// If it is not this turrets turn, a combatant must have come in range and will be shot.
        /// </summary>
        public void Shoot(Fighter _target, bool _isTurn)
        {
            //Refactor - range obstructions?

            if (!_isTurn)
            {
                if (fightersInRange.Contains(_target)) return;
                fightersInRange.Add(_target);
            }
            else
            {
                target = GetRandomTarget();
            }

            if (_target == null) return;

            LookAtTarget(_target.transform);
            bulletProjectile.target = _target;
            bulletProjectile.PerformAbilityBehavior();
        }

        /// <summary>
        /// Called when a combatant enters the turrets range.
        /// </summary>
        public void ShootNewContester(Fighter _target, GridBlock _targetBlock)
        {
            //Refactor - only works if the current combatant moves to _targetBlock, not if they simply move over it.
            if (_target == null) return;
            Shoot(_target, false);
        }

        /// <summary>
        /// Gets a random target based on all of the fighters currently in range of the turret.
        /// </summary>
        private Fighter GetRandomTarget()
        {
            int fightersInRangeCount = fightersInRange.Count;

            if (fightersInRangeCount <= 0) return null;

            return fightersInRange[RandomGenerator.GetRandomNumber(0, fightersInRangeCount - 1)];
        }

        /// <summary>
        /// Rotates the gun of the turret to look at a target.
        /// </summary>
        public void LookAtTarget(Transform _target)
        {
            Vector3 targetPosition = _target.position;
            Vector3 lookPosition = new Vector3(targetPosition.x, gunTransform.position.y, targetPosition.z);

            gunTransform.LookAt(lookPosition);
        }

        /// <summary>
        /// Sets up the bullet projectile of this turret.
        /// </summary>
        public void SetProjectile(AbilityBehavior _newProjectile)
        {
            _newProjectile.SetupAbility(fighter, null, changeAmount, false, abilityLifetime);
            bulletProjectile = _newProjectile as Projectile;
            bulletProjectile.transform.parent = bulletLaunchTransform;
            bulletProjectile.onAbilityDeath += ResetProjectile;
            ResetProjectile(_newProjectile);
        }
        
        /// <summary>
        /// Resets the bullet projectile of this turret.
        /// </summary>
        public void ResetProjectile(AbilityBehavior _projectile)
        {
            Projectile projectile = _projectile as Projectile;
            projectile.gameObject.SetActive(false);
            projectile.transform.localPosition = Vector3.zero;
            projectile.target = null;
        }

        /// <summary>
        /// Sets the attack radius (list of gridblocks) and subscribes to the fighter update event on each block.
        /// </summary>
        public void SetupAttackRadius(List<GridBlock> _attackRadius)
        {
            foreach (GridBlock gridBlock in _attackRadius)
            {
                if(gridBlock != null && !attackRadius.Contains(gridBlock))
                {
                    attackRadius.Add(gridBlock);
                    gridBlock.onContestedFighterUpdate += ShootNewContester;

                    Fighter contestedFighter = gridBlock.contestedFighter;

                    if (contestedFighter != null && !IsTeammate(contestedFighter)) fightersInRange.Add(contestedFighter);
                }
            }
        }

        public override void PerformAbilityBehavior()
        {
            myBlock = (GridBlock)target;
            myBlock.isMovable = false;
        }

        public override void OnAbilityDeath()
        {
            myBlock.isMovable = true;

            foreach (GridBlock gridBlock in attackRadius)
            {
                gridBlock.onContestedFighterUpdate -= ShootNewContester;
            }

            attackRadius.Clear();

            base.OnAbilityDeath();
        }

        public Transform GetAimTransform()
        {
            return transform;
        }

        public bool IsTeammate(Fighter _target)
        {
            if (caster == null || _target == null) return false;
            return caster.unitInfo.isPlayer == _target.unitInfo.isPlayer;
        }
    }
}