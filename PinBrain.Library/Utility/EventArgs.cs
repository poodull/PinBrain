using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Utility
{
    public class EventArgs<T> : EventArgs
    {
        private T _eventData;
        public EventArgs(T eventData)
        { _eventData = eventData; }
        public T EventData
        { get { return _eventData; } }
    }
}
