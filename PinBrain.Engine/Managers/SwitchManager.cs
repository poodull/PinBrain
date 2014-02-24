#define USEARDUINO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;

using log4net;

using PinBrain.Library.Switch;
using PinBrain.Devices.IO.IPAC;

using PinBrain.Engine.Constants;
using PinBrain.Devices.IO.Serial;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace PinBrain.Engine.Managers
{
    public class SwitchManager
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ISwitchDriver _driver;

        private static SwitchManager _instance;

        private Task _switchChangesHandleTask;
        private CancellationTokenSource _cancelTokenSource;
        /// <summary>
        /// This is the action called to handle the switch state.  Modes will attach their action to this.
        /// </summary>
        private Action<Switch> _switchHandlerAction;
        /// <summary>
        /// Queue of switch events that need to be processed.
        /// </summary>
        protected ConcurrentQueue<Switch> _switchChanges;
        /// <summary>
        /// Diagnostic to find out how long switches take.
        /// </summary>
        System.Diagnostics.Stopwatch _switchHandleTime = new System.Diagnostics.Stopwatch();

        private SwitchManager()
        {
            _log.Info("Initializing SwitchManager...");

            _switchChanges = new ConcurrentQueue<Switch>();

            //need to load the switch driver dynamically.
            //_driver = new IpacDriver(getSwitchesForGame());
            //_driver = new KeyCatcherForm(58);
#if USEARDUINO
            //TODO: Load the com port from app.config
            _driver = new SolenoidArduino("COM6", SwitchConstants.GetAllSwitchesForGame(), new Action<List<Switch>>(enqueueSwitchChanges));
#else
            //_driver = new ButtonForm(SwitchConstants.GetAllSwitchesForGame(), new Action<List<Switch>>(switchChangedHandler));
            _driver = new ButtonForm(SwitchConstants.GetAllSwitchesForGame(), new Action<List<Switch>>(enqueueSwitchChanges));
#endif
            //This task just publishes the switch changes received from the driver.
            _cancelTokenSource = new CancellationTokenSource(); //used to stop the task in dispose
            _switchChangesHandleTask = new Task(() => handleSwitchChangesTask(_cancelTokenSource.Token),
                _cancelTokenSource.Token, TaskCreationOptions.LongRunning);
            _switchChangesHandleTask.Start();
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns></returns>
        protected static SwitchManager getInstance()
        {
            if (_instance == null)
                _instance = new SwitchManager();

            return _instance;
        }

        public static void RegisterSwitchHandler(Action<Switch> changedHandler)
        {
            getInstance()._switchHandlerAction = changedHandler; //TODO: make it work for more than one handler?  like a monitor?
        }

        /// <summary>
        /// Enqueues the switch changes coming from the driver
        /// </summary>
        /// <param name="changedSwitches"></param>
        private void enqueueSwitchChanges(List<Switch> changedSwitches)
        {
            //GameManager.SwitchStateChanged(changedSwitches.ToArray());
            foreach (Switch sw in changedSwitches)
            {
                _switchChanges.Enqueue(sw);
            }
        }

        private void handleSwitchChangesTask(CancellationToken cancelToken)
        {
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    while (_instance._switchChanges.Count == 0 && !cancelToken.IsCancellationRequested)
                        Thread.Sleep(1);
                    if (cancelToken.IsCancellationRequested)
                        continue; //break out
                    Switch changed;
                    if (!_switchChanges.TryDequeue(out changed))
                    {
                        //can't dequeue?
                        continue;
                    }
                    _switchHandleTime.Restart();

                    if (_switchHandlerAction != null)
                        _switchHandlerAction(changed);
                    else
                        _log.Warn("There is no switch handler right now!");

                    _log.DebugFormat("Switch {0} {1} took {2} ms.", changed.Name, changed.State, _instance._switchHandleTime.ElapsedMilliseconds);
                } //end while (!cancelled)
            }
            catch (Exception ex)
            {
                _log.Error("Error in Switch Manager handling a switch change!", ex);
            }
            _log.Info("Leaving Switch Changed Handling Thread.");
        }


        internal static void Reset()
        {
            getInstance().reset();
        }

        private void reset()
        {
            _switchChanges = new ConcurrentQueue<Switch>();
            //Make sure we can talk to the device.
            RotateStage(SwitchConstants.StagePositions.CenterExit);
        }

        /// <summary>
        /// Gets the last known state of the selected switch.
        /// </summary>
        /// <param name="switchIndex"></param>
        /// <returns></returns>
        public static Switch GetSwitch(SwitchConstants.Switches switchVal)
        {
            //return _instance._lastState[(int)switchVal];
            return _instance._driver.SwitchArrayState[(int)switchVal];
        }

        /// <summary>
        /// Gets the last known state of the selected switch.
        /// </summary>
        /// <param name="switchIndex"></param>
        /// <returns></returns>
        public static SwitchState GetSwitchState(SwitchConstants.Switches switchVal)
        {
            //return _instance._lastState[(int)switchVal].State;
            return _instance._driver.SwitchArrayState[(int)switchVal].State;
        }

        ///// <summary>
        ///// Gets all the switches in the array.  Might be faster than getting them individually
        ///// </summary>
        ///// <returns></returns>
        //public static Switch[] GetSwitches()
        //{
        //    Switch[] ret = new Switch[_instance._lastState.Length];
        //    _instance._lastState.CopyTo(ret, 0);
        //    return ret;
        //}

        /// <summary>
        /// Writes to the switch device to fire the autoplunge mechanism.
        /// </summary>
        public static void Autoplunge()
        {
            _log.Debug("Autoplunging Ball!");
            _instance._driver.AutoPlunge();
        }

        /// <summary>
        /// Writes to the switch device to fire the ball eject onto the shooter lane.
        /// </summary>
        public static void EjectBall()
        {
            _log.Debug("Ejecting Ball!");
            _instance._driver.EjectBall();
        }

        private void sendCommand(string command)
        {
            //_driver.SendCommand(command); //This belongs in a SolenoidDriver class.
        }

        internal static void EnableFlippers(bool isEnabled)
        {
            _instance.enableFlippers(isEnabled);
        }

        private void enableFlippers(bool isEnabled)
        {
            //sendCommand(string.Format(CMDENABLEFLIPPERS, isEnabled ? 1 : 0));
            _log.DebugFormat("Flippers are {0}abled.", isEnabled ? "En" : "Dis");
            _instance._driver.EnableFlippers();
        }

        internal static void FireKnocker()
        {
            _log.Debug("KNOCKER!");
            _instance._driver.FireKnocker();
        }

        internal static void RotateStage(SwitchConstants.StagePositions position)
        {
            _log.Debug("Rotating STAGE to " + position);
            _instance._driver.RotateStage((int)position);
        }

        internal static void RotateCross(bool isClosed)
        {
            _log.Debug("Rotating Cross to " + isClosed);
            _instance._driver.RotateCross(isClosed);
        }
    }
}
