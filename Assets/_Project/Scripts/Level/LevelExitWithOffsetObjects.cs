using System.Collections.Generic;
using System.Linq;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Level {
    public class LevelExitWithOffsetObjects : AbstractLevelExit {
        [SerializeField] private List<ObjectsWiithOffset> _objects;

        protected override bool CheckCompleteCondition() {
            if (_objects.IsEmpty()) {
                return false;
            }
            
            Vector2Int zeroPosition = _objects.First().Prefab.gridPosition;
            
            foreach (var obj in _objects) {
                if (obj.Prefab.gridPosition != zeroPosition + obj.Offset) {
                    return false;
                }
            }

            return true;
        }
    }
    
    [System.Serializable]
    public class ObjectsWiithOffset {
        public Vector2Int Offset;
        public GridObject Prefab;
    }
}