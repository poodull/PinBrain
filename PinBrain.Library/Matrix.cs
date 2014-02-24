using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinBrain.Library
{
    public interface IMatrix
    {
        /// <summary>
        /// Gets/Sets the state of the entire matrix.
        /// </summary>
        int[][] MatrixState
        { get; set; }

        /// <summary>
        /// Gets the state of a bulb by row and column.
        /// </summary>
        /// <param name="col">column index</param>
        /// <param name="row">row index</param>
        /// <returns></returns>
        bool GetState(int col, int row);

        /// <summary>
        /// Sets the state of a bulb by row and column.
        /// </summary>
        /// <param name="col">column index</param>
        /// <param name="row">row index</param>
        void SetState(int col, int row);
    }
}
