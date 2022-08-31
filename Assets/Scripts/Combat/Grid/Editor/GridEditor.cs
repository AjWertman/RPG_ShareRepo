using UnityEngine;
using UnityEditor;

namespace RPGProject.Combat.Grid
{
    /// <summary>
    /// Allows for custom grid behavior from the editor
    /// </summary>
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
}