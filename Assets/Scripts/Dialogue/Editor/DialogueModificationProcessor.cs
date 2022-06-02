using System.IO;
using UnityEditor;

namespace RPGProject.Dialogue
{
    public class DialogueModificationProcessor : AssetModificationProcessor
    {
        private static AssetMoveResult OnWillMoveAsset(string _sourcePath, string _destinationPath)
        {
            DialogueScripObj dialogue = AssetDatabase.LoadMainAssetAtPath(_sourcePath) as DialogueScripObj;

            if (dialogue == null)
            {
                return AssetMoveResult.DidNotMove;
            }
            if (Path.GetDirectoryName(_sourcePath) != Path.GetDirectoryName(_destinationPath))
            {
                return AssetMoveResult.DidNotMove;
            }

            dialogue.name = Path.GetFileNameWithoutExtension(_destinationPath);

            return AssetMoveResult.DidNotMove;
        }
    }
}

