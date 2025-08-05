// StateInterfaces.cs (v1.1 - No changes from v1.0)
// This file can be named "StateInterfaces.cs" or similar.
// It contains the core contracts for the state machine system.

namespace Platformer
{
    /// <summary>
    /// Defines the contract for a state. Any class that is a state
    /// must implement these methods.
    /// </summary>
    public interface IState
    {
        void OnEnter();   // Called once when the state is first entered.
        void Update();    // Called every frame via MonoBehaviour.Update().
        void FixedUpdate(); // Called every physics step via MonoBehaviour.FixedUpdate().
        void OnExit();    // Called once when the state is exited.
    }

    /// <summary>
    /// Defines the contract for a predicate, which is a condition
    /// used to decide if a state transition should occur.
    /// </summary>
    public interface IPredicate
    {
        bool Evaluate(); // Returns true if the condition is met, false otherwise.
    }

    /// <summary>
    /// Defines the contract for a transition, which connects two states.
    /// </summary>
    public interface ITransition
    {
        IState To { get; }         // The state to transition to.
        IPredicate Condition { get; } // The condition that must be met to transition.
    }
}