using System;
using System.Collections.Generic;

namespace GUIGUI17F
{
    /// <summary>
    /// state machine transition data
    /// </summary>
    public class StateTransition
    {
        public Enum StartState;
        public Enum TriggerEvent;
        public Enum EndState;
        public MulticastDelegate InvokeAction;
        
        public StateTransition(Enum startState, Enum triggerEvent, Enum endState, MulticastDelegate invokeAction)
        {
            StartState = startState;
            TriggerEvent = triggerEvent;
            EndState = endState;
            InvokeAction = invokeAction;
        }
    }
    
    public class StateMachine
    {
        private List<StateTransition> _transitions;
        private Enum _state;

        /// <summary>
        /// current state
        /// </summary>
        public Enum State => _state;

        public StateMachine(Enum originState)
        {
            _transitions = new List<StateTransition>();
            _state = originState;
        }
        
        public void AddTransition(StateTransition transition)
        {
            StateTransition currentTransition = _transitions.Find(item => item.StartState.Equals(transition.StartState) && item.TriggerEvent.Equals(transition.TriggerEvent));
            if (currentTransition == null)
            {
                _transitions.Add(transition);
            }
            else if (currentTransition.EndState.Equals(transition.EndState))
            {
                Delegate newDelegate = Delegate.Combine(currentTransition.InvokeAction, transition.InvokeAction);
                currentTransition.InvokeAction = newDelegate as MulticastDelegate;
            }
            else
            {
                currentTransition.EndState = transition.EndState;
                currentTransition.InvokeAction = transition.InvokeAction;
            }
        }
        
        public void RemoveTransition(Enum startState, Enum triggerEvent)
        {
            _transitions.RemoveAll(item => item.StartState.Equals(startState) && item.TriggerEvent.Equals(triggerEvent));
        }
        
        public void RemoveInvokeAction(Enum startState, Enum triggerEvent, MulticastDelegate invokeAction)
        {
            StateTransition transition = _transitions.Find(item => item.StartState.Equals(startState) && item.TriggerEvent.Equals(triggerEvent));
            if (transition != null && transition.InvokeAction != null)
            {
                Delegate newDelegate = Delegate.RemoveAll(transition.InvokeAction, invokeAction);
                transition.InvokeAction = newDelegate as MulticastDelegate;
            }
        }
        
        /// <summary>
        /// trigger a transition with an event and invoke the related actions
        /// </summary>
        /// <param name="triggerEvent">the event type used for trigger the state transition</param>
        /// <param name="args">the arguments used for deliver to the related actions</param>
        /// <returns>the related actions return value, null if nothing matches</returns>
        public object DoEvent(Enum triggerEvent, params object[] args)
        {
            StateTransition transition = _transitions.Find(item => item.StartState.Equals(this._state) && item.TriggerEvent.Equals(triggerEvent));
            if (transition != null)
            {
                _state = transition.EndState;
                if (transition.InvokeAction != null)
                {
                    return transition.InvokeAction.DynamicInvoke(args);
                }
            }
            return null;
        }
    }
}