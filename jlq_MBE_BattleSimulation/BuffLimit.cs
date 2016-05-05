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
        public BuffLimit(Character buffee, Character buffer, int time, IEnumerable<Point> points, Game game)
            : base(buffee, buffer, time, "监禁：被限制在区域内", game)
        {
            this.points = points;
            BuffAffect = (bee, ber) =>
            {
                Buffee.HandleNormalMove -= Buffee.Move;
                Buffee.HandleNormalMove += Move;
            };
            BuffCancels += (bee, ber) =>
            {
                Buffee.HandleNormalMove -= Move;
                Buffee.HandleNormalMove += Buffee.Move;
            };
        }

        private IEnumerable<Point> points; 

        private void Move(Point point)
        {
            if (!points.Contains(point))
            {
                Game.IllegalMessageBox("你被监禁，不能移动到此处！");
            }
            Buffee.Move(point);
        }
    }
}
