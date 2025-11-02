using Rg.Plugins.Popup.Pages;

using System;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace ResinTimer.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BaseDialog : PopupPage
    {
        public event EventHandler OnClose;

        public BaseDialog(string title, View contentView)
        {
            InitializeComponent();

            DialogTitle.Text = title;
            DialogContainer.Children.Add(contentView);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            OnClose?.Invoke(this, new EventArgs());
        }
    }
}