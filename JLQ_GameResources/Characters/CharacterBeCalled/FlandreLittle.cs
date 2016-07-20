using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using JLQ_GameResources.Buffs.SingleBuff;
using JLQ_GameResources.Characters.SingleCharacter;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.CharacterBeCalled
{
    /// <summary>芙兰 召唤出的分身</summary>
    public class FlandreLittle : Flandre
    {
        public FlandreLittle(PadPoint position, Group group, float dataParameter, Game game)
            : base(0, position, group, game)
        {
            this._dataParameter = dataParameter;
            this.AttackX = dataParameter;
            this.DefenceX = dataParameter;
        }

        private readonly float _dataParameter;

        public override int MaxHp => (int)(_dataParameter*base.MaxHp);

        public override void PreparingSection()
        {
            base.PreparingSection();
            game.ButtonSC.DoAction(b => b.IsEnabled = false);
        }

        public override bool DoAttack(Character target, float times = 1)
        {
            var b = base.DoAttack(target, times);
            var buff = new BuffBlooding(target, this, int.MaxValue, game);
            return b;
        }

        public override void BeAttacked(int damage, Character attacker)
        {
            base.BeAttacked(damage, attacker);
            if (attacker == null) return;
            var buff = new BuffBlooding(attacker, this, 3*this.Interval, game);
        }
    }
}
