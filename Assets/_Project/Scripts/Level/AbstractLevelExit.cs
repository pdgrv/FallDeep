using System;
using UnityEngine;

namespace Scripts.Level {
    public abstract class AbstractLevelExit : MonoBehaviour {
        protected GridField Grid;
        private Action _onLevelComplete;
        
        public void Setup(GridField grid, Action onLevelComplete) {
            Grid = grid;
            _onLevelComplete = onLevelComplete;
            grid.OnUpdate += OnGridUpdate;
        }
        
        private void OnGridUpdate() {
            if (enabled && CheckCompleteCondition()) {
                Complete();
            }
        }
        
        public void Complete() {
            Grid.OnUpdate -= OnGridUpdate;
            _onLevelComplete?.Invoke();
        }

        protected abstract bool CheckCompleteCondition();
    }
}