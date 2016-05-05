using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    class BuffLimit : BuffExecuteImmediately
    {
        public BuffLimit(Character buffee, Character buffer, int time, Point point, Game game)
            : base(buffee, buffer, time, "监禁：被限制在区域内", game)
        {
            this.origin = point;
            BuffAffect = (bee, ber) => Buffee.HandleEnemyBlock = ps => ps.Concat(_points);
            BuffCancels += (bee, ber) => Buffee.HandleEnemyBlock = ps => from p in ps select p;
        }

        private IEnumerable<Point> _points => Game.PadPoints.Where(p =>
        {
            var rx = Math.Abs(p.X - origin.X);
            var ry = Math.Abs(p.Y - origin.Y);
            return (rx == 2 && ry <= 2) || (ry == 2 && rx <= 2);
        });
        private readonly Point origin;

    }
}
