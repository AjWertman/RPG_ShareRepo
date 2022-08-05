using RPGProject.Combat.Grid;
using UnityEngine;
namespace RPGProject.Combat
{
    [CreateAssetMenu(fileName = " New EnemyCluster", menuName = "Character/Create New Enemy Cluster", order = 1)]
    public class EnemyCluster : ScriptableObject
    {
        public UnitStartingPosition[] enemies;
    }
}