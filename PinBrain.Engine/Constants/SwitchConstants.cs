using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Switch;

namespace PinBrain.Engine.Constants
{
    public class SwitchConstants
    {
        //Enum of the switches for this game.  This is a 1-based array.
        public enum Switches
        {
            PlumbTilt,
            Start,
            Coin,
            SlamTilt,
            MenuSelect,
            MenuBack,
            MenuNext,
            MenuExit,
            Outhole, //first switch (0) for solenoid driver.  TODO: add multiplexing for more IO on solenoid driver
            RightBallTrough,
            RightMidBallTrough,
            MidBallTrough,
            LeftMidBallTrough,
            LeftBallTrough,
            BallShooterLane,
            RightFlipperEOS,
            LeftFlipperEOS,
            LeftDrain,
            LeftReturn,
            LeftSling,
            RightSling,
            RightReturn,
            RightDrain,
            DraculaD,
            DraculaR,
            DraculaA,
            DraculaC,
            DraculaU,
            DraculaL,
            DraculaA2,
            BallPopper,
            DropTargetA,
            DropTargetB,
            DropTargetC,
            //DropTargetD,
            BelmontB,
            BelmontE,
            BelmontL,
            BelmontM,
            BelmontO,
            BelmontN,
            BelmontT,
            LeftOuterOrbit,
            RampExit,
            LeftInnerOrbit,
            BossTarget,
            CenterExit,
            CenterScoop,
            RightInnerOrbit,
            CapturedBall,
            RightScoop,
            RightOuterOrbit,
            TopOuterOrbit,
            LeftPop,
            TopPop,
            LowerPop,
            LeftFlipper,
            RightFlipper
        }

        public enum StagePositions
        {
            Unknown = -1,
            BossTarget = 0,
            CenterExit = 1,
            TrophyScoop = 2
        }

        public static PinBrain.Library.Switch.Switch[] GetAllSwitchesForGame()
        {
            //load the game specific switches here.
            Array switchEnums = Enum.GetValues(typeof(SwitchConstants.Switches));
            Switch[] switches = new Switch[switchEnums.Length];
            int index = 0;
            foreach (var switchEnum in switchEnums)
            {
                switches[index++] = new Switch((int)switchEnum, switchEnum.ToString());
            }
            return switches;
        }
    }
}
