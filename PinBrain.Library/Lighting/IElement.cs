using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Lighting
{
    interface IElement
    {
        /// <summary>
        /// Turns the element on or off
        /// </summary>
        bool IsOn
        { get; set; }
    }
}
