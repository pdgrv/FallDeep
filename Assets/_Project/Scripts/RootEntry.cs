using System.Collections.Generic;
using Scripts.CameraControl;
using Scripts.Character;
using Scripts.Level;
using UnityEngine;

namespace Scripts {
    public class RootEntry : MonoSingleton<RootEntry> {
        public static LevelSystem LevelSystem;
        public static CameraSystem CameraSystem;
        public static PlayerSystem PlayerSystem;

        public List<LevelRoot> _levels;
        public Camera _mainCamera;
        public GridObject Player;

        protected override void Awake() {
            base.Awake();
            LevelSystem = new LevelSystem(_levels);
            CameraSystem = new CameraSystem(_mainCamera);
            PlayerSystem = new PlayerSystem(Player);
        }

        private void Start() {
            LevelSystem.SetupAndStart(0);
        }
        
        private void Update() {
            LevelSystem.Update();
            PlayerSystem.Update();
        }

        public void CompleteGame() {
            Debug.LogWarning("Game is completed");
        }
    }
}