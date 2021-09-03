using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TalentCharacterPage : ContentPage
    {
        public List<Character> Characters { get; set; }

        private string[] items;

        public TalentCharacterPage(string[] itemNames)
        {
            InitializeComponent();

            Characters = new List<Character>();
            items = itemNames;

            foreach (var character in from c in AppEnvironment.genshinDB.characters
                                      where CheckCharacter(c.TalentItem)
                                      select c)
            {
                Characters.Add(new Character(character));
            }

            Characters.Sort((x, y) => x.CharacterInfo.Name.CompareTo(y.CharacterInfo.Name));

            BindingContext = this;
        }

        private bool CheckCharacter(List<string> talentItems)
        {
            if (talentItems.Contains("All"))
            {
                return true;
            }
            
            foreach (var item in items)
            {
                if (talentItems.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        private void CollectionView_ChildAdded(object sender, ElementEventArgs e)
        {
            StackLayout rootLayout = e.Element as StackLayout;
            Label cName = rootLayout.Children[2] as Label;

            if (cName.Text.Length > 8)
            {
                //double newSize = cName.FontSize * 0.8;
                //double gap = cName.FontSize - newSize;

                //cName.Margin = new Thickness(cName.Margin.Left, cName.Margin.Top, cName.Margin.Right, cName.Margin.Bottom + gap);
                //cName.FontSize = newSize;

                //cName.FontSize *= 0.8;
            }
        }
    }

    public class Character
    {
        public GenshinDB_Core.Types.Character CharacterInfo { get; private set; }
        public string LocationName { get; private set; }
        public string IconString => $"Character_{CharacterInfo.Name.Replace(' ', '_')}_Thumb.png";
        public string ElementIconString => $"Element_{CharacterInfo.ElementType:F}.png";

        public Character(GenshinDB_Core.Types.Character character)
        {
            CharacterInfo = character;
            LocationName = AppEnvironment.genshinDB.FindLangDic(character.Name);
        }
    }
}