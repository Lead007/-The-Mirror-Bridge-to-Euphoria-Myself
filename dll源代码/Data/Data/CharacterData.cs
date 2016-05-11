using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>角色数据类</summary>
    [Serializable]
    public class CharacterData
    {
        /// <summary>角色名字</summary>
        public string Name;
        /// <summary>屏幕上显示的文字</summary>
        public string Display = "";
        /// <summary>最大血量</summary>
        public int MaxHp = 120;
        /// <summary>攻击</summary>
        public int Attack = 100;
        /// <summary>防御</summary>
        public int Defence = 80;
        /// <summary>命中</summary>
        public int HitRate = 80;
        /// <summary>闪避</summary>
        public int DodgeRate = 60;
        /// <summary>近战补正</summary>
        public float CloseAmendment = 1.0f;
        /// <summary>行动间隔</summary>
        public int Interval = 30;
        /// <summary>机动</summary>
        public int MoveAbility = 3;
        /// <summary>普攻范围</summary>
        public int AttackRange = 3;
        /// <summary>符卡名</summary>
        public string[] ScName = { "", "", "", "" };
        /// <summary>符卡描述</summary>
        public string[] ScDisc = { "", "", "", "" };

    }
}
