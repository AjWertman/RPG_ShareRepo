using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    //Creates the character meshes that will be used for combat
    public class CharacterMeshPool : MonoBehaviour
    {
        [SerializeField] GameObject[] playerMeshPrefabs = null;

        [Range(1, 4)] [SerializeField] int amountOfGenericsToCreate = 4;
        [SerializeField] GameObject[] genericEnemyMeshPrefabs = null;

        Dictionary<CharacterMeshKey, CharacterMesh> uniqueMeshes = new Dictionary<CharacterMeshKey, CharacterMesh>();

        //Refactor - each zone has "Zone enemies"
        [SerializeField] GameObject[] uniqueEnemyMeshPrefabs = null;
        Dictionary<CharacterMeshKey, List<CharacterMesh>> genericMeshes = new Dictionary<CharacterMeshKey, List<CharacterMesh>>();

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

                uniqueMeshes.Add(characterMeshKey, characterMesh);

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

                uniqueMeshes.Add(characterMeshKey, characterMesh);

                enemyMeshInstance.SetActive(false);
            }
        }

        private void CreateGenericEnemyMeshes()
        {
            foreach (GameObject genericEnemyMesh in genericEnemyMeshPrefabs)
            {
                CharacterMeshKey meshKey = genericEnemyMesh.GetComponent<CharacterMesh>().GetCharacterMeshKey();
                List<CharacterMesh> genericMeshList = new List<CharacterMesh>();

                for (int i = 0; i < amountOfGenericsToCreate; i++)
                {
                    GameObject enemyMeshInstance = Instantiate(genericEnemyMesh, transform);
                    CharacterMesh characterMesh = enemyMeshInstance.GetComponent<CharacterMesh>();
                    genericMeshList.Add(characterMesh);
                    enemyMeshInstance.SetActive(false);
                }

                genericMeshes.Add(meshKey, genericMeshList);
            }
        }

        public CharacterMesh GetMesh(CharacterMeshKey _characterMeshKey)
        {
            CharacterMesh newMesh = null;

            if (IsUniqueMesh(_characterMeshKey))
            {
                newMesh = uniqueMeshes[_characterMeshKey];
            }
            else
            {
                foreach (CharacterMesh genericMesh in genericMeshes[_characterMeshKey])
                {
                    if (genericMesh.gameObject.activeSelf) continue;

                    newMesh = genericMesh;
                    break;
                }
            }

            return newMesh;
        }

        public void ResetCharacterMeshPool()
        {
            foreach (CharacterMesh mesh in GetAllMeshes())
            {
                mesh.transform.parent = transform;
                mesh.transform.localPosition = Vector3.zero;
                mesh.gameObject.SetActive(false);
            }
        }

        private IEnumerable<CharacterMesh> GetAllMeshes()
        {
            foreach (CharacterMesh mesh in uniqueMeshes.Values)
            {
                yield return mesh;
            }
            foreach (List<CharacterMesh> meshList in genericMeshes.Values)
            {
                foreach (CharacterMesh mesh in meshList)
                {
                    yield return mesh;
                }
            }
        }

        private bool IsUniqueMesh(CharacterMeshKey _characterMeshKey)
        {
            return uniqueMeshes.ContainsKey(_characterMeshKey);
        }
    }
}
