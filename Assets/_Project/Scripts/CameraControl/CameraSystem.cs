using UnityEngine;

namespace Scripts.CameraControl {
    public class CameraSystem {
        private readonly Camera _mainCamera;

        public CameraSystem(UnityEngine.Camera mainCamera) {
            _mainCamera = mainCamera;
        }

        public void Update() {
        }
    }
}