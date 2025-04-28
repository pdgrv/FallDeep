using System;
using UnityEngine;

namespace Scripts.Level {
    public class LevelRoot : MonoBehaviour {
        [SerializeField] private GridField Grid;
        [SerializeField] private AbstractLevelExit Exit;

        public void SetupAndStart(Action onLevelComplete) {
            Grid.SpawnPlayerOnGrid(RootEntry.PlayerSystem.Player);
            Exit.Setup(Grid, onLevelComplete);
        }
    }
}