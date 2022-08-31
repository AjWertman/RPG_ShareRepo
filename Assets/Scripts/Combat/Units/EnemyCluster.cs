using RPGProject.Combat.Grid;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// A scriptable object containing a group of enemy units and their starting position.
    /// </summary>
    [CreateAssetMenu(fileName = " New EnemyCluster", menuName = "Character/Create New Enemy Cluster", order = 1)]
    public class EnemyCluster : ScriptableObject
    {
        public UnitStartingPosition[] enemies;
    }
}