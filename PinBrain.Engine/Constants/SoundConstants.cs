using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Engine.Constants
{
    public class SoundConstants
    {
        /// <summary>
        /// The number of the enum matches the sfx clip to play, not a unique number!
        /// </summary>
        public enum Sfx
        {
            None = -1,
            Coin = 0,
            Credit = 2,
            Start = 0,
            Item = 3, 
            Heart = 3, //should be shorter and simpler than Item
            //Select = 3,
            //Next = 4,
            //Attack1,
            //Attack2,
            //Attack3,
            //Attack4,
            //Magic1,
            //Magic2,
            //Magic3,
            //Magic4,
            Death1=1,
            HurryUp = 0
        }

        public enum Music
        {
            LevelStart = 0,
            Level1Normal = 4,//1,
            Level2Normal = 2
        }
    }
}
