using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using log4net;

using PinBrain.Devices.Display.Flash;
using PinBrain.Engine.Constants;
using PinBrain.Library.Feature;
using System.Threading.Tasks;
using PinBrain.Library.Map;

namespace PinBrain.Engine.Managers
{
    public class DisplayManager
    {
        /// <summary>
        /// Republished event from any animation's completed event.
        /// </summary>
        public static event DisplayEventHandler OnAnimationCompleted;

        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static DisplayManager _instance;
        private FlashHost _display;

        /// <summary>
        /// The current mode we're supposed to be displaying.
        /// </summary>
        private string _currentMode = DisplayConstants.UNKNOWN;
        Task _playerStatusChangedHandler;
        CancellationTokenSource _cancelTokenSource;

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        private DisplayManager()
        {
            _log.Info("Initiating DisplayManager...");
            ////this is where we load the display controller.
            ////I should reflect the implementation I need,
            ////but for now we'll hardcode it.

            _display = new FlashHost();
            _display.OnAnimationCompleted += new DisplayEventHandler(_display_OnAnimationCompleted);

            _cancelTokenSource = new CancellationTokenSource(); //used to stop the task in dispose
            _playerStatusChangedHandler = new Task(() => handlePlayerStatusChanges(_cancelTokenSource.Token),
                _cancelTokenSource.Token, TaskCreationOptions.LongRunning);
            _playerStatusChangedHandler.Start();
        }
        private void handlePlayerStatusChanges(CancellationToken cancelToken)
        {
            try
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    while (!PlayerStatus.IsDirty && !cancelToken.IsCancellationRequested)
                        Thread.Sleep(1);
                    if (cancelToken.IsCancellationRequested)
                        continue; //break out
                    SetGameStatus(PlayerStatus.CurrentPlayer);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in handlePlayerStatusChanges.", ex);
            }
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        protected static DisplayManager getInstance()
        {
            if (_instance == null)
                _instance = new DisplayManager();

            return _instance;
        }

        private void _display_OnAnimationCompleted(DisplayEventArgs e)
        {
            if (OnAnimationCompleted != null)
                OnAnimationCompleted(e);
        }

        public static void PlaySequence(string sequenceName)
        {
            PlaySequence(sequenceName, -1);
        }

        public static void PlaySequence(string sequenceName, int scoreValue)
        {
            getInstance().playSequence(sequenceName, scoreValue);
        }

        internal static void Reset()
        {
            getInstance()._display.Reset();
        }

        internal static void GoToHighScores()
        {
            if (GameManager.CurrentInputMode == InputMode.Attract)
                getInstance()._display.SendCommandBackground(DisplayConstants.Modes.AttractMode.Commands.GOTOHIGHSCORES);
        }

        internal static void GotoTitleScreen()
        {
            //if currentmode == attract
            if (GameManager.CurrentInputMode == InputMode.Attract)
                getInstance()._display.SendCommandBackground(DisplayConstants.Modes.AttractMode.Commands.GOTOTITLE);
        }

        internal static void StartPressed()
        {
            if (GameManager.CurrentInputMode == InputMode.Attract)
                getInstance()._display.SendCommandBackground(DisplayConstants.Modes.AttractMode.Commands.STARTPRESSED);
        }

        internal static void CharacterPrevious()
        {
            getInstance()._display.CharacterPrevious();
        }

        internal static void CharacterNext()
        {
            getInstance()._display.CharacterNext();
        }

        internal static string GetCharacterSelection()
        {
            return getInstance()._display.GetCharacterSelection();
        }

        //there's something wonky about these sub modes I don't like.
        //a disply 'mode' is more like the background movie playing at the time, not functionality
        private void playSequence(string sequenceName, int scoreValue)
        {
            if (string.IsNullOrEmpty(sequenceName) || sequenceName == _currentMode)
                return;
            _log.DebugFormat("Playing Display Sequence {0}", sequenceName);
            switch (sequenceName.ToUpperInvariant())
            {
                case DisplayConstants.Modes.AttractMode.ATTRACT:
                    _currentMode = DisplayConstants.Modes.AttractMode.ATTRACT;
                    _display.ShowAttract(DisplayConstants.Modes.AttractMode.ATTRACT);
                    break;
                case DisplayConstants.Modes.ActiveGameMode.SubModes.CharacterSelectMode.CHARSELECT:
                    _currentMode = DisplayConstants.Modes.ActiveGameMode.SubModes.CharacterSelectMode.CHARSELECT;
                    _display.ShowCharacterSelect(DisplayConstants.Modes.ActiveGameMode.SubModes.CharacterSelectMode.CHARSELECT);
                    break;
                case DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE:
                    _currentMode = DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE;
                    _display.ShowStartGame(DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE);
                    break;
                case DisplayConstants.Modes.ActiveGameMode.SubModes.MultiballMode.MULTIBALL:
                    _display.ShowMultiball(DisplayConstants.Modes.ActiveGameMode.SubModes.MultiballMode.MULTIBALL);
                    break;
                default:
                    break;
            }
        }

        internal static void PlayCutScene(string scene, Dictionary<string, string> args = null)
        {
            if (string.IsNullOrEmpty(scene))
                return;
            _log.DebugFormat("Playing Cut Scene {0}", scene);
            switch (scene)
            {
                case DisplayConstants.CutScenes.Bonuses.COLLECTBONUS:
                    getInstance()._display.ShowCollectBonus(scene, PlayerStatus.CurrentPlayer);
                    break;
                case DisplayConstants.CutScenes.Bonuses.STAGECLEARED:
                    getInstance()._display.ShowStageCleared(scene, PlayerStatus.CurrentPlayer);
                    break;
                case DisplayConstants.CutScenes.Bonuses.SHOOTAGAIN:
                    getInstance()._display.ShowShootAgain(scene, PlayerStatus.CurrentPlayer);
                    break;
                case DisplayConstants.CutScenes.MapMode.MAP:
                    getInstance()._display.ShowMap(scene, PlayerStatus.CurrentPlayer);
                    break;
                case DisplayConstants.CutScenes.ActiveGameMode.ADDLETTER:
                    getInstance()._display.ShowAddLetter(scene, PlayerStatus.CurrentPlayer, args);
                    break;
                case DisplayConstants.CutScenes.Test.BALLSEARCH:
                    //_currentMode = DisplayConstants.Modes.Test; //does this change the mode at all?
                    getInstance()._display.ShowBallSearch(DisplayConstants.CutScenes.Test.BALLSEARCH);
                    break;
            }
        }

        internal static void SetGameStatus(IPlayerStatus playerStatus)
        {
            if (_instance._currentMode != DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE)
                return;
            getInstance()._display.SetGameStatus(playerStatus);
            PlayerStatus.IsDirty = false;  //THIS SHOULD BE THE ONLY PLACE TO SET THIS
        }

        internal static void AddEnemy(IEnemy e)
        {
            if (_instance._currentMode != DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE)
                return;
            getInstance()._display.AddEnemies(e.EnemyType.ToString(), e.Points, 1);
        }

        internal static void KillEnemy(IEnemy e)
        {
            if (_instance._currentMode != DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE)
                return;
            getInstance()._display.KillEnemy(e.EnemyType.ToString());
        }

        internal static void ShowNavigation(NavigationPaths pathsAvailable)
        {
            if (_instance._currentMode != DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE)
                return;
            getInstance()._display.ShowNavigation(pathsAvailable);
        }

        internal static void ClearEnemies()
        {
            if (_instance._currentMode != DisplayConstants.Modes.ActiveGameMode.ACTIVEGAMEMODE)
                return;
            getInstance()._display.ClearEnemies();
        }
    }
}
