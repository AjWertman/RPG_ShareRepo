using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        foreach(DialogueNode node in GetAllDialogueNodes())
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

    public IEnumerable<DialogueNode> GetAllChildrenNodes(DialogueNode parentNode)
    {
        foreach(string childID in parentNode.GetNodeChildren())
        {
            if (nodeLookup.ContainsKey(childID))
            {
                yield return nodeLookup[childID];
            }
        }
    }

    public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
    {
        foreach(DialogueNode node in GetAllChildrenNodes(currentNode))
        {
            if (node.IsPlayerSpeaking())
            {
                yield return node;
            }
        }
    }

    public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
    {
        foreach (DialogueNode node in GetAllChildrenNodes(currentNode))
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
    public void CreateNode(DialogueNode parentNode)
    {
        DialogueNode newNode = MakeNode(parentNode);

        Undo.RegisterCreatedObjectUndo(newNode, "Created New Node");

        Undo.RecordObject(this, "Added New Node");
        AddNode(newNode);
    }

    public void DeleteNode(DialogueNode nodeToDelete)
    {
        Undo.RecordObject(this, "Deleted Dialogue Node");

        dialogueNodes.Remove(nodeToDelete);

        OnValidate();

        CleanDanglingChildren(nodeToDelete);

        Undo.DestroyObjectImmediate(nodeToDelete);
    }

    private DialogueNode MakeNode(DialogueNode parentNode)
    {
        DialogueNode newNode = CreateInstance<DialogueNode>();
        newNode.name = System.Guid.NewGuid().ToString();

        if (parentNode != null)
        {
            parentNode.AddNodeChild(newNode.name);
            newNode.SetPlayerSpeaking(!parentNode.IsPlayerSpeaking());
            newNode.SetNodePosition(parentNode.GetNodeRect().position + newNodeOffset);
        }

        return newNode;
    }

    private void AddNode(DialogueNode newNode)
    {
        dialogueNodes.Add(newNode);

        OnValidate();
    }

    private void CleanDanglingChildren(DialogueNode nodeToDelete)
    {
        foreach (DialogueNode dialogueNode in GetAllDialogueNodes())
        {
            dialogueNode.RemoveNodeChild(nodeToDelete.name);
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
            foreach(DialogueNode node in GetAllDialogueNodes())
            {
                if(AssetDatabase.GetAssetPath(node) == "")
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
