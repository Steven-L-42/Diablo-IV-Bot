namespace PixelAimbot
{
    partial class frmMinimized
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMinimized));
            this.labelMinimizedState = new System.Windows.Forms.Label();
            this.labelRuntimer = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.timerRuntimer = new System.Windows.Forms.Timer(this.components);
            this.imageBgra = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBgra)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMinimizedState
            // 
            this.labelMinimizedState.BackColor = System.Drawing.Color.Transparent;
            this.labelMinimizedState.Font = new System.Drawing.Font("Nirmala UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMinimizedState.ForeColor = System.Drawing.Color.GhostWhite;
            this.labelMinimizedState.Location = new System.Drawing.Point(160, -1);
            this.labelMinimizedState.Name = "labelMinimizedState";
            this.labelMinimizedState.Size = new System.Drawing.Size(309, 28);
            this.labelMinimizedState.TabIndex = 0;
            this.labelMinimizedState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelRuntimer
            // 
            this.labelRuntimer.BackColor = System.Drawing.Color.Transparent;
            this.labelRuntimer.Font = new System.Drawing.Font("Nirmala UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRuntimer.ForeColor = System.Drawing.Color.GhostWhite;
            this.labelRuntimer.Location = new System.Drawing.Point(464, 0);
            this.labelRuntimer.Name = "labelRuntimer";
            this.labelRuntimer.Size = new System.Drawing.Size(130, 28);
            this.labelRuntimer.TabIndex = 1;
            this.labelRuntimer.Text = "00:00:00";
            this.labelRuntimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("Nirmala UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(82)))), ((int)(((byte)(197)))));
            this.labelTitle.Location = new System.Drawing.Point(12, -1);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(140, 28);
            this.labelTitle.TabIndex = 12;
            this.labelTitle.Text = "Symbiotic D4 :";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timerRuntimer
            // 
            this.timerRuntimer.Interval = 1000;
            this.timerRuntimer.Tick += new System.EventHandler(this.timerRuntimer_Tick);
            // 
            // imageBgra
            // 
            this.imageBgra.BackColor = System.Drawing.Color.Transparent;
            this.imageBgra.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.imageBgra.Enabled = false;
            this.imageBgra.Location = new System.Drawing.Point(565, 0);
            this.imageBgra.Name = "imageBgra";
            this.imageBgra.Size = new System.Drawing.Size(29, 27);
            this.imageBgra.TabIndex = 13;
            this.imageBgra.TabStop = false;
            // 
            // frmMinimized
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::PixelAimbot.Properties.Resources.minimizedFormDiablo;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(594, 28);
            this.ControlBox = false;
            this.Controls.Add(this.imageBgra);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelRuntimer);
            this.Controls.Add(this.labelMinimizedState);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMinimized";
            this.Opacity = 0.8D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmMinimized";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.imageBgra)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label labelMinimizedState;
        public System.Windows.Forms.Label labelRuntimer;
        public System.Windows.Forms.Label labelTitle;
        public System.Windows.Forms.Timer timerRuntimer;
        private Emgu.CV.UI.ImageBox imageBgra;
    }
}