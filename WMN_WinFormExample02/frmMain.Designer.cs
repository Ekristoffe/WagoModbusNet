namespace WMN_WinFormExample02
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
            this.cbxBaudrate = new System.Windows.Forms.ComboBox();
            this.cbxDatabits = new System.Windows.Forms.ComboBox();
            this.cbxStopbits = new System.Windows.Forms.ComboBox();
            this.lblStopbits = new System.Windows.Forms.Label();
            this.lblHandshake = new System.Windows.Forms.Label();
            this.cbxHandshake = new System.Windows.Forms.ComboBox();
            this.lblParity = new System.Windows.Forms.Label();
            this.cbxParity = new System.Windows.Forms.ComboBox();
            this.lblDatabits = new System.Windows.Forms.Label();
            this.cbxPort = new System.Windows.Forms.ComboBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblBaudrate = new System.Windows.Forms.Label();
            this.gbxSlave = new System.Windows.Forms.GroupBox();
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
            this.txtSlaveId = new System.Windows.Forms.TextBox();
            this.lblSlaveId = new System.Windows.Forms.Label();
            this.gbxConfIp.SuspendLayout();
            this.gbxSlave.SuspendLayout();
            this.gbxWrite.SuspendLayout();
            this.gbxRead.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxConfIp
            // 
            this.gbxConfIp.Controls.Add(this.cbxBaudrate);
            this.gbxConfIp.Controls.Add(this.cbxDatabits);
            this.gbxConfIp.Controls.Add(this.cbxStopbits);
            this.gbxConfIp.Controls.Add(this.lblStopbits);
            this.gbxConfIp.Controls.Add(this.lblHandshake);
            this.gbxConfIp.Controls.Add(this.cbxHandshake);
            this.gbxConfIp.Controls.Add(this.lblParity);
            this.gbxConfIp.Controls.Add(this.cbxParity);
            this.gbxConfIp.Controls.Add(this.lblDatabits);
            this.gbxConfIp.Controls.Add(this.cbxPort);
            this.gbxConfIp.Controls.Add(this.btnDisconnect);
            this.gbxConfIp.Controls.Add(this.btnConnect);
            this.gbxConfIp.Controls.Add(this.lblPort);
            this.gbxConfIp.Controls.Add(this.lblBaudrate);
            this.gbxConfIp.Location = new System.Drawing.Point(12, 12);
            this.gbxConfIp.Name = "gbxConfIp";
            this.gbxConfIp.Size = new System.Drawing.Size(332, 184);
            this.gbxConfIp.TabIndex = 14;
            this.gbxConfIp.TabStop = false;
            this.gbxConfIp.Text = "Config";
            // 
            // cbxBaudrate
            // 
            this.cbxBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxBaudrate.FormattingEnabled = true;
            this.cbxBaudrate.Location = new System.Drawing.Point(11, 93);
            this.cbxBaudrate.Name = "cbxBaudrate";
            this.cbxBaudrate.Size = new System.Drawing.Size(136, 21);
            this.cbxBaudrate.TabIndex = 25;
            // 
            // cbxDatabits
            // 
            this.cbxDatabits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDatabits.FormattingEnabled = true;
            this.cbxDatabits.Location = new System.Drawing.Point(166, 93);
            this.cbxDatabits.Name = "cbxDatabits";
            this.cbxDatabits.Size = new System.Drawing.Size(60, 21);
            this.cbxDatabits.TabIndex = 24;
            // 
            // cbxStopbits
            // 
            this.cbxStopbits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStopbits.FormattingEnabled = true;
            this.cbxStopbits.Location = new System.Drawing.Point(236, 93);
            this.cbxStopbits.Name = "cbxStopbits";
            this.cbxStopbits.Size = new System.Drawing.Size(60, 21);
            this.cbxStopbits.TabIndex = 23;
            // 
            // lblStopbits
            // 
            this.lblStopbits.AutoSize = true;
            this.lblStopbits.Location = new System.Drawing.Point(233, 77);
            this.lblStopbits.Name = "lblStopbits";
            this.lblStopbits.Size = new System.Drawing.Size(45, 13);
            this.lblStopbits.TabIndex = 22;
            this.lblStopbits.Text = "Stopbits";
            // 
            // lblHandshake
            // 
            this.lblHandshake.AutoSize = true;
            this.lblHandshake.Location = new System.Drawing.Point(164, 130);
            this.lblHandshake.Name = "lblHandshake";
            this.lblHandshake.Size = new System.Drawing.Size(62, 13);
            this.lblHandshake.TabIndex = 20;
            this.lblHandshake.Text = "Handshake";
            // 
            // cbxHandshake
            // 
            this.cbxHandshake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxHandshake.FormattingEnabled = true;
            this.cbxHandshake.Location = new System.Drawing.Point(166, 146);
            this.cbxHandshake.Name = "cbxHandshake";
            this.cbxHandshake.Size = new System.Drawing.Size(130, 21);
            this.cbxHandshake.TabIndex = 19;
            // 
            // lblParity
            // 
            this.lblParity.AutoSize = true;
            this.lblParity.Location = new System.Drawing.Point(11, 130);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(33, 13);
            this.lblParity.TabIndex = 18;
            this.lblParity.Text = "Parity";
            // 
            // cbxParity
            // 
            this.cbxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxParity.FormattingEnabled = true;
            this.cbxParity.Location = new System.Drawing.Point(14, 146);
            this.cbxParity.Name = "cbxParity";
            this.cbxParity.Size = new System.Drawing.Size(133, 21);
            this.cbxParity.TabIndex = 17;
            // 
            // lblDatabits
            // 
            this.lblDatabits.AutoSize = true;
            this.lblDatabits.Location = new System.Drawing.Point(163, 77);
            this.lblDatabits.Name = "lblDatabits";
            this.lblDatabits.Size = new System.Drawing.Size(46, 13);
            this.lblDatabits.TabIndex = 16;
            this.lblDatabits.Text = "Databits";
            // 
            // cbxPort
            // 
            this.cbxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPort.FormattingEnabled = true;
            this.cbxPort.Location = new System.Drawing.Point(14, 32);
            this.cbxPort.Name = "cbxPort";
            this.cbxPort.Size = new System.Drawing.Size(92, 21);
            this.cbxPort.TabIndex = 15;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(137, 32);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 14;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(221, 32);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 13;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(11, 16);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 8;
            this.lblPort.Text = "Port";
            // 
            // lblBaudrate
            // 
            this.lblBaudrate.AutoSize = true;
            this.lblBaudrate.Location = new System.Drawing.Point(11, 77);
            this.lblBaudrate.Name = "lblBaudrate";
            this.lblBaudrate.Size = new System.Drawing.Size(50, 13);
            this.lblBaudrate.TabIndex = 7;
            this.lblBaudrate.Text = "Baudrate";
            // 
            // gbxSlave
            // 
            this.gbxSlave.Controls.Add(this.gbxWrite);
            this.gbxSlave.Controls.Add(this.gbxRead);
            this.gbxSlave.Controls.Add(this.txtSlaveId);
            this.gbxSlave.Controls.Add(this.lblSlaveId);
            this.gbxSlave.Enabled = false;
            this.gbxSlave.Location = new System.Drawing.Point(12, 202);
            this.gbxSlave.Name = "gbxSlave";
            this.gbxSlave.Size = new System.Drawing.Size(332, 322);
            this.gbxSlave.TabIndex = 16;
            this.gbxSlave.TabStop = false;
            this.gbxSlave.Text = "Slave";
            // 
            // gbxWrite
            // 
            this.gbxWrite.Controls.Add(this.txtWriteValue);
            this.gbxWrite.Controls.Add(this.txtWriteAddress);
            this.gbxWrite.Controls.Add(this.btnWriteRegister);
            this.gbxWrite.Controls.Add(this.lblWriteValue);
            this.gbxWrite.Controls.Add(this.lblWriteAddress);
            this.gbxWrite.Location = new System.Drawing.Point(6, 223);
            this.gbxWrite.Name = "gbxWrite";
            this.gbxWrite.Size = new System.Drawing.Size(316, 89);
            this.gbxWrite.TabIndex = 18;
            this.gbxWrite.TabStop = false;
            this.gbxWrite.Text = "WriteRegister";
            // 
            // txtWriteValue
            // 
            this.txtWriteValue.Location = new System.Drawing.Point(119, 46);
            this.txtWriteValue.Name = "txtWriteValue";
            this.txtWriteValue.Size = new System.Drawing.Size(55, 20);
            this.txtWriteValue.TabIndex = 14;
            this.txtWriteValue.Text = "4711";
            // 
            // txtWriteAddress
            // 
            this.txtWriteAddress.Location = new System.Drawing.Point(19, 46);
            this.txtWriteAddress.Name = "txtWriteAddress";
            this.txtWriteAddress.Size = new System.Drawing.Size(71, 20);
            this.txtWriteAddress.TabIndex = 13;
            this.txtWriteAddress.Text = "0";
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
            this.lblWriteValue.Location = new System.Drawing.Point(117, 30);
            this.lblWriteValue.Name = "lblWriteValue";
            this.lblWriteValue.Size = new System.Drawing.Size(34, 13);
            this.lblWriteValue.TabIndex = 11;
            this.lblWriteValue.Text = "Value";
            // 
            // lblWriteAddress
            // 
            this.lblWriteAddress.AutoSize = true;
            this.lblWriteAddress.Location = new System.Drawing.Point(16, 30);
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
            this.gbxRead.Location = new System.Drawing.Point(6, 58);
            this.gbxRead.Name = "gbxRead";
            this.gbxRead.Size = new System.Drawing.Size(316, 159);
            this.gbxRead.TabIndex = 17;
            this.gbxRead.TabStop = false;
            this.gbxRead.Text = "Read Input Register";
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(11, 61);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(285, 84);
            this.txtData.TabIndex = 15;
            // 
            // txtReadCount
            // 
            this.txtReadCount.Location = new System.Drawing.Point(120, 34);
            this.txtReadCount.Name = "txtReadCount";
            this.txtReadCount.Size = new System.Drawing.Size(55, 20);
            this.txtReadCount.TabIndex = 14;
            this.txtReadCount.Text = "5";
            // 
            // txtReadAddress
            // 
            this.txtReadAddress.Location = new System.Drawing.Point(14, 34);
            this.txtReadAddress.Name = "txtReadAddress";
            this.txtReadAddress.Size = new System.Drawing.Size(71, 20);
            this.txtReadAddress.TabIndex = 13;
            this.txtReadAddress.Text = "0";
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(221, 32);
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
            this.lblReadCount.Location = new System.Drawing.Point(116, 18);
            this.lblReadCount.Name = "lblReadCount";
            this.lblReadCount.Size = new System.Drawing.Size(35, 13);
            this.lblReadCount.TabIndex = 11;
            this.lblReadCount.Text = "Count";
            // 
            // lblReadAddress
            // 
            this.lblReadAddress.AutoSize = true;
            this.lblReadAddress.Location = new System.Drawing.Point(16, 18);
            this.lblReadAddress.Name = "lblReadAddress";
            this.lblReadAddress.Size = new System.Drawing.Size(45, 13);
            this.lblReadAddress.TabIndex = 10;
            this.lblReadAddress.Text = "Address";
            // 
            // txtSlaveId
            // 
            this.txtSlaveId.Location = new System.Drawing.Point(66, 23);
            this.txtSlaveId.Name = "txtSlaveId";
            this.txtSlaveId.Size = new System.Drawing.Size(55, 20);
            this.txtSlaveId.TabIndex = 16;
            this.txtSlaveId.Text = "1";
            // 
            // lblSlaveId
            // 
            this.lblSlaveId.AutoSize = true;
            this.lblSlaveId.Location = new System.Drawing.Point(17, 26);
            this.lblSlaveId.Name = "lblSlaveId";
            this.lblSlaveId.Size = new System.Drawing.Size(48, 13);
            this.lblSlaveId.TabIndex = 15;
            this.lblSlaveId.Text = "Slave-ID";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 534);
            this.Controls.Add(this.gbxSlave);
            this.Controls.Add(this.gbxConfIp);
            this.Name = "frmMain";
            this.Text = "WagoModbusNet  WinForm Example 02";
            this.gbxConfIp.ResumeLayout(false);
            this.gbxConfIp.PerformLayout();
            this.gbxSlave.ResumeLayout(false);
            this.gbxSlave.PerformLayout();
            this.gbxWrite.ResumeLayout(false);
            this.gbxWrite.PerformLayout();
            this.gbxRead.ResumeLayout(false);
            this.gbxRead.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxConfIp;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblBaudrate;
        private System.Windows.Forms.Label lblDatabits;
        private System.Windows.Forms.ComboBox cbxPort;
        private System.Windows.Forms.Label lblParity;
        private System.Windows.Forms.ComboBox cbxParity;
        private System.Windows.Forms.Label lblHandshake;
        private System.Windows.Forms.ComboBox cbxHandshake;
        private System.Windows.Forms.ComboBox cbxStopbits;
        private System.Windows.Forms.Label lblStopbits;
        private System.Windows.Forms.ComboBox cbxDatabits;
        private System.Windows.Forms.ComboBox cbxBaudrate;
        private System.Windows.Forms.GroupBox gbxSlave;
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
        private System.Windows.Forms.TextBox txtSlaveId;
        private System.Windows.Forms.Label lblSlaveId;
    }
}

