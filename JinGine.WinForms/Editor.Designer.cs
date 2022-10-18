namespace JinGine.WinForms
{
    partial class Editor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._vScrollBar = new System.Windows.Forms.VScrollBar();
            this._hScrollBar = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // _vScrollBar
            // 
            this._vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this._vScrollBar.Location = new System.Drawing.Point(483, 0);
            this._vScrollBar.Name = "_vScrollBar";
            this._vScrollBar.Padding = new System.Windows.Forms.Padding(0, 5, 0, 10);
            this._vScrollBar.Size = new System.Drawing.Size(17, 500);
            this._vScrollBar.TabIndex = 0;
            this._vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VScrollBar_Scroll);
            // 
            // _hScrollBar
            // 
            this._hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._hScrollBar.Location = new System.Drawing.Point(0, 483);
            this._hScrollBar.Name = "_hScrollBar";
            this._hScrollBar.Size = new System.Drawing.Size(483, 17);
            this._hScrollBar.TabIndex = 1;
            this._hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HScrollBar_Scroll);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._hScrollBar);
            this.Controls.Add(this._vScrollBar);
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(500, 500);
            this.SizeChanged += new System.EventHandler(this.Editor_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Editor_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Editor_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private VScrollBar _vScrollBar;
        private HScrollBar _hScrollBar;
    }
}
