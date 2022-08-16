namespace ResinTimerUWPTray
{
    partial class TrayInfoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ResinInfoPanel = new System.Windows.Forms.Panel();
            this.ResinSyncStatusIcon = new System.Windows.Forms.PictureBox();
            this.ResinRemainTimeInfoLabel = new System.Windows.Forms.Label();
            this.ResinInfoLabel = new System.Windows.Forms.Label();
            this.ResinIconPictureBox = new System.Windows.Forms.PictureBox();
            this.RealmCoinInfoPanel = new System.Windows.Forms.Panel();
            this.RealmCoinSyncStatusIcon = new System.Windows.Forms.PictureBox();
            this.RealmCoinRemainTimeInfoLabel = new System.Windows.Forms.Label();
            this.RealmCoinInfoLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.RealmFriendshipInfoPanel = new System.Windows.Forms.Panel();
            this.RealmFriendshipRemainTimeInfoLabel = new System.Windows.Forms.Label();
            this.RealmFriendshipInfoLabel = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.ResinInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResinSyncStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResinIconPictureBox)).BeginInit();
            this.RealmCoinInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RealmCoinSyncStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.RealmFriendshipInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // ResinInfoPanel
            // 
            this.ResinInfoPanel.BackColor = System.Drawing.Color.Transparent;
            this.ResinInfoPanel.Controls.Add(this.ResinSyncStatusIcon);
            this.ResinInfoPanel.Controls.Add(this.ResinRemainTimeInfoLabel);
            this.ResinInfoPanel.Controls.Add(this.ResinInfoLabel);
            this.ResinInfoPanel.Controls.Add(this.ResinIconPictureBox);
            this.ResinInfoPanel.Location = new System.Drawing.Point(12, 12);
            this.ResinInfoPanel.Name = "ResinInfoPanel";
            this.ResinInfoPanel.Size = new System.Drawing.Size(193, 47);
            this.ResinInfoPanel.TabIndex = 1;
            // 
            // ResinSyncStatusIcon
            // 
            this.ResinSyncStatusIcon.Image = global::ResinTimerUWPTray.Properties.Resource.sync;
            this.ResinSyncStatusIcon.Location = new System.Drawing.Point(173, 0);
            this.ResinSyncStatusIcon.Name = "ResinSyncStatusIcon";
            this.ResinSyncStatusIcon.Size = new System.Drawing.Size(20, 20);
            this.ResinSyncStatusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ResinSyncStatusIcon.TabIndex = 3;
            this.ResinSyncStatusIcon.TabStop = false;
            this.ResinSyncStatusIcon.Visible = false;
            // 
            // ResinRemainTimeInfoLabel
            // 
            this.ResinRemainTimeInfoLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ResinRemainTimeInfoLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.ResinRemainTimeInfoLabel.Location = new System.Drawing.Point(54, 26);
            this.ResinRemainTimeInfoLabel.Name = "ResinRemainTimeInfoLabel";
            this.ResinRemainTimeInfoLabel.Size = new System.Drawing.Size(113, 18);
            this.ResinRemainTimeInfoLabel.TabIndex = 2;
            this.ResinRemainTimeInfoLabel.Text = "∞";
            this.ResinRemainTimeInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResinInfoLabel
            // 
            this.ResinInfoLabel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ResinInfoLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.ResinInfoLabel.Location = new System.Drawing.Point(54, 3);
            this.ResinInfoLabel.Name = "ResinInfoLabel";
            this.ResinInfoLabel.Size = new System.Drawing.Size(113, 23);
            this.ResinInfoLabel.TabIndex = 1;
            this.ResinInfoLabel.Text = "...";
            this.ResinInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ResinIconPictureBox
            // 
            this.ResinIconPictureBox.Image = global::ResinTimerUWPTray.Properties.Resource.resin;
            this.ResinIconPictureBox.Location = new System.Drawing.Point(3, 3);
            this.ResinIconPictureBox.Name = "ResinIconPictureBox";
            this.ResinIconPictureBox.Size = new System.Drawing.Size(40, 41);
            this.ResinIconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ResinIconPictureBox.TabIndex = 0;
            this.ResinIconPictureBox.TabStop = false;
            // 
            // RealmCoinInfoPanel
            // 
            this.RealmCoinInfoPanel.BackColor = System.Drawing.Color.Transparent;
            this.RealmCoinInfoPanel.Controls.Add(this.RealmCoinSyncStatusIcon);
            this.RealmCoinInfoPanel.Controls.Add(this.RealmCoinRemainTimeInfoLabel);
            this.RealmCoinInfoPanel.Controls.Add(this.RealmCoinInfoLabel);
            this.RealmCoinInfoPanel.Controls.Add(this.pictureBox1);
            this.RealmCoinInfoPanel.Location = new System.Drawing.Point(12, 65);
            this.RealmCoinInfoPanel.Name = "RealmCoinInfoPanel";
            this.RealmCoinInfoPanel.Size = new System.Drawing.Size(193, 47);
            this.RealmCoinInfoPanel.TabIndex = 3;
            // 
            // RealmCoinSyncStatusIcon
            // 
            this.RealmCoinSyncStatusIcon.Image = global::ResinTimerUWPTray.Properties.Resource.sync;
            this.RealmCoinSyncStatusIcon.Location = new System.Drawing.Point(173, 0);
            this.RealmCoinSyncStatusIcon.Name = "RealmCoinSyncStatusIcon";
            this.RealmCoinSyncStatusIcon.Size = new System.Drawing.Size(20, 20);
            this.RealmCoinSyncStatusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RealmCoinSyncStatusIcon.TabIndex = 4;
            this.RealmCoinSyncStatusIcon.TabStop = false;
            this.RealmCoinSyncStatusIcon.Visible = false;
            // 
            // RealmCoinRemainTimeInfoLabel
            // 
            this.RealmCoinRemainTimeInfoLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RealmCoinRemainTimeInfoLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.RealmCoinRemainTimeInfoLabel.Location = new System.Drawing.Point(54, 26);
            this.RealmCoinRemainTimeInfoLabel.Name = "RealmCoinRemainTimeInfoLabel";
            this.RealmCoinRemainTimeInfoLabel.Size = new System.Drawing.Size(113, 18);
            this.RealmCoinRemainTimeInfoLabel.TabIndex = 2;
            this.RealmCoinRemainTimeInfoLabel.Text = "∞";
            this.RealmCoinRemainTimeInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RealmCoinInfoLabel
            // 
            this.RealmCoinInfoLabel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RealmCoinInfoLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.RealmCoinInfoLabel.Location = new System.Drawing.Point(54, 3);
            this.RealmCoinInfoLabel.Name = "RealmCoinInfoLabel";
            this.RealmCoinInfoLabel.Size = new System.Drawing.Size(113, 23);
            this.RealmCoinInfoLabel.TabIndex = 1;
            this.RealmCoinInfoLabel.Text = "...";
            this.RealmCoinInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ResinTimerUWPTray.Properties.Resource.realm_currency;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 41);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // RealmFriendshipInfoPanel
            // 
            this.RealmFriendshipInfoPanel.BackColor = System.Drawing.Color.Transparent;
            this.RealmFriendshipInfoPanel.Controls.Add(this.RealmFriendshipRemainTimeInfoLabel);
            this.RealmFriendshipInfoPanel.Controls.Add(this.RealmFriendshipInfoLabel);
            this.RealmFriendshipInfoPanel.Controls.Add(this.pictureBox2);
            this.RealmFriendshipInfoPanel.Location = new System.Drawing.Point(12, 118);
            this.RealmFriendshipInfoPanel.Name = "RealmFriendshipInfoPanel";
            this.RealmFriendshipInfoPanel.Size = new System.Drawing.Size(193, 47);
            this.RealmFriendshipInfoPanel.TabIndex = 4;
            // 
            // RealmFriendshipRemainTimeInfoLabel
            // 
            this.RealmFriendshipRemainTimeInfoLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RealmFriendshipRemainTimeInfoLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.RealmFriendshipRemainTimeInfoLabel.Location = new System.Drawing.Point(54, 26);
            this.RealmFriendshipRemainTimeInfoLabel.Name = "RealmFriendshipRemainTimeInfoLabel";
            this.RealmFriendshipRemainTimeInfoLabel.Size = new System.Drawing.Size(113, 18);
            this.RealmFriendshipRemainTimeInfoLabel.TabIndex = 2;
            this.RealmFriendshipRemainTimeInfoLabel.Text = "∞";
            this.RealmFriendshipRemainTimeInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RealmFriendshipInfoLabel
            // 
            this.RealmFriendshipInfoLabel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.RealmFriendshipInfoLabel.ForeColor = System.Drawing.Color.SteelBlue;
            this.RealmFriendshipInfoLabel.Location = new System.Drawing.Point(54, 3);
            this.RealmFriendshipInfoLabel.Name = "RealmFriendshipInfoLabel";
            this.RealmFriendshipInfoLabel.Size = new System.Drawing.Size(113, 23);
            this.RealmFriendshipInfoLabel.TabIndex = 1;
            this.RealmFriendshipInfoLabel.Text = "...";
            this.RealmFriendshipInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ResinTimerUWPTray.Properties.Resource.friendship;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 41);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // TrayInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 272);
            this.Controls.Add(this.RealmFriendshipInfoPanel);
            this.Controls.Add(this.RealmCoinInfoPanel);
            this.Controls.Add(this.ResinInfoPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrayInfoForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TrayInfoForm";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.TrayInfoForm_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TrayInfoForm_FormClosed);
            this.Shown += new System.EventHandler(this.TrayInfoForm_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TrayInfoForm_Paint);
            this.ResinInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResinSyncStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ResinIconPictureBox)).EndInit();
            this.RealmCoinInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RealmCoinSyncStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.RealmFriendshipInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Panel ResinInfoPanel;
        private PictureBox ResinIconPictureBox;
        private Label ResinInfoLabel;
        private Label ResinRemainTimeInfoLabel;
        private Panel RealmCoinInfoPanel;
        private Label RealmCoinRemainTimeInfoLabel;
        private Label RealmCoinInfoLabel;
        private PictureBox pictureBox1;
        private Panel RealmFriendshipInfoPanel;
        private Label RealmFriendshipRemainTimeInfoLabel;
        private Label RealmFriendshipInfoLabel;
        private PictureBox pictureBox2;
        private PictureBox ResinSyncStatusIcon;
        private PictureBox RealmCoinSyncStatusIcon;
    }
}