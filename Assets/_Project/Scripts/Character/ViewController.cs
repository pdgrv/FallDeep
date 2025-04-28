using UnityEngine;

public class ViewController : MonoBehaviour {
    [SerializeField] private Animator _animator;

    private AnimationController _animationController;

    private void Start() {
        if (_animator) {
            _animationController ??= new AnimationController(_animator);
        }
    }

    public void UpdateState(AnimationController.AnimStates targetAnimState) {
        if (_animationController != null) {
            _animationController.UpdateState(targetAnimState);
        }
    }

    public void UpdateState(bool isMoving, bool isInteraction) {
        if (_animationController != null) {
            if (isMoving) {
                if (isInteraction) {
                    _animationController.UpdateState(AnimationController.AnimStates.IsPulling);
                } else {
                    _animationController.UpdateState(AnimationController.AnimStates.IsMoving);
                }
            } else {
                if (isInteraction) {
                    _animationController.UpdateState(AnimationController.AnimStates.IsInteractionIdle);
                } else {
                    _animationController.UpdateState(AnimationController.AnimStates.Idle);
                }
            }
        }
    }

    public class AnimationController {
        private readonly Animator _animator;

        public AnimationController(Animator animator) {
            _animator = animator;
        }

        private const string MoveBool = "IsMoving";
        private const string PushBool = "IsPushing";
        private const string PullBool = "IsPulling";

        public enum AnimStates {
            Idle,
            IsMoving,
            IsPushing,
            IsPulling,
            IsInteractionIdle
        }

        public void UpdateState(AnimStates targetAnimState) {
            switch (targetAnimState) {
                case AnimStates.Idle:
                    _animator.SetBool(MoveBool, false);
                    _animator.SetBool(PushBool, false);
                    _animator.SetBool(PullBool, false);
                    break;
                case AnimStates.IsMoving:
                    _animator.SetBool(MoveBool, true);
                    _animator.SetBool(PushBool, false);
                    _animator.SetBool(PullBool, false);
                    break;
                case AnimStates.IsPushing:
                    _animator.SetBool(MoveBool, true);
                    _animator.SetBool(PushBool, true);
                    _animator.SetBool(PullBool, false);
                    break;
                case AnimStates.IsPulling:
                    _animator.SetBool(MoveBool, true);
                    _animator.SetBool(PushBool, false);
                    _animator.SetBool(PullBool, true);
                    break;
                case AnimStates.IsInteractionIdle:
                    _animator.SetBool(MoveBool, false);
                    _animator.SetBool(PushBool, false);
                    break;
            }
        }
    }
}