﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ResinTimer.Resources;

using ManualCategory = ResinTimer.AppEnvironment.ManualCategory;

namespace ResinTimer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HowToUseViewerPage : ContentPage
    {
        public List<HowToUseContent> ManualContent { get; private set; }

        public HowToUseViewerPage(ManualCategory mCategory)
        {
            CreateContents(mCategory);

            InitializeComponent();

            BindingContext = this;

            Title = GetTitle(mCategory);
        }

        private void CreateContents(ManualCategory mCategory)
        {
            ManualContent = new List<HowToUseContent>();

            switch (mCategory)
            {
                case ManualCategory.TimerResin:
                    ManualContent.Add(new HowToUseContent("Manual_TimerResin_1.png", AppResources.HowToUse_Category_Timer_Resin_1));
                    //ManualContent.Add(new HowToUseContent("Manual_TimerResin_2.png", AppResources.HowToUse_Category_Timer_Resin_2));
                    break;
                case ManualCategory.TimerExpedition:
                    ManualContent.Add(new HowToUseContent("Manual_TimerExpedition_1.png", AppResources.HowToUse_Category_Timer_Expedition_1));
                    ManualContent.Add(new HowToUseContent("Manual_TimerExpedition_2.png", AppResources.HowToUse_Category_Timer_Expedition_2));
                    break;
                case ManualCategory.TimerGatheringItem:
                    ManualContent.Add(new HowToUseContent("Manual_TimerGatheringItem_1.png", AppResources.HowToUse_Category_Timer_GatheringItem_1));
                    ManualContent.Add(new HowToUseContent("Manual_TimerGatheringItem_2.png", AppResources.HowToUse_Category_Timer_GatheringItem_2));
                    break;
                case ManualCategory.TimerGadget:
                    ManualContent.Add(new HowToUseContent("Manual_TimerGadget_1.png", AppResources.HowToUse_Category_Timer_Gadget_1));
                    ManualContent.Add(new HowToUseContent("Manual_TimerGadget_2.png", AppResources.HowToUse_Category_Timer_Gadget_2));
                    break;
                case ManualCategory.TimerTalentBook:
                    ManualContent.Add(new HowToUseContent("Manual_TimerTalentBook_1.png", AppResources.HowToUse_Category_Timer_TalentBook_1));
                    break;
                case ManualCategory.WidgetResin:
                    ManualContent.Add(new HowToUseContent("Manual_WidgetResin_1.png", AppResources.HowToUse_Category_Widget_Resin_1));
                    ManualContent.Add(new HowToUseContent("Manual_WidgetResin_2.png", AppResources.HowToUse_Category_Widget_Resin_2));
                    break;
                case ManualCategory.WidgetTalentBook:
                    ManualContent.Add(new HowToUseContent("Manual_WidgetTalentBook_1.png", AppResources.HowToUse_Category_Widget_TalentBook_1));
                    break;
            }
        }

        private string GetTitle(ManualCategory mCategory)
        {
            var title = mCategory switch
            {
                ManualCategory.TimerResin => AppResources.HowToUse_Category_Timer_Resin,
                ManualCategory.TimerExpedition => AppResources.HowToUse_Category_Timer_Expedition,
                ManualCategory.TimerGatheringItem => AppResources.HowToUse_Category_Timer_GatheringItem,
                ManualCategory.TimerGadget => AppResources.HowToUse_Category_Timer_Gadget,
                ManualCategory.TimerTalentBook => AppResources.HowToUse_Category_Timer_TalentBook,
                ManualCategory.WidgetResin => AppResources.HowToUse_Category_Widget_Resin,
                ManualCategory.WidgetTalentBook => AppResources.HowToUse_Category_Widget_TalentBook,
                _ => string.Empty
            };

            return $"{AppResources.HowToUseViewerPage_PreTitle} - {title}";
        }
    }

    public class HowToUseContent
    {
        public string Image { get; }
        public string ExplainText { get; }

        public HowToUseContent(string imagePath, string explainText)
        {
            Image = imagePath;
            ExplainText = explainText;
        }
    }
}