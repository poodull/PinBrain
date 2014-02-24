namespace PinBrain.Devices.Display.Flash
{
    partial class FlashHost
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlashHost));
            this.flashBackground = new AxShockwaveFlashObjects.AxShockwaveFlash();
            this.flashForeground = new AxShockwaveFlashObjects.AxShockwaveFlash();
            ((System.ComponentModel.ISupportInitialize)(this.flashBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flashForeground)).BeginInit();
            this.SuspendLayout();
            // 
            // flashBackground
            // 
            this.flashBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flashBackground.Enabled = true;
            this.flashBackground.Location = new System.Drawing.Point(0, 0);
            this.flashBackground.Name = "flashBackground";
            this.flashBackground.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("flashBackground.OcxState")));
            this.flashBackground.Size = new System.Drawing.Size(792, 392);
            this.flashBackground.TabIndex = 0;
            // 
            // flashForeground
            // 
            this.flashForeground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flashForeground.Enabled = true;
            this.flashForeground.Location = new System.Drawing.Point(0, 0);
            this.flashForeground.Name = "flashForeground";
            this.flashForeground.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("flashForeground.OcxState")));
            this.flashForeground.Size = new System.Drawing.Size(792, 392);
            this.flashForeground.TabIndex = 1;
            // 
            // FlashHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 392);
            this.ControlBox = false;
            this.Controls.Add(this.flashForeground);
            this.Controls.Add(this.flashBackground);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FlashHost";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.flashBackground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flashForeground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxShockwaveFlashObjects.AxShockwaveFlash flashBackground;
        private AxShockwaveFlashObjects.AxShockwaveFlash flashForeground;
        
    }
}