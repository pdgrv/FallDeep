using System.Collections.Generic;

namespace Scripts.Level {
    public class LevelSystem {
        private List<LevelRoot> _levels;

        private int CurrentLevel = 0;
        
        public LevelSystem(List<LevelRoot> levels) {
            _levels = levels;
        }
        
        public void SetupAndStart(int level) {
            if (level < _levels.Count) {
                var targetLevel = _levels[level];
                targetLevel.SetupAndStart(OnLevelComplete);
            } else {
                RootEntry.Instance.CompleteGame();
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