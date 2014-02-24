using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.BookKeeping
{
    public interface ICreditManager
    {
        /// <summary>
        /// Gets/Sets the number of credits
        /// </summary>
        int CurrentCredits
        { get; set; }

        /// <summary>
        /// Reset the credit count manager
        /// </summary>
        void Reset();
    }
}
