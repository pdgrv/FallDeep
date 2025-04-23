using UnityEngine;

namespace Scripts.Movement {
    public interface IMovable {
        void Tick();
        void SetInput(Vector3 direction);
        void SetInputGrid(Vector2Int direction);
        Vector3 GetDirection();
    }
}