﻿using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

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

    Dialogue selectedDialogue = null;
    Vector2 scrollPosition = new Vector2();

    const float canvasSize = 4000f;
    const float backgroundSize = 50f;

    [MenuItem("Window/DialogueEditor")]
    public static void ShowEditorWindow()
    {
        GetWindow(typeof(DialogueEditor), false, "DialogueEditor");
    }

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int lineNumber)
    {
        Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
        
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
        Dialogue newDialogue =Selection.activeObject as Dialogue;

        if(newDialogue != null)
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
            Rect texCoords = new Rect(0,0,canvasSize/ backgroundSize,canvasSize/backgroundSize);

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

            if(deletingNode != null)
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

            if(draggingNode != null)
            {
                draggingOffset = draggingNode.GetNodeRect().position - Event.current.mousePosition;
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
            draggingNode.SetNodePosition(Event.current.mousePosition  + draggingOffset);

            GUI.changed = true;
        }
        else if (Event.current.type == EventType.MouseDrag && isDraggingCanvas)
        {
            scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

            GUI.changed = true;
        }
        else if(Event.current.type == EventType.MouseUp && draggingNode != null)
        {
            draggingNode = null;
        }
        else if(Event.current.type == EventType.MouseUp && isDraggingCanvas)
        {
            isDraggingCanvas = false;
        }
    }

    private void DrawConnections(DialogueNode dialogueNode)
    {
        Vector3 startPos = new Vector2(dialogueNode.GetNodeRect().xMax, dialogueNode.GetNodeRect().center.y);

        foreach (DialogueNode childNode in selectedDialogue.GetAllChildrenNodes(dialogueNode))
        {      
            Vector3 endPos = new Vector2(childNode.GetNodeRect().xMin, childNode.GetNodeRect().center.y);
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

    private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
    {
        DialogueNode foundNode = null;

        foreach(DialogueNode node in selectedDialogue.GetAllDialogueNodes())
        {
            if (node.GetNodeRect().Contains(mousePosition))
            {
                foundNode = node;
            }
        }

        return foundNode;
    }

    private void DrawNode(DialogueNode dialogueNode)
    {
        GUIStyle style = nodeStyle;
        if (dialogueNode.IsPlayerSpeaking())
        {
            style = playerNodeStyle;
        }

        GUILayout.BeginArea(new Rect(dialogueNode.GetNodeRect()), style);

        dialogueNode.SetNodeText(EditorGUILayout.TextField(dialogueNode.GetNodeText()));

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("x"))
        {
            deletingNode = dialogueNode;
        }

        if (GUILayout.Button("+"))
        {
            creatingNode = dialogueNode;
        }
        GUILayout.EndHorizontal();

        DrawLinkButtons(dialogueNode);

        GUILayout.EndArea();
    }

    private void DrawLinkButtons(DialogueNode dialogueNode)
    {
        if (linkingParentNode == null)
        {
            if (GUILayout.Button("Link"))
            {
                linkingParentNode = dialogueNode;
            }
        }
        else if(linkingParentNode == dialogueNode)
        {
            if (GUILayout.Button("Cancel"))
            {
                linkingParentNode = null;
            }
        }
        else if (linkingParentNode.GetNodeChildren().Contains(dialogueNode.name))
        {
            if (GUILayout.Button("Unlink"))
            {        
                linkingParentNode.RemoveNodeChild(dialogueNode.name);
                linkingParentNode = null;
            }
        }
        else
        {
            if (GUILayout.Button("Child"))
            {
                linkingParentNode.AddNodeChild((dialogueNode.name));
                linkingParentNode = null;
            }
        }
    }
}
