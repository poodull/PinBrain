using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;
using System.Threading.Tasks;
using System.Threading;
using PinBrain.Engine.Constants;
using PinBrain.Library.Switch;
using PinBrain.Library.Utility;

namespace PinBrain.Engine.Managers
{
    public enum BallSaverStates
    {
        Off = 0,
        /// <summary>
        /// Waiting for the player to plunge the ball.
        /// </summary>
        Waiting = 1,
        On
    }

    public class TroughManager
    {
        //public event EventHandler<EventArgs<BallSaverStates>> BallSaverStateChanged;

        #region * Private members *
        private static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static TroughManager _instance = new TroughManager();

        private Task _troughWatcherTask;
        private CancellationTokenSource _cancelTokenSource;

        /// <summary>
        /// Flag for if the save ball timer is on.
        /// </summary>
        private BallSaverStates _ballSaverState = BallSaverStates.Off;

        /// <summary>
        /// field keeps the number of balls expected on the field.
        /// </summary>
        private int _ballsOnField = 0;

        /// <summary>
        /// the number of balls to be ejected into the shooter lane.
        /// </summary>
        private int _ballsToEject = 0;

        /// <summary>
        /// Flag to determine if all the balls that should be on the field are 
        /// to be auto-plunged.
        /// </summary>
        private bool _isAutoPlungeOn = false;

        /// <summary>
        /// Timer to turn off the ball save timeout.
        /// </summary>
        private Timer _ballSaverTimer;

        /// <summary>
        /// Flag to tell if there's a ball to be launched in the shooter lane.
        /// We can be waiting for the player to launch, or expecting an autoplunge.
        /// </summary>
        private bool _isBallInShooterLane;

        /// <summary>
        /// Total number of balls to expect the game to support.
        /// </summary>
        public const int TOTALBALLS = 5;
        #endregion

        #region * Constructor *
        /// <summary>
        /// Singleton pattern.
        /// </summary>
        private TroughManager()
        {
            _log.Info("Initiating TroughManager...");

            _cancelTokenSource = new CancellationTokenSource(); //used to stop the task in dispose
            _troughWatcherTask = new Task(() => troughWatcherHandler(_cancelTokenSource.Token),
                _cancelTokenSource.Token, TaskCreationOptions.LongRunning);
            _troughWatcherTask.Start();

            //ballsavertimer automatically turns off the ballsaver flag 
            _ballSaverTimer = new Timer(new TimerCallback((state) =>
            {
                if (BallSaverState == BallSaverStates.On)
                {
                    BallSaverState = BallSaverStates.Off;
                    //if (BallSaverStateChanged != null)
                    //    BallSaverStateChanged(null, new EventArgs<BallSaverStates>(BallSaverStates.Off));
                    _log.Info("Ballsaver Elapsed and is now OFF");
                }
            }));
        }
        #endregion

        #region * Public Static Properties *

        //deprecated.  Not a reliable way to determine if the player is out of balls.
        //public static int BallsOnField { get { return _instance._ballsOnField; } }

        //private BallSaverStates _ballSaverState = BallSaverStates.Off;
        public static BallSaverStates BallSaverState
        {
            get { return _instance._ballSaverState; }
            set
            {
                switch (value)
                {
                    case BallSaverStates.Off:
                        _instance._ballSaverTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        break;
                    case BallSaverStates.On:
                        _instance._ballSaverTimer.Change(GameManagerConstants.BALLSAVERTIMEOUT, Timeout.Infinite);
                        break;
                    case BallSaverStates.Waiting: //do nothing
                    default:
                        break;
                }
                _instance._ballSaverState = value;
                //if (_instance.BallSaverStateChanged != null)
                //    _instance.BallSaverStateChanged(null, new EventArgs<BallSaverStates>(value));
            }
        }
        #endregion

        /// <summary>
        /// Resets all counts except physical switch results
        /// </summary>
        public static void Reset()
        {
            _instance._ballsOnField = 0;
            _instance._ballsToEject = 0;
            _instance._isAutoPlungeOn = false;
        }

        /// <summary>
        /// Blocking call to wait for all balls to return to the trough.
        /// timeout is the time to wait.
        /// FlushAll forces all balls out of captivity including shooter lane
        /// </summary>
        public static bool WaitForAllBalls(int timeoutMs, bool flushAll)
        {
            if (_instance._isBallInShooterLane && flushAll)
                _instance._isAutoPlungeOn = true;

            _instance.countBallsOnField();

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            while (_instance._ballsOnField != 0)
            {
                _instance.countBallsOnField();
                if (sw.ElapsedMilliseconds > timeoutMs)
                    break;
                Thread.Sleep(20);
            }
            return _instance._ballsOnField == 0;
        }

        public static bool BallDrained(bool isTilted)
        {
            return _instance.ballDrained(isTilted);
        }

        /// <summary>
        /// Adds a ball to be ejected to the shooter lane.
        /// </summary>
        public static void EjectBall()
        {
            //add to number of balls to eject.  
            //we create a counter and timer watchdog that monitors how many balls should be on the playfield.
            //if there's a ball in the trough already, we start auto-plunging.
            _instance._ballsToEject++;
        }

        /// <summary>
        /// Starts the ballsaver timer going
        /// </summary>
        public static void StartBallSaverTimer()
        {
            BallSaverState = BallSaverStates.On; //start timer
            _log.InfoFormat("Ballsaver is now ON and will timeout in {0}", GameManagerConstants.BALLSAVERTIMEOUT);
            _instance._ballSaverTimer.Change(GameManagerConstants.BALLSAVERTIMEOUT, Timeout.Infinite);
            //if (_instance.BallSaverStateChanged != null)
            //    _instance.BallSaverStateChanged(null, new EventArgs<BallSaverStates>(BallSaverStates.On));
        }


        #region * Private Methods *
        /// <summary>
        /// This task watches for state changes in the trough switches as well as the shooter lane.
        /// It generally shouldn't care about what state the game is in because the logic to eject is controlled via
        /// autoplunge properties, etc.
        /// </summary>
        /// <param name="cancellationToken"></param>
        private void troughWatcherHandler(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) //change to allow for Task Cancelling
            {
                System.Threading.Thread.Sleep(1000); //sleep first, so we can insure a 1 second lapse.

                //while (GameManager.CurrentInputMode == InputMode.NormalPlay)
                //{
                //    //if there any number of auto-plunged balls, start ejecting everything
                //    //if there is a ball in the shooter lane, try to auto-plunge and wait until the lane clears (2s).
                //    //SwitchManager.GetState(SwitchConstants.Switches.BallShooterLane).State = On;
                //    //switchmanager.autoplunge();
                //    System.Threading.Thread.Sleep(2000);
                //}

                if (_isBallInShooterLane)
                {
                    if (_isAutoPlungeOn)  //clear the way if there's a ball waiting.
                    {
                        SwitchManager.Autoplunge();
                    }
                }
                else if (_ballsToEject > 0)
                {
                    SwitchManager.EjectBall();
                    _ballsToEject--;
                    if (_ballsToEject > 0) //there is a queue of balls (multiball?)
                        _isAutoPlungeOn = true; //turn on autoplunging to flush this queue out.
                }
                else //no ball in shooter lane and no balls queued to eject. turning off autoplunge.
                {
                    //_isAutoPlungeOn = false; //CAN'T DO THIS!!! RACE CONDITION WOULD TURN OFF AUTOPLUNGE, ORPHANING LAST BALL
                    //The only place autoplunge is turned off is in RESET, caused by a new game or player.
                    //we've got nothing to do. lets see if we're stuck.
                    //Switch[] state = SwitchManager.GetSwitches();
                    //_isBallInShooterLane = state[(int)SwitchConstants.Switches.BallShooterLane].State == SwitchState.On;
                }

                //now lets count the balls we should have.
                countBallsOnField();
            }
        }

        private void countBallsOnField()
        {
            //now lets count the balls we should have.
            _isBallInShooterLane = SwitchManager.GetSwitchState(SwitchConstants.Switches.BallShooterLane) == SwitchState.On;
            int ballsInTrough = SwitchManager.GetSwitchState(SwitchConstants.Switches.RightBallTrough) == SwitchState.On ? 1 : 0;
            ballsInTrough += SwitchManager.GetSwitchState(SwitchConstants.Switches.RightMidBallTrough) == SwitchState.On ? 1 : 0;
            ballsInTrough += SwitchManager.GetSwitchState(SwitchConstants.Switches.MidBallTrough) == SwitchState.On ? 1 : 0;
            ballsInTrough += SwitchManager.GetSwitchState(SwitchConstants.Switches.LeftMidBallTrough) == SwitchState.On ? 1 : 0;
            ballsInTrough += SwitchManager.GetSwitchState(SwitchConstants.Switches.LeftBallTrough) == SwitchState.On ? 1 : 0;
            _ballsOnField = TOTALBALLS - ballsInTrough;
        }

        private bool ballDrained(bool isTilted)
        {
            //check if the ball timer has expired
            if (!isTilted && BallSaverState != BallSaverStates.Off) //waiting or on
            {
                _log.Info("BALL SAVED!!!  Ball Saver is active.  Ejecting ball, turning OFF ballsaver.");
                _isAutoPlungeOn = true;
                EjectBall();
                BallSaverState = BallSaverStates.Off;
                countBallsOnField();
                return false;
            }
            countBallsOnField();
            _log.Debug("Ball Drained.  Balls on field: " + _ballsOnField);
            return true;
        }
        #endregion
    }
}
