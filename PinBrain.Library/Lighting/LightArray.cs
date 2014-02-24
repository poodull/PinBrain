using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library.Lighting
{
    public class LightArray
    {
        /// <summary>
        /// the light array.
        /// </summary>
        int[][] _lights;

        int _columnCount;
        int _depthCount;

        /// <summary>
        /// The index of the depth iterator
        /// </summary>
        int _depthIndex;

        public int ColumnCount
        {
            get { return _columnCount; }
        }

        public int DepthCount
        {
            get { return _depthCount; }
        }

        public LightArray(int columns, int depth)
        {
            _columnCount = columns;
            _depthCount = depth;

            _lights = new int[][] { new int[columns], new int[depth] };
        }

        public void ResetDepthIndex()
        {
            _depthIndex = 0;
        }

        public void MoveNext()
        {
            _depthIndex++;
            if (_depthIndex > _depthCount)
                _depthIndex = 0;
        }

        public int this[int colIndex]
        {
            get { return _lights[colIndex][_depthIndex]; }
            set { }
        }
    }
}
