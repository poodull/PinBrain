using System;

using log4net;

using PinBrain.Engine.Constants;
using PinBrain.Library.Feature;
using PinBrain.Library.Switch;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace PinBrain.Engine.Managers
{
    public enum InputMode
    {
        NoInput,
        NormalPlay,
        SelectPlayer,
        Merchant,
        Attract,
        Test
    }
    public partial class GameManager
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static GameManager _instance;

        protected IMode _currentMode;

        private InputMode _inputMode = InputMode.Attract;

        public static InputMode CurrentInputMode
        {
            get { return _instance._inputMode; }
            private set { _instance._inputMode = value; }
        }

        protected bool _isTilted = false;

        ////protected CancellationTokenSource _troughBallManagerCancelSource;

        /// <summary>
        /// Queue of switch events that need to be processed.
        /// </summary>
        protected ConcurrentQueue<Switch> _switchChanges;

        /// <summary>
        /// Diagnostic to find out how long switches take.
        /// </summary>
        System.Diagnostics.Stopwatch _switchHandleTime = new System.Diagnostics.Stopwatch();

        private GameManager()
        {
            _log.Info("Initializing GameManager...");

            _switchChanges = new ConcurrentQueue<Switch>();

            //_troughBallManagerCancelSource = new CancellationTokenSource();
            //Task.Factory.StartNew(() => troughBallManagerTask(), _troughBallManagerCancelSource.Token);
            //_ballSaverTimer = new Timer(new TimerCallback((state) => { _isSaveBallTimerOn = false; }));
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns></returns>
        protected static GameManager getInstance()
        {
            if (_instance == null)
                _instance = new GameManager();

            return _instance;
        }

        #region * Game state management methods *
        /// <summary>
        /// Occurs when the tilt switches are tripped.
        /// </summary>
        /// <param name="warnings"></param>
        public void TiltWarning(int warnings) { }

        /// <summary>
        /// Occurs when the number of tilt warnings are exceeded.
        /// </summary>
        public void Tilted()
        {
            _isTilted = true;
        }

        /// <summary>
        /// The tilt recovery timer waits for all the balls to drain before
        /// continuing on.
        /// </summary>
        public void TiltRecovery() { }

        /// <summary>
        /// Adds a player to the player count.
        /// </summary>
        internal void AddPlayer()
        {
            if (PlayerStatus.CurrentPlayer == null || PlayerStatus.CurrentPlayer.NumPlayers < 4)
            {
                PlayerStatus.AddPlayer();
                SoundManager.PlaySfx(SoundConstants.Sfx.Start);
            }
        }
        /// <summary>
        /// Adds a credit
        /// </summary>
        internal void AddCredit()
        {
            SoundManager.PlaySfx(SoundConstants.Sfx.Credit);
        }

        /// <summary>
        /// Initialize the table for a new game.
        /// </summary>
        public void ResetForNewGame()
        {
            PlayerStatus.Reset();
            AddPlayer();

            //TroughManager.CheckForStuckBall()
            //check quickly
            if (!TroughManager.WaitForAllBalls(5, false))
            {
                DisplayManager.PlayCutScene(DisplayConstants.CutScenes.Test.BALLSEARCH);
            }
            //ok, we've got a stuck ball somewhere.
            int attempts = 0;
            while (attempts < 5 && !TroughManager.WaitForAllBalls(5000, true))
            {
                DisplayManager.PlayCutScene(DisplayConstants.CutScenes.Test.BALLSEARCH);
                attempts++;
            }

            _isTilted = false;
        }

        /// <summary>
        /// Reinitialize the Table for a new ball (either a new ball after
        /// the player has lost one or we have moved onto the next player.
        /// </summary>
        public void ResetForNewPlayer()
        {
            TroughManager.Reset();
            _isTilted = false;
            Task.Factory.StartNew(() =>
            {
                if (PlayerStatus.CurrentPlayer.PlayerCharacter == Characters.unknown)
                {
                    _inputMode = InputMode.SelectPlayer;
                    DisplayManager.PlaySequence(DisplayConstants.Modes.ActiveGameMode.SubModes.CharacterSelectMode.CHARSELECT); //send high scores
                }
                else
                {
                    //Need to task this out otherwise we get a AccessViolation and currupt memory.  unknown
                    StartPlayIntro();
                }
            });
        }

        /// <summary>
        /// The timer used to delay the start of a game to allow attract
        /// sequence to complete.  When it expires it calls CreateNewBall().
        /// </summary>
        public void FirstBallTimerExpired() { } //not used

        /// <summary>
        /// The player has lost his ball (there are no more balls on the 
        /// playfield).
        /// </summary>
        public void EndOfBall()
        {
            _log.Debug("End of Ball for Player " + PlayerStatus.PlayerUp);
            SwitchManager.EnableFlippers(false);
            _inputMode = InputMode.NoInput;
            SoundManager.StopAllMusic();
            SoundManager.PlaySfx(SoundConstants.Sfx.Death1);
            CollectBonus();
        }

        /// <summary>
        /// Collect and reset the bonus points for this player.
        /// </summary>
        public void CollectBonus()
        {
            _log.Debug("Calculating Bonus for Player " + PlayerStatus.PlayerUp);
            //calculate bonus
            DisplayManager.PlayCutScene(DisplayConstants.CutScenes.Bonuses.COLLECTBONUS);
            //now actually add the value to his score
            //PlayerStatus.CurrentPlayer.Score = ????
        }

        /// <summary>
        /// The Timer which delays the machine to allow any bonus points to 
        /// be added up has expired.  Check for Extra Balls and last ball.
        /// </summary>
        public void BonusCollected()
        {
            //animation's over, now check for extra ball or advance the player.
            if (PlayerStatus.CurrentPlayer.ExtraBalls > 0)
            {
                PlayerStatus.CurrentPlayer.ExtraBalls--; //you can keep winning infinite extra balls without a count on how many were used.
            }
            else
            {
                //THIS SHOULD BE THE ONLY PLACE WE ADVANCE THE PLAYER
                PlayerStatus.CurrentPlayer.Ball++;
                PlayerStatus.CurrentPlayer.ResetPlayerStatus();
                PlayerStatus.NextPlayer(); //advance the player if there are no more extra balls
            }
            //Check if the last player's ball count is over the total limit.
            if (PlayerStatus.LastPlayer.Ball > 3)//TODO: Add Balls Per Play config setting
                EndOfGame();
            else
            {
                ResetForNewPlayer(); //right location for this?
            }
        }

        /// <summary>
        /// Plays the player intro (MAP) and primes the table for the player status.
        /// </summary>
        public void StartPlayIntro()
        {
            //select user
            //ask the credit manager if we have credits
            PlayerStatus.CurrentPlayer.PlayerHealthStatus = "Player "
                + PlayerStatus.PlayerUp + " is up";

            DisplayManager.PlaySequence(DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE);
            //Character is selected, now let's show the map.
            DisplayManager.PlayCutScene(DisplayConstants.CutScenes.MapMode.MAP);
            SoundManager.PlayMusic(SoundConstants.Music.LevelStart, false);
            SoundManager.PlayMusic(SoundConstants.Music.Level1Normal, true);
            //DisplayManager.SetGameStatus(PlayerStatus.CurrentPlayer);
            SwitchManager.EnableFlippers(true);
        }

        /// <summary>
        /// Occurs when the end of bonus display and it either ends the game
        /// or moves to next player.
        /// </summary>
        public void EndOfBallComplete() { }

        /// <summary>
        /// Called at the end of the game, after all players have lost all 
        /// balls.
        /// </summary>
        public void EndOfGame()
        {
            PlayerStatus.Reset();
            //Have to Task this out otherwise we get a memory access violation
            Task.Factory.StartNew(() =>
            {
                ModeStart(GameManagerConstants.GameModes.ATTRACT);
            });
            _inputMode = InputMode.Attract;
        }

        /// <summary>
        /// Starts the Match sequence.
        /// </summary>
        public void Match() { }

        /// <summary>
        /// Award one or more credits.
        /// </summary>
        /// <param name="credits"></param>
        public void AwardFreeCredit(int credits)
        {
            //fire knocker
            SwitchManager.FireKnocker();
            for (int i = 0; i < credits; i++)
            {
                AddCredit();
            }
        }

        /// <summary>
        /// All players done, go back to Attract.
        /// </summary>
        public void EndPlay()
        {
            DisplayManager.PlaySequence(DisplayConstants.Modes.AttractMode.ATTRACT); //send high scores
        }

        /// <summary>
        /// Player lost a ball.  This is where we decrement the number of 
        /// balls on the playfield and test for end of game or end of ball.
        /// </summary>
        public void BallDrained()
        {
            bool ballLost = TroughManager.BallDrained(_isTilted);

            //check if that's the last ball
            if (ballLost) //that last outlane counted.
            {
                //time to check the trough if it's full
                Task.Factory.StartNew(() =>
                {
                    int counter = 2;
                    bool isTroughFull;
                    do
                    {
                        isTroughFull = TroughManager.WaitForAllBalls(300, false);
                        counter--;
                    } while (!isTroughFull || counter >= 0);
                    if (isTroughFull) EndOfBall();
                });
            }
            else //ball was saved.  play animation
            {
                DisplayManager.PlayCutScene(DisplayConstants.CutScenes.Bonuses.SHOOTAGAIN);
            }
        }

        //private void troughBallManagerTask()
        //{
        //    while (!_troughBallManagerCancelSource.IsCancellationRequested) //change to allow for Task Cancelling
        //    {
        //        while (_ballsOnField > 0)
        //        {
        //            //if there any number of auto-plunged balls, start ejecting everything
        //            //if there is a ball in the shooter lane, try to auto-plunge and wait until the lane clears (2s).
        //            //SwitchManager.GetState(SwitchConstants.Switches.BallShooterLane).State = On;
        //            //switchmanager.autoplunge();
        //            System.Threading.Thread.Sleep(2000);
        //        }
        //        System.Threading.Thread.Sleep(2000);
        //    }
        //}

        /// <summary>
        /// Add points to the score and update the scoreboard.
        /// </summary>
        /// <param name="points">The number of points scored.</param>
        public void AddScore(int points)
        {
            PlayerStatus.CurrentPlayerScore += points;
        }

        /// <summary>
        /// The player has completed a jackpot score.
        /// </summary>
        /// <param name="points">The number of points for this jackpot.</param>
        public void AddJackpot(int points)
        {
            PlayerStatus.CurrentPlayerScore += points;
            //PlayerStatus.CurrentPlayer.JackpotCount++;
        }

        /// <summary>
        /// Increment the playfield multiplier by this amount.
        /// </summary>
        /// <param name="amount"></param>
        public void IncrementBonusMultiplier(float amount)
        {
            PlayerStatus.CurrentPlayer.BonusMultiplier += amount;
        }

        /// <summary>
        /// Set the Bonus Multiplier to the specified amount.
        /// </summary>
        /// <param name="level">The bonus level.</param>
        public void SetBonusMultiplier(float level)
        {
            PlayerStatus.CurrentPlayer.BonusMultiplier = level;
        }

        /// <summary>
        /// Award a special to the current player
        /// </summary>
        public void AwardSpecial() { }

        /// <summary>
        /// The player won an extra ball
        /// </summary>
        public void AwardExtraBall() { PlayerStatus.CurrentPlayer.ExtraBalls++; }

        private void reset()
        {

        }
        #endregion

        internal static void Reset()
        {
            getInstance().reset();
        }

        /// <summary>
        /// Changes the current mode.  Use GameManagerConstants for values.
        /// </summary>
        internal static void ModeStart(GameManagerConstants.GameModes mode)
        {
            switch (mode)
            {
                case GameManagerConstants.GameModes.TEST:
                case GameManagerConstants.GameModes.ATTRACT:
                    //getInstance().EndPlay();
                    if (getInstance()._currentMode != null)
                        getInstance()._currentMode.Dispose();
                    getInstance()._currentMode = new AttractMode();
                    break;
                //case GameManagerConstants.GameModes.STARTGAME:
                //    if (getInstance()._currentMode != null)
                //        getInstance()._currentMode.Dispose();
                //    getInstance()._currentMode = new ActiveGameMode();
                //    break;
                case GameManagerConstants.GameModes.NORMALPLAY:
                    if (getInstance()._currentMode != null)
                        getInstance()._currentMode.Dispose();
                    getInstance()._currentMode = new NormalPlayMode();
                    //getInstance().ResetForNewGame(); //this is done in the constructor
                    break;
                case GameManagerConstants.GameModes.TROPHYROOM:
                    if (getInstance()._currentMode != null)
                        getInstance()._currentMode.Dispose();
                    getInstance()._currentMode = new TrophyRoomMode();
                    break;
                default:
                    _log.Error("UNKNOWN MODE CALLED! " + mode);
                    break;
            }
        }
    }
}
