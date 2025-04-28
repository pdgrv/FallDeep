using System.Linq;
using UnityEngine;

namespace Scripts.Interaction {
    public class FreeGridInteractable : MonoBehaviour, IInteractable {
        public GridObject GridObject;
        [SerializeField] private float _interactionMoveSpeed = 2f;
        
        public void CreateInteraction(Vector2Int direction) {
            GridObject targetObject = FindTargetObject();
            if (targetObject != null) {
                bool canMoveSelf = GridObject.CanMove(direction, targetObject.GetOccupiedCoords().ToList());

                if (canMoveSelf) {
                    var targetMoved = targetObject.TryMoveInDirection(new GridObject.MoveActionParams()
                    {
                        direction =  direction,
                        moveSpeed = _interactionMoveSpeed,
                        rotationSpeed = 0f,
                        ignoredCells = GridObject.GetOccupiedCoords().ToList()
                    });
                    
                    if (targetMoved) {
                        GridObject.TryMoveInDirection(new GridObject.MoveActionParams()
                        {
                            direction = direction,
                            moveSpeed = _interactionMoveSpeed,
                            rotationSpeed = 0f,
                            ignoredCells = targetObject.GetOccupiedCoords().ToList(),
                            withInteraction = true,
                        });
                    }
                }
            }
        }

        public bool CheckInteractableObject() => FindTargetObject() != null;
        public void CreateInteractionStart(bool isInteracting) {
            if (CheckInteractableObject()) {
                GridObject.UpdateInteraction(isInteracting);
            }
        }

        private GridObject FindTargetObject() {
            var targetPos = GridObject.gridPosition + GridObject.gridRotation;
            var targetCell = GridObject.AttachedField.GetCell(targetPos);
            return targetCell?.Occupant;
        }
    }
}