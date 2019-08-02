namespace WMN_WinFormExample01
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
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblHost = new System.Windows.Forms.Label();
            this.gbxUnit = new System.Windows.Forms.GroupBox();
            this.gbxWrite = new System.Windows.Forms.GroupBox();
            this.txtWriteValue = new System.Windows.Forms.TextBox();
            this.txtWriteAddress = new System.Windows.Forms.TextBox();
            this.btnWriteRegister = new System.Windows.Forms.Button();
            this.lblWriteValue = new System.Windows.Forms.Label();
            this.lblWriteAddress = new System.Windows.Forms.Label();
            this.gbxRead = new System.Windows.Forms.GroupBox();
            this.txtData = new System.Windows.Forms.TextBox();
            this.txtReadCount = new System.Windows.Forms.TextBox();
            this.txtReadAddress = new System.Windows.Forms.TextBox();
            this.btnRead = new System.Windows.Forms.Button();
            this.lblReadCount = new System.Windows.Forms.Label();
            this.lblReadAddress = new System.Windows.Forms.Label();
            this.txtUnitId = new System.Windows.Forms.TextBox();
            this.lblUnitId = new System.Windows.Forms.Label();
            this.gbxConfIp.SuspendLayout();
            this.gbxUnit.SuspendLayout();
            this.gbxWrite.SuspendLayout();
            this.gbxRead.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxConfIp
            // 
            this.gbxConfIp.Controls.Add(this.btnDisconnect);
            this.gbxConfIp.Controls.Add(this.btnConnect);
            this.gbxConfIp.Controls.Add(this.txtPort);
            this.gbxConfIp.Controls.Add(this.txtHost);
            this.gbxConfIp.Controls.Add(this.lblPort);
            this.gbxConfIp.Controls.Add(this.lblHost);
            this.gbxConfIp.Location = new System.Drawing.Point(1, 12);
            this.gbxConfIp.Name = "gbxConfIp";
            this.gbxConfIp.Size = new System.Drawing.Size(328, 82);
            this.gbxConfIp.TabIndex = 11;
            this.gbxConfIp.TabStop = false;
            this.gbxConfIp.Text = "Config";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(227, 48);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 21);
            this.btnDisconnect.TabIndex = 14;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(227, 19);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 21);
            this.btnConnect.TabIndex = 13;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
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
            // gbxUnit
            // 
            this.gbxUnit.Controls.Add(this.gbxWrite);
            this.gbxUnit.Controls.Add(this.gbxRead);
            this.gbxUnit.Controls.Add(this.txtUnitId);
            this.gbxUnit.Controls.Add(this.lblUnitId);
            this.gbxUnit.Location = new System.Drawing.Point(1, 100);
            this.gbxUnit.Name = "gbxUnit";
            this.gbxUnit.Size = new System.Drawing.Size(328, 349);
            this.gbxUnit.TabIndex = 13;
            this.gbxUnit.TabStop = false;
            this.gbxUnit.Text = "Unit";
            // 
            // gbxWrite
            // 
            this.gbxWrite.Controls.Add(this.txtWriteValue);
            this.gbxWrite.Controls.Add(this.txtWriteAddress);
            this.gbxWrite.Controls.Add(this.btnWriteRegister);
            this.gbxWrite.Controls.Add(this.lblWriteValue);
            this.gbxWrite.Controls.Add(this.lblWriteAddress);
            this.gbxWrite.Location = new System.Drawing.Point(6, 253);
            this.gbxWrite.Name = "gbxWrite";
            this.gbxWrite.Size = new System.Drawing.Size(316, 89);
            this.gbxWrite.TabIndex = 18;
            this.gbxWrite.TabStop = false;
            this.gbxWrite.Text = "WriteRegister";
            // 
            // txtWriteValue
            // 
            this.txtWriteValue.Location = new System.Drawing.Point(142, 46);
            this.txtWriteValue.Name = "txtWriteValue";
            this.txtWriteValue.Size = new System.Drawing.Size(55, 20);
            this.txtWriteValue.TabIndex = 14;
            this.txtWriteValue.Text = "4711";
            // 
            // txtWriteAddress
            // 
            this.txtWriteAddress.Location = new System.Drawing.Point(35, 46);
            this.txtWriteAddress.Name = "txtWriteAddress";
            this.txtWriteAddress.Size = new System.Drawing.Size(71, 20);
            this.txtWriteAddress.TabIndex = 13;
            this.txtWriteAddress.Text = "12288";
            // 
            // btnWriteRegister
            // 
            this.btnWriteRegister.Location = new System.Drawing.Point(221, 44);
            this.btnWriteRegister.Name = "btnWriteRegister";
            this.btnWriteRegister.Size = new System.Drawing.Size(75, 23);
            this.btnWriteRegister.TabIndex = 12;
            this.btnWriteRegister.Text = "Write";
            this.btnWriteRegister.UseVisualStyleBackColor = true;
            this.btnWriteRegister.Click += new System.EventHandler(this.btnWriteRegister_Click);
            // 
            // lblWriteValue
            // 
            this.lblWriteValue.AutoSize = true;
            this.lblWriteValue.Location = new System.Drawing.Point(139, 30);
            this.lblWriteValue.Name = "lblWriteValue";
            this.lblWriteValue.Size = new System.Drawing.Size(34, 13);
            this.lblWriteValue.TabIndex = 11;
            this.lblWriteValue.Text = "Value";
            // 
            // lblWriteAddress
            // 
            this.lblWriteAddress.AutoSize = true;
            this.lblWriteAddress.Location = new System.Drawing.Point(32, 30);
            this.lblWriteAddress.Name = "lblWriteAddress";
            this.lblWriteAddress.Size = new System.Drawing.Size(45, 13);
            this.lblWriteAddress.TabIndex = 10;
            this.lblWriteAddress.Text = "Address";
            // 
            // gbxRead
            // 
            this.gbxRead.Controls.Add(this.txtData);
            this.gbxRead.Controls.Add(this.txtReadCount);
            this.gbxRead.Controls.Add(this.txtReadAddress);
            this.gbxRead.Controls.Add(this.btnRead);
            this.gbxRead.Controls.Add(this.lblReadCount);
            this.gbxRead.Controls.Add(this.lblReadAddress);
            this.gbxRead.Location = new System.Drawing.Point(6, 51);
            this.gbxRead.Name = "gbxRead";
            this.gbxRead.Size = new System.Drawing.Size(316, 196);
            this.gbxRead.TabIndex = 17;
            this.gbxRead.TabStop = false;
            this.gbxRead.Text = "Read Input Register";
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(11, 86);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(285, 84);
            this.txtData.TabIndex = 15;
            // 
            // txtReadCount
            // 
            this.txtReadCount.Location = new System.Drawing.Point(142, 46);
            this.txtReadCount.Name = "txtReadCount";
            this.txtReadCount.Size = new System.Drawing.Size(55, 20);
            this.txtReadCount.TabIndex = 14;
            this.txtReadCount.Text = "5";
            // 
            // txtReadAddress
            // 
            this.txtReadAddress.Location = new System.Drawing.Point(35, 46);
            this.txtReadAddress.Name = "txtReadAddress";
            this.txtReadAddress.Size = new System.Drawing.Size(71, 20);
            this.txtReadAddress.TabIndex = 13;
            this.txtReadAddress.Text = "12288";
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(221, 44);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(75, 23);
            this.btnRead.TabIndex = 12;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // lblReadCount
            // 
            this.lblReadCount.AutoSize = true;
            this.lblReadCount.Location = new System.Drawing.Point(139, 30);
            this.lblReadCount.Name = "lblReadCount";
            this.lblReadCount.Size = new System.Drawing.Size(35, 13);
            this.lblReadCount.TabIndex = 11;
            this.lblReadCount.Text = "Count";
            // 
            // lblReadAddress
            // 
            this.lblReadAddress.AutoSize = true;
            this.lblReadAddress.Location = new System.Drawing.Point(32, 30);
            this.lblReadAddress.Name = "lblReadAddress";
            this.lblReadAddress.Size = new System.Drawing.Size(45, 13);
            this.lblReadAddress.TabIndex = 10;
            this.lblReadAddress.Text = "Address";
            // 
            // txtUnitId
            // 
            this.txtUnitId.Location = new System.Drawing.Point(60, 22);
            this.txtUnitId.Name = "txtUnitId";
            this.txtUnitId.Size = new System.Drawing.Size(52, 20);
            this.txtUnitId.TabIndex = 16;
            this.txtUnitId.Text = "0";
            // 
            // lblUnitId
            // 
            this.lblUnitId.AutoSize = true;
            this.lblUnitId.Location = new System.Drawing.Point(14, 25);
            this.lblUnitId.Name = "lblUnitId";
            this.lblUnitId.Size = new System.Drawing.Size(40, 13);
            this.lblUnitId.TabIndex = 15;
            this.lblUnitId.Text = "Unit-ID";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 453);
            this.Controls.Add(this.gbxUnit);
            this.Controls.Add(this.gbxConfIp);
            this.Name = "frmMain";
            this.Text = "WagoModbusNet  WinForm Example 01";
            this.gbxConfIp.ResumeLayout(false);
            this.gbxConfIp.PerformLayout();
            this.gbxUnit.ResumeLayout(false);
            this.gbxUnit.PerformLayout();
            this.gbxWrite.ResumeLayout(false);
            this.gbxWrite.PerformLayout();
            this.gbxRead.ResumeLayout(false);
            this.gbxRead.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxConfIp;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.GroupBox gbxUnit;
        private System.Windows.Forms.GroupBox gbxWrite;
        private System.Windows.Forms.TextBox txtWriteValue;
        private System.Windows.Forms.TextBox txtWriteAddress;
        private System.Windows.Forms.Button btnWriteRegister;
        private System.Windows.Forms.Label lblWriteValue;
        private System.Windows.Forms.Label lblWriteAddress;
        private System.Windows.Forms.GroupBox gbxRead;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.TextBox txtReadCount;
        private System.Windows.Forms.TextBox txtReadAddress;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Label lblReadCount;
        private System.Windows.Forms.Label lblReadAddress;
        private System.Windows.Forms.TextBox txtUnitId;
        private System.Windows.Forms.Label lblUnitId;
    }
}

