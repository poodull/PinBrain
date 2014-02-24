using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Feature;
using PinBrain.Library;
using PinBrain.Engine.Map;
using PinBrain.Library.Map;
using Pinbrain.Library.Utility;

namespace PinBrain.Engine.Managers
{
    public class PlayerStatus : IPlayerStatus
    {
        //private static PlayerStatus _instance;
        public const float MAXBONUSMULTIPLIER = 6;
        public const float MAXEXTRABALL = 2;
        public const long EXTRABALLPOINTS = 1000000;
        public const int WEAPONMAX = 3;
        public const int MAGICMAX = 3;

        private static object _lockObject = new object();
        private static int _playerUp = 1; //the index of the current player. one based
        private static long[] _scores = new long[4];//the score for all players must be available for the scoreboard
        private static List<PlayerStatus> _players = new List<PlayerStatus>(4); //The array of players playing.  up to four.

        private int _ball = 1;
        private Characters _playerCharacter = Characters.unknown;
        private int _weapon;
        private int _magic;
        private int _shieldCount;
        private int _crossCount;
        private int _tiltWarnings;
        private string _playerHealthStatus;
        private int _extraBalls;
        private int _lockedBalls;

        private Room _room = RoomFactory.GetRoom(Levels.Ballroom, 0); //1st room.

        //bonus stuff
        private float _bonusMultiplier = 1;
        private int _enemiesKilled = 0;
        private int _hearts = 0;
        private int _jackpots = 0;
        private int _stageBonus = 0;
        private bool[] _belmont = new bool[7];
        private bool[] _dracula = new bool[7];
        private int _itemHits = 0;

        /// <summary>
        /// Singleton.. or factory???
        /// </summary>
        private PlayerStatus()
        { }

        #region * Static Properties *
        public static PlayerStatus CurrentPlayer
        {
            get
            {
                lock (_lockObject)
                {
                    if (_players.Count == 0 || _playerUp == 0) return null;
                    return _players[_playerUp - 1]; //This fails on race condition
                }
            }
        }

        public static long CurrentPlayerScore
        {
            get
            {
                lock (_lockObject)
                {
                    if (_players.Count == 0 || _playerUp == 0) return 0;
                    return _scores[_playerUp - 1];
                }
            }
            set
            {
                lock (_lockObject)
                {
                    if (_players.Count == 0 || _playerUp == 0)
                        return;
                    _scores[_playerUp - 1] = value;
                    IsDirty = true;
                }
            }
        }

        /// <summary>
        /// The last player in the lineup.
        /// </summary>
        public static PlayerStatus LastPlayer
        {
            get
            {
                lock (_lockObject)
                {
                    if (_players.Count == 0 || _playerUp == 0) return null;
                    return _players[_players.Count - 1];
                }
            }
        }

        public static void NextPlayer()
        {
            _playerUp++;
            if (_playerUp > _players.Count)
                _playerUp = 1;
            IsDirty = true;
        }

        public static void AddPlayer()
        {
            if (_players.Count < 4)
            {
                lock (_lockObject)
                {
                    _players.Add(new PlayerStatus());
                    _scores[_players.Count - 1] = 0;
                    //System.Diagnostics.Debug.WriteLine("Adding Player.  Count is now: " + _players.Count);
                    if (_players.Count == 1 && _playerUp == 0)
                        _playerUp = 1; //race condition between adding first player and advancing player
                    IsDirty = true;
                }
            }
        }

        internal static void Reset()
        {
            lock (_lockObject)
            {
                _players.Clear();
                IsDirty = true;
            }
        }

        public static int PlayerUp //same as PlayerUp
        {
            get { return _playerUp; }
            set
            {
                _playerUp = value;
                IsDirty = true;
            }
        }
        #endregion

        //this is just for flash.
        public int CurrentPlayerIndex
        {
            get { return _playerUp; }
        }

        /// <summary>
        /// Number of enemies killed in this stage.
        /// </summary>
        public int EnemiesKilled
        {
            get { return _enemiesKilled; }
            set { _enemiesKilled = value; }
        }
        #region * Room Stuff *

        internal void Navigate(NavigationPaths direction)
        {
            Levels currentlevel = _room.Level;
            if (_room.RoomType == RoomTypes.Trophy)
            {
                _room = RoomFactory.GetRoom(currentlevel + 1, 0); //new level, 1st room
            }
            else //just a normal room
            {
                switch (direction)
                {
                    case NavigationPaths.Up:
                        _room = RoomFactory.GetRoom(currentlevel, _room.RoomUpIndex);
                        break;
                    case NavigationPaths.Straight:
                        _room = RoomFactory.GetRoom(currentlevel, _room.RoomStraightIndex);
                        break;
                    case NavigationPaths.Down:
                        _room = RoomFactory.GetRoom(currentlevel, _room.RoomDownIndex);
                        break;
                }
            }
            IsDirty = true;
        }

        internal RoomTypes RoomType
        { get { return _room.RoomType; } }

        public List<IEnemy> EnemiesInRoom
        { get { return _room.GetEnemies(); } }

        public NavigationPaths Paths
        { get { return _room.Paths; } }

        public Levels LevelIndex
        {
            get { return _room.Level; }
        }

        public int RoomIndex
        {
            get { return _room.Id; }
        }

        public Items RoomItem
        {
            get { return _room.Item; }
        }

        public string RoomItemString
        {
            get { return _room.Item.ToString(); }
        }

        public void CollectItem()
        {
            _room.IsItemCollected = true;
        }

        /// <summary>
        /// Attacks a random enemy in the room with a power multiplier.
        /// isSplash tramples leftover damage.  So you can set this to false and have 
        /// infinite points to kill just one enemy.
        /// </summary>
        public List<IEnemy> Attack(bool isMagic, int power, bool isSplash = false)
        {
            int damage = power; //calculate damage based on stats
            List<IEnemy> dead = _room.DamageEnemy(damage, isSplash);
            int points = 0;
            foreach (IEnemy d in dead)
            {
                points += d.Points;
                _enemiesKilled++;
            }
            CurrentPlayerScore += points;
            return dead;
        }
        #endregion

        /// <summary>
        /// Number of hearts Collected in this stage.
        /// </summary>
        public int Hearts
        {
            get { return _hearts; }
            set { _hearts = value; }
        }

        /// <summary>
        /// Stage clear bonus.
        /// </summary>
        public int StageBonus
        {
            get { return _stageBonus; }
            set { _stageBonus = value; }
        }

        /// <summary>
        /// The number of Tilt Warnings this player has exhausted so during this ball.
        /// </summary>
        public int TiltWarnings
        {
            get { return _tiltWarnings; }
            set
            {
                _tiltWarnings = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// The ball count the player is on.
        /// </summary>
        public int Ball
        {
            get { return _ball; }
            set
            {
                _ball = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// The number of extra balls the player has collected.
        /// This should only be >0 if the player is current player.
        /// There should be a cap of 2? extra balls per round.
        /// </summary>
        public int ExtraBalls
        {
            get { return _extraBalls; }
            set
            {
                if (ExtraBalls == MAXEXTRABALL)
                    _scores[_playerUp] += EXTRABALLPOINTS;
                else
                    _extraBalls = value;
                IsDirty = true;
            }
        }

        /// <summary>
        /// Get/Set which player index this is (1~4)
        /// </summary>
        public Characters PlayerCharacter
        {
            get { return _playerCharacter; }
            set
            {
                _playerCharacter = value;
                ResetPlayerStatus();
            }
        }
        /// <summary>
        /// Resets the player's status (weapons, shield, cross etc) to basic or 
        /// </summary>
        public void ResetPlayerStatus()
        {
            _tiltWarnings = 0;
            //_playerHealtStatus = HealthStates.Normal;
            for (int i = 0; i < 7; i++)
            {
                _belmont[i] = false;
                _dracula[i] = false;
            }
            switch (_playerCharacter)
            {
                case Characters.Sypha:
                    _shieldCount = int.MaxValue;
                    _crossCount = int.MaxValue;
                    break;
                case Characters.Maria:
                    _shieldCount = int.MaxValue;
                    _crossCount = int.MaxValue;
                    break;
                case Characters.Grant:
                    _shieldCount = 2;
                    _crossCount = 2;
                    break;
                case Characters.Richter:
                    _shieldCount = 1;
                    _crossCount = 1;
                    break;
                case Characters.Alucard:
                    _shieldCount = 0;
                    _crossCount = 0;
                    break;
                default:
                    _shieldCount = 0;
                    _crossCount = 0;
                    _scores[0] = 0;
                    _scores[1] = 0;
                    _scores[2] = 0;
                    _scores[3] = 0;
                    _playerHealthStatus = string.Empty;
                    break;
            }
            _room = RoomFactory.Restart(_room);
            _magic = 0;
            _weapon = 0;
            _extraBalls = 0;
            _hearts = 0;
            _enemiesKilled = 0;
            _stageBonus = 0;
            IsDirty = true;
        }

        public long[] Scores
        {
            get { return _scores; }
            set
            {
                _scores = value;
                IsDirty = true;
            }
        }

        public int NumPlayers
        {
            get
            {
                lock (_lockObject)
                {
                    return _players.Count;
                }
            }
        }

        public int Weapon
        {
            get { return _weapon; }
            set
            {
                _weapon = value; //bounds check?
                IsDirty = true;
            }
        }

        public int Magic
        {
            get { return _magic; }
            set
            {
                _magic = value;
                IsDirty = true;
            }
        }

        public bool HasShield
        {
            get { return _shieldCount > 0; }
            set
            {
                _shieldCount += value ? 1 : -1;
                IsDirty = true;
            } //ugly. should be another method for add/remove
        }

        public bool HasCross
        {
            get { return _crossCount > 0; }
            set
            {
                _crossCount += value ? 1 : -1;
                IsDirty = true;
            }
        }

        public float BonusMultiplier
        {
            get { return _bonusMultiplier; }
            set
            {
                _bonusMultiplier = Math.Min(value, MAXBONUSMULTIPLIER);
                IsDirty = true;
            }
        }

        public string PlayerHealthStatus
        {
            get { return _playerHealthStatus; }
            set
            {
                _playerHealthStatus = value;
                IsDirty = true;
            }
        }

        public bool[] Belmont
        {
            get { return _belmont; }
            set { _belmont = value; }
        }

        public bool[] Dracula
        {
            get { return _dracula; }
            set { _dracula = value; }
        }

        /// <summary>
        /// Adds a letter. Returns TRUE if letter is new
        /// </summary>
        internal bool AddBelmontLetter(int letterIndex)
        {
            bool ret = !_belmont[letterIndex];
            _belmont[letterIndex] = true;
            IsDirty = true;
            return ret;
        }

        /// <summary>
        /// Adds a letter. Returns TRUE if letter is new
        /// </summary>
        internal bool AddDraculaLetter(int letterIndex)
        {
            bool ret = !_dracula[letterIndex];
            _dracula[letterIndex] = true;
            IsDirty = true;
            return ret;
        }

        /// <summary>
        /// Gets or sets whether the PlayerStatus has been
        /// </summary>
        public static bool IsDirty { get; set; }
    }
}
