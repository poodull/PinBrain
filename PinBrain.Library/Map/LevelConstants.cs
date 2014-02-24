using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Map
{
    public enum Levels
    {
        /// <summary>
        /// 1st Level
        /// </summary>
        Ballroom = 0,
        /// <summary>
        /// 2nd Level
        /// </summary>
        Library = 1,
        /// <summary>
        /// 3rd Level
        /// </summary>
        Catacombs = 2,
        /// <summary>
        /// 4th Level
        /// </summary>
        ClockTower = 3,
        /// <summary>
        /// 5th Level
        /// </summary>
        Chapel = 4
    }

    public enum LevelColors
    {
        orange, sky, brown, lime, green, purple, gray, blood
    }

    public enum RoomTypes
    {
        Start = 0,
        Normal = 1,
        Boss = 2,
        Trophy = 3
    }

    public enum EnemyTypes
    {
        zombie,
        zombieB,
        medusahead,
        medusaheadB,
        fishmen,
        fishmenB
    }
    //[moneysmall, moneymedium, moneybig, magicup, weaponup, bonusx, balllock, heart, candle]
    public enum Items
    {
        candle = 0,
        moneysmall = 1,
        moneymedium = 2,
        moneybig = 3,
        weaponup = 4,
        magicup = 5,
        bonusx = 6,
        balllock = 7,
        heart = 8//,
        //Shield = 9, 
        //Cross = 10
    }
    
    [Flags]
    public enum NavigationPaths
    {
        None = 0,
        Up = 1,
        Straight = 2,
        Down = 4,
        Blocked = 8
    }

    public class LevelConstants
    {
    }
}
