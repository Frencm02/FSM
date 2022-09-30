using FSM;

namespace FSMTests
{
    [TestClass]
    public class FSMTransitionBeforePolledAndTimeoutStateTest
    {
        StateMachine? stateMachine;
        readonly AutoResetEvent autoResetEvent = new(false);

        bool idle, running, error, initialize, runningPolled, runningTimeout = false;

        [TestMethod]
        public void TransitionBeforePolledAndTimeoutState()
        {

            State s1 = new("Idle", FsmAction_Idle);
            State s2 = new("Running", FsmAction_Running, FsmAction_RunningTimeout, 1000, FsmAction_RunningPolled, 900);
            State s3 = new("Error", FsmAction_Error);
            State s4 = new("Initialized", FsmAction_Initialized);
            s1.ParentState = s4;
            s2.ParentState = s4;
            s3.ParentState = s4;


            Transition t1 = new(s1, s2, "Run");
            Transition t2 = new(s2, s1, "Stop");
            Transition t3 = new(s4, s1, "Initialize");
            Transition t4 = new(s4, s3, "Error");
            Transition t5 = new(s3, s1, "Recovered");

            List<Transition> transitions = new()
            {
                t1,
                t2,
                t3,
                t4,
                t5
            };


            stateMachine = new StateMachine(transitions, s4);
            stateMachine.StateChanging += StateMachine_StateChanging;
            stateMachine.Start();

            Assert.IsTrue(stateMachine.CurrentStateName() == "Initialized");
            stateMachine.FireEventNamed("Initialize");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");
            stateMachine.FireEventNamed("Run");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Running");
            stateMachine.FireEventNamed("Error");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Error");
            stateMachine.FireEventNamed("Recovered");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");

            autoResetEvent.WaitOne(TimeSpan.FromSeconds(2));

            Assert.IsTrue(idle);
            Assert.IsTrue(running);
            Assert.IsTrue(error);
            Assert.IsTrue(initialize);
            Assert.IsFalse(runningPolled);
            Assert.IsFalse(runningTimeout);
        }

        private void StateMachine_StateChanging(object? sender, StateMachine.FSMTransitionEventArgs e)
        {
            Console.WriteLine(e.timeIssued.ToString() +
                         "\t" + e.transitionEvent +
                         "\t" + (e?.fromState?.Name ?? "null") +
                         " -> " + (e?.toState?.Name ?? "null"));
        }

        public void FsmAction_Idle()
        {

            idle = true;


        }
        public void FsmAction_Running()
        {
            running = true;

        }
        public void FsmAction_RunningPolled()
        {
            runningPolled = true;


        }
        public void FsmAction_RunningTimeout()
        {
            runningTimeout = true;


        }
        public void FsmAction_Error()
        {
            error = true;

        }
        public void FsmAction_Initialized()
        {
            initialize = true;

        }
    }
}
