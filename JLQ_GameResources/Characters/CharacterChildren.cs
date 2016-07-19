using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using MoreEnumerable;
using RandomHelper;

namespace JLQ_GameResources.Characters
{
    /// <summary>移动无视敌方角色的碰撞箱的角色</summary>
    public abstract class CharacterTeleportMoving : Character
    {
        protected CharacterTeleportMoving(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {

        }

        /// <summary>重写基类的阻挡的敌人位置，返回一个空列表</summary>
        public override IEnumerable<PadPoint> EnemyBlock => new List<PadPoint>();
    }

    public abstract class CharacterHitBack : Character
    {
        protected CharacterHitBack(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {

        }

        protected abstract float HitBackGain { get; }
        protected abstract IEnumerable<Character> LegalHitBackTarget { get; }

        public override void BeAttacked(int damage, Character attacker)
        {
            base.BeAttacked(damage, attacker);
            //获取目标
            if (!this.LegalHitBackTarget.Any()) return;
            var target = random.RandomElement(this.LegalHitBackTarget);
            //判断是否命中
            if (HandleIsHit(target)) return;
            //造成无来源伤害
            var damageNew = (int)(damage * HitBackGain * FloatDamage);
            target.BeAttacked(damageNew, null);
        }
    }

    /// <summary>可能有多次普通的角色</summary>
    public abstract class CharacterMayRepeatedlyDoDamage : Character
    {
        protected CharacterMayRepeatedlyDoDamage(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {

        }

        /// <summary>普攻次数</summary>
        public int DamageTimes { get; set; } = 1;

        /// <summary>重写基类的造成伤害方法，改为造成多次伤害</summary>
        /// <param name="target">攻击目标</param>
        /// <param name="times">伤害值增益</param>
        /// <returns>是否存在暴击</returns>
        public override bool DoAttack(Character target, float times = 1)
        {
            var temp = false;
            for (var i = 0; i < DamageTimes; i++)
            {
                if (base.DoAttack(target, times)) temp = true;
            }
            return temp;
        }

        public override bool DoDanmakuAttack(Character target, float times = 1)
        {
            var temp = false;
            for (var i = 0; i < DamageTimes; i++)
            {
                if (base.DoDanmakuAttack(target, times)) temp = true;
            }
            return temp;
        }
    }

    /// <summary>普莉兹姆利巴角色</summary>
    public abstract class CharacterPrismriver : Character
    {
        protected CharacterPrismriver(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {

        }

    }

    /// <summary>骚灵角色</summary>
    public abstract class CharacterPoltergeist : CharacterPrismriver
    {
        protected CharacterPoltergeist(int id, PadPoint position, Group group, Game game)
            : base(id, position, group, game)
        {
            //符卡03
            //显示将被攻击的角色
            enterButton[2] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                game.Characters.Where(c => _cps.Any(cp => cp.IsInRangeAndEnemy(SC03Range, c))).SetLabelBackground();
            };
            SetDefaultLeaveSCButtonDelegate(2);
        }

        protected const int skillRange = 3;
        private const int SC03Range = 5;
        private const float SC03Gain = 0.7f;

        private IEnumerable<Character> _cps
            => game.Characters.Where(c => IsFriend(c) && c is CharacterPoltergeist /*TODO if mp enough*/);

        /// <summary>符卡03</summary>
        public override void SC03()
        {
            game.HandleIsTargetLegal = (SCee, point) => _cps.Any(c => c.IsInRangeAndEnemy(SC03Range, SCee));
            game.HandleTarget = SCee =>
            {
                _cps.DoAction(c => c.HandleDoDanmakuAttack(SCee, SC03Gain));
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }
    }
}
