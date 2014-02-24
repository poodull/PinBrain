using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Lighting
{
    public interface ILightSequence
    {
        /// <summary>
        /// Get the name of the host device this string runs on.
        /// </summary>
        string HostName { get; }

        /// <summary>
        /// Get the name of the sequence.
        /// </summary>
        string SequenceName { get;}
    }
}
