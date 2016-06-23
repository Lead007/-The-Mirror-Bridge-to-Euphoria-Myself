using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;

namespace JLQ_GameResources.Characters.CharacterBeCalled.AliceFigures
{
    /// <summary>爱丽丝·玛格特洛依德 符卡02召唤出的人偶 蓬莱</summary>
    public class AliceFigure2 : AliceFigure
    {
        public AliceFigure2(PadPoint position, Game game, Character binding, int maxHp, int defence)
            : base(position, Group.Middle, game)
        {
            CharacterBind = binding;
            SingleMaxHp = maxHp;
            SingleDefence = defence;
            this.BarHp.Maximum = SingleMaxHp;
            this.Hp = MaxHp;
        }

        public Character CharacterBind { get; }
        public int SingleMaxHp { get; }
        public int SingleDefence { get; }

        public override void BeAttacked(int damage, Character attacker)
        {
            var damageBind = (Math.Min(this.Hp, damage) << 1)/5;
            base.BeAttacked(damage, attacker);
            CharacterBind.BeAttacked(damageBind, attacker);
        }

        public override int CurrentTime
        {
            get { return int.MaxValue; }
            set { }
        }

        public override string ToString()
            => string.Format("HP: {0} / {1}\n防御： {2}\n闪避率： {3}\n绑定角色：{4}号{5}", Hp, MaxHp, Defence, DodgeRate,
                CharacterBind.ID, CharacterBind.GetType().Name);

        protected override void AddBuff(Buff buff)
        {
            
        }

        public override int Defence => SingleDefence;
        public override int MaxHp => SingleMaxHp;
    }
}
