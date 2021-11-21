using GenshinInfo.Models;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResinTimer.Models
{
    public class GachaLogGroup
    {
        public DateTime GachaDateTime { get; set; }
        public List<GachaInfo> GachaInfos { get; set; }

        public string PrintGachaDateTime => Utils.GetTimeString(GachaDateTime);
        public string GachaShortDateTime => GachaDateTime.ToString("yyyy,MM,dd,HH,mm,ss");

        public GachaLogGroup()
        {
            GachaInfos = new List<GachaInfo>();
        }

        public GachaLogGroup(string dateTimeStr) : this()
        {
            string[] splits = dateTimeStr.Split(',');

            GachaDateTime = new(
                int.Parse(splits[0]),
                int.Parse(splits[1]),
                int.Parse(splits[2]),
                int.Parse(splits[3]),
                int.Parse(splits[4]),
                int.Parse(splits[5]));
        }

        [JsonIgnore]
        public string TotalGachaInfos
        {
            get
            {
                StringBuilder sb = new();

                foreach (GachaInfo info in GachaInfos)
                {
                    sb.AppendLine($"☆{info.ItemRank} {info.ItemType} {info.ItemName}");
                }

                return sb.ToString();
            }
        }

        [JsonIgnore]
        public string SimpleGachaInfo
        {
            get
            {
                string characterStr = Resources.AppResources.GachaType_Character;
                string weaponStr = Resources.AppResources.GachaType_Weapon;

                var characters = from info in GachaInfos
                                 where info.ItemType.Equals(characterStr)
                                 select info;
                var weapons = from info in GachaInfos
                              where info.ItemType.Equals(weaponStr)
                              select info;

                StringBuilder sb = new();

                sb.Append($"{characterStr} : ");
                sb.Append($"{characters.Count()} ");
                sb.AppendLine(" ");

                sb.Append($"{weaponStr} : ");
                sb.Append($"{weapons.Count()} ");

                return sb.ToString();
            }
        }
    }
}
