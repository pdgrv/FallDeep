using UnityEngine;

[System.Serializable]
public class GridCell
{
    public enum CellType { Empty, Movable, Spawn, EnemySpawn, Blocked }

    public Vector2Int Coord { get; }
    public CellType Type { get; set; }
    public GridObject Occupant { get; set; }

    public bool IsWalkable => Type == CellType.Empty;

    public GridCell(Vector2Int coord)
    {
        Coord = coord;
        Type = CellType.Empty;
    }

    public bool HasSameObject(GridObject gridObject) {
        if (Occupant == null) {
            return false;
        }

        return Occupant == gridObject;
    }
}
