using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Lighting
{
    public class RgbLed
    {
        private int _id;
        public int RHiPort;
        public int GHiPort;
        public int BHiPort;
        public int LoPort;

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public RgbLed(int id)
        { Id = id; }
    }
}
