using System;
using UnityEngine;

namespace Scripts.Level {
    public class LevelExit : MonoBehaviour {
        public Vector2Int LevelExitCoords;

        private GridField _grid;
        private Action _onLevelComplete;

        public void Setup(GridField grid, Action onLevelComplete) {
            _grid = grid;
            _onLevelComplete = onLevelComplete;
            grid.OnUpdate += OnGridUpdate;
        }

        private void OnGridUpdate() {
            var gridPosition = RootEntry.Instance.Player.gridPosition;

            if (gridPosition == LevelExitCoords && enabled) {
                Complete();
            }
        }

        public void Complete() {
            _grid.OnUpdate -= OnGridUpdate;
            _onLevelComplete?.Invoke();
        }
    }
}