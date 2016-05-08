using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>被限制在特定区域内的buff</summary>
    public class BuffLimit : BuffExecuteImmediately
    {
        /// <summary>构造函数</summary>
        /// <param name="buffee">buff承受者</param>
        /// <param name="buffer">buff发出者</param>
        /// <param name="time">持续时间</param>
        /// <param name="origin">中心点</param>
        /// <param name="handle">判断点是否为墙的委托</param>
        /// <param name="game">游戏对象</param>
        public BuffLimit(Character buffee, Character buffer, int time, Point origin, DIsPointWall handle, Game game)
            : base(buffee, buffer, time, "监禁：被限制在区域内", false, game)
        {
            this._origin = origin;
            this._handleIsPointWall = handle;
            BuffAffect += (bee, ber) => Buffee.HandleEnemyBlock = ps => ps.Concat(_points);
            BuffCancels += (bee, ber) => Buffee.HandleEnemyBlock = ps => from p in ps select p;
        }

        private IEnumerable<Point> _points => Game.PadPoints.Where(p => _handleIsPointWall(_origin, p));
        private readonly Point _origin;
        private readonly DIsPointWall _handleIsPointWall;

    }
}
