using Scripts.Movement;

namespace Scripts.Character {
    public class PlayerSystem {
        private readonly GridObject _player;

        public GridObject Player => _player;

        public PlayerSystem(GridObject player) {
            _player = player;
        }
        
        public void Update() {
        }
    }
}