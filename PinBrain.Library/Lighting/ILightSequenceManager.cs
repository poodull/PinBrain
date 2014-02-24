using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Lighting
{
    public interface ILightSequenceManager
    {
        void AddSequence(ILightSequence sequence);
        void PlaySequence(string sequenceName);
        void PlaySequence(string sequenceName, bool repeat);
        void StopSequence(string sequenceName);
        void StopAll();
    }
}
