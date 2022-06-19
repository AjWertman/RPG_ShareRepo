using RPGProject.Core;
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

        Dictionary<CharacterKey, CharacterMesh> uniqueMeshes = new Dictionary<CharacterKey, CharacterMesh>();

        //Refactor - each zone has "Zone enemies" - trigger or on awake in scene? Make non-persistant
        [SerializeField] GameObject[] uniqueEnemyMeshPrefabs = null;
        Dictionary<CharacterKey, List<CharacterMesh>> genericMeshes = new Dictionary<CharacterKey, List<CharacterMesh>>();

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
                CharacterKey characterKey = characterMesh.GetCharacterKey();

                uniqueMeshes.Add(characterKey, characterMesh);

                playerMeshInstance.SetActive(false);
            }
        }

        private void CreateUniqueEnemyMesh()
        {
            foreach (GameObject uniqueEnemyMesh in uniqueEnemyMeshPrefabs)
            {
                GameObject enemyMeshInstance = Instantiate(uniqueEnemyMesh, transform);
                CharacterMesh characterMesh = enemyMeshInstance.GetComponent<CharacterMesh>();
                CharacterKey characterKey = characterMesh.GetCharacterKey();

                uniqueMeshes.Add(characterKey, characterMesh);

                enemyMeshInstance.SetActive(false);
            }
        }

        private void CreateGenericEnemyMeshes()
        {
            foreach (GameObject genericEnemyMesh in genericEnemyMeshPrefabs)
            {
                CharacterKey characterKey = genericEnemyMesh.GetComponent<CharacterMesh>().GetCharacterKey();
                List<CharacterMesh> genericMeshList = new List<CharacterMesh>();

                for (int i = 0; i < amountOfGenericsToCreate; i++)
                {
                    GameObject enemyMeshInstance = Instantiate(genericEnemyMesh, transform);
                    CharacterMesh characterMesh = enemyMeshInstance.GetComponent<CharacterMesh>();
                    genericMeshList.Add(characterMesh);
                    enemyMeshInstance.SetActive(false);
                }

                genericMeshes.Add(characterKey, genericMeshList);
            }
        }

        public CharacterMesh GetMesh(CharacterKey _characterKey)
        {
            CharacterMesh newMesh = null;

            if (IsUniqueMesh(_characterKey))
            {
                newMesh = uniqueMeshes[_characterKey];
            }
            else
            {
                foreach (CharacterMesh genericMesh in genericMeshes[_characterKey])
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

        private bool IsUniqueMesh(CharacterKey _characterKey)
        {
            return uniqueMeshes.ContainsKey(_characterKey);
        }
    }
}
