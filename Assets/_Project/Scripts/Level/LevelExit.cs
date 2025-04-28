using UnityEngine;

namespace Scripts.Level {
    public class LevelExit : AbstractLevelExit {
        public Vector2Int LevelExitCoords;
        
        protected override bool CheckCompleteCondition() {
            var gridPosition = RootEntry.Instance.Player.gridPosition;
            return gridPosition == LevelExitCoords;
        }
    }
}