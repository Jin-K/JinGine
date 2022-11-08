namespace JinGine.WinForms
{
    partial class EditorTextViewer
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
            this._hScrollBar = new System.Windows.Forms.HScrollBar();
            this._vScrollBar = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // _hScrollBar
            // 
            this._hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._hScrollBar.Location = new System.Drawing.Point(0, 133);
            this._hScrollBar.Name = "_hScrollBar";
            this._hScrollBar.Size = new System.Drawing.Size(133, 17);
            this._hScrollBar.TabIndex = 0;
            this._hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnHScrollBarScroll);
            // 
            // _vScrollBar
            // 
            this._vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this._vScrollBar.Location = new System.Drawing.Point(133, 0);
            this._vScrollBar.Name = "_vScrollBar";
            this._vScrollBar.Size = new System.Drawing.Size(17, 150);
            this._vScrollBar.TabIndex = 1;
            this._vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnVScrollBarScroll);
            // 
            // EditorTextViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this._hScrollBar);
            this.Controls.Add(this._vScrollBar);
            this.Name = "EditorTextViewer";
            this.SizeChanged += new System.EventHandler(this.OnSizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private HScrollBar _hScrollBar;
        private VScrollBar _vScrollBar;
    }
}
