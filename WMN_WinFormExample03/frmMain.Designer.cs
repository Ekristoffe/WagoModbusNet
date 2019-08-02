namespace WMN_WinFormExample03
{
    partial class frmMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbxConfIp = new System.Windows.Forms.GroupBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblHost = new System.Windows.Forms.Label();
            this.gbxRead = new System.Windows.Forms.GroupBox();
            this.btnReadBool = new System.Windows.Forms.Button();
            this.btnReadString = new System.Windows.Forms.Button();
            this.btnReadReal = new System.Windows.Forms.Button();
            this.btnReadUDINT = new System.Windows.Forms.Button();
            this.btnReadDINT = new System.Windows.Forms.Button();
            this.btnReadWord = new System.Windows.Forms.Button();
            this.gbxWrite = new System.Windows.Forms.GroupBox();
            this.cbxBoolValue = new System.Windows.Forms.ComboBox();
            this.btnWriteDint = new System.Windows.Forms.Button();
            this.txtDintValue = new System.Windows.Forms.TextBox();
            this.btnWriteUdint = new System.Windows.Forms.Button();
            this.txtUdintValue = new System.Windows.Forms.TextBox();
            this.btnWriteReal = new System.Windows.Forms.Button();
            this.txtRealValue = new System.Windows.Forms.TextBox();
            this.btnWriteString = new System.Windows.Forms.Button();
            this.txtStringValue = new System.Windows.Forms.TextBox();
            this.btnWriteBool = new System.Windows.Forms.Button();
            this.txtData = new System.Windows.Forms.TextBox();
            this.gbxConfIp.SuspendLayout();
            this.gbxRead.SuspendLayout();
            this.gbxWrite.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxConfIp
            // 
            this.gbxConfIp.Controls.Add(this.txtPort);
            this.gbxConfIp.Controls.Add(this.txtHost);
            this.gbxConfIp.Controls.Add(this.lblPort);
            this.gbxConfIp.Controls.Add(this.lblHost);
            this.gbxConfIp.Location = new System.Drawing.Point(12, 12);
            this.gbxConfIp.Name = "gbxConfIp";
            this.gbxConfIp.Size = new System.Drawing.Size(317, 82);
            this.gbxConfIp.TabIndex = 12;
            this.gbxConfIp.TabStop = false;
            this.gbxConfIp.Text = "Config";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(131, 38);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(55, 20);
            this.txtPort.TabIndex = 10;
            this.txtPort.Text = "502";
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(14, 38);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(100, 20);
            this.txtHost.TabIndex = 9;
            this.txtHost.Text = "192.168.1.16";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(128, 22);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 8;
            this.lblPort.Text = "Port";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(11, 22);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(32, 13);
            this.lblHost.TabIndex = 7;
            this.lblHost.Text = "Host ";
            // 
            // gbxRead
            // 
            this.gbxRead.Controls.Add(this.btnReadBool);
            this.gbxRead.Controls.Add(this.btnReadString);
            this.gbxRead.Controls.Add(this.btnReadReal);
            this.gbxRead.Controls.Add(this.btnReadUDINT);
            this.gbxRead.Controls.Add(this.btnReadDINT);
            this.gbxRead.Controls.Add(this.btnReadWord);
            this.gbxRead.Location = new System.Drawing.Point(12, 100);
            this.gbxRead.Name = "gbxRead";
            this.gbxRead.Size = new System.Drawing.Size(317, 93);
            this.gbxRead.TabIndex = 18;
            this.gbxRead.TabStop = false;
            this.gbxRead.Text = "Read";
            // 
            // btnReadBool
            // 
            this.btnReadBool.Location = new System.Drawing.Point(14, 28);
            this.btnReadBool.Name = "btnReadBool";
            this.btnReadBool.Size = new System.Drawing.Size(92, 23);
            this.btnReadBool.TabIndex = 20;
            this.btnReadBool.Text = "Read_BOOL";
            this.btnReadBool.UseVisualStyleBackColor = true;
            this.btnReadBool.Click += new System.EventHandler(this.btnReadBool_Click);
            // 
            // btnReadString
            // 
            this.btnReadString.Location = new System.Drawing.Point(210, 57);
            this.btnReadString.Name = "btnReadString";
            this.btnReadString.Size = new System.Drawing.Size(92, 23);
            this.btnReadString.TabIndex = 19;
            this.btnReadString.Text = "Read_STRING";
            this.btnReadString.UseVisualStyleBackColor = true;
            this.btnReadString.Click += new System.EventHandler(this.btnReadString_Click);
            // 
            // btnReadReal
            // 
            this.btnReadReal.Location = new System.Drawing.Point(210, 28);
            this.btnReadReal.Name = "btnReadReal";
            this.btnReadReal.Size = new System.Drawing.Size(92, 23);
            this.btnReadReal.TabIndex = 18;
            this.btnReadReal.Text = "Read_REAL";
            this.btnReadReal.UseVisualStyleBackColor = true;
            this.btnReadReal.Click += new System.EventHandler(this.btnReadReal_Click);
            // 
            // btnReadUDINT
            // 
            this.btnReadUDINT.Location = new System.Drawing.Point(112, 57);
            this.btnReadUDINT.Name = "btnReadUDINT";
            this.btnReadUDINT.Size = new System.Drawing.Size(92, 23);
            this.btnReadUDINT.TabIndex = 17;
            this.btnReadUDINT.Text = "Read_UDINT";
            this.btnReadUDINT.UseVisualStyleBackColor = true;
            this.btnReadUDINT.Click += new System.EventHandler(this.btnReadUDINT_Click);
            // 
            // btnReadDINT
            // 
            this.btnReadDINT.Location = new System.Drawing.Point(112, 28);
            this.btnReadDINT.Name = "btnReadDINT";
            this.btnReadDINT.Size = new System.Drawing.Size(92, 23);
            this.btnReadDINT.TabIndex = 16;
            this.btnReadDINT.Text = "Read_DINT";
            this.btnReadDINT.UseVisualStyleBackColor = true;
            this.btnReadDINT.Click += new System.EventHandler(this.btnReadDINT_Click);
            // 
            // btnReadWord
            // 
            this.btnReadWord.Location = new System.Drawing.Point(14, 57);
            this.btnReadWord.Name = "btnReadWord";
            this.btnReadWord.Size = new System.Drawing.Size(92, 23);
            this.btnReadWord.TabIndex = 12;
            this.btnReadWord.Text = "Read_WORD";
            this.btnReadWord.UseVisualStyleBackColor = true;
            this.btnReadWord.Click += new System.EventHandler(this.btnReadWord_Click);
            // 
            // gbxWrite
            // 
            this.gbxWrite.Controls.Add(this.cbxBoolValue);
            this.gbxWrite.Controls.Add(this.btnWriteDint);
            this.gbxWrite.Controls.Add(this.txtDintValue);
            this.gbxWrite.Controls.Add(this.btnWriteUdint);
            this.gbxWrite.Controls.Add(this.txtUdintValue);
            this.gbxWrite.Controls.Add(this.btnWriteReal);
            this.gbxWrite.Controls.Add(this.txtRealValue);
            this.gbxWrite.Controls.Add(this.btnWriteString);
            this.gbxWrite.Controls.Add(this.txtStringValue);
            this.gbxWrite.Controls.Add(this.btnWriteBool);
            this.gbxWrite.Location = new System.Drawing.Point(12, 199);
            this.gbxWrite.Name = "gbxWrite";
            this.gbxWrite.Size = new System.Drawing.Size(317, 211);
            this.gbxWrite.TabIndex = 19;
            this.gbxWrite.TabStop = false;
            this.gbxWrite.Text = "Write";
            // 
            // cbxBoolValue
            // 
            this.cbxBoolValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBoolValue.FormattingEnabled = true;
            this.cbxBoolValue.Items.AddRange(new object[] {
            "TRUE",
            "FALSE"});
            this.cbxBoolValue.Location = new System.Drawing.Point(14, 24);
            this.cbxBoolValue.Name = "cbxBoolValue";
            this.cbxBoolValue.Size = new System.Drawing.Size(100, 21);
            this.cbxBoolValue.TabIndex = 23;
            // 
            // btnWriteDint
            // 
            this.btnWriteDint.Location = new System.Drawing.Point(210, 136);
            this.btnWriteDint.Name = "btnWriteDint";
            this.btnWriteDint.Size = new System.Drawing.Size(92, 23);
            this.btnWriteDint.TabIndex = 22;
            this.btnWriteDint.Text = "Write_DINT";
            this.btnWriteDint.UseVisualStyleBackColor = true;
            this.btnWriteDint.Click += new System.EventHandler(this.btnWriteDint_Click);
            // 
            // txtDintValue
            // 
            this.txtDintValue.Location = new System.Drawing.Point(14, 138);
            this.txtDintValue.Name = "txtDintValue";
            this.txtDintValue.Size = new System.Drawing.Size(100, 20);
            this.txtDintValue.TabIndex = 21;
            this.txtDintValue.Text = "-5555555";
            // 
            // btnWriteUdint
            // 
            this.btnWriteUdint.Location = new System.Drawing.Point(210, 174);
            this.btnWriteUdint.Name = "btnWriteUdint";
            this.btnWriteUdint.Size = new System.Drawing.Size(92, 23);
            this.btnWriteUdint.TabIndex = 20;
            this.btnWriteUdint.Text = "Write_UDINT";
            this.btnWriteUdint.UseVisualStyleBackColor = true;
            this.btnWriteUdint.Click += new System.EventHandler(this.btnWriteUdint_Click);
            // 
            // txtUdintValue
            // 
            this.txtUdintValue.Location = new System.Drawing.Point(14, 174);
            this.txtUdintValue.Name = "txtUdintValue";
            this.txtUdintValue.Size = new System.Drawing.Size(100, 20);
            this.txtUdintValue.TabIndex = 19;
            this.txtUdintValue.Text = "11223344";
            // 
            // btnWriteReal
            // 
            this.btnWriteReal.Location = new System.Drawing.Point(210, 102);
            this.btnWriteReal.Name = "btnWriteReal";
            this.btnWriteReal.Size = new System.Drawing.Size(92, 23);
            this.btnWriteReal.TabIndex = 18;
            this.btnWriteReal.Text = "Write_REAL";
            this.btnWriteReal.UseVisualStyleBackColor = true;
            this.btnWriteReal.Click += new System.EventHandler(this.btnWriteReal_Click);
            // 
            // txtRealValue
            // 
            this.txtRealValue.Location = new System.Drawing.Point(14, 102);
            this.txtRealValue.Name = "txtRealValue";
            this.txtRealValue.Size = new System.Drawing.Size(100, 20);
            this.txtRealValue.TabIndex = 17;
            this.txtRealValue.Text = "21,5678";
            // 
            // btnWriteString
            // 
            this.btnWriteString.Location = new System.Drawing.Point(210, 63);
            this.btnWriteString.Name = "btnWriteString";
            this.btnWriteString.Size = new System.Drawing.Size(92, 23);
            this.btnWriteString.TabIndex = 16;
            this.btnWriteString.Text = "Write_STRING";
            this.btnWriteString.UseVisualStyleBackColor = true;
            this.btnWriteString.Click += new System.EventHandler(this.btnWriteString_Click);
            // 
            // txtStringValue
            // 
            this.txtStringValue.Location = new System.Drawing.Point(14, 65);
            this.txtStringValue.Name = "txtStringValue";
            this.txtStringValue.Size = new System.Drawing.Size(190, 20);
            this.txtStringValue.TabIndex = 15;
            this.txtStringValue.Text = "Yet another test string...";
            // 
            // btnWriteBool
            // 
            this.btnWriteBool.Location = new System.Drawing.Point(210, 24);
            this.btnWriteBool.Name = "btnWriteBool";
            this.btnWriteBool.Size = new System.Drawing.Size(92, 23);
            this.btnWriteBool.TabIndex = 12;
            this.btnWriteBool.Text = "Write_BOOL";
            this.btnWriteBool.UseVisualStyleBackColor = true;
            this.btnWriteBool.Click += new System.EventHandler(this.btnWriteBool_Click);
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(12, 416);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(317, 93);
            this.txtData.TabIndex = 20;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 517);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.gbxWrite);
            this.Controls.Add(this.gbxRead);
            this.Controls.Add(this.gbxConfIp);
            this.Name = "frmMain";
            this.Text = "WagoModbusNet  WinForm Example 03";
            this.gbxConfIp.ResumeLayout(false);
            this.gbxConfIp.PerformLayout();
            this.gbxRead.ResumeLayout(false);
            this.gbxWrite.ResumeLayout(false);
            this.gbxWrite.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxConfIp;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.GroupBox gbxRead;
        private System.Windows.Forms.Button btnReadWord;
        private System.Windows.Forms.GroupBox gbxWrite;
        private System.Windows.Forms.Button btnWriteBool;
        private System.Windows.Forms.Button btnReadDINT;
        private System.Windows.Forms.Button btnReadString;
        private System.Windows.Forms.Button btnReadReal;
        private System.Windows.Forms.Button btnReadUDINT;
        private System.Windows.Forms.Button btnReadBool;
        private System.Windows.Forms.Button btnWriteReal;
        private System.Windows.Forms.TextBox txtRealValue;
        private System.Windows.Forms.Button btnWriteString;
        private System.Windows.Forms.TextBox txtStringValue;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TextBox txtUdintValue;
        private System.Windows.Forms.Button btnWriteUdint;
        private System.Windows.Forms.Button btnWriteDint;
        private System.Windows.Forms.TextBox txtDintValue;
        private System.Windows.Forms.ComboBox cbxBoolValue;
    }
}

