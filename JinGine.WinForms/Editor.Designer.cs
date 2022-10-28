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
            this._editorTextViewer = new JinGine.WinForms.EditorTextViewer();
            this._panel = new System.Windows.Forms.Panel();
            this._lineLabel = new System.Windows.Forms.Label();
            this._columnLabel = new System.Windows.Forms.Label();
            this._offsetLabel = new System.Windows.Forms.Label();
            this._panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _editorTextViewer
            // 
            this._editorTextViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editorTextViewer.Font = new System.Drawing.Font("Courier New", 8.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._editorTextViewer.Location = new System.Drawing.Point(0, 0);
            this._editorTextViewer.Name = "_editorTextViewer";
            this._editorTextViewer.Size = new System.Drawing.Size(500, 477);
            this._editorTextViewer.TabIndex = 2;
            // 
            // _panel
            // 
            this._panel.Controls.Add(this._offsetLabel);
            this._panel.Controls.Add(this._columnLabel);
            this._panel.Controls.Add(this._lineLabel);
            this._panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panel.Location = new System.Drawing.Point(0, 477);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(500, 23);
            this._panel.TabIndex = 3;
            // 
            // _lineLabel
            // 
            this._lineLabel.AutoSize = true;
            this._lineLabel.Location = new System.Drawing.Point(3, 3);
            this._lineLabel.Name = "_lineLabel";
            this._lineLabel.Size = new System.Drawing.Size(71, 15);
            this._lineLabel.TabIndex = 0;
            this._lineLabel.Text = "Line: 123456";
            // 
            // _columnLabel
            // 
            this._columnLabel.AutoSize = true;
            this._columnLabel.Location = new System.Drawing.Point(80, 3);
            this._columnLabel.Name = "_columnLabel";
            this._columnLabel.Size = new System.Drawing.Size(86, 15);
            this._columnLabel.TabIndex = 1;
            this._columnLabel.Text = "Column: 12345";
            // 
            // _offsetLabel
            // 
            this._offsetLabel.AutoSize = true;
            this._offsetLabel.Location = new System.Drawing.Point(172, 3);
            this._offsetLabel.Name = "_offsetLabel";
            this._offsetLabel.Size = new System.Drawing.Size(87, 15);
            this._offsetLabel.TabIndex = 2;
            this._offsetLabel.Text = "Offset: 1234567";
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
        private Label _offsetLabel;
        private Label _columnLabel;
    }
}
