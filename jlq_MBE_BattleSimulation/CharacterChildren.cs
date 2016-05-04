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
    abstract class CharacterMovingIgnoreEnemy:Character
    {
        /// <summary>CharacterMovingIgnoreEnemy类的构造函数</summary>
        /// <param name="id">ID</param>
        /// <param name="position">角色位置</param>
        /// <param name="group">角色阵营</param>
        /// <param name="random">随机数对象</param>
        /// <param name="game">游戏对象</param>
        protected CharacterMovingIgnoreEnemy(int id, Point position, Group group, Random random, Game game)
            : base(id, position, group, random, game)
        {
            
        }

        public override IEnumerable<Character> EnemyBlock => new List<Character>();
    }
}
