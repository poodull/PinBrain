using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinBrain.Library.Switch;
using System.Collections;

namespace PinBrain.Devices.IO.Serial
{
    partial class SolenoidArduino
    {
        /// <summary>
        /// This is the offset of the total switch enum where this board starts.
        /// We're making the assumption that we will handle switches in blocks.
        /// </summary>
        public const int SWITCHOFFSET = 8;

        #region * Switch Pin IDs Copied on 3/19/13 from CVMM_2 *
        //const uint8_t PI_OUTH = MYPIN_48;	    //	Outhole                     1.1
        //const uint8_t PI_TROR = MYPIN_47;	    //	Right Trough                1.2
        //const uint8_t PI_TROMR = MYPIN_46;	//	Middle Right Trough         1.4
        //const uint8_t PI_TROM = MYPIN_45;	    //	Middle Trough               1.8  
        //const uint8_t PI_TROML = MYPIN_44;	//	Middle Left Trough          1.16  
        //const uint8_t PI_TROL = MYPIN_43;	    //	Left Trough                 1.32  
        //const uint8_t PI_SLANE = MYPIN_42;	//	Shooter Lane                1.64  

        //const uint8_t PI_REOS = MYPIN_38;	    //	Right Flipper EOS           2.1   
        //const uint8_t PI_LEOS = MYPIN_39;	    //	LeftFlipper EOS             2.2   
        //const uint8_t PI_OPTLDRN = MYPIN_41;	//	Opto Left Drain             2.4   
        //const uint8_t PI_OPTLRET = MYPIN_25;	//	Opto Left Return            2.8   
        //const uint8_t PI_LSLING = MYPIN_40;	//	Left Slingshot              2.16  
        //const uint8_t PI_RSLING = MYPIN_37;	//	Right Slingshot             2.32  
        //const uint8_t PI_OPTRRET = MYPIN_36;	//	Opto Right Return           2.64  

        //const uint8_t PI_OPTRDRN = MYPIN_35;	//	Opto Right Drain            3.1   
        //const uint8_t PI_DRACD = MYPIN_21;	//	Dracula Target D            3.2   
        //const uint8_t PI_DRACR = MYPIN_22;	//	Dracula Target R            3.4   
        //const uint8_t PI_DRACA = MYPIN_23;	//	Dracula Target A            3.8
        //const uint8_t PI_DRACC = MYPIN_24;	//	Dracula Target C            3.16
        //const uint8_t PI_DRACU = MYPIN_33;	//	Dracula Target U            3.32
        //const uint8_t PI_DRACL = MYPIN_32;	//	Dracula Target L            3.64

        //const uint8_t PI_DRACA2 = MYPIN_31;	//	Dracula Target A2           4.1
        //const uint8_t PI_VUK = MYPIN_34;	    //	Ball Elevator               4.2
        //const uint8_t PI_DROPA = MYPIN_20;	//	Drop Target A               4.4
        //const uint8_t PI_DROPB = MYPIN_29;	//	Drop Target B               4.8
        //const uint8_t PI_DROPC = MYPIN_30;	//	Drop Target C               4.16
        //const uint8_t PI_BELMB = MYPIN_19;	//	Belmont Target B            4.32
        //const uint8_t PI_BELME = MYPIN_18;	//	Belmont Target E            4.64

        //const uint8_t PI_BELML = MYPIN_9;	    //	Belmont Target L            5.1
        //const uint8_t PI_BELMM = MYPIN_17;	//	Belmont Target M            5.2
        //const uint8_t PI_BELMO = MYPIN_26;	//	Belmont Target O            5.4
        //const uint8_t PI_BELMN = MYPIN_27;	//	Belmont Target N            5.8
        //const uint8_t PI_BELMT = MYPIN_28;	//	Belmont Target T            5.16
        //const uint8_t PI_ORBLO = MYPIN_8;	    //	Orbit Left Outer            5.32
        //const uint8_t PI_RAMPEX = MYPIN_3;	//	Ramp Exit (nav upstairs)    5.64

        //const uint8_t PI_ORBLI = MYPIN_7;	    //	Orbit Left Inner            6.1
        //const uint8_t PI_BOSS = MYPIN_6;	    //	Boss Target                 6.2
        //const uint8_t PI_CENTEX = MYPIN_4;	//	Center Exit (nav forward)   6.4
        //const uint8_t PI_CENTSC = MYPIN_5;	//	Center Scoop                6.8
        //const uint8_t PI_ORBRI = MYPIN_13;	//	Orbit Right Inner           6.16
        //const uint8_t PI_CANDLE = MYPIN_14;	//	Captured Ball Target        6.32
        //const uint8_t PI_RIGHTEX = MYPIN_16;	//	Right Scoop (nav down)      6.64

        //const uint8_t PI_ORBRO = MYPIN_15;	//	Orbit Right Outer           7.1
        //const uint8_t PI_ORBTO = MYPIN_2;	    //	Orbit Top Outer             7.2
        //const uint8_t PI_LPOP = MYPIN_11;	    //	Left Pop Bumper             7.4
        //const uint8_t PI_TPOP = MYPIN_10;	    //	Top Pop Bumper              7.8
        //const uint8_t PI_RPOP = MYPIN_12;	    //	Lower Pop Bumper            7.16
        //const uint8_t PI_LFLIP = MYPIN_50;	//	Left Flipper                7.32
        //const uint8_t PI_RFLIP = MYPIN_51;	//	Right Flipper               7.64
        #endregion

        #region * Solenoid IDs Copied on 2/22 fro CVMM_2 *
        ////' SOLENOID BANK 0 is 50v
        //const byte SOL00 = 0; //' sol0.0 is the left power //chained to sol0.1
        //const byte SOL01 = 1; //' sol0.1 is the left hold
        //const byte SOL02 = 2; //' sol0.2 is the right power //chained to sol0.3
        //const byte SOL03 = 3; //' sol0.3 is the right hold
        //const byte SOL04 = 4; //' sol0.4 is the trough eject
        //const byte SOL05 = 5; //' sol0.5 is the autoplunger
        //const byte SOL06 = 6; //' sol0.6 is the shield kickback
        //const byte SOL07 = 7; //' sol0.7 is the vuk
        ////' SOLENOID BANK 1 is 50v
        //const byte SOL10 = 8; //' sol1.0 is the drop target a activate
        //const byte SOL11 = 9; //' sol1.1 is the drop target b activate
        //const byte SOL12 = 10; //' sol1.2 is the drop target c activate
        //const byte SOL13 = 11; //' sol1.3 is the 
        //const byte SOL14 = 12; //' sol1.4 is the 
        //const byte SOL15 = 13; //' sol1.5 is the 
        //const byte SOL16 = 14; //' sol1.6 is the 
        //const byte SOL17 = 15; //' sol1.7 is the 
        ////' SOLENOID BANK 2 is 20v
        //const byte SOL20 = 16; //' sol2.0 is the top pop
        //const byte SOL21 = 17; //' sol2.1 is the left pop
        //const byte SOL22 = 18; //' sol2.2 is the right pop
        //const byte SOL23 = 19; //' sol2.3 is the left sling
        //const byte SOL24 = 20; //' sol2.4 is the right sling
        //const byte SOL25 = 21; //' sol2.5 is the drop target a reset
        //const byte SOL26 = 22; //' sol2.6 is the drop target b reset
        //const byte SOL27 = 23; //' sol2.7 is the drop target c reset
        ////' SOLENOID BANK 3 is 20v
        //const byte SOL30 = 24; //' sol3.0 is the f1 flasher
        //const byte SOL31 = 25; //' sol3.1 is the f2 flasher
        //const byte SOL32 = 26; //' sol3.2 is the f3 flasher
        //const byte SOL33 = 27; //' sol3.3 is the f4 flasher
        //const byte SOL34 = 28; //' sol3.4 is the f5 flasher
        //const byte SOL35 = 29; //' sol3.5 is the f6 flasher
        //const byte SOL36 = 30; //' sol3.6 is the f7 flasher
        //const byte SOL37 = 31; //' sol3.7 is the f8 flasher
        //const byte UNDEF = 254; //undefined const
        #endregion
        public enum Solenoids
        {
            LeftPower, //chained to sol0.1
            LeftHold,
            RightPower, //chained to sol0.3
            RightHold,
            TroughEject,
            AutoPlunger,
            ShieldKickback,
            Vuk,
            DropAActivate,
            DropBActivate,
            DropCActivate,
            SOL13, //' sol1.3 is the 
            SOL14, //' sol1.4 is the 
            SOL15, //' sol1.5 is the 
            SOL16, //' sol1.6 is the 
            SOL17,  //' sol1.7 is the 
            TopPop,
            LeftPop,
            RightPop,
            LeftSling,
            RightSling,
            DropAReset,
            DropBReset,
            DropCReset
            //const byte SOL30 = 24; //' sol3.0 is the f1 flasher
            //const byte SOL31 = 25; //' sol3.1 is the f2 flasher
            //const byte SOL32 = 26; //' sol3.2 is the f3 flasher
            //const byte SOL33 = 27; //' sol3.3 is the f4 flasher
            //const byte SOL34 = 28; //' sol3.4 is the f5 flasher
            //const byte SOL35 = 29; //' sol3.5 is the f6 flasher
            //const byte SOL36 = 30; //' sol3.6 is the f7 flasher
            //const byte SOL37 = 31; //' sol3.7 is the f8 flasher
        }

        public enum Motors
        {
            CenterStage = 0,
            Elevator = 1,
            Cross = 2
        }

        private List<Switch> translatePinMask(byte[] newStateMask)
        {
            List<Switch> changed = new List<Switch>();
            //this creates an array of bits.  However, the bits are reversed and the 1st bit is unused per byte.
            //so bits[0,8,16,32,40...] will always be 0
            //and bits[1] is really switch index 6
            //and bits[2] is really switch index 5
            //and bits[3] is really switch index 4
            //and bits[4] is really switch index 3
            //and bits[5] is really switch index 2
            //and bits[6] is really switch index 1
            //and bits[7] is really switch index 0
            bool[] bits = newStateMask.SelectMany(getBits).ToArray();

            int switchIndex = SWITCHOFFSET;
            for (int bank = 0; bank < 8; bank++)
            {
                for (int slot = 8; slot > 1; slot--)
                {
                    int bitArrayIndex = (bank * 8) + slot - 1;

                    SwitchState onOff = bits[bitArrayIndex] ? SwitchState.On : SwitchState.Off;
                    _state[switchIndex].State = onOff;
                    if (_state[switchIndex].LastState != onOff)
                        changed.Add(_state[switchIndex]);
                    switchIndex++;
                    if (switchIndex >= _state.Length)
                        break;
                }
                if (switchIndex >= _state.Length)
                    break;
            }

            return changed;
        }

        /// <summary>
        /// Takes a byte and returns an array[8] of bits (bools)
        ///_switchStates = switchBytes.SelectMany(getBits).ToArray();
        /// </summary>
        IEnumerable<bool> getBits(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }
    }
}
