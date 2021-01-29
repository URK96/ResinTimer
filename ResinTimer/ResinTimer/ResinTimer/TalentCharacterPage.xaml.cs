using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TalentCharacterPage : ContentPage
    {
        public List<Character> Characters { get; set; }

        public TalentCharacterPage(string itemName)
        {
            InitializeComponent();

            Characters = new List<Character>();

            foreach (var character in from c in AppEnvironment.genshinDB.characters where (c.TalentItem.Contains(itemName) || c.TalentItem.Contains("All")) select c)
            {
                Characters.Add(new Character(character));
            }

            BindingContext = this;
        }
    }

    public class Character
    {
        public GenshinDB_Core.Character CharacterInfo { get; private set; }
        public string LocationName { get; private set; }
        public string IconString => $"Character_{CharacterInfo.Name}_Thumb.png";
        public string ElementIconString => $"Element_{CharacterInfo.ElementType:F}.png";

        public Character(GenshinDB_Core.Character character)
        {
            CharacterInfo = character;
            LocationName = AppEnvironment.genshinDB.FindLangDic(character.Name);
        }
    }
}