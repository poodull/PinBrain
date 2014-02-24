using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Lighting
{
    public class LightMatrix
    {
        /// <summary>
        /// The row array.  
        /// </summary>
        private LightArray _rows;

        /// <summary>
        /// The column array.
        /// </summary>
        private LightArray _cols;

        /// <summary>
        /// The light array has the columns and rows and a 3rd dimension, 
        /// depth, which adds time.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="depth"></param>
        public LightMatrix(int rows, int columns, int depth)
        {
            _rows = new LightArray(rows, depth);
            _cols = new LightArray(columns, depth);
        }
    }
}
