using JinGine.WinForms.Controls;

namespace JinGine.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._statusBar = new JinGine.WinForms.Controls.StatusBar();
            this._tabsControl = new System.Windows.Forms.TabControl();
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this.SuspendLayout();
            // 
            // _statusBar
            // 
            this._statusBar.BackColor = System.Drawing.SystemColors.Control;
            this._statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._statusBar.Location = new System.Drawing.Point(0, 421);
            this._statusBar.Name = "_statusBar";
            this._statusBar.Padding = new System.Windows.Forms.Padding(35, 3, 3, 3);
            this._statusBar.Size = new System.Drawing.Size(800, 29);
            this._statusBar.TabIndex = 0;
            // 
            // _tabsControl
            // 
            this._tabsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabsControl.Location = new System.Drawing.Point(0, 24);
            this._tabsControl.Margin = new System.Windows.Forms.Padding(0);
            this._tabsControl.Name = "_tabsControl";
            this._tabsControl.Padding = new System.Drawing.Point(0, 0);
            this._tabsControl.SelectedIndex = 0;
            this._tabsControl.Size = new System.Drawing.Size(800, 397);
            this._tabsControl.TabIndex = 0;
            // 
            // _menuStrip
            // 
            this._menuStrip.Location = new System.Drawing.Point(0, 0);
            this._menuStrip.Name = "_menuStrip";
            this._menuStrip.Size = new System.Drawing.Size(800, 24);
            this._menuStrip.TabIndex = 1;
            this._menuStrip.Text = "menuStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._tabsControl);
            this.Controls.Add(this._menuStrip);
            this.Controls.Add(this._statusBar);
            this.MainMenuStrip = this._menuStrip;
            this.Name = "MainForm";
            this.Text = "MainWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private StatusBar _statusBar;
        private TabControl _tabsControl;
        private MenuStrip _menuStrip;
    }
}