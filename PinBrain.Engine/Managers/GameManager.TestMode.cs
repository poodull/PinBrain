using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Feature;
using PinBrain.Engine.Constants;
using PinBrain.Library.Switch;
using System.Threading;
using System.Threading.Tasks;


namespace PinBrain.Engine.Managers
{
    partial class GameManager
    {
        private class TestMode : IMode
        {
            private bool _isDisposed = false;
            Task _switchHandleTask;
            CancellationTokenSource _cancelTokenSource;

            public TestMode()
            {
                //DisplayManager.PlaySequence(DisplayConstants.ATTRACT); //send high scores
                SwitchManager.RegisterSwitchHandler(handleSwitchChanges);
            }

            #region * DISPOSE *
            //Call Dispose to free resources explicitly
            public void Dispose()
            {
                Dispose(true);
                //If dispose is called already then say GC to skip finalize on this instance.
                GC.SuppressFinalize(this);
            }

            ~TestMode()
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
                        if (_cancelTokenSource != null)
                            _cancelTokenSource.Cancel();
                    }
                }
            }
            #endregion

            private void handleSwitchChanges(Switch changed)
            {
                switch (changed.Id)
                {
                    case (int)SwitchConstants.Switches.PlumbTilt:
                        break;
                    case (int)SwitchConstants.Switches.Start:
                        break;
                    case (int)SwitchConstants.Switches.Coin:
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
                        break;
                    case (int)SwitchConstants.Switches.RightSling:
                        break;
                    case (int)SwitchConstants.Switches.RightReturn:
                        break;
                    case (int)SwitchConstants.Switches.RightDrain:
                        break;
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
                    //case (int)SwitchConstants.Switches.DropTargetD:
                    //    break;
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
                        break;
                    case (int)SwitchConstants.Switches.TopPop:
                        break;
                    case (int)SwitchConstants.Switches.LowerPop:
                        break;
                    case (int)SwitchConstants.Switches.LeftFlipper:
                        break;
                    case (int)SwitchConstants.Switches.RightFlipper:
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
