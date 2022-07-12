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

    //////////////////////////////////////////////////////////////
    //[SerializeField] int currentRow = 0;
    //[SerializeField] int currentColumn = 0;

    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();

    //    GridSystem gridSystem = (GridSystem)target;

    //    AddRowPiece(gridSystem);
    //    AddColumnPiece(gridSystem);

    //    if (GUILayout.Button("Delete Grid"))
    //    {
    //        gridSystem.DeleteGrid();
    //    }
    //}

    //private void AddRowPiece(GridSystem _gridSystem)
    //{
    //    GUILayout.BeginHorizontal();

    //    currentColumn = EditorGUILayout.IntField("Column", currentColumn);
    //    if (GUILayout.Button("Add Row Piece"))
    //    {
    //        _gridSystem.AddRowPiece(currentColumn);
    //    }

    //    GUILayout.EndHorizontal();
    //}

    //private void AddColumnPiece(GridSystem _gridSystem)
    //{
    //    GUILayout.BeginHorizontal();

    //    currentRow = EditorGUILayout.IntField("Row", 0);
    //    if (GUILayout.Button("Add Column Piece"))
    //    {
    //        _gridSystem.AddColumnPiece(currentRow);
    //    }

    //    GUILayout.EndHorizontal();
    //}
}
