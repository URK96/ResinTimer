using ResinTimer.Models.Materials;
using ResinTimer.Resources;

using System.Collections.Generic;
using System.Linq;

using AppEnv = ResinTimer.AppEnvironment;
using TalentEnv = ResinTimer.TalentEnvironment;

namespace ResinTimer.TimerPages
{
    public class TalentItemTimerPage : BaseMaterialTimerPage
    {
        public TalentItemTimerPage()
        {
            Title = AppResources.TalentItemTimerPage_Title;
            Items = TalentEnv.Instance.Items;
        }

        protected override void OnAppearing()
        {
            TalentEnv.Instance.UpdateNowTalentBooks();

            base.OnAppearing();
        }

        internal override async void ShowDetailInfo(object selectedItem)
        {
            base.ShowDetailInfo(selectedItem);

            var currentItem = selectedItem as TalentListItem;

            if (currentItem == null)
            {
                return;
            }

            List<string> items = new();

            if (currentItem.Item.ItemName.Equals("All"))
            {
                items.AddRange(from item in AppEnv.genshinDB.talentItems
                               where item.Location.Equals(currentItem.Item.Location)
                               select item.ItemName);
            }
            else
            {
                items.Add(currentItem.Item.ItemName);
            }

            await Navigation.PushAsync(new TalentCharacterPage(items.ToArray()), true);
        }
    }
}