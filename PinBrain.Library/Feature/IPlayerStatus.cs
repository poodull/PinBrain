using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Map;

namespace PinBrain.Library.Feature
{
    public enum Characters
    {
        Sypha = 0,
        Maria = 1,
        Grant = 2,
        Richter = 3,
        Alucard = 4,
        unknown = 99
    }

    public interface IPlayerStatus
    {
        int Ball { get; }
        Characters PlayerCharacter { get; }
        long[] Scores { get; }
        /// <summary>
        /// This sucker doesn't belong here because it has nothing to do with a SINGLE player.
        /// however, we need it here so that FLASHHOST doesn't need to know about the PlayerManager, which
        /// owns _playerUp.  Individual player objects don't need to know who is up.
        /// </summary>
        int CurrentPlayerIndex { get; }
        int NumPlayers { get; }
        int Weapon { get; }
        int Magic { get; }
        bool HasShield { get; }
        bool HasCross { get; }
        int TiltWarnings { get; }
        Levels LevelIndex { get; }
        int RoomIndex { get; }
        string PlayerHealthStatus { get; }
        float BonusMultiplier { get; }
        int EnemiesKilled { get; }
        int Hearts { get; }
        int StageBonus { get; }
        bool[] Belmont { get; }
        bool[] Dracula { get; }

        string RoomItemString { get; }
        List<IEnemy> EnemiesInRoom { get; }
        NavigationPaths Paths { get; }
    }
}
