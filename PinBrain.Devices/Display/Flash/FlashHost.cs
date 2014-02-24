using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using log4net;
using PinBrain.Library.Feature;
using PinBrain.Library.Map;

namespace PinBrain.Devices.Display.Flash
{
    /// <summary>
    /// </summary>
    /// <remarks>don't forget to add 
    /// ExternalInterface.addCallback("MyExposedFunction", null, 
    /// MyActionScriptFunction"); to the flash function</remarks>
    public partial class FlashHost : Form
    {
        #region * Members *
        public event DisplayEventHandler OnAnimationCompleted;

        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string METHODSTRING = "<invoke name=\"{0}\" returntype=\"xml\"><arguments>{1}</arguments></invoke>";
        //private const string METHODSTRING = "<invoke name=\"{0}\" returntype=\"xml\"><arguments><string>{1}</string></arguments></invoke>";
        private const string METHODEMPTY = "<invoke name=\"{0}\" returntype=\"xml\"><arguments></arguments></invoke>";
        private const string METHODSETGAMESTATUS = "<invoke name=\"SetGameStatus\" returntype=\"xml\"><arguments><string>{0}</string><string>{1}</string><string>{2}</string><string>{3}</string><string>{4}</string><string>{5}</string><string>{6}</string></arguments></invoke>";

#if DEBUG
        const string MOVIELOCATION = @"..\..\..\Dll\Flash";
#else
        const string MOVIELOCATION = @".";
#endif
        const string PLAYERSTATUSMOVIE = @"\CMM.swf";
        const string MULTIBALLMOVIE = @"\Multiball.Succubus.swf";
        const string ATTRACTMOVIE = @"\AttractMode.swf";
        const string CHARSELECTMOVIE = @"\CharacterSelect.swf";
        const string MAPMOVIE = @"\LevelMap.swf";
        const string COLLECTBONUSMOVIE = @"\CollectBonus.swf";
        const string SHOWLETTERMOVIE = @"\SpellBonusMovie.swf";
        const string BALLSEARCHMOVIE = @"\BallSearch.swf";

        readonly string _playerstatusSwfFile;
        readonly string _multiballSwfFile;
        readonly string _attactSwfFile;
        readonly string _characterSelectSwfFile;
        readonly string _mapSwfFile;
        readonly string _collectBonuSwfFile;
        readonly string _showLetterSwfFile;
        readonly string _ballSearchSwfFile;

        IPlayerStatus _currentPlayerStatus;
        int _cardsLeftToTurn = 0;

        private object _lock = new object();
        #endregion

        #region * Constructor and init *
        public FlashHost()
        {
            InitializeComponent();

            flashBackground.FlashCall +=
                new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(
                    this.flashBackground_FlashCallBack);
            flashForeground.FlashCall +=
                new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(
                    flashForeground_FlashCallBack);

            resizeScreen();
            flashForeground.Visible = false;
            flashBackground.Visible = true;

            //This needs to be loaded from file.
            _playerstatusSwfFile = new FileInfo(MOVIELOCATION + PLAYERSTATUSMOVIE).FullName;
            _multiballSwfFile = new FileInfo(MOVIELOCATION + MULTIBALLMOVIE).FullName;
            _attactSwfFile = new FileInfo(MOVIELOCATION + ATTRACTMOVIE).FullName;
            _characterSelectSwfFile = new FileInfo(MOVIELOCATION + CHARSELECTMOVIE).FullName;
            _mapSwfFile = new FileInfo(MOVIELOCATION + MAPMOVIE).FullName;
            _collectBonuSwfFile = new FileInfo(MOVIELOCATION + COLLECTBONUSMOVIE).FullName;
            _showLetterSwfFile = new FileInfo(MOVIELOCATION + SHOWLETTERMOVIE).FullName;
            _ballSearchSwfFile = new FileInfo(MOVIELOCATION + BALLSEARCHMOVIE).FullName;

            this.Show();
            _log.Info(Application.MessageLoop);
        }

        private void resizeScreen()
        {
            //int w = Screen.PrimaryScreen.Bounds.Width;
            //int h = (int)((double)w / 800.0 * 400.0);
            int w = 800; //debug
            int h = 400; //debug
            this.Width = w + 8;
            this.Height = h + 4;
            this.Left = -4;
            this.Top = -4;// Screen.PrimaryScreen.Bounds.Height - h;
        }
        #endregion

        #region * Flash calls and callbacks *
        private void flashBackground_FlashCallBack(object sender,
            AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            string text = e.request;
            if (text.Contains("ANIMATIONCOMPLETED"))
            {
                if (OnAnimationCompleted != null)
                    OnAnimationCompleted(new DisplayEventArgs(flashBackground.Tag, "ANIMATIONCOMPLETED"));
            }
            if (text.StartsWith("<invoke name=\"API"))
                _log.Warn("Background API Definition: " + text);
            else
                _log.Debug("Flash Background Callback: " + e.request);
        }

        private void flashForeground_FlashCallBack(object sender,
            AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            string text = e.request;
            if (text.Contains("FINISHEDMULTIBALL") ||
                text.Contains("FINISHEDBALLLOCK") ||
                text.Contains("FINISHEDJACKPOT") ||
                text.Contains("ANIMATIONCOMPLETED"))
            {
                lock (_lock)
                {
                    if (InvokeRequired)
                        return;
                    flashForeground.Visible = false;
                    flashForeground.Stop(); //foreground is used for cutscenes and may have sounds that don't stop.
                }
                if (OnAnimationCompleted != null)
                    OnAnimationCompleted(new DisplayEventArgs(flashForeground.Tag, "ANIMATIONCOMPLETED"));
            }
            else if (text.Contains("@spellfinished="))
            {
                _cardsLeftToTurn--;
                if (_cardsLeftToTurn <= 0 && flashForeground.Movie.Equals(_showLetterSwfFile))
                {
                    lock (_lock)
                    {
                        if (InvokeRequired)
                            return;
                        flashForeground.Visible = false;
                    }
                    if (OnAnimationCompleted != null)
                        OnAnimationCompleted(new DisplayEventArgs(flashForeground.Tag, "ANIMATIONCOMPLETED"));
                }
            }
            if (text.StartsWith("<invoke name=\"API")) //DEBUG
                _log.Warn("Foreground API Definition: " + text);
            else
                _log.Debug("Flash Foreground Callback: " + text);
        }

        private string callFlashString(AxShockwaveFlashObjects.AxShockwaveFlash flash,
            string method, params object[] vals)
        {
            string response = string.Empty;

            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var param in vals)
                {
                    sb.AppendFormat("<string>{0}</string>", param);
                }

                _log.DebugFormat("Calling method: {0}[{1}]", method, sb.ToString());
                lock (_lock)
                {
                    response = flash.CallFunction(string.Format(METHODSTRING, method, sb.ToString()));
                }
                if (response != "<undefined/>") //void
                    _log.DebugFormat("Response from method: {0}[{1}] = {2}", method, sb.ToString(), response);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Calling method {0} [{1}] on flash {2}. {3}",
                    method, sb, flash.Name, ex.ToString());
            }
            return response;
        }

        private string callFlashEmpty(AxShockwaveFlashObjects.AxShockwaveFlash flash,
            string method)
        {
            string response = string.Empty;

            try
            {
                _log.DebugFormat("Calling method: {0}", method);
                lock (_lock)
                {
                    response = flash.CallFunction(string.Format(METHODEMPTY, method));
                }
                if (response != "<undefined/>") //void
                    _log.DebugFormat("Response from method: {0} = {1}", method, response);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Calling method {0} on flash {1}. {2}",
                    method, flash.Name, ex.ToString());
            }
            return response;
        }

        //public void CallFunction(string methodName, string varName, int val)
        //{
        //    flash1.CallFunction(string.Format(METHODSTRING, methodName,
        //        varName, val));
        //}
        #endregion

        #region * Player Status *
        /// <summary>
        /// Shown on the FOREGROUND
        /// </summary>
        /// <param name="status"></param>
        public void SetGameStatus(IPlayerStatus status)
        {
            if (status == null) return; //nothing to display

            _currentPlayerStatus = status; //keep a pointer to what should be displayed if we're not showing it now.
            if (flashBackground.Movie != _playerstatusSwfFile)
            {
                _log.Error("Cannot SetGameStatus because current movie on background is " + flashBackground.Movie);
                return;
            }
            try
            {
                //we may not be showing the GameScreen.
                //_log.Debug("NOW SHOWING ONE BACKGROUND: " + flashBackground.Movie);
                lock (_lock)
                {
                    if (flashBackground.Movie != _playerstatusSwfFile)
                    {
                        _log.Error("Cannot SetGameStatus because current movie on background is " + flashBackground.Movie);
                        return;
                    }
                    string response = flashBackground.CallFunction(string.Format(METHODSETGAMESTATUS,
                        _currentPlayerStatus.Ball,
                        _currentPlayerStatus.Scores[0],
                        _currentPlayerStatus.Scores[1],
                        _currentPlayerStatus.Scores[2],
                        _currentPlayerStatus.Scores[3],
                        _currentPlayerStatus.CurrentPlayerIndex,
                        _currentPlayerStatus.NumPlayers));
                    //TODO: This method needs to have all of these functions as well.
                    callFlashString(flashBackground, "SetCharacterIndex", (int)_currentPlayerStatus.PlayerCharacter);
                    callFlashString(flashBackground, "SetShield", _currentPlayerStatus.HasShield ? "1" : "0");
                    callFlashString(flashBackground, "SetCross", _currentPlayerStatus.HasCross ? "1" : "0");
                    callFlashString(flashBackground, "SetMagicLevel", _currentPlayerStatus.Magic);
                    callFlashString(flashBackground, "SetWeaponLevel", _currentPlayerStatus.Weapon);
                    callFlashString(flashBackground, "SetPlayerStatus", _currentPlayerStatus.PlayerHealthStatus);
                    callFlashString(flashBackground, "SetBonusValue", _currentPlayerStatus.BonusMultiplier);
                    callFlashString(flashBackground, "SetItem", _currentPlayerStatus.RoomItemString);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error setting GameStatus.", ex);
            }
        }

        /// <summary>
        /// function RemoveAllEnemies():void
        /// removes enemies on the screen without death animation.
        /// </summary>
        public void ClearEnemies()
        {
            if (flashBackground.Movie != _playerstatusSwfFile)
            {
                _log.Error("Cannot ClearEnemies because current movie on background is " + flashBackground.Movie);
                return;
            }
            try
            {
                lock (_lock)
                {
                    if (flashBackground.Movie != _playerstatusSwfFile)
                    {
                        _log.Error("Cannot ClearEnemies because current movie on background is " + flashBackground.Movie);
                        return;
                    }
                    callFlashEmpty(flashBackground, "RemoveAllEnemies");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error ClearEnemies to background.", ex);
            }
        }

        /// <summary>
        /// AddEnemy(type:String, count:String, score:String):String 
        ///	type: [zombie,medusahead,fishmen]
        ///	count: integer //number to spawn
        ///	score: integer //score to display per kill
        /// </summary>
        public void AddEnemies(string type, int points, int count)
        {
            if (flashBackground.Movie != _playerstatusSwfFile)
            {
                _log.Error("Cannot AddEnemies because current movie on background is " + flashBackground.Movie);
                return;
            }
            try
            {
                lock (_lock)
                {
                    if (flashBackground.Movie != _playerstatusSwfFile)
                    {
                        _log.Error("Cannot AddEnemies because current movie on background is " + flashBackground.Movie);
                        return;
                    }
                    callFlashString(flashBackground, "AddEnemy", type, count, points);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error adding Enemies to background.", ex);
            }
        }
        /// <summary>
        /// KillEnemy(type:String):String 
        /// type: [zombie,medusahead,fishmen]
        /// </summary>
        public void KillEnemy(string type)
        {
            if (flashBackground.Movie != _playerstatusSwfFile)
            {
                _log.Error("Cannot KillEnemy because current movie on background is " + flashBackground.Movie);
                return;
            }
            try
            {
                lock (_lock)
                {
                    if (flashBackground.Movie != _playerstatusSwfFile)
                    {
                        _log.Error("Cannot KillEnemy because current movie on background is " + flashBackground.Movie);
                        return;
                    }
                    string ret = callFlashString(flashBackground, "KillEnemy", type);
                    _log.WarnFormat("There are {0} more {1} left!", ret.Replace("<string>", string.Empty).Replace("</string>", string.Empty), type);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error killing Enemies in background.", ex);
            }
        }

        /// <summary>
        /// This is the navigation screen that shows the player which direction he can go.  
        /// There is no closing animation. (YET)
        /// Setting a parameter to false will disable that direction.
        /// function ShowNavigation(
        ///left:String, 	//boolean value.  Accepts [true, TRUE, 1] and [false, FALSE, 0]
        ///middle:String,	//boolean value.  Accepts [true, TRUE, 1] and [false, FALSE, 0]
        ///right:String 	//boolean value.  Accepts [true, TRUE, 1] and [false, FALSE, 0]
        ///):void
        /// </summary>
        /// <param name="status"></param>
        public void ShowNavigation(NavigationPaths pathsAvailable)
        {
            if (flashBackground.Movie != _playerstatusSwfFile)
            {
                _log.Error("Cannot ShowNavigation because current movie on background is " + flashBackground.Movie);
                return;
            }
            try
            {
                lock (_lock)
                {
                    if (flashBackground.Movie != _playerstatusSwfFile)
                    {
                        _log.Error("Cannot ShowNavigation because current movie on background is " + flashBackground.Movie);
                        return;
                    }
                    int up = pathsAvailable.HasFlag(NavigationPaths.Up) ? 1 : 0;
                    int straight = pathsAvailable.HasFlag(NavigationPaths.Straight) ? 1 : 0;
                    int down = pathsAvailable.HasFlag(NavigationPaths.Down) ? 1 : 0;
                    callFlashString(flashBackground, "ShowNavigation", up, straight, down);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error ShowNavigation in background.", ex);
            }
        }

        #endregion

        /// <summary>
        /// Shown on the BACKGROUND
        /// </summary>
        /// <param name="key"></param>
        public void ShowAttract(string key)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowAttract(key);
                }));
            }
            else
            {
                flashForeground.Visible = false;
                flashBackground.Visible = true;
                flashForeground.LoadMovie(0, " ");
                flashBackground.LoadMovie(0, _attactSwfFile);
                //callFlashEmpty(flashBackground, "ShowAtractMode");
                //Need to seed the high scores.
            }
        }

        /// <summary>
        /// Shown on the BACKGROUND
        /// </summary>
        /// <param name="key"></param>
        public void ShowStartGame(string key)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowStartGame(key);
                }));
            }
            else
            {
                lock (_lock)
                {

                    flashForeground.Visible = false;
                    flashBackground.Visible = true;
                    _log.DebugFormat("Loading swf file: {0} into Background.", _playerstatusSwfFile);
                    flashBackground.Movie = _playerstatusSwfFile;
                    flashBackground.Tag = key;
                }
            }
        }

        #region * Character Selection *
        /// <summary>
        /// Shown on the FOREGROUND
        /// </summary>
        /// <param name="key"></param>
        public void ShowCharacterSelect(string key)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowCharacterSelect(key);
                }));
            }
            else
            {
                flashForeground.Visible = true;
                //flashBackground.Visible = false;
                _log.DebugFormat("Loading swf file: {0} into Foreground.", _characterSelectSwfFile);
                flashForeground.Movie = _characterSelectSwfFile;
                flashForeground.Tag = key;
            }
        }

        public void CharacterPrevious()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    CharacterPrevious();
                }));
            }
            else
            {
                if (flashForeground.Movie == _characterSelectSwfFile)
                    SendCommandForeground("Previous");
            }
        }

        public void CharacterNext()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    CharacterNext();
                }));
            }
            else
            {
                if (flashForeground.Movie == _characterSelectSwfFile)
                    SendCommandForeground("Next");
            }
        }

        public string GetCharacterSelection()
        {
            return QueryForeground("GetSelected").Replace("<string>", string.Empty).Replace("</string>", string.Empty); //has to work
        }
        #endregion

        #region * Send Command/Query *
        public void Reset()
        {
            //TODO: Reset the display. to what exactly?
        }

        public void SendCommandForeground(string command)
        {
            callFlashEmpty(flashForeground, command);
        }

        public void SendCommandBackground(string command)
        {
            callFlashEmpty(flashBackground, command);
        }

        public string QueryForeground(string command)
        {
            return callFlashEmpty(flashForeground, command);
        }

        public string QueryBackground(string command)
        {
            return callFlashEmpty(flashBackground, command);
        }
        #endregion

        #region * MAP *
        public void ShowMap(string key, IPlayerStatus status)
        {
            if (status == null) return; //nothing to display

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowMap(key, status);
                }));
            }
            else
            {
                lock (_lock)
                {
                    _currentPlayerStatus = status; //keep a pointer to what should be displayed if we're not showing it now.
                    flashForeground.Visible = true;
                    //flashBackground.Visible = false;
                    _log.DebugFormat("Loading swf file: {0} into Foreground.", _mapSwfFile);
                    flashForeground.Movie = _mapSwfFile;
                    flashForeground.Tag = key; //not used

                    //flash is 1 based array
                    callFlashString(flashForeground, "ShowLevelMap", (int)_currentPlayerStatus.LevelIndex + 1, _currentPlayerStatus.RoomIndex + 1);
                }
            }
        }
        #endregion

        #region * BONUS *
        /// <summary>
        /// Shown on the FOREGROUND
        /// </summary>
        /// <param name="key"></param>
        public void ShowMultiball(string key)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowMultiball(key);
                }));
            }
            else
            {
                if (flashForeground.Visible)
                    return;
                lock (_lock)
                {
                    flashForeground.LoadMovie(0, " ");
                    flashForeground.Visible = true;
                    flashForeground.LoadMovie(0, _multiballSwfFile);
                    callFlashEmpty(flashForeground, "ShowMultiball");
                }
                flashForeground.Tag = key;
            }
        }
        /// <summary>
        /// Shown on the FOREGROUND
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ball"></param>
        public void ShowBallLock(string key, int ball)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowBallLock(key, ball);
                }));
            }
            else
            {
                lock (_lock)
                {

                    if (flashForeground.Visible)
                        return;
                    flashForeground.LoadMovie(0, " ");
                    flashForeground.Visible = true;
                    flashForeground.LoadMovie(0, _multiballSwfFile);
                    callFlashString(flashForeground, "ShowBallLock", ball);
                    flashForeground.Tag = key;
                }
            }
        }
        public void ShowCollectBonus(string key, IPlayerStatus status)
        {
            if (status == null) return; //nothing to display

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowCollectBonus(key, status);
                }));
            }
            else
            {
                _currentPlayerStatus = status; //keep a pointer to what should be displayed if we're not showing it now.
                flashForeground.Visible = true;
                //flashBackground.Visible = false;
                _log.DebugFormat("Loading swf file: {0}  into Background.", _collectBonuSwfFile);
                flashForeground.Movie = _collectBonuSwfFile;
                flashForeground.Tag = key; //not used

                string stageBonus = status.StageBonus.ToString(); // "1000000";
                string bonusLevel = status.BonusMultiplier.ToString(); // "2"; 
                string hearts = status.Hearts.ToString(); // "85";
                string heartValue = "1000"; //?? where should I get this?
                string enemiesKilled = status.EnemiesKilled.ToString(); // "200";
                string enemyValue = "500"; //?? where should I get this?
                callFlashString(flashForeground, "CollectBonus", "endofball", status.Scores[status.CurrentPlayerIndex], stageBonus, bonusLevel, hearts, heartValue, enemiesKilled, enemyValue);
                //CollectBonus(reason:String[stagecleared,endofball], baseScore:String, stageBonus:String, bonusLvl:String, hearts:String, heartValue:String, enemies:String, enemyValue:String):void");
            }
        }

        public void ShowStageCleared(string key, IPlayerStatus status)
        {
            if (status == null) return; //nothing to display

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowStageCleared(key, status);
                }));
            }
            else
            {
                _currentPlayerStatus = status; //keep a pointer to what should be displayed if we're not showing it now.
                flashForeground.Visible = true;
                //flashBackground.Visible = false;
                _log.DebugFormat("Loading swf file: {0}  into Background.", _collectBonuSwfFile);
                flashForeground.Movie = _collectBonuSwfFile;
                flashForeground.Tag = key; //not used

                string stageBonus = status.StageBonus.ToString(); // "1000000";
                string bonusLevel = status.BonusMultiplier.ToString(); // "2"; 
                string hearts = status.Hearts.ToString(); // "85";
                string heartValue = "1000"; //?? where should I get this?
                string enemiesKilled = status.EnemiesKilled.ToString(); // "200";
                string enemyValue = "500"; //?? where should I get this?
                callFlashString(flashForeground, "CollectBonus", "stagecleared", status.Scores[status.CurrentPlayerIndex], stageBonus, bonusLevel, hearts, heartValue, enemiesKilled, enemyValue);
                //CollectBonus(reason:String[stagecleared,endofball], baseScore:String, stageBonus:String, bonusLvl:String, hearts:String, heartValue:String, enemies:String, enemyValue:String):void");
            }
        }

        public void ShowShootAgain(string scene, IPlayerStatus status)//might need to break this out to it's own movie so it can interrupt.
        {
            if (flashBackground.Movie != _playerstatusSwfFile)
            {
                _log.Error("Cannot ShowShootAgain because current movie on background is " + flashBackground.Movie);
                return;
            }
            try
            {
                lock (_lock)
                {
                    if (flashBackground.Movie != _playerstatusSwfFile)
                    {
                        _log.Error("Cannot ShowShootAgain because current movie on background is " + flashBackground.Movie);
                        return;
                    }
                    callFlashEmpty(flashBackground, "ShootAgain");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error calling ShowShootAgain.", ex);
            }
        }

        /// <summary>
        /// SpellBonusMovie.fla
        ///This is the Card-flipping movie.  There are 2 rows of 7 cards each.  Top cards spell BELMONT, 
        ///bottom cards spell DRACULA.  To flip a card, call either SpellBellmont or SpellDracula with the correct string array.
        ///Flips a card or forces cards to be seen/hidden.  The string parameter is a string array where each character can 
        ///be an a=Flip now, A=Set to Flipped, x=unFlipped, and .=Unchanged.
        ///ie, a string of “aAxx...” would mean to flip the 1st card, show the 2nd card flipped, force the 3rd and 4th cards 
        ///to be unflipped and keep the remaining three cards in whatever state they are now.
        ///function SpellBelmont(belmont:String):void
        ///After a card is flipped with animation and the animation is completed, this event will return which card has finished.
        ///values are:
        /// @spellfinished=BELMONTSPELLBCARD
        /// @spellfinished=BELMONTSPELLECARD
        /// @spellfinished=BELMONTSPELLLCARD
        /// @spellfinished=BELMONTSPELLMCARD
        /// @spellfinished=BELMONTSPELLOCARD
        /// @spellfinished=BELMONTSPELLNCARD
        /// @spellfinished=BELMONTSPELLTCARD
        /// @spellfinished=DRACULASPELLDCARD
        /// @spellfinished=DRACULASPELLRCARD
        /// @spellfinished=DRACULASPELLACARD
        /// @spellfinished=DRACULASPELLCCARD
        /// @spellfinished=DRACULASPELLUCARD
        /// @spellfinished=DRACULASPELLLCARD
        /// @spellfinished=DRACULASPELLA2CARD
        /// event @spellfinished
        /// </summary>
        public void ShowAddLetter(string key, IPlayerStatus status, Dictionary<string, string> args)
        {
            if (status == null || args == null) return; //nothing to display

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowAddLetter(key, status, args);
                }));
            }
            else
            {
                flashForeground.Visible = true;

                bool wasPlaying = false;
                if (flashForeground.Movie == _showLetterSwfFile)
                    wasPlaying = true; //if we're playing, we don't reinit values
                else
                {
                    flashForeground.Movie = _showLetterSwfFile;
                    _cardsLeftToTurn = 0; //turning this back on means we might have orphaned animations
                }
                flashForeground.Tag = key; //not used

                bool isBelmont = false;
                if (args.ContainsKey("BELMONT"))
                    isBelmont = true;

                char[] belmont;
                char[] dracula;
                if (wasPlaying)
                {
                    belmont = new char[7] { '.', '.', '.', '.', '.', '.', '.' }; //leave previous values
                    dracula = new char[7] { '.', '.', '.', '.', '.', '.', '.' }; //leave previous values
                }
                else
                {
                    belmont = new char[7] { 'x', 'x', 'x', 'x', 'x', 'x', 'x' };
                    dracula = new char[7] { 'x', 'x', 'x', 'x', 'x', 'x', 'x' };
                    for (int i = 0; i < 7; i++)
                    {
                        belmont[i] = status.Belmont[i] ? 'A' : 'x'; //x is off, Uppercase is flipped                        
                        dracula[i] = status.Dracula[i] ? 'A' : 'x'; //x is off, Uppercase is flipped
                    }
                }
                if (args.ContainsKey("FLIPPED"))
                {
                    int newCard = int.Parse(args["FLIPPED"]); // parse the index of the flipped card
                    if (isBelmont)
                        belmont[newCard] = 'a';
                    else
                        dracula[newCard] = 'a';
                }
                _cardsLeftToTurn++;
                callFlashString(flashForeground, "SpellBelmont", new string(belmont));
                callFlashString(flashForeground, "SpellDracula", new string(dracula));
            }
        }
        #endregion

        #region * BallSearch *
        public void ShowBallSearch(string key)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ShowBallSearch(key);
                }));
            }
            else
            {
                lock (_lock)
                {
                    flashForeground.Visible = true;
                    //flashBackground.Visible = false;
                    _log.DebugFormat("Loading swf file: {0} into Foreground.", _ballSearchSwfFile);
                    flashForeground.Movie = _ballSearchSwfFile;
                    flashForeground.Tag = key; //not used
                }
            }
        }
        #endregion
    }
}
