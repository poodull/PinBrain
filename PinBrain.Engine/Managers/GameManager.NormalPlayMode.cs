using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Switch;
using PinBrain.Library.Feature;
using PinBrain.Engine.Constants;
using System.Threading;
using System.Threading.Tasks;
using PinBrain.Library.Map;

namespace PinBrain.Engine.Managers
{
    partial class GameManager
    {
        private class NormalPlayMode : IMode
        {
            public NormalPlayMode()
            {
                _log.Info("Normal Play Mode Started");
                _instance.ResetForNewPlayer();

                DisplayManager.OnAnimationCompleted += new DisplayEventHandler(DisplayManager_OnAnimationCompleted);
                SwitchManager.RegisterSwitchHandler(handleSwitchChanges);
            }

            #region * Dispose *
            private bool _isDisposed = false;
            //Call Dispose to free resources explicitly
            public void Dispose()
            {
                Dispose(true);
                //If dispose is called already then say GC to skip finalize on this instance.
                GC.SuppressFinalize(this);
            }

            ~NormalPlayMode()
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
                        if (CurrentInputMode == InputMode.NormalPlay)
                        {
                            if (changed.State == SwitchState.On)
                                _instance.BallDrained();
                        }
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
                        //if (_state[i].State == SwitchState.On)
                        //_instance.KickBack();
                        break;
                    case (int)SwitchConstants.Switches.BallShooterLane:
                        if (changed.State == SwitchState.Off && CurrentInputMode == InputMode.NormalPlay)
                        {
                            //was on, now is off and we're waiting to start the ball saver timer.
                            if (TroughManager.BallSaverState == BallSaverStates.Waiting)
                            {
                                TroughManager.StartBallSaverTimer();
                            }
                        }
                        break;
                    case (int)SwitchConstants.Switches.RightFlipperEOS:
                        break;
                    case (int)SwitchConstants.Switches.LeftFlipperEOS:
                        break;
                    case (int)SwitchConstants.Switches.LeftDrain:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            if (!_instance._isTilted) //this might need to be higher up.. maybe an input mode
                            {
                                _instance.AddScore(ScoreConstants.OUTLANE);
                                if (PlayerStatus.CurrentPlayer.HasShield)
                                {
                                    //SolenoidManager.FireKickBack();
                                    PlayerStatus.CurrentPlayer.HasShield = false; //decrement
                                }
                            }
                        }
                        break;
                    case (int)SwitchConstants.Switches.LeftReturn:
                        break;
                    case (int)SwitchConstants.Switches.LeftSling:
                    case (int)SwitchConstants.Switches.RightSling:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            _instance.AddScore(ScoreConstants.NONTARGET);
                        }
                        break;
                    case (int)SwitchConstants.Switches.RightReturn:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            _instance.AddScore(ScoreConstants.NONTARGET);
                        }
                        break;
                    case (int)SwitchConstants.Switches.RightDrain:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            if (!_instance._isTilted)
                            {
                                _instance.AddScore(ScoreConstants.OUTLANE);
                                PlayerStatus.CurrentPlayer.HasCross = false; //decrement
                            }
                        }
                        break;
                    case (int)SwitchConstants.Switches.DraculaD:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addDraculaLetter(0);
                        break;
                    case (int)SwitchConstants.Switches.DraculaR:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addDraculaLetter(1);
                        break;
                    case (int)SwitchConstants.Switches.DraculaA:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addDraculaLetter(2);
                        break;
                    case (int)SwitchConstants.Switches.DraculaC:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addDraculaLetter(3);
                        break;
                    case (int)SwitchConstants.Switches.DraculaU:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addDraculaLetter(4);
                        break;
                    case (int)SwitchConstants.Switches.DraculaL:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addDraculaLetter(5);
                        break;
                    case (int)SwitchConstants.Switches.DraculaA2:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addDraculaLetter(6);
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
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addBelmontLetter(0);
                        break;
                    case (int)SwitchConstants.Switches.BelmontE:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addBelmontLetter(1);
                        break;
                    case (int)SwitchConstants.Switches.BelmontL:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addBelmontLetter(2);
                        break;
                    case (int)SwitchConstants.Switches.BelmontM:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addBelmontLetter(3);
                        break;
                    case (int)SwitchConstants.Switches.BelmontO:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addBelmontLetter(4);
                        break;
                    case (int)SwitchConstants.Switches.BelmontN:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addBelmontLetter(5);
                        break;
                    case (int)SwitchConstants.Switches.BelmontT:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addBelmontLetter(6);
                        break;
                    case (int)SwitchConstants.Switches.LeftOuterOrbit:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            _instance.AddScore(ScoreConstants.NONTARGET);
                        }
                        break;
                    case (int)SwitchConstants.Switches.RampExit:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            if (PlayerStatus.CurrentPlayer.Paths.HasFlag(NavigationPaths.Up))
                                navigate(NavigationPaths.Up);
                            else
                                attack(false, int.MaxValue, false); //kill 1 enemy
                        }
                        break;
                    case (int)SwitchConstants.Switches.LeftInnerOrbit:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            _instance.AddScore(ScoreConstants.NONTARGET);
                        }
                        break;
                    case (int)SwitchConstants.Switches.BossTarget:
                        break;
                    case (int)SwitchConstants.Switches.CenterExit:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            if (PlayerStatus.CurrentPlayer.Paths.HasFlag(NavigationPaths.Straight))
                                navigate(NavigationPaths.Straight);
                            else
                                attack(false, int.MaxValue, false); //kill 1 enemy
                        }
                        break;
                    case (int)SwitchConstants.Switches.CenterScoop:
                        break;
                    case (int)SwitchConstants.Switches.RightInnerOrbit:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            _instance.AddScore(ScoreConstants.NONTARGET);
                        }
                        break;
                    case (int)SwitchConstants.Switches.CapturedBall:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                            addItem();
                        break;
                    case (int)SwitchConstants.Switches.RightScoop:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            if (PlayerStatus.CurrentPlayer.Paths.HasFlag(NavigationPaths.Down))
                                navigate(NavigationPaths.Down);
                            else
                                attack(false, int.MaxValue, false); //kill 1 enemy
                        }
                        break;
                    case (int)SwitchConstants.Switches.RightOuterOrbit:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            _instance.AddScore(ScoreConstants.NONTARGET);
                        }
                        break;
                    case (int)SwitchConstants.Switches.TopOuterOrbit:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            _instance.AddScore(ScoreConstants.NONTARGET);
                        }
                        break;
                    case (int)SwitchConstants.Switches.LeftPop:
                    case (int)SwitchConstants.Switches.TopPop:
                    case (int)SwitchConstants.Switches.LowerPop:
                        if (changed.State == SwitchState.On && CurrentInputMode == InputMode.NormalPlay)
                        {
                            addHeart();
                            attack(false, 1, true);
                        }
                        break;
                    case (int)SwitchConstants.Switches.LeftFlipper:
                    case (int)SwitchConstants.Switches.RightFlipper:

                        if (CurrentInputMode == InputMode.SelectPlayer)
                        {
                            if (changed.State == SwitchState.Off) //flip players on buttonUP
                                if (changed.Id == (int)SwitchConstants.Switches.LeftFlipper)
                                    DisplayManager.CharacterPrevious();
                                else
                                    DisplayManager.CharacterNext();
                            else
                            {
                                //find out if both buttons are currently on.
                                Switch left = SwitchManager.GetSwitch(SwitchConstants.Switches.LeftFlipper);
                                Switch right = SwitchManager.GetSwitch(SwitchConstants.Switches.RightFlipper);

                                if (left.State == SwitchState.On &&
                                    right.State == SwitchState.On)
                                {
                                    if (PlayerStatus.CurrentPlayer.PlayerCharacter == Characters.unknown)
                                    {
                                        string player = DisplayManager.GetCharacterSelection();
                                        switch (player.ToLower())
                                        {
                                            case "maria":
                                                PlayerStatus.CurrentPlayer.PlayerCharacter = Characters.Maria;
                                                break;
                                            case "sypha":
                                                PlayerStatus.CurrentPlayer.PlayerCharacter = Characters.Sypha;
                                                break;
                                            case "grant":
                                                PlayerStatus.CurrentPlayer.PlayerCharacter = Characters.Grant;
                                                break;
                                            case "alucard":
                                                PlayerStatus.CurrentPlayer.PlayerCharacter = Characters.Alucard;
                                                break;
                                            case "richter":
                                            default:
                                                PlayerStatus.CurrentPlayer.PlayerCharacter = Characters.Richter;
                                                break;
                                        }
                                        _instance.StartPlayIntro();
                                    }
                                }
                            }
                        }
                        //else
                        //    continue; //TODO: check for long press status update
                        break;
                    default:
                        break;
                }//end switch
            }

            private void navigate(NavigationPaths direction)
            {
                //if we're here, it's because we can be (not blocked)

                PlayerStatus.CurrentPlayer.Navigate(direction);
                if (PlayerStatus.CurrentPlayer.RoomType != RoomTypes.Trophy)
                    enterRoom();
                else
                    enterTrophyRoom();
            }

            private void enterTrophyRoom()
            {
                ModeStart(GameManagerConstants.GameModes.TROPHYROOM);
                //rotate center stage to CenterScoop and wait for CenterScoop trigger
                //Disable scoring
                //if the user drains the ball, we just don't give them a captured ball bonus
            }

            private void attack(bool isMagic, int power, bool isSplash = false)
            {
                foreach (IEnemy e in PlayerStatus.CurrentPlayer.Attack(isMagic, power, isSplash))
                {
                    _log.WarnFormat("killing a {0}", e.EnemyType);
                    DisplayManager.KillEnemy(e);
                }
                if (!PlayerStatus.CurrentPlayer.Paths.HasFlag(NavigationPaths.Blocked))
                {
                    if (PlayerStatus.CurrentPlayer.RoomType == RoomTypes.Boss) //!BOSS IS DEAD!!!
                    {
                        //play DEADBOSS animation
                        //Go to trophy room 
                        navigate(NavigationPaths.Straight);
                    }
                    else
                    {
                        DisplayManager.ShowNavigation(PlayerStatus.CurrentPlayer.Paths);
                    }
                }
            }

            public void DisplayManager_OnAnimationCompleted(DisplayEventArgs e)
            {
                try
                {
                    //what animation completed?
                    if (e == null || e.SceneName == null)
                        return;
                    switch (e.SceneName)
                    {
                        case DisplayConstants.CutScenes.MapMode.MAP:
                            CurrentInputMode = InputMode.NormalPlay;
                            //we displayed the map, time to show the user status.
                            //if (!statusUpdate) //what if we just long-pressed for status update?
                            beginPlay();
                            break;
                        case DisplayConstants.CutScenes.Bonuses.COLLECTBONUS:
                            //Call CollectBonusCompleted
                            GameManager._instance.BonusCollected();
                            break;
                        case DisplayConstants.CutScenes.ActiveGameMode.ADDLETTER:
                            //what to do if we've finished this animation?
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
                enterRoom();

                TroughManager.BallSaverState = BallSaverStates.Waiting; //waiting for ejection
                TroughManager.EjectBall();
            }

            /// <summary>
            /// Adds enemies to the room and gets navigation set up right.
            /// </summary>
            private void enterRoom()
            {
                DisplayManager.ClearEnemies();
                //add the enemies
                foreach (IEnemy e in PlayerStatus.CurrentPlayer.EnemiesInRoom)
                {
                    DisplayManager.AddEnemy(e);
                }
                DisplayManager.ShowNavigation(PlayerStatus.CurrentPlayer.Paths);
            }

            private void addBelmontLetter(int letterIndex)
            {
                if (!_instance._isTilted)
                {
                    if (PlayerStatus.CurrentPlayer.AddBelmontLetter(letterIndex)) //true if new card
                    {
                        Dictionary<string, string> args = new Dictionary<string, string>();
                        args.Add("BELMONT", "TRUE");
                        args.Add("FLIPPED", letterIndex.ToString());
                        DisplayManager.PlayCutScene(DisplayConstants.CutScenes.ActiveGameMode.ADDLETTER, args);
                        _instance.AddScore(ScoreConstants.LETTERNEW);
                    }
                    else
                        _instance.AddScore(ScoreConstants.LETTER);
                }
            }

            private void addDraculaLetter(int letterIndex)
            {
                if (!_instance._isTilted)
                {
                    if (PlayerStatus.CurrentPlayer.AddDraculaLetter(letterIndex)) //true if new card
                    {
                        Dictionary<string, string> args = new Dictionary<string, string>();
                        args.Add("DRACULA", "TRUE");
                        args.Add("FLIPPED", letterIndex.ToString());
                        DisplayManager.PlayCutScene(DisplayConstants.CutScenes.ActiveGameMode.ADDLETTER, args);
                        _instance.AddScore(ScoreConstants.LETTERNEW);
                    }
                    else
                        _instance.AddScore(ScoreConstants.LETTER);
                }
            }

            private void addItem()
            {
                if (!_instance._isTilted)
                {
                    //TODO: logic if we should collect
                    collectItem();
                }

            }

            private void addHeart()
            {
                if (!_instance._isTilted)
                {
                    PlayerStatus.CurrentPlayer.Hearts++;
                    handleScoreChanged(ScoreConstants.ITEM_HEART, SoundConstants.Sfx.Heart, LightingConstants.Lfx.None);
                }
            }

            private void collectItem()
            {
                //todo 
                switch (PlayerStatus.CurrentPlayer.RoomItem)
                {
                    case Items.moneysmall:
                        handleScoreChanged(ScoreConstants.ITEM_MONEYSMALL, SoundConstants.Sfx.Item, LightingConstants.Lfx.Candle1111);
                        break;
                    case Items.moneymedium:
                        handleScoreChanged(ScoreConstants.ITEM_MONEYMEDIUM, SoundConstants.Sfx.Item, LightingConstants.Lfx.Candle1111);
                        break;
                    case Items.moneybig:
                        handleScoreChanged(ScoreConstants.ITEM_MONEYBIG, SoundConstants.Sfx.Item, LightingConstants.Lfx.Candle1111);
                        break;
                    case Items.weaponup:
                        if (PlayerStatus.CurrentPlayer.Weapon < PlayerStatus.WEAPONMAX)
                            PlayerStatus.CurrentPlayer.Weapon++;
                        handleScoreChanged(ScoreConstants.ITEM_HEART, SoundConstants.Sfx.Item, LightingConstants.Lfx.Candle1111);
                        break;
                    case Items.magicup:
                        if (PlayerStatus.CurrentPlayer.Magic < PlayerStatus.MAGICMAX)
                            PlayerStatus.CurrentPlayer.Magic++;
                        handleScoreChanged(ScoreConstants.ITEM_HEART, SoundConstants.Sfx.Item, LightingConstants.Lfx.Candle1111);
                        break;
                    case Items.bonusx:
                        if (PlayerStatus.CurrentPlayer.BonusMultiplier < PlayerStatus.MAXBONUSMULTIPLIER)
                            PlayerStatus.CurrentPlayer.BonusMultiplier++;
                        handleScoreChanged(ScoreConstants.ITEM_HEART, SoundConstants.Sfx.Item, LightingConstants.Lfx.Candle1111);
                        break;
                    case Items.balllock:

                        break;
                    case Items.candle:
                    case Items.heart:
                        addHeart();
                        break;
                    default:
                        handleScoreChanged(ScoreConstants.ITEM_HEART, SoundConstants.Sfx.Item, LightingConstants.Lfx.None);
                        break;
                }
                PlayerStatus.CurrentPlayer.CollectItem();
                DisplayManager.SetGameStatus(PlayerStatus.CurrentPlayer);
            }

            private void handleScoreChanged(int points, SoundConstants.Sfx sound, LightingConstants.Lfx lightEffect)
            {
                if (points > 0)
                    GameManager.getInstance().AddScore(points);
                if (sound != SoundConstants.Sfx.None)
                    SoundManager.PlaySfx(sound);
                if (lightEffect != LightingConstants.Lfx.None)
                    LightManager.PlaySequence(lightEffect);

            }
        }
    }
}
