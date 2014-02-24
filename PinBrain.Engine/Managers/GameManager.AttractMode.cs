using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Feature;
using PinBrain.Library.Switch;
using PinBrain.Engine.Constants;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace PinBrain.Engine.Managers
{
    public partial class GameManager
    {
        private class AttractMode : IMode
        {
            private bool _isDisposed = false;

            public AttractMode()
            {
                _log.Info("Attract Mode Started");
                DisplayManager.PlaySequence(DisplayConstants.Modes.AttractMode.ATTRACT); //send high scores
                DisplayManager.OnAnimationCompleted += new DisplayEventHandler(DisplayManager_OnAnimationCompleted);
                SwitchManager.RegisterSwitchHandler(handleSwitchChanges);
                SwitchManager.Reset();
            }

            #region * Dispose *
            //Call Dispose to free resources explicitly
            public void Dispose()
            {
                Dispose(true);
                //If dispose is called already then say GC to skip finalize on this instance.
                GC.SuppressFinalize(this);
            }

            ~AttractMode()
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
                        case DisplayConstants.Modes.AttractMode.ATTRACT:
                            DisplayManager.PlaySequence(DisplayConstants.Modes.AttractMode.ATTRACT); //send high scores
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

            private void handleSwitchChanges(Switch changed)
            {
                if (_isDisposed)
                    return;
                switch (changed.Id)
                {
                    case (int)SwitchConstants.Switches.Start:
                        if (changed.State == SwitchState.On)
                        {
                            if (CurrentInputMode == InputMode.Attract)
                            {
                                //Start button Calls 'StartPressed' flash API
                                DisplayManager.StartPressed();

                                _instance.ResetForNewGame();
                                ModeStart(GameManagerConstants.GameModes.NORMALPLAY);
                            }
                        }
                        break;
                    case (int)SwitchConstants.Switches.Coin:
                        if (changed.State == SwitchState.On)
                        {
                            if (CurrentInputMode == InputMode.Attract)
                            {
                                //add credit
                                GameManager.getInstance().AddCredit();
                                DisplayManager.GotoTitleScreen();
                            }
                        }
                        break;
                    case (int)SwitchConstants.Switches.LeftFlipper:
                        DisplayManager.GoToHighScores();
                        break;
                    case (int)SwitchConstants.Switches.RightFlipper:
                        DisplayManager.GotoTitleScreen();
                        break;
                    default:
                        break; //do nothing.
                }//end switch
            }
            //_log.DebugFormat("Switch {0} {1} took {2} ms.", changed.Name, changed.State, _instance._switchHandleTime.ElapsedMilliseconds);
            //    }// end while (!cancelled)

            //}
            //catch (Exception ex)
            //{
            //    _log.Error("Error in Attract Mode:", ex);
            //}
            //_log.Info("Leaving Normal Mode Switch Handler");

        }
    }
}
