using System;
using Scripts.Interaction;
using UnityEngine;

namespace Scripts {
    public class RootEntry : MonoBehaviour {
        public static InteractionController InteractionController;
        private void Awake() {
            InteractionController = new InteractionController();
        }
    }
}