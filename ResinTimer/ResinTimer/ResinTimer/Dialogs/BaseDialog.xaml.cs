using Rg.Plugins.Popup.Pages;

using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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