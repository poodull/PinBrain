using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Feature
{
    /// <summary>
    /// Modes handle the specific actions within a game mode like handling
    /// switch state, lighting, flash, etc.
    /// </summary>
    public interface IMode : IDisposable
    {
        ///// <summary>
        ///// Driven by SwitchManager here, each mode handles only what it wants.
        ///// </summary>
        //void SwitchStateChanged(Switch.Switch[] _state);
    }
}
