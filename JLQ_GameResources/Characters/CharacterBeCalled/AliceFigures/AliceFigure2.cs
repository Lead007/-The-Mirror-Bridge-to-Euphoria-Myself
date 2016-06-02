using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.CharacterBeCalled.AliceFigures
{
    /// <summary>爱丽丝·玛格特洛依德 符卡02召唤出的人偶 蓬莱</summary>
    public class AliceFigure2 : AliceFigure
    {
        public AliceFigure2(Point position, Game game, Character binding)
            : base(position, Group.Middle, game)
        {
            CharacterBind = binding;
        }

        public Character CharacterBind { get; }

        public override void BeAttacked(int damage, Character attacker)
        {
            base.BeAttacked(damage, attacker);
            CharacterBind.BeAttacked((damage << 1)/5, attacker);
        }
    }
}
