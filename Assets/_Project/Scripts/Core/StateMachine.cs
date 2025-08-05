// StateMachine.cs (v1.1 - No changes from v1.0)
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// A generic state machine engine. It manages states and transitions.
    /// Attach this component to any GameObject that needs state-driven behavior.
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        public IState CurrentState { get; private set; }
        
        // A property for debugging, to easily see the current state in the Inspector.
        public string DebugCurrentState => CurrentState?.GetType().Name;

        // A list of transitions that can happen from *any* other state.
        private readonly List<ITransition> _anyTransitions = new List<ITransition>();
        
        // A dictionary that maps a state Type to its node, which contains its outgoing transitions.
        private readonly Dictionary<Type, StateNode> _nodes = new Dictionary<Type, StateNode>();

        /// <summary>
        /// Internal class to store a state and its associated transitions.
        /// </summary>
        private class StateNode
        {
            public IState State { get; }
            // **FIX**: Using a List instead of a HashSet to guarantee transition priority.
            // Transitions will be checked in the order they are added.
            public List<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new List<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }

        /// <summary>
        /// Called every frame. Checks for transitions and updates the current state.
        /// </summary>
        public void Tick()
        {
            if (CurrentState == null) return;

            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }
            
            CurrentState.Update();
        }

        /// <summary>
        /// Called every fixed physics step. Updates the current state's physics.
        /// </summary>
        public void FixedTick()
        {
            if (CurrentState == null) return;
            CurrentState.FixedUpdate();
        }

        /// <summary>
        /// Sets the initial state of the machine without calling OnExit on the previous state.
        /// </summary>
        public void SetState(IState state)
        {
            CurrentState = state;
            CurrentState?.OnEnter();
        }

        /// <summary>
        /// Changes the current state, calling OnExit and OnEnter appropriately.
        /// </summary>
        public void ChangeState(IState state)
        {
            if (state == CurrentState) return;

            CurrentState?.OnExit();
            CurrentState = state;
            CurrentState?.OnEnter();
        }

        /// <summary>
        /// Adds a transition from one specific state to another.
        /// </summary>
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            var node = GetOrAddNode(from);
            node.AddTransition(to, condition);
        }

        /// <summary>
        /// Adds a transition that can occur from any state.
        /// </summary>
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            _anyTransitions.Add(new Transition(to, condition));
        }

        /// <summary>
        /// Checks for a valid transition from the current state.
        /// </summary>
        private ITransition GetTransition()
        {
            // "Any" transitions have the highest priority.
            foreach (var transition in _anyTransitions)
            {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            // If no "any" transition is found, check transitions from the current state.
            if (CurrentState != null && _nodes.TryGetValue(CurrentState.GetType(), out var node))
            {
                foreach (var transition in node.Transitions)
                {
                    if (transition.Condition.Evaluate())
                        return transition;
                }
            }

            return null;
        }

        private StateNode GetOrAddNode(IState state)
        {
            var type = state.GetType();
            if (!_nodes.TryGetValue(type, out var node))
            {
                node = new StateNode(state);
                _nodes[type] = node;
            }
            return node;
        }
    }
}