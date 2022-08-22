using RPGProject.Combat.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class Turret : AbilityBehavior
    {
        [SerializeField] Fighter testTarget = null;
        //Had to place the TurretGun into an empty parent object to have better look at functionality
        [SerializeField] Transform gunTransform = null;
        [SerializeField] Transform bulletLaunchTransform = null;
        [SerializeField] Transform lookTransform = null;

        public AbilityObjectKey projectileKey = AbilityObjectKey.Missile;

        Fighter fighter = null;
        Projectile bulletProjectile = null;
        GridBlock myBlock = null;

        List<Fighter> enemyFighters = new List<Fighter>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Shoot(testTarget);
            }
        }

        public void Shoot(Fighter _target)
        {
            LookAtTarget(_target.transform);
            target = _target;
            bulletProjectile.PerformAbilityBehavior();
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

        public override void PerformAbilityBehavior()
        {
            GridBlock myBlock = (GridBlock)target;
            myBlock.isMovable = false;

            bool isCasterPlayer = caster.unitInfo.isPlayer;

            foreach (Fighter fighter in FindObjectsOfType<Fighter>())
            {
                if (isCasterPlayer != fighter.unitInfo.isPlayer)
                {
                    float distance = GetDistance(fighter.transform);
                    enemyFighters.Add(fighter);
                }
            }

            fighter = GetComponent<Fighter>();
        }

        public override void OnAbilityDeath()
        {
            myBlock.isMovable = true;

            base.OnAbilityDeath();
        }

        private float GetDistance(Transform _fighterTransform)
        {
            Vector3 fighterPosition = new Vector3(_fighterTransform.position.x, 0, _fighterTransform.position.z);
            Vector3 myPosition = new Vector3(transform.position.x, 0, transform.position.z);

            return Vector3.Distance(fighterPosition, myPosition);
        }
    }
}