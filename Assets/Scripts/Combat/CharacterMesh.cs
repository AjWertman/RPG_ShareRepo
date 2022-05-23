using UnityEngine;

public class CharacterMesh : MonoBehaviour
{
    [SerializeField] CharacterMeshKey characterMeshKey = CharacterMeshKey.Aj;
    [SerializeField] Sprite characterFaceImage = null;
    //SpellContainers[] containers = null;

    public CharacterMeshKey GetCharacterMeshKey()
    {
        return characterMeshKey;
    }
}
