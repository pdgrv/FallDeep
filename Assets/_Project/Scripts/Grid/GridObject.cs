using System;
using UnityEngine;
using System.Collections.Generic;
using Scripts;

[ExecuteAlways]
public class GridObject : MonoBehaviour {
    [Header("Grid Settings")]
    public GridCell.CellType cellType = GridCell.CellType.Blocked;
    public bool IsRotatable = false;

    [Tooltip("Координаты привязки внутри GridField")]
    public Vector2Int gridPosition;
    public Vector2Int gridRotation;


    [SerializeField] private Transform _viewOrigin;
    [SerializeField] private ViewController _viewController;
    

    [Tooltip("Смещения клеток, которые занимает объект, относительно origin")]
    [SerializeField] private Vector2Int[] offsets = new Vector2Int[] { Vector2Int.zero };
    
    private Vector2Int _previousGridPosition;

    public IEnumerable<Vector2Int> GetOccupiedCoords()
    {
        foreach (var offset in offsets)
            yield return gridPosition + offset;
    }

    public GridField AttachedField => _attachedField ??= GetComponentInParent<GridField>();
    private GridField _attachedField = null;

    public void ChangeAttachedField(GridField newField) {
        newField.UpdateGrid();
        var spawnCell = newField.GetSpawnCell();

        if (spawnCell.Occupant != null && spawnCell.Occupant != RootEntry.Instance.Player) {
            spawnCell.Occupant.Delete();
        }
        
        _attachedField = newField;
        Vector2Int spawnCellCoords = spawnCell.Coord;
        transform.SetParent(newField.transform);
        gridPosition = spawnCellCoords;
        AutoAlignToGridOrigin();
        newField.UpdateGrid();
    }

    private void Delete() {
        Destroy(gameObject);
    }

    public void AutoAlignToGridOrigin()
    {
        var field = AttachedField;
        if (field == null || field.Grid == null) return;

        Vector3 worldPos = field.GetWorldPosition(gridPosition);
        transform.position = worldPos + VisualOffset;
        _previousGridPosition = gridPosition;
    }

    public static Vector3 VisualOffset = new Vector3(1f, 0.5f, 0.5f);
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var field = AttachedField;
        if (field == null) return;

        Gizmos.color = Color.red;
        foreach (var coord in GetOccupiedCoords())
        {
            Vector3 pos = field.GetWorldPosition(coord);
            Gizmos.DrawWireCube(pos + Vector3.one * 0.5f, Vector3.one);
        }
    }
#endif

    private void Awake()
    {
        if (Application.isPlaying) AutoAlignToGridOrigin();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        AutoAlignToGridOrigin();
    }
#endif

    private Vector3? _currentTarget;
    private Quaternion? _currentRotation;
    MoveActionParams _moveActionParams;

    private void Update() {
        var currentPos = transform.position;

        if (IsMoving()) {
            var target = _currentTarget.Value;
            if ((currentPos - target).sqrMagnitude >= 0.001f) {
                var step = Time.deltaTime * _moveActionParams?.moveSpeed ?? 0f;
                var newPos = Vector3.MoveTowards(currentPos, target, step);
                transform.position = newPos;
            } else {
                StopMoving();
            }
        }

        if (IsRotatable && _currentRotation.HasValue && _viewOrigin != null) {
            var currentRot = _viewOrigin.rotation;
            var target = _currentRotation.Value;
            
            if (Quaternion.Angle(currentRot, target) < 0.01f) {
                StopRotation();
                // gridRotation = Vector2Int. 
                return;
            }

            var step = Time.deltaTime * _moveActionParams?.rotationSpeed ?? 0f;
            _viewOrigin.rotation = Quaternion.RotateTowards(_viewOrigin.rotation, target, step);
        }
    }

    public bool TryMoveInDirection(MoveActionParams moveActionParams) {
        if (cellType is not GridCell.CellType.Movable and not GridCell.CellType.Spawn and not GridCell.CellType.EnemySpawn) {
            return false;
        }
        
        if (IsMoving()) {
            return false;
        }
        
        TryRotateObject(moveActionParams.direction, moveActionParams);

        if (!CanMove(moveActionParams.direction, moveActionParams.ignoredCells)) {
            return false;
        }

        var direction = moveActionParams.direction;
        var target = gridPosition + direction;

        _previousGridPosition = gridPosition;
        moveActionParams.startPosition = gridPosition;
        
        gridPosition = target;
        _moveActionParams = moveActionParams;
        _currentTarget = AttachedField.GetWorldPosition(target) + VisualOffset;
        if (_viewController) {
            _viewController.UpdateState(true, moveActionParams.withInteraction);
        }

        AttachedField.UpdateGrid();
        return true;
    }

    public bool CanMove(Vector2Int direction, List<Vector2Int> ignoredCells = null) {
        if (cellType is not GridCell.CellType.Movable and not GridCell.CellType.Spawn and not GridCell.CellType.EnemySpawn) {
            return false;
        }
        
        if (IsMoving()) {
            return false;
        }
        
        var target = gridPosition + direction;

        var targetList = new List<Vector2Int>()
        {
            target,
        };

        foreach (var occupiedCoords in GetOccupiedCoords()) {
            targetList.Add(occupiedCoords + direction);
        }
        
        foreach (var occupiedTargets in targetList) {
            if (CheckBlocked(occupiedTargets, ignoredCells)) {
                return false;
            }
        }

        return true;
    }

    private bool IsMoving() {
        return  _currentTarget != null && _currentTarget.HasValue;
    }

    private void TryRotateObject(Vector2Int direction, MoveActionParams moveActionParams) {
        if (!IsRotatable || moveActionParams.rotationSpeed == 0f) {
            return;
        }
        
        var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.y));
        _currentRotation = lookRotation;
        gridRotation = direction;
    }

    private bool CheckBlocked(Vector2Int target, List<Vector2Int> ignoredCells) {
        var field = AttachedField;
        if (field == null) return true;

        if (!field.IsInside(target)) {
            return true;
        }

        if (ignoredCells != null && ignoredCells.Contains(target)) {
            return false;
        }
        
        var cell = field.GetCell(target);

        if (cell == null) {
            Debug.LogWarning($"Cell is null on {target.ToString()}");
            return true;
        }

        if (cell.IsWalkable || cell.HasSameObject(this)) {
            return false;
        }

        return true;
    }

    private void StopMoving() {
        if (_viewController) {
            _viewController.UpdateState(false, false);
        }
        _currentTarget = null;
    }

    private void StopRotation() {
        _currentRotation = null;
    }

    public Vector2Int GetDirection() {
        return gridRotation;
    }

    public class MoveActionParams {
        public float moveSpeed;
        public float rotationSpeed;
        public List<Vector2Int> ignoredCells = new();
        public Vector2Int startPosition;
        public Vector2Int direction;
        public bool withInteraction;
    }
}