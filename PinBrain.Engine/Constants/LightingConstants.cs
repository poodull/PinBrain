using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Engine.Constants
{
    public static class LightingConstants
    {
        public const string GILIGHTSTRING = "GILIGHTSTRING";

        /// <summary>
        /// These are full lighting effects, meaning this one enum represents a series of lighting states
        /// for one or more lights.  
        /// </summary>
        public enum Lfx //I'm really not sure about this...
        {
            None,
            AllOn,
            AllOff,
            Attract,
            /// <summary>
            /// Flashes the last Candle, others are solid.
            /// </summary>
            Candle0001,
            /// <summary>
            /// Flashes the last 2 Candles, others are solid.
            /// </summary>      
            Candle0011,
            /// <summary>
            /// Flashes the last 3 Candles, others are solid.
            /// </summary>  
            Candle0111,
            /// <summary>
            /// All Candles are solid.
            /// </summary>  
            Candle1111,
            /// <summary>
            /// Guides the player to shoot the center scoop.  Flashes slowly. 
            /// All other light fx are static and ignored.
            /// </summary>
            CollectTrophySlow,
            /// <summary>
            /// Guides the player to shoot the center scoop.  Flashes Quickly.
            /// All other light fx are static and ignored.
            /// </summary>
            CollectTrophyFast, 
        }
    }
}
