using System;
using System.Collections.Generic;
using System.Text;

namespace PinBrain.Library.Lighting
{
    public class Led
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public int HiPort = 0;
        public int LoPort = 0;

        public Led(int id)
        { Id = id; }
    }
}
