using UnityEngine;

public class CharacterMesh : MonoBehaviour
{
    [SerializeField] CharacterMeshKey characterMeshKey = CharacterMeshKey.Aj;

    public CharacterMeshKey GetCharacterMeshKey()
    {
        return characterMeshKey;
    }
}
