using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JLQ_GameBase;
using MoreEnumerable;

namespace JLQ_GameResources.Characters.CharacterBeCalled
{
    /// <summary>爱丽丝·玛格特洛依德 召唤出的人偶</summary>
    public abstract class AliceFigure : Character
    {
        protected AliceFigure(PadPoint position, Group group, Game game)
            : base(0, position, group, game)
        {

        }

        public override void PreparingSection()
        {
            base.PreparingSection();
            game.ButtonSC.DoAction(b => b.IsEnabled = false);
        }

        #region SC

        public override void SC01()
        {
            throw new NotImplementedException();
        }

        public override void SC02()
        {
            throw new NotImplementedException();
        }

        public override void SC03()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
