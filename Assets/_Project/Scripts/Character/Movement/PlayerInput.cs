using System;
using Scripts.Interaction;
using UnityEngine;

namespace Scripts.Movement {
    public class PlayerInput : MonoBehaviour {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private FreeMovable movement;
        [SerializeField] private FreeGridInteractable playerInteractable;

        private IMovable _movable;
        private IInteractable _playerInteractable;

        private Plane groundPlane;

        private void Awake() {
            if (movement) {
                Initialize(movement, playerInteractable, movement.transform, mainCamera);
            }
        }

        public void Initialize(IMovable movable, IInteractable interactable, Transform root, Camera cam) {
            _movable = movable;
            _playerInteractable = interactable;
            mainCamera = cam;
            groundPlane = new Plane(Vector3.up, Vector3.zero);
        }

        private void Update() {
            bool isInteracting = Input.GetKey(KeyCode.Space);
            int vertical = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            int horizontal = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
            Vector2Int input = new Vector2Int(horizontal, vertical);
            
            if (isInteracting) {
                HandleInteraction(input);
            } else {
                HandleMovement(input);
            }
        }

        private void HandleInteraction(Vector2Int input) {
            if (input == Vector2Int.zero) {
                return;
            }
            
            _playerInteractable?.CreateInteraction(input);
        }

        private void HandleMovement(Vector2Int input) {
            if (input == Vector2Int.zero) {
                _movable.SetInput(Vector3.zero);
                _movable.SetInputGrid(Vector2Int.zero);
                return;
            }


            // Vector3 aimDirection = GetMouseDirectionFlat();

            _movable.SetInput(new Vector3(input.x, 0f, input.y).normalized);
            _movable.SetInputGrid(input);
        }

        // private Vector3 GetMouseDirectionFlat()
        // {
        //     Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //     if (groundPlane.Raycast(ray, out float enter))
        //     {
        //         Vector3 hit = ray.GetPoint(enter);
        //         Vector3 dir = (hit - _movable.GetPosition = position);
        //         dir.y = 0f;
        //         return dir.normalized;
        //     }
        //     return Vector3.zero;
        // }
    }
}