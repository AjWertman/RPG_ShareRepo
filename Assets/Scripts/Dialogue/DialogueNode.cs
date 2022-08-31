using RPGProject.Questing;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGProject.Dialogue
{
    /// <summary>
    /// Visualization of dialogue in the editor.
    /// </summary>
    [Serializable]
    public class DialogueNode : ScriptableObject
    {
        public bool isPlayerNode = false;
        public string overrideConversantName = "";
        [TextArea(10, 10)] public string nodeText;
        public List<string> children = new List<string>();
        public Rect rect = new Rect(0, 0, 200, 100);

        public string onEnterAction = "";
        public string onExitAction = "";

        public Condition condition;

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }

#if UNITY_EDITOR

        public void SetPlayerSpeaking(bool _isPlayer)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerNode = _isPlayer;
            EditorUtility.SetDirty(this);
        }

        public void SetNodeText(string _newText)
        {
            if (_newText != nodeText)
            {
                Undo.RecordObject(this, "Node Text Change");
                nodeText = _newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddNodeChild(string _childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(_childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveNodeChild(string _childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(_childID);
            EditorUtility.SetDirty(this);
        }

        public void SetNodePosition(Vector2 _newPostion)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = _newPostion;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}