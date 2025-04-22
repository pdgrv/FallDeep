using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Interaction {
    public class FreeGridInteractable : MonoBehaviour, IInteractable {
        public GridObject GridObject;

        public void CreateInteraction(Vector2Int direction) {
            GridObject targetObject = FindTargetObject();
            if (targetObject != null) {
                var targetMoved = targetObject.TryMoveInDirection(direction, new GridObject.MoveActionParams()
                {
                    moveSpeed = 5f,
                    rotationSpeed = 0f,
                    ignoredCells = GridObject.GetOccupiedCoords().ToList()
                });

                if (targetMoved) {
                    GridObject.TryMoveInDirection(direction, new GridObject.MoveActionParams()
                    {
                        moveSpeed = 5f,
                        rotationSpeed = 0f,
                        ignoredCells = targetObject.GetOccupiedCoords().ToList()
                    });
                }
            }
        }

        private GridObject FindTargetObject() {
            var targetPos = GridObject.gridPosition + GridObject.gridRotation;
            var targetCell = GridObject.AttachedField.GetCell(targetPos);
            return targetCell?.Occupant;
        }
    }
}