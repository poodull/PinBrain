//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using PinBrain.Library.Feature;
//using PinBrain.Library.Switch;
//using PinBrain.Engine.Constants;

//namespace PinBrain.Engine.Managers
//{
//    partial class GameManager
//    {
//        private class ActiveGameMode : IMode
//        {
//            private bool _isDisposed = false;

//            /// <summary>
//            /// Constructor.  Game mode just started, so no player status is set
//            /// </summary>
//            public ActiveGameMode()
//            {
//                PlayerStatus.Reset();
//                DisplayManager.OnAnimationCompleted += new DisplayEventHandler(DisplayManager_OnAnimationCompleted);
//                DisplayManager.PlaySequence(DisplayConstants.Modes.ActiveGameMode.SubModes.CharacterSelectMode.CHARSELECT); //send high scores
//            }

//            #region * Dispose *
//            //Call Dispose to free resources explicitly
//            public void Dispose()
//            {
//                Dispose(true);
//                //If dispose is called already then say GC to skip finalize on this instance.
//                GC.SuppressFinalize(this);
//            }

//            ~ActiveGameMode()
//            {
//                Dispose(false);
//            }

//            //Implement dispose to free resources
//            protected virtual void Dispose(bool disposing)
//            {
//                if (!_isDisposed)
//                {
//                    _isDisposed = true;
//                    // Released unmanaged Resources
//                    if (disposing)
//                    {
//                        // Released managed Resources
//                    }
//                }
//            }
//            #endregion

//            public void SwitchStateChanged(Switch[] _state)
//            {
//                for (int i = 0; i < _state.Length; i++)
//                {
//                    switch (_state[i].Id)
//                    {
//                        case (int)SwitchConstants.Switches.PlumbTilt:
//                            break;
//                        case (int)SwitchConstants.Switches.Start:
//                            //select user
//                            if (_state[i].State == SwitchState.On)
//                            {
//                                string player = DisplayManager.GetCharacterSelection();
//                                switch (player.ToLower())
//                                {
//                                    case "maria":
//                                        PlayerStatus.CurrentPlayer().SetCharacter(Characters.Maria);
//                                        break;
//                                    case "sypha":
//                                        PlayerStatus.CurrentPlayer().SetCharacter(Characters.Sypha);
//                                        break;
//                                    case "grant":
//                                        PlayerStatus.CurrentPlayer().SetCharacter(Characters.Grant);
//                                        break;
//                                    case "alucard":
//                                        PlayerStatus.CurrentPlayer().SetCharacter(Characters.Alucard);
//                                        break;
//                                    case "richter":
//                                    default:
//                                        PlayerStatus.CurrentPlayer().SetCharacter(Characters.Richter);
//                                        break;
//                                }

//                                //Character is selected, now let's show the map.
//                                DisplayManager.PlayCutScene(DisplayConstants.CutScenes.MapMode.Map);
//                            }
//                            break;
//                        case (int)SwitchConstants.Switches.Coin:
//                            if (_state[i].State == SwitchState.On)
//                                _instance.AddCredit(); //
//                            break;
//                        case (int)SwitchConstants.Switches.SlamTilt:
//                            break;
//                        case (int)SwitchConstants.Switches.MenuSelect:
//                            break;
//                        case (int)SwitchConstants.Switches.MenuBack:
//                            break;
//                        case (int)SwitchConstants.Switches.MenuNext:
//                            break;
//                        case (int)SwitchConstants.Switches.MenuExit:
//                            break;
//                        case (int)SwitchConstants.Switches.Outhole:
//                            break;
//                        case (int)SwitchConstants.Switches.RightBallTrough:
//                            break;
//                        case (int)SwitchConstants.Switches.RightMidBallTrough:
//                            break;
//                        case (int)SwitchConstants.Switches.MidBallTrough:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftMidBallTrough:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftBallTrough:
//                            break;
//                        case (int)SwitchConstants.Switches.BallShooterLane:
//                            break;
//                        case (int)SwitchConstants.Switches.RightFlipperEOS:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftFlipperEOS:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftDrain:
//                            if (_state[i].State == SwitchState.On)
//                                _instance.BallDrained();
//                            break;
//                        case (int)SwitchConstants.Switches.LeftReturn:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftSling:
//                            break;
//                        case (int)SwitchConstants.Switches.RightSling:
//                            break;
//                        case (int)SwitchConstants.Switches.RightReturn:
//                            break;
//                        case (int)SwitchConstants.Switches.RightDrain:
//                            if (_state[i].State == SwitchState.On)
//                                _instance.BallDrained();
//                            break;
//                        case (int)SwitchConstants.Switches.DraculaD:
//                        case (int)SwitchConstants.Switches.DraculaR:
//                        case (int)SwitchConstants.Switches.DraculaA:
//                        case (int)SwitchConstants.Switches.DraculaC:
//                        case (int)SwitchConstants.Switches.DraculaU:
//                        case (int)SwitchConstants.Switches.DraculaL:
//                        case (int)SwitchConstants.Switches.DraculaA2:
//                            if (_state[i].State == SwitchState.On)
//                                _instance.addDraculaLetter(_state[i].Id);
//                            break;
//                        case (int)SwitchConstants.Switches.BallPopper:
//                            break;
//                        case (int)SwitchConstants.Switches.DropTargetA:
//                            break;
//                        case (int)SwitchConstants.Switches.DropTargetB:
//                            break;
//                        case (int)SwitchConstants.Switches.DropTargetC:
//                            break;
//                        case (int)SwitchConstants.Switches.DropTargetD:
//                            break;
//                        case (int)SwitchConstants.Switches.BelmontB:
//                        case (int)SwitchConstants.Switches.BelmontE:
//                        case (int)SwitchConstants.Switches.BelmontL:
//                        case (int)SwitchConstants.Switches.BelmontM:
//                        case (int)SwitchConstants.Switches.BelmontO:
//                        case (int)SwitchConstants.Switches.BelmontN:
//                        case (int)SwitchConstants.Switches.BelmontT:
//                            if (_state[i].State == SwitchState.On)
//                                _instance.addBelmontLetter(_state[i].Id);
//                            break;
//                        case (int)SwitchConstants.Switches.LeftOuterOrbit:
//                            break;
//                        case (int)SwitchConstants.Switches.RampExit:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftInnerOrbit:
//                            break;
//                        case (int)SwitchConstants.Switches.BossTarget:
//                            break;
//                        case (int)SwitchConstants.Switches.CenterExit:
//                            break;
//                        case (int)SwitchConstants.Switches.CenterScoop:
//                            break;
//                        case (int)SwitchConstants.Switches.RightInnerOrbit:
//                            break;
//                        case (int)SwitchConstants.Switches.CapturedBall:
//                            if (_state[i].State == SwitchState.On)
//                                _instance.addItem();
//                            break;
//                        case (int)SwitchConstants.Switches.RightScoop:
//                            break;
//                        case (int)SwitchConstants.Switches.RightOuterOrbit:
//                            break;
//                        case (int)SwitchConstants.Switches.TopOuterOrbit:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftPop:
//                            break;
//                        case (int)SwitchConstants.Switches.TopPop:
//                            break;
//                        case (int)SwitchConstants.Switches.LowerPop:
//                            break;
//                        case (int)SwitchConstants.Switches.LeftFlipper:
//                            if (_state[i].State == SwitchState.On)
//                                DisplayManager.CharacterPrevious();
//                            break;
//                        case (int)SwitchConstants.Switches.RightFlipper:
//                            if (_state[i].State == SwitchState.On)
//                                DisplayManager.CharacterNext();
//                            break;
//                        default:
//                            break;
//                    }
//                }
//            }

//            public void DisplayManager_OnAnimationCompleted(DisplayEventArgs e)
//            {
//                //what animation completed?
//                if (e == null || e.SceneName == null)
//                    return;
//                switch (e.SceneName)
//                {
//                    default:
//                        break;
//                }
//            }

//        }
//    }
//}
