using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FSM
{
    public class State : INotifyPropertyChanged
    {

        Timer? pollTimer;
        Timer? timeoutTimer;
        private string? name;

        public State(string? name, Action? clientMethod, Action? clientTimeoutMethod = null, int timeoutIntervalMS = 0, Action? clientPolledMethod = null, int pollIntervalMS = 0, State? parentState = null)
        {
            Name = name;
            ParentState = parentState;
            ClientMethod = clientMethod;
            ClientTimeoutMethod = clientTimeoutMethod;
            TimeoutIntervalMS = timeoutIntervalMS;
            ClientPolledMethod = clientPolledMethod;
            PollIntervalMS = pollIntervalMS;
            ChildStates = new ObservableCollection<State>();
        }
        public State()
        {
            ChildStates = new ObservableCollection<State>();
        }


        public string? Name { get => name; set { name = value; NotifyPropertyChanged(); } }
        public State? ParentState { get; set; }
        public ObservableCollection<State> ChildStates { get; set; }
        public Action? ClientMethod { get; set; }
        public Action? ClientTimeoutMethod { get; set; }
        public Action? ClientPolledMethod { get; }
        public int PollIntervalMS { get; }
        public int TimeoutIntervalMS { get; set; }

        public void Start()
        {
            if (ClientMethod is not null)
                Task.Factory.StartNew(ClientMethod);
            if (ClientPolledMethod is not null && PollIntervalMS != 0)
                SetupPollTimer();
            if (ClientTimeoutMethod is not null && TimeoutIntervalMS != 0)
                SetupTimeoutTimer();
        }
        public void Stop()
        {
            if (ClientPolledMethod is not null && PollIntervalMS != 0)
                pollTimer?.Dispose();
            if (ClientTimeoutMethod is not null && TimeoutIntervalMS != 0)
                timeoutTimer?.Dispose();
        }
        private void SetupPollTimer()
        {
            if (ClientPolledMethod is not null)
            {
                pollTimer = new Timer(PollTimerElapsed, null, PollIntervalMS, PollIntervalMS);
            }
        }
        private void SetupTimeoutTimer()
        {
            if (ClientTimeoutMethod is not null)
            {
                timeoutTimer = new Timer(TimeoutTimerElapsed, null, TimeoutIntervalMS, TimeoutIntervalMS);
            }
        }
        private void PollTimerElapsed(object? stateInfo)
        {
            if (ClientPolledMethod is not null)
                Task.Factory.StartNew(ClientPolledMethod);
        }
        private void TimeoutTimerElapsed(object? stateInfo)
        {
            if (ClientTimeoutMethod is not null)
                Task.Factory.StartNew(ClientTimeoutMethod);
        }

        public void AddChildState(State aChild)
        {
            if (aChild is not null)
            {
                aChild.ParentState = this;
                ChildStates.Add(aChild);
            }
        }
        public bool RemoveChildState(State aChild)
        {
            if (aChild is not null)
            {
                aChild.ParentState = null;
                return ChildStates.Remove(aChild);
            }
            return false;
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
