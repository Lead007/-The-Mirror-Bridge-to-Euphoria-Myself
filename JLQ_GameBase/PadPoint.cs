using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    /// <summary>棋盘坐标</summary>
    [Serializable]
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

        /// <summary>更安全的创建棋盘点对象的方法</summary>
        /// <param name="column">列坐标</param>
        /// <param name="row">行坐标</param>
        /// <returns>创建结果，null则在棋盘边界外</returns>
        public static PadPoint? CreatePadPoint(int column, int row)
        {
            if (column < 0 || column > Game.Column || row < 0 || row > Game.Row) return null;
            return new PadPoint(column,row);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PadPoint)) return false;
            return this == (PadPoint) obj;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public static bool operator ==(PadPoint point1, PadPoint point2)
            => point1.Column == point2.Column && point1.Row == point2.Row;

        public static bool operator !=(PadPoint point1, PadPoint point2) => !(point1 == point2);

        public override string ToString() => string.Format("({0},{1})", Column, Row);
    }
}
