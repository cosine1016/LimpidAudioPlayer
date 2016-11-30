namespace LAP_Text_Localizer
{
    partial class LangSetting
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
            this.LCIDL = new System.Windows.Forms.Label();
            this.LCIDNum = new System.Windows.Forms.NumericUpDown();
            this.LanguageCode = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LAPVersionMT = new System.Windows.Forms.MaskedTextBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.LCIDNum)).BeginInit();
            this.SuspendLayout();
            // 
            // LCIDL
            // 
            this.LCIDL.AutoSize = true;
            this.LCIDL.Location = new System.Drawing.Point(12, 9);
            this.LCIDL.Name = "LCIDL";
            this.LCIDL.Size = new System.Drawing.Size(86, 12);
            this.LCIDL.TabIndex = 0;
            this.LCIDL.Text = "Locale ID(LCID)";
            // 
            // LCIDNum
            // 
            this.LCIDNum.Location = new System.Drawing.Point(104, 7);
            this.LCIDNum.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.LCIDNum.Name = "LCIDNum";
            this.LCIDNum.Size = new System.Drawing.Size(120, 19);
            this.LCIDNum.TabIndex = 1;
            this.LCIDNum.Value = new decimal(new int[] {
            1033,
            0,
            0,
            0});
            // 
            // LanguageCode
            // 
            this.LanguageCode.AutoSize = true;
            this.LanguageCode.Location = new System.Drawing.Point(230, 9);
            this.LanguageCode.Name = "LanguageCode";
            this.LanguageCode.Size = new System.Drawing.Size(0, 12);
            this.LanguageCode.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Support Version";
            // 
            // LAPVersionMT
            // 
            this.LAPVersionMT.Location = new System.Drawing.Point(104, 41);
            this.LAPVersionMT.Mask = "##0.##0.##0.##0";
            this.LAPVersionMT.Name = "LAPVersionMT";
            this.LAPVersionMT.Size = new System.Drawing.Size(120, 19);
            this.LAPVersionMT.TabIndex = 4;
            this.LAPVersionMT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(12, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(212, 34);
            this.button1.TabIndex = 5;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LangSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 112);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LAPVersionMT);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LanguageCode);
            this.Controls.Add(this.LCIDNum);
            this.Controls.Add(this.LCIDL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LangSetting";
            this.Text = "Language Setting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LangSetting_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.LCIDNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LCIDL;
        private System.Windows.Forms.NumericUpDown LCIDNum;
        private System.Windows.Forms.Label LanguageCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox LAPVersionMT;
        private System.Windows.Forms.Button button1;
    }
}