using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    [Serializable]
    /// <summary>棋盘坐标</summary>
    public struct PadPoint
    {
        /// <summary>列坐标</summary>
        public int Column { get; set; }

        /// <summary>行坐标</summary>
        public int Row { get; set; }

        public PadPoint(int column, int row)
        {
            this.Column = column;
            this.Row = row;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PadPoint)) return false;
            return this == (PadPoint) obj;
        }

        public static bool operator ==(PadPoint point1, PadPoint point2)
            => point1.Column == point2.Column && point1.Row == point2.Row;

        public static bool operator !=(PadPoint point1, PadPoint point2) => !(point1 == point2);
    }
}
