using FSM;

namespace FSMTests
{
    [TestClass]
    public class FSMTest
    {
        StateMachine? stateMachine;
        readonly AutoResetEvent autoResetEvent = new(false);

        bool idle, running, error, initialize = false;

        [TestMethod]
        public void BasicFSMTest()
        {
            ResetTest();
            SetupFSM();
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

            autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(idle);
            Assert.IsTrue(running);
            Assert.IsTrue(error);
            Assert.IsTrue(initialize);
        }
        [TestMethod]
        public void EmptyTransitionTests()
        {

            ResetTest();
            SetupFSM();
            stateMachine.Start();

            Assert.IsTrue(stateMachine.CurrentStateName() == "Initialized");
            stateMachine.FireEventNamed("Initialize");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");
            stateMachine.FireEventNamed("");

            Assert.IsFalse(stateMachine.CurrentStateName() == "Running");
            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");
            stateMachine.FireEventNamed("Error");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Error");
            stateMachine.FireEventNamed("Recovered");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");

            autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(idle);
            Assert.IsFalse(running);
            Assert.IsTrue(error);
            Assert.IsTrue(initialize);


        }

        [TestMethod]
        public void NullTransitionTests()
        {

            ResetTest();
            SetupFSM();
            stateMachine.Start();

            Assert.IsTrue(stateMachine.CurrentStateName() == "Initialized");
            stateMachine.FireEventNamed("Initialize");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");
            stateMachine.FireEventNamed(null);

            Assert.IsFalse(stateMachine.CurrentStateName() == "Running");
            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");
            stateMachine.FireEventNamed("Error");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Error");
            stateMachine.FireEventNamed("Recovered");

            Assert.IsTrue(stateMachine.CurrentStateName() == "Idle");

            autoResetEvent.WaitOne(TimeSpan.FromSeconds(1));

            Assert.IsTrue(idle);
            Assert.IsFalse(running);
            Assert.IsTrue(error);
            Assert.IsTrue(initialize);


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
        public void FsmAction_Error()
        {
            error = true;

        }
        public void FsmAction_Initialized()
        {
            initialize = true;

        }

        public void ResetTest()
        {
            idle = false;
            running = false; 
            error = false; 
            initialize = false;
        }
        public void SetupFSM()
        {
            //Decided not to use a local function as an example.
#pragma warning disable IDE0039 // Use local function
            Action a1 = delegate () { FsmAction_Idle(); };
            Action a2 = delegate () { FsmAction_Running(); };
            Action a3 = delegate () { FsmAction_Error(); };
            Action a4 = delegate () { FsmAction_Initialized(); };
#pragma warning restore IDE0039 // Use local function



            State s1 = new("Idle", a1);
            State s2 = new("Running", a2);
            State s3 = new("Error", a3);
            State s4 = new("Initialized", a4);
            s4.AddChildState(s1);
            s4.AddChildState(s2);
            s4.AddChildState(s3);


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
        }
    }
}