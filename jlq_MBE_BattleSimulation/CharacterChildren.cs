using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JLQ_MBE_BattleSimulation
{
    /// <summary>移动无视敌方角色的碰撞箱的角色</summary>
    public abstract class CharacterMovingIgnoreEnemy:Character
    {
        /// <summary>构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected CharacterMovingIgnoreEnemy(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {

        }

        /// <summary>重写基类的阻挡的敌人位置，返回一个空列表</summary>
        public override IEnumerable<Point> EnemyBlock => new List<Point>();
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
}
