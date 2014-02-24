using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Devices.IO.Serial
{
    public static class SolenoidDriverCommands
    {
        /// <summary>
        /// Enables a solenoid by index. "#E{0}."
        /// NOTE NOT IMPLEMENTED AS OF 2/21/12
        /// </summary>
        public const string EnableSolenoid = "#E{0}.";

        /// <summary>
        /// Enables all solenoids. "#EA."
        /// </summary>
        public const string EnableSolenoidAll = "#EA.";

        /// <summary>
        /// Disables a solenoid by index. "#D{0}."
        /// NOTE NOT IMPLEMENTED AS OF 2/21/12
        /// </summary>
        public const string DisableSolenoid = "#D{0}.";

        /// <summary>
        /// Disables all solenoids. "#DA."
        /// </summary>
        public const string DisableSolenoidAll = "#DA.";

        /// <summary>
        /// Fires a single solenoid. "#F{0}."
        /// This should be changed to fire it for a specific length of time
        /// </summary>
        public const string FireSolenoid = "#F{0}.";

        /// <summary>
        /// Rotates a Motor. "#M{0}{1}"
        /// This rotates a motor to a specific position
        /// NOTE: NOT IMPLEMENTED IN THE ARDUINO AS OF 10/22/12
        /// </summary>
        public const string RotateMotor = "#M{0}{1}";

        public const string SwitchSpew = "#?{0}.";

        public const string DebugMode = "#!{0}.";
    }
}
