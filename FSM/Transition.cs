using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FSM
{
    public class Transition : INotifyPropertyChanged
    {
        private State? fromState;
        private State? toState;
        private string? transitionEvent;

        public Transition(State? fromState, State? toState, string? transitionEvent)
        {
            FromState = fromState;
            ToState = toState;
            TransitionEvent = transitionEvent;
        }

        public State? FromState
        {
            get => fromState;
            set { fromState = value; NotifyPropertyChanged(); }
        }
        public State? ToState
        {
            get => toState;
            set { toState = value; NotifyPropertyChanged(); }

        }
        public string? TransitionEvent
        {
            get => transitionEvent;
            set { transitionEvent = value; NotifyPropertyChanged(); }
        }





        public event PropertyChangedEventHandler? PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
