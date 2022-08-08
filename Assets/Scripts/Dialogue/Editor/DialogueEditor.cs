using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPGProject.Dialogue
{
    public class DialogueEditor : EditorWindow
    {
        [NonSerialized] GUIStyle nodeStyle = null;
        [NonSerialized] GUIStyle playerNodeStyle = null;

        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset = new Vector2();

        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;

        [NonSerialized] bool isDraggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset = new Vector2();

        DialogueScripObj selectedDialogue = null;
        Vector2 scrollPosition = new Vector2();

        const float canvasSize = 4000f;
        const float backgroundSize = 50f;

        [MenuItem("Window/DialogueEditor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "DialogueEditor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int _instanceID, int _lineNumber)
        {
            DialogueScripObj dialogue = EditorUtility.InstanceIDToObject(_instanceID) as DialogueScripObj;

            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnSelectionChange()
        {
            DialogueScripObj newDialogue = Selection.activeObject as DialogueScripObj;

            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue != null)
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvasRect = GUILayoutUtility.GetRect(canvasSize, canvasSize);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);

                GUI.DrawTextureWithTexCoords(canvasRect, backgroundTexture, texCoords);

                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllDialogueNodes())
                {
                    DrawConnections(dialogueNode);
                }
                foreach (DialogueNode dialogueNode in selectedDialogue.GetAllDialogueNodes())
                {
                    DrawNode(dialogueNode);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);

                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.rect.position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    isDraggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetNodePosition(Event.current.mousePosition + draggingOffset);

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && isDraggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && isDraggingCanvas)
            {
                isDraggingCanvas = false;
            }
        }

        private void DrawConnections(DialogueNode _dialogueNode)
        {
            Vector3 startPos = new Vector2(_dialogueNode.rect.xMax, _dialogueNode.rect.center.y);

            foreach (DialogueNode childNode in selectedDialogue.GetAllChildrenNodes(_dialogueNode))
            {
                Vector3 endPos = new Vector2(childNode.rect.xMin, childNode.rect.center.y);
                Vector3 controlPointOffset = endPos - startPos;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier
                    (
                    startPos, endPos,
                    startPos + controlPointOffset, endPos - controlPointOffset,
                    Color.white, null, 4f
                    );
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 _mousePosition)
        {
            DialogueNode foundNode = null;

            foreach (DialogueNode node in selectedDialogue.GetAllDialogueNodes())
            {
                if (node.rect.Contains(_mousePosition))
                {
                    foundNode = node;
                }
            }

            return foundNode;
        }

        private void DrawNode(DialogueNode _dialogueNode)
        {
            GUIStyle style = nodeStyle;
            if (_dialogueNode.isPlayerNode)
            {
                style = playerNodeStyle;
            }

            GUILayout.BeginArea(new Rect(_dialogueNode.rect), style);

            _dialogueNode.SetNodeText(EditorGUILayout.TextField(_dialogueNode.nodeText));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                deletingNode = _dialogueNode;
            }

            if (GUILayout.Button("+"))
            {
                creatingNode = _dialogueNode;
            }
            GUILayout.EndHorizontal();

            DrawLinkButtons(_dialogueNode);

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode _dialogueNode)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = _dialogueNode;
                }
            }
            else if (linkingParentNode == _dialogueNode)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.children.Contains(_dialogueNode.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    linkingParentNode.RemoveNodeChild(_dialogueNode.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddNodeChild((_dialogueNode.name));
                    linkingParentNode = null;
                }
            }
        }
    }
}