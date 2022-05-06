using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCluster", menuName = "Unit/Create New Enemy Cluster", order = 1)]
public class EnemyCluster : ScriptableObject
{
    [Header("Max 4 enemies")]
    [SerializeField] Character[] enemyCluster = null;

    public Character[] GetEnemies()
    {
        return enemyCluster;
    }
}
