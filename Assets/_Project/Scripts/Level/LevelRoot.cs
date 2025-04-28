using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Scripts.Level {
    public class LevelRoot : MonoBehaviour {
        [SerializeField] private GridField Grid;
        [SerializeField] private AbstractLevelExit Exit;
        [SerializeField] private CinemachineCamera _cmCam;

        public void SetupAndStart(Action onLevelComplete) {
            Grid.SpawnPlayerOnGrid(RootEntry.PlayerSystem.Player);
            Exit.Setup(Grid, onLevelComplete);
            _cmCam.gameObject.SetActive(true);
        }

        public void CompleteLevel() {
            
        }
    }
}