using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Devices.IO.Serial
{
    public static class ArduinoCommands
    {
        /// <summary>
        /// Forces a switch state to change.
        /// s{0}{1} = switchIndexByte,0:1
        /// </summary>
        public const string ForceSwitchState = "s{0}{1}";

        /// <summary>
        /// Forces a solenoid state to on/off for a specific duration.
        /// f{0}{1}{2} = solenoidIndexByte,0:1
        /// </summary>
        public const string ForceSolenoidState = "f{0}{1}";
    }
}
