using ResinTimer.Resources;

using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

using TrayContext = ResinTimerUWPTray.ResinTimerUWPTrayContext;

namespace ResinTimerUWPTray
{
    public partial class TrayInfoForm : Form
    {
        private const string UpdateAllRequestKey = "UpdateAllRequest";
        private const string ResinInfoKey = "ResinInfo";
        private const string RealmCoinInfoKey = "RealmCoinInfo";
        private const string RealmFriendshipInfoKey = "RealmFriendshipInfo";

        public static TrayInfoForm Instance = null;

        private (int Now, int Total, TimeSpan RemainTime, bool IsSync)? _resinInfo = null;
        private (int Now, int Total, TimeSpan RemainTime, bool IsSync)? _realmCoinInfo = null;
        private (int Now, int Total, TimeSpan RemainTime)? _realmFriendshipInfo = null;

        private System.Windows.Forms.Timer _updateUITimer;
        private System.Timers.Timer _updateDataTimer;

        public TrayInfoForm()
        {
            Instance = this;

            InitializeComponent();
            SetTrayInfoFormPosition();

            if (TrayContext.Instance.Connection is not null)
            {
                TrayContext.Instance.Connection.RequestReceived += Connection_RequestReceived;

                _updateUITimer = new()
                {
                    Interval = (int)TimeSpan.FromSeconds(1).TotalMilliseconds
                };
                _updateUITimer.Tick += UpdateUITimer_Tick;

                _updateDataTimer = new()
                {
                    Interval = TimeSpan.FromSeconds(1).TotalMilliseconds
                };
                _updateDataTimer.Elapsed += UpdateDataTimer_Elapsed;

                _updateDataTimer.Start();
                _updateUITimer.Start();
            }
            else
            {
                ShowNoConnectionIndicator();
            }

            Focus();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            WindowUtils.EnableAcrylic(this, Color.Transparent);

            base.OnHandleCreated(e);
        }

        private void SetSyncIconVisibility()
        {
            ResinSyncStatusIcon.Visible = _resinInfo?.IsSync ?? false;
            RealmCoinSyncStatusIcon.Visible = _realmCoinInfo?.IsSync ?? false;
        }

        private void ShowNoConnectionIndicator()
        {
            ResinInfoPanel.Visible = false;
            RealmCoinInfoPanel.Visible = false;
            RealmFriendshipInfoPanel.Visible = false;

            Label retryLabel = new()
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 11f, FontStyle.Bold, GraphicsUnit.Point),
                Text = AppResources.UWPTrayInfo_NoConnection
            };

            Controls.Add(retryLabel);
        }

        private void SetTrayInfoFormPosition()
        {
            Screen cursorPosScreen = Screen.FromPoint(Cursor.Position);

            Left = cursorPosScreen.WorkingArea.Right - Width;
            Top = cursorPosScreen.WorkingArea.Bottom - Height;
        }

        private async void UpdateDataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await TrayContext.Instance?.SendToUWP(new()
            {
                { UpdateAllRequestKey, null }
            });
        }

        private void UpdateUITimer_Tick(object sender, EventArgs e)
        {
            if (_resinInfo is not null)
            {
                var (now, total, remainTime, _) = _resinInfo.Value;

                ResinInfoLabel.Text = $"{now} / {total}";
                ResinRemainTimeInfoLabel.Text = remainTime > TimeSpan.Zero ?
                    $"{remainTime.Hours:D2} : {remainTime.Minutes:D2} {AppResources.TimerMainPage_Remain}" :
                    AppResources.TimerMainPage_Complete;
            }
            else
            {
                ResinInfoLabel.Text = "...";
                ResinRemainTimeInfoLabel.Text = "∞";
            }

            if (_realmCoinInfo is not null)
            {
                var (now, total, remainTime, _) = _realmCoinInfo.Value;

                RealmCoinInfoLabel.Text = $"{now} / {total}";
                RealmCoinRemainTimeInfoLabel.Text = remainTime > TimeSpan.Zero ?
                    $"{remainTime.Hours:D2} : {remainTime.Minutes:D2} {AppResources.TimerMainPage_Remain}" :
                    AppResources.TimerMainPage_Complete;
            }
            else
            {
                RealmCoinInfoLabel.Text = "...";
                RealmCoinRemainTimeInfoLabel.Text = "∞";
            }

            if (_realmFriendshipInfo is not null)
            {
                var (now, total, remainTime) = _realmFriendshipInfo.Value;

                RealmFriendshipInfoLabel.Text = $"{now} / {total}";
                RealmFriendshipRemainTimeInfoLabel.Text = remainTime > TimeSpan.Zero ?
                    $"{remainTime.Hours:D2} : {remainTime.Minutes:D2} {AppResources.TimerMainPage_Remain}" :
                    AppResources.TimerMainPage_Complete;
            }
            else
            {
                RealmFriendshipInfoLabel.Text = "...";
                RealmFriendshipRemainTimeInfoLabel.Text = "∞";
            }

            SetSyncIconVisibility();
        }

        private void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            ValueSet message = args.Request.Message;

            if (message.ContainsKey(ResinInfoKey))
            {
                if (message.TryGetValue("NowResin", out object now) &&
                    message.TryGetValue("MaxResin", out object max) &&
                    message.TryGetValue("ResinRemainTime", out object remainTime)&&
                    message.TryGetValue("IsResinSync", out object isSync))
                {
                    _resinInfo = ((int, int, TimeSpan, bool)?)(now, max, remainTime, isSync);
                }
                else
                {
                    _resinInfo = null;
                }
            }

            if (message.ContainsKey(RealmCoinInfoKey))
            {
                if (message.TryGetValue("NowRC", out object now) &&
                    message.TryGetValue("MaxRC", out object max) &&
                    message.TryGetValue("RCRemainTime", out object remainTime) &&
                    message.TryGetValue("IsRealmCoinSync", out object isSync))
                {
                    _realmCoinInfo = ((int, int, TimeSpan, bool)?)(now, max, remainTime, isSync);
                }
                else
                {
                    _realmCoinInfo = null;
                }
            }

            if (message.ContainsKey(RealmFriendshipInfoKey))
            {
                if (message.TryGetValue("NowRF", out object now) &&
                    message.TryGetValue("MaxRF", out object max) &&
                    message.TryGetValue("RFRemainTime", out object remainTime))
                {
                    _realmFriendshipInfo = ((int, int, TimeSpan)?)(now, max, remainTime);
                }
                else
                {
                    _realmFriendshipInfo = null;
                }
            }
        }

        private void CloseTrayInfoForm()
        {
            _updateDataTimer?.Stop();
            _updateUITimer?.Stop();

            if (TrayContext.Instance.Connection is not null)
            {
                TrayContext.Instance.Connection.RequestReceived -= Connection_RequestReceived;
            }

            Close();
        }

        private void TrayInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Instance = null;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            CloseTrayInfoForm();
        }

        private void TrayInfoForm_Paint(object sender, PaintEventArgs e)
        {
            //const int BorderWidth = 3;

            //Pen pen = new(Color.FromArgb(0x780682F6))
            //{
            //    Width = BorderWidth
            //};

            //int startOffset = BorderWidth / 2;
            //int endOffset = BorderWidth;

            //e.Graphics.DrawRectangle(pen, new Rectangle(startOffset, startOffset, 
            //                                            Width - endOffset, Height - endOffset));

            e.Graphics.Clear(Color.Transparent);
        }

        private void TrayInfoForm_Deactivate(object sender, EventArgs e)
        {
            CloseTrayInfoForm();
        }

        private void TrayInfoForm_Shown(object sender, EventArgs e)
        {
            UpdateDataTimer_Elapsed(sender, null);
        }
    }
}
