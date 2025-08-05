using UnityEngine;

namespace Platformer
{
    // This is the base class for all player states.
    // It no longer needs a reference to the Animator.
    public abstract class State : IState
    {
        protected readonly PlayerController player;
        protected readonly StateMachine stateMachine;

        protected State(PlayerController player, StateMachine stateMachine)
        {
            this.player = player;
            this.stateMachine = stateMachine;
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }
}
