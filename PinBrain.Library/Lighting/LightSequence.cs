using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Lighting
{
    public class LightSequence : ILightSequence
    {
        private string _name = string.Empty;
        private string _host = string.Empty;

        public LightSequence(string name)
        {
            _name = name;
        }

        #region ILightSequence Members

        public string SequenceName
        {
            get { return _name; }
            protected set { _name = value; }
        }

        public string HostName
        {
            get { return _host; }
            protected set { _host = value; }
        }

        //public virtual void Play() { }

        //public virtual void Stop() { }

        //public virtual bool IsRunning()
        //{ return true; }

        #endregion
    }
}
