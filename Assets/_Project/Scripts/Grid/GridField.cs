using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class GridField : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Vector2Int gridSize = new(10, 10);
    [SerializeField] private List<GridCell> cells = new();

    public Vector2Int Size => gridSize;
    public Grid Grid => grid;

    public GridCell GetCell(Vector2Int coord)
    {
        if (!IsInside(coord)) return null;
        return cells[coord.y * gridSize.x + coord.x];
    }

    public bool IsInside(Vector2Int coord)
    {
        return coord.x >= 0 && coord.y >= 0 && coord.x < gridSize.x && coord.y < gridSize.y;
    }

    public Vector2Int GetCellFromWorld(Vector3 worldPos)
    {
        var cell = grid.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }

    public Vector3 GetWorldPosition(Vector2Int coord)
    {
        return grid.CellToWorld((Vector3Int)coord); //сделать тут оффсет
    }
    
#if UNITY_EDITOR
    public void RefreshFromSceneObjects()
    {
        Undo.RecordObject(this, "Refresh GridField");

        foreach (var cell in cells) {
            cell.Type = GridCell.CellType.Empty;
            cell.Occupant = null;
        }
        
        var gridObjects = GetComponentsInChildren<GridObject>(true);
        foreach (var obj in gridObjects)
        {
            foreach (var coord in obj.GetOccupiedCoords())
            {
                var cell = GetCell(coord);
                if (cell != null) {
                    cell.Type = obj.cellType;
                    cell.Occupant = obj;
                }
            }
        }

        EditorUtility.SetDirty(this);
    }

    private void OnValidate()
    {
        int total = gridSize.x * gridSize.y;
        if (cells.Count != total)
        {
            cells.Clear();
            for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++) {
                var cell = new GridCell(new Vector2Int(x, y));
                cells.Add(cell);
            }
        }
    }
#endif
}