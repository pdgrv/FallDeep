using System;
using Scripts.Movement;
using UnityEngine;

public class FreeMovable : MonoBehaviour, IMovable {
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 30f;

    private GridObject GridObject => GetComponent<GridObject>();

    private Transform characterTransform;

    private void Start() {
        if (characterTransform == null) {
            characterTransform = transform;
        }
    }

    public void Initialize(Transform root) {
        characterTransform = root;
    }

    public void Tick() { }

    public void SetInput(Vector3 direction) {
        //for free movement
    }

    public void SetInputGrid(Vector2Int direction) {
        if (direction.x == 0 && direction.y == 0) {
            return;
        }
        
        var moveParams = new GridObject.MoveActionParams()
        {
            moveSpeed = _moveSpeed,
            rotationSpeed = _rotationSpeed,
            direction = direction
        };
        GridObject.TryMoveInDirection(moveParams);
    }

    public Vector3 GetDirection() {
        return GridObject.AttachedField.GetWorldPosition(GridObject.GetDirection());
    }
}