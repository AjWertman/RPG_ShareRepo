using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class DialogueNode : ScriptableObject
{
    [SerializeField] bool isPlayerNode = false;
    [SerializeField] string overrideConversantName = "";
    [TextArea(10, 10)][SerializeField] string text;
    [SerializeField] List<string> children = new List<string>();
    [SerializeField] Rect rect = new Rect(0,0,200,100);

    [SerializeField] string onEnterAction = "";
    [SerializeField] string onExitAction = "";

    [SerializeField] Condition condition;

    public bool IsPlayerSpeaking()
    {
        return isPlayerNode;
    }

    public string GetOverrideName()
    {
        return overrideConversantName;
    }

    public string GetNodeText()
    {
        return text;
    }

    public List<string> GetNodeChildren()
    {
        return children;
    }

    public Rect GetNodeRect()
    {
        return rect;
    }

    public string GetOnEnterAction()
    {
        return onEnterAction;
    }

    public string GetOnExitAction()
    {
        return onExitAction;
    }

    public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
    {
        return condition.Check(evaluators);
    }

#if UNITY_EDITOR

    public void SetPlayerSpeaking(bool isPlayer)
    {
        Undo.RecordObject(this, "Change Dialogue Speaker");
        isPlayerNode = isPlayer;
        EditorUtility.SetDirty(this);
    }

    public void SetNodeText(string newText)
    {
        if (newText != text)
        {
            Undo.RecordObject(this, "Node Text Change");
            text = newText;
            EditorUtility.SetDirty(this);
        }
    }

    public void AddNodeChild(string childID)
    {
        Undo.RecordObject(this, "Add Dialogue Link");
        children.Add(childID);
        EditorUtility.SetDirty(this);
    }

    public void RemoveNodeChild(string childID)
    {
        Undo.RecordObject(this, "Remove Dialogue Link");
        children.Remove(childID);
        EditorUtility.SetDirty(this);
    }

    public void SetNodePosition(Vector2 newPostion)
    {
        Undo.RecordObject(this, "Move Dialogue Node");
        rect.position = newPostion;
        EditorUtility.SetDirty(this);
    }
#endif
}
