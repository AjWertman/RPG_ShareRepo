using UnityEngine;
namespace RPGProject.Combat
{
    [CreateAssetMenu(fileName = " New EnemyCluster", menuName = "Character/Create New Enemy Cluster", order = 1)]
    public class EnemyCluster : ScriptableObject
    {
        [Header("Max 4 enemies")]
        [SerializeField] Unit[] enemyCluster = null;

        public Unit[] GetEnemies()
        {
            return enemyCluster;
        }
    }
}