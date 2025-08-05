// StateImplementations.cs (v1.1 - No changes from v1.0)
// This file can be named "StateImplementations.cs" or similar.
// It contains the concrete classes that implement the state machine contracts.

using System;

namespace Platformer
{
    /// <summary>
    /// A basic wrapper to evaluate inline conditions (lambda expressions).
    /// This allows you to define transition conditions without creating a new class for each one.
    /// </summary>
    public class FunkPredicate : IPredicate
    {
        private readonly Func<bool> _condition;

        public FunkPredicate(Func<bool> condition)
        {
            _condition = condition;
        }

        public bool Evaluate() => _condition();
    }

    /// <summary>
    /// Represents a state transition, holding a target state and its condition.
    /// </summary>
    public class Transition : ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }

        public Transition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}