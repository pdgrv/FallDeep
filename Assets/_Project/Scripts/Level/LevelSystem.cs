using System.Collections.Generic;
using Scripts.Utils;

namespace Scripts.Level {
    public class LevelSystem {
        private List<LevelRoot> _levels;

        private int CurrentLevel = 0;
        
        public LevelSystem(List<LevelRoot> levels) {
            _levels = levels;
        }
        
        public void SetupAndStart(int level) {
            if (_levels.IsIndexInBounds(level)) {
                var targetLevel = _levels[level];
                targetLevel.SetupAndStart(OnLevelComplete);
            } else {
                RootEntry.Instance.CompleteGame();
            }
        }

        public void CompleteLevel(int level) {
            if (_levels.IsIndexInBounds(level)) {
                var targetLevel = _levels[level];
                targetLevel.CompleteLevel();
            }
        }

        private void OnLevelComplete() {
            CurrentLevel++;
            SetupAndStart(CurrentLevel);
        }

        public void Update() {
            
        }
    }
}