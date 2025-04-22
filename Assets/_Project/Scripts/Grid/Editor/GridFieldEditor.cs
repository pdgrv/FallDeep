#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridField))]
public class GridFieldEditor : Editor
{
    private GridField field;
    private int selectedType;

    private void OnEnable()
    {
        field = (GridField)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Инспектор: стандартные поля
        DrawPropertiesExcluding(serializedObject, "cells");

        // Кисть
        var names = System.Enum.GetNames(typeof(GridCell.CellType));
        selectedType = EditorGUILayout.Popup("Brush", selectedType, names);

        EditorGUILayout.Space();
        DrawGrid();

        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("🔄 Refresh from GridObjects in scene"))
        {
            field.RefreshFromSceneObjects();
        }
    }

    private void DrawGrid()
    {
        var size = field.Size;

        if (size.x <= 0 || size.y <= 0 || field.Grid == null)
        {
            EditorGUILayout.HelpBox("Установи Grid и размер > 0", MessageType.Warning);
            return;
        }

        for (int y = size.y - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < size.x; x++)
            {
                var coord = new Vector2Int(x, y);
                var cell = field.GetCell(coord);
                if (cell == null)
                {
                    GUILayout.Label("X", GUILayout.Width(25));
                    continue;
                }

                GUI.backgroundColor = GetColor(cell.Type);
                if (GUILayout.Button(cell.Type.ToString()[0].ToString(), GUILayout.Width(25)))
                {
                    Undo.RecordObject(field, "Paint Cell (Inspector)");
                    cell.Type = (GridCell.CellType)selectedType;
                    EditorUtility.SetDirty(field);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (field == null || field.Grid == null) return;

        var size = field.Size;
        var grid = field.Grid;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var coord = new Vector2Int(x, y);
                var cell = field.GetCell(coord);
                if (cell == null) continue;

                Vector3 p0 = field.GetWorldPosition(coord) - GridObject.VisualOffset;
                Vector3 p1 = field.GetWorldPosition(coord + Vector2Int.right) - GridObject.VisualOffset;
                Vector3 p2 = field.GetWorldPosition(coord + Vector2Int.one) - GridObject.VisualOffset;
                Vector3 p3 = field.GetWorldPosition(coord + Vector2Int.up) - GridObject.VisualOffset;

                // Цвет
                Handles.color = GetColor(cell.Type) * new Color(1, 1, 1, 0.3f);
                Handles.DrawSolidRectangleWithOutline(new[] { p0, p1, p2, p3 }, Handles.color, Color.black);

                // Символ
                Vector3 center = (p0 + p2) * 0.5f;
                GUIStyle style = new GUIStyle(GUI.skin.label) {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    normal = { textColor = Color.white }
                };
                Handles.Label(center, cell.Type.ToString()[0].ToString(), style);

                // Обработка клика по клетке с Shift
                if (Event.current.type == EventType.MouseDown &&
                    Event.current.shift && Event.current.button == 0)
                {
                    Plane plane = new Plane(Vector3.up, 0f);
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (plane.Raycast(ray, out float enter))
                    {
                        Vector3 hit = ray.GetPoint(enter);
                        Vector2Int hitCoord = field.GetCellFromWorld(hit);
                        if (hitCoord == coord)
                        {
                            Undo.RecordObject(field, "Paint Cell (Scene)");
                            cell.Type = (GridCell.CellType)selectedType;
                            EditorUtility.SetDirty(field);
                            Event.current.Use();
                        }
                    }
                }
            }
        }
    }

    public static Color GetColor(GridCell.CellType type)
    {
        return type switch
        {
            GridCell.CellType.Blocked => Color.red,
            GridCell.CellType.Spawn => Color.green,
            GridCell.CellType.EnemySpawn => Color.yellow,
            GridCell.CellType.Movable => Color.blue,
            _ => Color.grey
        };
    }
}
#endif
