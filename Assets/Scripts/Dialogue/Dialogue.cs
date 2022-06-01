using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGProject._Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Create New Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] Vector2 newNodeOffset = new Vector2(250, 0);

        [SerializeField] List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        [SerializeField] bool isEssentialDialogue = false;

        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            nodeLookup.Clear();

            foreach (DialogueNode node in GetAllDialogueNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllDialogueNodes()
        {
            return dialogueNodes;
        }

        public bool IsEssentialDialogue()
        {
            return isEssentialDialogue;
        }

        public IEnumerable<DialogueNode> GetAllChildrenNodes(DialogueNode _parentNode)
        {
            foreach (string childID in _parentNode.GetNodeChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode _currentNode)
        {
            foreach (DialogueNode node in GetAllChildrenNodes(_currentNode))
            {
                if (node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode _currentNode)
        {
            foreach (DialogueNode node in GetAllChildrenNodes(_currentNode))
            {
                if (!node.IsPlayerSpeaking())
                {
                    yield return node;
                }
            }
        }

        public DialogueNode GetRootNode()
        {
            return dialogueNodes[0];
        }


#if UNITY_EDITOR
        public void CreateNode(DialogueNode _parentNode)
        {
            DialogueNode newNode = MakeNode(_parentNode);

            Undo.RegisterCreatedObjectUndo(newNode, "Created New Node");

            Undo.RecordObject(this, "Added New Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode _nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");

            dialogueNodes.Remove(_nodeToDelete);

            OnValidate();

            CleanDanglingChildren(_nodeToDelete);

            Undo.DestroyObjectImmediate(_nodeToDelete);
        }

        private DialogueNode MakeNode(DialogueNode _parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = System.Guid.NewGuid().ToString();

            if (_parentNode != null)
            {
                _parentNode.AddNodeChild(newNode.name);
                newNode.SetPlayerSpeaking(!_parentNode.IsPlayerSpeaking());
                newNode.SetNodePosition(_parentNode.GetNodeRect().position + newNodeOffset);
            }

            return newNode;
        }

        private void AddNode(DialogueNode _newNode)
        {
            dialogueNodes.Add(_newNode);

            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode _nodeToDelete)
        {
            foreach (DialogueNode dialogueNode in GetAllDialogueNodes())
            {
                dialogueNode.RemoveNodeChild(_nodeToDelete.name);
            }
        }
#endif

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (dialogueNodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllDialogueNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {

        }
    }
}