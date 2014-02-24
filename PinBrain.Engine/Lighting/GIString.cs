using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PinBrain.Library.Lighting;

using PinBrain.Devices.IO.LedWiz;
using PinBrain.Engine.Constants;

namespace PinBrain.Engine.Lighting
{
    /// <summary>
    /// The GI String uses a light array, not a matrix because it is 
    /// directly controlled.  The RGB GI String 
    /// </summary>
    public class GIString1AllWhite : LightSequence
    {
        //TODO: find 
        public enum AvailableColors
        {
            Off,
            HiWhite,
            HiRed,
            HiBlue,
            HiGreen,
            HiPaleBlue,
            HiOrange,
            HiYellow,
            LowWhite,
            LowRed,
            LowBlue,
            LowGreen,
            LowPaleBlue,
            LowOrange,
            LowYellow
        }

        /// <summary>
        /// The light array
        /// </summary>
        LightArray _lightArray;

        public GIString1AllWhite(string name)
            : base(name)
        {
            _lightArray = new LightArray(32, 1);

            HostName = HardwareConstants.LEDWIZ1;
            initSequence();
        }

        private void initSequence()
        {
            for (int i = 0; i < _lightArray.ColumnCount; i++)
            {
                _lightArray[i] = 254;
            }
        }

        public void SetColorAll(AvailableColors color)
        {
            int r, g, b;
            convertColorValues(color, out r, out g, out b);
            for (int i = 0; i < _lightArray.ColumnCount - 1; )
            {
                _lightArray[i] = r;
                _lightArray[i + 1] = g;
                _lightArray[i + 2] = b;
                i = i + 3;
            }
        }

        /// <summary>
        /// Converts the available color to an R, G, and B integer value
        /// for this device.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        private void convertColorValues(AvailableColors color, out int r,
            out int g, out int b)
        {
            r = g = b = 0;
            switch (color)
            {
                case AvailableColors.HiWhite:
                    r = g = b = 45;
                    break;
                case AvailableColors.HiRed:
                    r = 45;
                    g = b = 0;
                    break;
                case AvailableColors.HiBlue:
                    b = 45;
                    r = g = 0;
                    break;
                case AvailableColors.HiGreen:
                    g = 45;
                    r = b = 0;
                    break;
                case AvailableColors.HiPaleBlue:
                    b = 45;
                    r = g = 10;
                    break;
                case AvailableColors.HiOrange:
                    break;
                case AvailableColors.HiYellow:
                    break;
                case AvailableColors.LowWhite:
                    r = g = b = 20;
                    break;
                case AvailableColors.LowRed:
                    r = 20;
                    g = b = 0;
                    break;
                case AvailableColors.LowBlue:
                    b = 20;
                    r = g = 0;
                    break;
                case AvailableColors.LowGreen:
                    g = 20;
                    r = b = 0;
                    break;
                case AvailableColors.LowPaleBlue:
                    b = 20;
                    r = g = 5;
                    break;
                case AvailableColors.LowOrange:
                    break;
                case AvailableColors.LowYellow:
                    break;
                case AvailableColors.Off:
                default:
                    r = g = b = 0;
                    break;
            }
        }
    }
}
