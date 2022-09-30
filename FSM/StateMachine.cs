using System;
using System.Collections.Generic;

namespace FSM
{
    public class StateMachine
    {
        private readonly List<Transition>? Transitions;
        private State? CurrentState;
        public event EventHandler<FSMTransitionEventArgs>? StateChanging;
        public event EventHandler<FSMTransitionEventArgs>? StateChanged;


        public StateMachine(List<Transition>? transitions, State? currentState)
        {
            Transitions = transitions;
            CurrentState = currentState;
        }

        public class FSMTransitionEventArgs
        {
            public DateTime? timeIssued;
            public State? fromState;
            public State? toState;
            public string? transitionEvent;
            public FSMTransitionEventArgs(State? fromState, State? toState, string? transitionEvent)
            {
                this.timeIssued = DateTime.Now;
                this.fromState = fromState;
                this.toState = toState;
                this.transitionEvent = transitionEvent;
            }
        }
        private void OnStateChanging(FSMTransitionEventArgs stateChangingEvent)
        {
            StateChanging?.Invoke(this, stateChangingEvent);
        }
        private void OnStateChanged(FSMTransitionEventArgs stateChangedEvent)
        {
            StateChanged?.Invoke(this, stateChangedEvent);
        }

        public void Start()
        {
            CurrentState?.Start();
        }
        public void Stop()
        {
            CurrentState?.Stop();
        }

        public string? CurrentStateName()
        {
            return CurrentState?.Name;
        }
        public bool FireEventNamed(string eventName)
        {
            if (eventName is not null)
            {
                State? state = FindNextStateForEvent(eventName, CurrentState);
                if (state is not null)
                {
                    State? oldState = CurrentState;
                    OnStateChanging(new FSMTransitionEventArgs(CurrentState, state, eventName));
                    CurrentState?.Stop();
                    CurrentState = state;
                    CurrentState.Start();
                    OnStateChanged(new FSMTransitionEventArgs(oldState, CurrentState, eventName));

                    return true;
                }
            }
            OnStateChanging(new FSMTransitionEventArgs(CurrentState, null, eventName));
            return false;
        }

        private State? FindNextStateForEvent(string eventName, State? aState)
        {
            State? state = null;
            if (Transitions is not null)
            {
                foreach (var item in Transitions)
                {
                    //Find the next transition. If not found look at its parent state.
                    if (item.TransitionEvent == eventName && item.FromState == aState)
                    {
                        return item.ToState;
                    }
                }
                if (aState?.ParentState is not null)
                    return FindNextStateForEvent(eventName, aState?.ParentState);
            }
            return state;
        }
    }
}
