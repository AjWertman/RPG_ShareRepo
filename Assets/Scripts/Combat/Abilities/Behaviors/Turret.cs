using RPGProject.Combat.Grid;
using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
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

        public void Shoot(Fighter _target, bool _isTurn)
        {
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
            target = _target;
            bulletProjectile.target = target;
            bulletProjectile.PerformAbilityBehavior();
        }

        public void ShootNewContester(Fighter _target, GridBlock _targetBlock)
        {
            if (_target == null) return;
            Shoot(_target, false);
        }

        private Fighter GetRandomTarget()
        {
            int fightersInRangeCount = fightersInRange.Count;

            if (fightersInRangeCount <= 0) return null;

            return fightersInRange[RandomGenerator.GetRandomNumber(0, fightersInRangeCount - 1)];
        }

        public void LookAtTarget(Transform _target)
        {
            Vector3 targetPosition = _target.position;
            Vector3 lookPosition = new Vector3(targetPosition.x, gunTransform.position.y, targetPosition.z);

            gunTransform.LookAt(lookPosition);
        }

        public void SetProjectile(AbilityBehavior _newProjectile)
        {
            _newProjectile.SetupAbility(fighter, null, changeAmount, false, abilityLifetime);
            bulletProjectile = _newProjectile as Projectile;
            bulletProjectile.transform.parent = bulletLaunchTransform;
            bulletProjectile.onAbilityDeath += ResetProjectile;
            ResetProjectile(_newProjectile);
        }
        
        public void ResetProjectile(AbilityBehavior _projectile)
        {
            Projectile projectile = _projectile as Projectile;
            projectile.gameObject.SetActive(false);
            projectile.transform.localPosition = Vector3.zero;
            projectile.target = null;
        }

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

            foreach(GridBlock gridBlock in attackRadius)
            {
                gridBlock.onContestedFighterUpdate -= ShootNewContester;
            }

            attackRadius.Clear();

            base.OnAbilityDeath();
        }

        private float GetDistance(Transform _fighterTransform)
        {
            Vector3 fighterPosition = new Vector3(_fighterTransform.position.x, 0, _fighterTransform.position.z);
            Vector3 myPosition = new Vector3(transform.position.x, 0, transform.position.z);

            return Vector3.Distance(fighterPosition, myPosition);
        }

        public string Name()
        {
            return name;
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