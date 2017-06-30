using System;
namespace ShipData
{
    public class intPos2  {

	    int _col;
        public int Column { get { return _col; } }
        int _row;
        public int Row { get { return _row; } }

        public intPos2()
        {
            _col = 0;
            _row = 0;
        }
        public intPos2(int col, int row)
        {
            _col = col;
            _row = row;
        }
    }
}

