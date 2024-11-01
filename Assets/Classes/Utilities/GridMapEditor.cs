using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMap))]
public class GridMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector UI

        GridMap gridMap = (GridMap)target;

        // Check if the tileDictionary has been initialized and contains elements
        if (gridMap.tileDictionary != null && gridMap.tileDictionary.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("Tile Dictionary", EditorStyles.boldLabel);

            // Display each tile's coordinate and reference
            foreach (var entry in gridMap.tileDictionary)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Coordinate: {entry.Key}", GUILayout.Width(150));
                GUILayout.Label($"Tile: {entry.Value.name}", GUILayout.Width(150));
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Tile Dictionary is empty or not initialized.", MessageType.Warning);
        }
    }
}
