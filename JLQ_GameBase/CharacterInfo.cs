using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    /// <summary>角色信息</summary>
    [Serializable]
    public class CharacterInfo
    {
        /// <summary>角色位置</summary>
        public PadPoint Position { get; set; }
        /// <summary>角色阵营</summary>
        public Group CGroup { get; set; }
        /// <summary>角色显示</summary>
        public string Display { get; set; }
    }
}
