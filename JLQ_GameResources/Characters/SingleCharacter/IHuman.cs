using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLQ_GameResources.Characters.SingleCharacter
{
    /// <summary>人类种类</summary>
    public enum Human
    {
        /// <summary>人类</summary>
        FullHuman,
        /// <summary>半人</summary>
        HalfHuman,
        /// <summary>妹红</summary>
        Mokou
    }
    interface IHuman
    {
        Human HumanKind { get; }
    }
}
