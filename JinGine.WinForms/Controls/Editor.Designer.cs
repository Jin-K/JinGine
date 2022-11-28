namespace JinGine.WinForms.Controls
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
            this._editorTextViewer = new JinGine.WinForms.Controls.EditorTextViewer();
            this._panel = new System.Windows.Forms.Panel();
            this._posTextBox = new System.Windows.Forms.TextBox();
            this._posLabel = new System.Windows.Forms.Label();
            this._colTextBox = new System.Windows.Forms.TextBox();
            this._colLabel = new System.Windows.Forms.Label();
            this._lineTextBox = new System.Windows.Forms.TextBox();
            this._lineLabel = new System.Windows.Forms.Label();
            this._panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _editorTextViewer
            // 
            this._editorTextViewer.BackColor = System.Drawing.Color.White;
            this._editorTextViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editorTextViewer.Font = new System.Drawing.Font("Courier New", 8.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._editorTextViewer.Location = new System.Drawing.Point(0, 0);
            this._editorTextViewer.Name = "_editorTextViewer";
            this._editorTextViewer.Size = new System.Drawing.Size(500, 478);
            this._editorTextViewer.TabIndex = 2;
            // 
            // _panel
            // 
            this._panel.Controls.Add(this._posTextBox);
            this._panel.Controls.Add(this._posLabel);
            this._panel.Controls.Add(this._colTextBox);
            this._panel.Controls.Add(this._colLabel);
            this._panel.Controls.Add(this._lineTextBox);
            this._panel.Controls.Add(this._lineLabel);
            this._panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panel.Location = new System.Drawing.Point(0, 478);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(500, 22);
            this._panel.TabIndex = 3;
            // 
            // _posTextBox
            // 
            this._posTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._posTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._posTextBox.Location = new System.Drawing.Point(231, 3);
            this._posTextBox.Name = "_posTextBox";
            this._posTextBox.ReadOnly = true;
            this._posTextBox.Size = new System.Drawing.Size(42, 16);
            this._posTextBox.TabIndex = 5;
            // 
            // _posLabel
            // 
            this._posLabel.AutoSize = true;
            this._posLabel.Location = new System.Drawing.Point(178, 3);
            this._posLabel.Name = "_posLabel";
            this._posLabel.Size = new System.Drawing.Size(50, 15);
            this._posLabel.TabIndex = 2;
            this._posLabel.Text = "Position";
            // 
            // _colTextBox
            // 
            this._colTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._colTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._colTextBox.Location = new System.Drawing.Point(133, 3);
            this._colTextBox.Name = "_colTextBox";
            this._colTextBox.ReadOnly = true;
            this._colTextBox.Size = new System.Drawing.Size(42, 16);
            this._colTextBox.TabIndex = 4;
            // 
            // _colLabel
            // 
            this._colLabel.AutoSize = true;
            this._colLabel.Location = new System.Drawing.Point(80, 3);
            this._colLabel.Name = "_colLabel";
            this._colLabel.Size = new System.Drawing.Size(50, 15);
            this._colLabel.TabIndex = 1;
            this._colLabel.Text = "Column";
            // 
            // _lineTextBox
            // 
            this._lineTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._lineTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lineTextBox.Location = new System.Drawing.Point(35, 3);
            this._lineTextBox.Name = "_lineTextBox";
            this._lineTextBox.ReadOnly = true;
            this._lineTextBox.Size = new System.Drawing.Size(42, 16);
            this._lineTextBox.TabIndex = 3;
            // 
            // _lineLabel
            // 
            this._lineLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._lineLabel.AutoSize = true;
            this._lineLabel.Location = new System.Drawing.Point(3, 3);
            this._lineLabel.Name = "_lineLabel";
            this._lineLabel.Size = new System.Drawing.Size(29, 15);
            this._lineLabel.TabIndex = 0;
            this._lineLabel.Text = "Line";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._editorTextViewer);
            this.Controls.Add(this._panel);
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(500, 500);
            this._panel.ResumeLayout(false);
            this._panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private EditorTextViewer _editorTextViewer;
        private Panel _panel;
        private Label _lineLabel;
        private Label _posLabel;
        private Label _colLabel;
        private TextBox _lineTextBox;
        private TextBox _colTextBox;
        private TextBox _posTextBox;
    }
}
