namespace PinBrain.Devices.IO.IPAC
{
    partial class KeyCatcherForm
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
            this.SuspendLayout();
            // 
            // KeyCatcherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(110, 65);
            this.Name = "KeyCatcherForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyCatcherForm_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyCatcherForm_KeyUp);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyCatcherForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}