using UnityEngine;

namespace Scripts.Interaction {
    public interface IInteractable {
        void CreateInteraction(Vector2Int direction);
        bool CheckInteractableObject();
    }
}