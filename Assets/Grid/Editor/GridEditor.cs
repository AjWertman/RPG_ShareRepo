using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSystem))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridSystem gridSystem = (GridSystem)target;

        if (GUILayout.Button("Generate Grid"))
        {
            gridSystem.SetupGrid();
        }
    
        if (GUILayout.Button("Delete Grid"))
        {
            gridSystem.DeleteGrid();
        }
    }
}