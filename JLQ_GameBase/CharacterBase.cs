using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameBase
{
    public abstract class CharacterBase
    {
        #region 与ProgressBar绑定的属性
        /// <summary>血量</summary>
        public virtual int Hp { get; set; }

        /// <summary>灵力</summary>
        public virtual int Mp { get; set; }

        public virtual int CurrentTime { get; set; }
        #endregion
    }
}
