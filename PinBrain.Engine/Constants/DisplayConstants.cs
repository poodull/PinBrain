using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Engine.Constants
{
    public static class DisplayConstants
    {
        public const string UNKNOWN = "UNKNOWN";
        //public const string STARTGAME = "STARTGAME";
        public const string BALLLOCK = "BALLLOCK";

        /// <summary>
        /// These are the modes in the game and their related commands and queries.
        /// </summary>
        public static class Modes
        {
            public static class AttractMode
            {
                public const string ATTRACT = "ATTRACT";
                public static class Commands
                {
                    /// <summary>
                    /// Sends 'StartPressed' command
                    /// </summary>
                    public const string STARTPRESSED = "StartPressed";
                    /// <summary>
                    /// Sends 'NavigateTo("Title")' command
                    /// </summary>
                    public const string GOTOTITLE = "GotoTitle"; 
                    /// <summary>
                    /// Sends 'NavigateTo("HighScores")' command
                    /// </summary>
                    public const string GOTOHIGHSCORES = "GotoHighScores"; 
                }
            }
            public static class ActiveGameMode
            {
                public const string ACTIVEGAMEMODE = "ACTIVEGAMEMODE";

                public static class SubModes
                {
                    public static class CharacterSelectMode
                    {
                        public const string CHARSELECT = "CHARACTERSELECT";
                    }
                    public static class MultiballMode
                    {
                        public const string MULTIBALL = "MULTIBALL";
                    }
                    //trophy? //boss?
                }
            }
        }

        // all of these cut scenes require that the flash host is currently playing the active game movie.
        // so these should be moved inot the mode class structure.
        public static class CutScenes
        {
            public static class ActiveGameMode
            {
                public const string ADDLETTER = "AddLetter";
                public const string COLLECTTROPHY = "CollectTrophy";
            }
            public static class MapMode
            {
                public const string MAP = "MAP";
            }
            public static class Bonuses
            {
                public const string COLLECTBONUS = "CollectBonus";
                public const string STAGECLEARED = "StageClearedBonus";
                public const string SHOOTAGAIN = "ShootAgain";
            }
            public static class Test
            {
                public const string BALLSEARCH = "BallSearch";
            }
        }
    }
}
