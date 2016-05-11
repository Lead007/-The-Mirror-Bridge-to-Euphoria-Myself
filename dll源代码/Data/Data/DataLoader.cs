using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Data
{
    /// <summary>存储读取data.xml的静态方法的静态类</summary>
    public static class DataLoader
    {
        /// <summary>读取data.xml</summary>
        /// <param name="data">XML文件对象</param>
        /// <returns>数据列表</returns>
        public static List<CharacterData> LoadDatas(XmlDocument data)
        {
            var list = new List<CharacterData>();
            var xnl = data.SelectSingleNode("/data/datas").ChildNodes;
            var xnscl = data.SelectSingleNode("/data/sc").ChildNodes;
            for (int i = 0, count = xnl.Count; i < count; i++)
            {
                //读取角色数据
                var cd = new CharacterData();
                var xe = (XmlElement)xnl.Item(i);
                var xesc = (XmlElement)xnscl.Item(i);
                cd.Name = xe.GetAttribute("id");
                var xnll = xe.ChildNodes;
                var xnscll = xesc.ChildNodes;
                cd.Display = xnll.Item(0).InnerText;
                cd.MaxHp = Convert.ToInt32(xnll.Item(1).InnerText);
                cd.Attack = Convert.ToInt32(xnll.Item(2).InnerText);
                cd.Defence = Convert.ToInt32(xnll.Item(3).InnerText);
                cd.HitRate = Convert.ToInt32(xnll.Item(4).InnerText);
                cd.DodgeRate = Convert.ToInt32(xnll.Item(5).InnerText);
                cd.CloseAmendment = Convert.ToSingle(xnll.Item(6).InnerText);
                cd.Interval = Convert.ToInt32(xnll.Item(7).InnerText);
                cd.MoveAbility = Convert.ToInt32(xnll.Item(8).InnerText);
                cd.AttackRange = Convert.ToInt32(xnll.Item(9).InnerText);
                //读取符卡描述
                for (var j = 0; j < 4; j++)
                {
                    cd.ScName[j] = xnscll.Item(j).InnerText;
                    cd.ScDisc[j] = xnscll.Item(j + 4).InnerText;
                }

                list.Add(cd);
            }
            return list;
        }
    }
}
