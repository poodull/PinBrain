using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Feature;
using System.Threading;
using System.Threading.Tasks;
using PinBrain.Engine.Constants;
using PinBrain.Library.Switch;

namespace PinBrain.Engine.Managers
{
    partial class GameManager
    {
        /// <summary>
        /// Timed mode.  This should really be a derived class (TimedModeBase) with
        /// all the timing pre-coded.
        /// </summary>
        public class TrophyRoomMode : IMode
        {
            private bool _isDisposed = false;

            /// <summary>
            /// The total amount of time(ms) allowed to complete this mode.
            /// </summary>
            private int BONUSCOLLECTTIMEOUTMS = 20000; //needs to be a difficulty setting

            /// <summary>
            /// The countdown timer 'while' loop
            /// </summary>
            private Timer _collectBonusTimer;

            /// <summary>
            /// The time this mode officially started.
            /// </summary>
            private System.Diagnostics.Stopwatch _timeStarted;

            /// <summary>
            /// Flag to determine if the player has collected the bonus yet.
            /// </summary>
            private bool _isBonusCollected = false;

            /// <summary>
            /// Player is no longer eligible to collect the bonus (timeout, tilt, balldrain)
            /// </summary>
            private bool _isBonusFailed = false;

            public TrophyRoomMode()
            {
                _log.Info("Trophy Room Mode Started");
                DisplayManager.OnAnimationCompleted += new DisplayEventHandler(DisplayManager_OnAnimationCompleted);
                SwitchManager.RegisterSwitchHandler(handleSwitchChanges);

                //Rotate Playfield to CenterScoop position
                SwitchManager.RotateStage(SwitchConstants.StagePositions.TrophyScoop);
                SwitchManager.RotateCross(false);

                //Play "Collect Trophy" instructional video
                DisplayManager.PlayCutScene(DisplayConstants.CutScenes.ActiveGameMode.COLLECTTROPHY);
                LightManager.PlaySequence(LightingConstants.Lfx.CollectTrophySlow);

                _collectBonusTimer = new Timer(new TimerCallback(collectBonusTimerHandler),
                    null, 1000, 1000); //ticks every second
                _timeStarted = new System.Diagnostics.Stopwatch();
                _timeStarted.Start();
            }

            /// <summary>
            /// Timer handler to manage ball state, lights, sound, etc.
            /// </summary>
            /// <param name="state"></param>
            private void collectBonusTimerHandler(object state)
            {
                //this is called every second.
                //check if we've collected already(race condition with switches)
                if (_isBonusCollected || _isBonusFailed)
                {
                    //do we turn off the timer or just let it dispose?
                    return;
                }

                //check how much time is remaining
                long timeleft = BONUSCOLLECTTIMEOUTMS - _timeStarted.ElapsedMilliseconds;

                //if > 10 seconds, do nothing
                if (timeleft > 10000)
                {
                    return;
                }
                else if (timeleft > 9050) //leave some jitter time (or just put in a _soundPlayed flag?)
                {
                    //if = 10 seconds, start panic song and lighting
                    SoundManager.PlaySfx(SoundConstants.Sfx.HurryUp);
                    LightManager.PlaySequence(LightingConstants.Lfx.CollectTrophyFast);
                }
                else if (timeleft <= 0 && !_isBonusFailed)
                {
                    endPlayTimeout();
                }

            }

            #region * Dispose *
            //Call Dispose to free resources explicitly
            public void Dispose()
            {
                Dispose(true);
                //If dispose is called already then say GC to skip finalize on this instance.
                GC.SuppressFinalize(this);
            }

            ~TrophyRoomMode()
            {
                Dispose(false);
            }

            //Implement dispose to free resources
            protected virtual void Dispose(bool disposing)
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    // Released unmanaged Resources
                    if (disposing)
                    {
                        // Released managed Resources
                        DisplayManager.OnAnimationCompleted -= DisplayManager_OnAnimationCompleted;
                        //SwitchManager.DeRegisterSwitchHandler(handleSwitchChanges);
                        if (_collectBonusTimer != null)
                            _collectBonusTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                }
            }
            #endregion

            public void DisplayManager_OnAnimationCompleted(DisplayEventArgs e)
            {
                try
                {
                    //what animation completed?
                    if (e == null || e.SceneName == null)
                        return;
                    switch (e.SceneName)
                    {
                        case DisplayConstants.CutScenes.Bonuses.STAGECLEARED:
                            //Stage is cleared, now lets rotate and begin the next level
                            ModeStart(GameManagerConstants.GameModes.NORMALPLAY);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }

            private void beginPlay()
            {
                //rotate the stage to centerscoop
                //start the timer
            }

            private void endPlayTimeout()
            {
                //the player ran out of time trying to collect the trophy but has not lost the ball yet.
                //kill the flippers until we lose the ball.
                SwitchManager.EnableFlippers(false);
                _isBonusFailed = true; //no bonus
            }

            private void endPlayBallLost()
            {
                //Either the flippers were killed, so we're here now, or we just lost the ball trying to collect the trophy
                //play the StageCleared animation without the bonus.
                _isBonusFailed = true; //no bonus
            }

            private void endPlayTrophyCollected()
            {
                //Hurray!  play StageCleared with the bonus.
                _isBonusCollected = true;
            }


            private void handleSwitchChanges(Switch changed)
            {
                if (_isDisposed)
                    return;
                switch (changed.Id)
                {
                    case (int)SwitchConstants.Switches.PlumbTilt:
                        _instance.TiltWarning(PlayerStatus.CurrentPlayer.TiltWarnings++);
                        break;
                    case (int)SwitchConstants.Switches.Start:
                        if (changed.State == SwitchState.On)
                        {
                            //can only add a player if the current player is on ball 1
                            if (PlayerStatus.CurrentPlayer.Ball == 1)
                            {
                                _instance.AddPlayer();
                            }
                        }
                        break;
                    case (int)SwitchConstants.Switches.Coin:
                        if (changed.State == SwitchState.On)
                            _instance.AddCredit(); //
                        break;
                    case (int)SwitchConstants.Switches.SlamTilt:
                        break;
                    case (int)SwitchConstants.Switches.MenuSelect:
                        break;
                    case (int)SwitchConstants.Switches.MenuBack:
                        break;
                    case (int)SwitchConstants.Switches.MenuNext:
                        break;
                    case (int)SwitchConstants.Switches.MenuExit:
                        break;
                    case (int)SwitchConstants.Switches.Outhole:
                        endPlayBallLost();
                        break;
                    case (int)SwitchConstants.Switches.RightBallTrough:
                        break;
                    case (int)SwitchConstants.Switches.RightMidBallTrough:
                        break;
                    case (int)SwitchConstants.Switches.MidBallTrough:
                        break;
                    case (int)SwitchConstants.Switches.LeftMidBallTrough:
                        break;
                    case (int)SwitchConstants.Switches.LeftBallTrough:
                        break;
                    case (int)SwitchConstants.Switches.BallShooterLane:
                        break;
                    case (int)SwitchConstants.Switches.RightFlipperEOS:
                        break;
                    case (int)SwitchConstants.Switches.LeftFlipperEOS:
                        break;
                    case (int)SwitchConstants.Switches.LeftDrain:
                        break;
                    case (int)SwitchConstants.Switches.LeftReturn:
                        break;
                    case (int)SwitchConstants.Switches.LeftSling:
                    case (int)SwitchConstants.Switches.RightSling:
                        break;
                    case (int)SwitchConstants.Switches.RightReturn:
                        break;
                    case (int)SwitchConstants.Switches.RightDrain:
                    case (int)SwitchConstants.Switches.DraculaD:
                        break;
                    case (int)SwitchConstants.Switches.DraculaR:
                        break;
                    case (int)SwitchConstants.Switches.DraculaA:
                        break;
                    case (int)SwitchConstants.Switches.DraculaC:
                        break;
                    case (int)SwitchConstants.Switches.DraculaU:
                        break;
                    case (int)SwitchConstants.Switches.DraculaL:
                        break;
                    case (int)SwitchConstants.Switches.DraculaA2:
                        break;
                    case (int)SwitchConstants.Switches.BallPopper:
                        break;
                    case (int)SwitchConstants.Switches.DropTargetA:
                        break;
                    case (int)SwitchConstants.Switches.DropTargetB:
                        break;
                    case (int)SwitchConstants.Switches.DropTargetC:
                        break;
                    case (int)SwitchConstants.Switches.BelmontB:
                        break;
                    case (int)SwitchConstants.Switches.BelmontE:
                        break;
                    case (int)SwitchConstants.Switches.BelmontL:
                        break;
                    case (int)SwitchConstants.Switches.BelmontM:
                        break;
                    case (int)SwitchConstants.Switches.BelmontO:
                        break;
                    case (int)SwitchConstants.Switches.BelmontN:
                        break;
                    case (int)SwitchConstants.Switches.BelmontT:
                        break;
                    case (int)SwitchConstants.Switches.LeftOuterOrbit:
                        break;
                    case (int)SwitchConstants.Switches.RampExit:
                        break;
                    case (int)SwitchConstants.Switches.LeftInnerOrbit:
                        break;
                    case (int)SwitchConstants.Switches.BossTarget:
                        break;
                    case (int)SwitchConstants.Switches.CenterExit:
                        break;
                    case (int)SwitchConstants.Switches.CenterScoop:
                        break;
                    case (int)SwitchConstants.Switches.RightInnerOrbit:
                        break;
                    case (int)SwitchConstants.Switches.CapturedBall:
                        break;
                    case (int)SwitchConstants.Switches.RightScoop:
                        break;
                    case (int)SwitchConstants.Switches.RightOuterOrbit:
                        break;
                    case (int)SwitchConstants.Switches.TopOuterOrbit:
                        break;
                    case (int)SwitchConstants.Switches.LeftPop:
                    case (int)SwitchConstants.Switches.TopPop:
                    case (int)SwitchConstants.Switches.LowerPop:
                        break;
                    case (int)SwitchConstants.Switches.LeftFlipper:
                    case (int)SwitchConstants.Switches.RightFlipper:
                        break;
                    default:
                        break;
                }//end switch

            }
        }
    }
}

