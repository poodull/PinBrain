using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Engine.Constants
{
    public class GameManagerConstants
    {

        public enum GameModes
        {

            /// <summary>
            /// Attract mode cycles the attract movie waiting for Credit, Start, or 
            /// Flipper switches. The only other states it can get to are TEST and StartGame.
            /// </summary>
            ATTRACT,
            ///// <summary>
            ///// Start Game mode plays the character select movie.  After the player 
            ///// chooses or a timeout expires, the mode moves to NormalPlay
            ///// This state can get to TEST and NormalPlay.
            ///// </summary>
            //STARTGAME,
            /// <summary>
            /// Normal Play mode is normal targeting and scoring.  Mode changes when 
            /// player loses the ball or advances player status (multiball, Boss mode, etc).
            /// </summary>
            NORMALPLAY,
            /// <summary>
            /// This extends normal play mode, but there is no scoring except for the
            /// CenterScoop.  We also only listen to Tilt and Ball Drain to advance the player.
            /// We should also time this mode to force the player to score or die.
            /// </summary>
            TROPHYROOM,
            //MERCHANTAWARD,
            //MERCHANTMULTI,
            /// <summary>
            /// Test mode is the operator debugging screen.  
            /// This mode can get to Attract.
            /// </summary>
            TEST
        }
        ///// <summary>
        ///// Attract mode cycles the attract movie waiting for Credit, Start, or 
        ///// Flipper switches. The only other states it can get to are TEST and StartGame.
        ///// </summary>
        //public const string ATTRACT = "ATTRACT";

        ///// <summary>
        ///// Start Game mode plays the character select movie.  After the player 
        ///// chooses or a timeout expires, the mode moves to NormalPlay
        ///// This state can get to TEST and NormalPlay.
        ///// </summary>
        //public const string STARTGAME = "STARTGAME";

        ///// <summary>
        ///// Normal Play mode is normal targeting and scoring.  Mode changes when 
        ///// player loses the ball or advances player status (multiball, Boss mode, etc).
        ///// </summary>
        //public const string NORMALPLAY = "NORMALPLAY";

        ///// <summary>
        ///// This extends normal play mode, but there is no scoring except for the
        ///// CenterScoop.  We also only listen to Tilt and Ball Drain to advance the player.
        ///// We should also time this mode to force the player to score or die.
        ///// </summary>
        //public const string TROPHYROOM = "TROPHYROOM";


        //public const string MERCHANTAWARD = "MERCHANTAWARD";

        //public const string MERCHANTMULTI = "MERCHANTMULTI";

        ///// <summary>
        ///// Test mode is the operator debugging screen.  
        ///// This mode can get to Attract.
        ///// </summary>
        //public const string TEST = "TEST";

        /// <summary>
        /// The default ball saver timeout.  This should be a game setting.
        /// </summary>
        public const int BALLSAVERTIMEOUT = 15000;
    }
}
