using System.Collections.Generic;
using UnityEngine;

public class CharacterMeshPool : MonoBehaviour
{
    [SerializeField] GameObject[] playerMeshPrefabs = null;
    [SerializeField] GameObject[] uniqueEnemyMeshPrefabs = null;

    [Range(1, 4)] [SerializeField] int amountOfGenericsToCreate = 4;
    [SerializeField] GameObject[] genericEnemyMeshPrefabs = null;

    Dictionary<CharacterMeshKey, GameObject> uniqueMeshes = new Dictionary<CharacterMeshKey, GameObject>();
    Dictionary<CharacterMeshKey, List<GameObject>> genericMeshes = new Dictionary<CharacterMeshKey, List<GameObject>>();

    private void Awake()
    {
        CreateCharacterMeshPool();
    }

    private void CreateCharacterMeshPool()
    {
        CreateUniqueMeshes();
        CreateGenericEnemyMeshes();
    }

    private void CreateUniqueMeshes()
    {
        CreatePlayerMeshes();
        CreateUniqueEnemyMesh();
    }

    private void CreatePlayerMeshes()
    {
        foreach (GameObject playerMesh in playerMeshPrefabs)
        {
            GameObject playerMeshInstance = Instantiate(playerMesh, transform);
            CharacterMesh characterMesh = playerMeshInstance.GetComponent<CharacterMesh>();
            CharacterMeshKey characterMeshKey = characterMesh.GetCharacterMeshKey();

            uniqueMeshes.Add(characterMeshKey, playerMeshInstance);

            playerMeshInstance.SetActive(false);
        }
    }

    private void CreateUniqueEnemyMesh()
    {
        foreach (GameObject uniqueEnemyMesh in uniqueEnemyMeshPrefabs)
        {
            GameObject enemyMeshInstance = Instantiate(uniqueEnemyMesh, transform);
            CharacterMesh characterMesh = enemyMeshInstance.GetComponent<CharacterMesh>();
            CharacterMeshKey characterMeshKey = characterMesh.GetCharacterMeshKey();

            uniqueMeshes.Add(characterMeshKey, enemyMeshInstance);

            enemyMeshInstance.SetActive(false);
        }
    }

    private void CreateGenericEnemyMeshes()
    {
        foreach(GameObject genericEnemyMesh in genericEnemyMeshPrefabs)
        {
            CharacterMeshKey meshKey = genericEnemyMesh.GetComponent<CharacterMesh>().GetCharacterMeshKey();
            List<GameObject> genericMeshList = new List<GameObject>();

            for (int i = 0; i < amountOfGenericsToCreate; i++)
            {
                GameObject enemyMeshInstance = Instantiate(genericEnemyMesh, transform);
                genericMeshList.Add(enemyMeshInstance);
                enemyMeshInstance.SetActive(false);
            }

            genericMeshes.Add(meshKey, genericMeshList);
        }
    }

    public GameObject GetMesh(CharacterMeshKey characterMeshKey) 
    {
        GameObject newMesh = null;

        if (IsUniqueMesh(characterMeshKey))
        {
            newMesh = uniqueMeshes[characterMeshKey];
        }
        else
        {
            foreach (GameObject genericMesh in genericMeshes[characterMeshKey])
            {
                if (genericMesh.activeSelf) continue;

                newMesh = genericMesh;
                break;
            }
        }

        return newMesh;
    }

    private bool IsUniqueMesh(CharacterMeshKey characterMeshKey)
    {
        return uniqueMeshes.ContainsKey(characterMeshKey);
    }
}
