using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation.Characters
{
    /// <summary>移动无视敌方角色的碰撞箱的角色</summary>
    public abstract class CharacterTeleportMoving:Character
    {
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected CharacterTeleportMoving(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {

        }

        /// <summary>重写基类的阻挡的敌人位置，返回一个空列表</summary>
        public override IEnumerable<Point> EnemyBlock => new List<Point>();
    }

    public abstract class CharacterHitBack : Character
    {
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected CharacterHitBack(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {

        }

        protected abstract IEnumerable<Character> LegalHitBackTarget { get; }

        public override void BeAttacked(int damage, Character attacker)
        {
            base.BeAttacked(damage, attacker);
            var legalTarget = this.LegalHitBackTarget.ToArray();
            if (legalTarget.Length == 0) return;
            var index = random.Next(legalTarget.Length);
            var target = legalTarget[index];
            //判断是否命中
            if (HandleIsHit(target)) return;
            //造成无来源伤害
            var damageNew = (int)(damage * 0.3 * FloatDamage);
            target.BeAttacked(damageNew, null);
        }
    }

    /// <summary>可能有多次普通的角色</summary>
    public abstract class CharacterMayRepeatedlyDoDamage : Character
    {
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected CharacterMayRepeatedlyDoDamage(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
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
                if (DoAttack(target, times)) temp = true;
            }
            return temp;
        }
    }

    /// <summary>普莉兹姆利巴角色</summary>
    public abstract class CharacterPrismriver : Character
    {
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected CharacterPrismriver(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {

        }

    }

    /// <summary>骚灵角色</summary>
    public abstract class CharacterPoltergeist : CharacterPrismriver
    {
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected CharacterPoltergeist(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {
            //符卡03
            //显示将被攻击的角色
            enterButton[2] = (s, ev) =>
            {
                game.DefaultButtonAndLabels();
                game.Characters.Where(c => _cps.Any(cp => cp.IsInRangeAndEnemy(SC03Range, c)))
                    .Aggregate(GameColor.BaseColor, (cu, c) => c.LabelDisplay.Background = GameColor.LabelBackground);
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
                foreach (var c in _cps)
                {
                    c.HandleDoDanmakuAttack(SCee, SC03Gain);
                }
            };
        }
        /// <summary>结束符卡03</summary>
        public override void EndSC03()
        {
            base.EndSC03();
        }

        public override void SCShow()
        {
            AddSCButtonEvent(2);
        }

        public override void ResetSCShow()
        {
            RemoveSCButtonEvent(2);
        }
    }

}
