namespace WMN_WinFormExample04
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
            this.gbxReadCoils = new System.Windows.Forms.GroupBox();
            this.txtReadCoilAddress = new System.Windows.Forms.TextBox();
            this.btnReadCoils = new System.Windows.Forms.Button();
            this.lblReadAddress = new System.Windows.Forms.Label();
            this.gbxWrite = new System.Windows.Forms.GroupBox();
            this.txtWriteAddress = new System.Windows.Forms.TextBox();
            this.btnWriteCoils = new System.Windows.Forms.Button();
            this.lblWriteAddress = new System.Windows.Forms.Label();
            this.txtData = new System.Windows.Forms.TextBox();
            this.gbxConfIp.SuspendLayout();
            this.gbxReadCoils.SuspendLayout();
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
            this.gbxConfIp.Size = new System.Drawing.Size(328, 82);
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
            // gbxReadCoils
            // 
            this.gbxReadCoils.Controls.Add(this.txtReadCoilAddress);
            this.gbxReadCoils.Controls.Add(this.btnReadCoils);
            this.gbxReadCoils.Controls.Add(this.lblReadAddress);
            this.gbxReadCoils.Location = new System.Drawing.Point(12, 187);
            this.gbxReadCoils.Name = "gbxReadCoils";
            this.gbxReadCoils.Size = new System.Drawing.Size(328, 79);
            this.gbxReadCoils.TabIndex = 18;
            this.gbxReadCoils.TabStop = false;
            this.gbxReadCoils.Text = "Read Coils (FC1)";
            // 
            // txtReadCoilAddress
            // 
            this.txtReadCoilAddress.Location = new System.Drawing.Point(35, 46);
            this.txtReadCoilAddress.Name = "txtReadCoilAddress";
            this.txtReadCoilAddress.Size = new System.Drawing.Size(71, 20);
            this.txtReadCoilAddress.TabIndex = 13;
            this.txtReadCoilAddress.Text = "512";
            // 
            // btnReadCoils
            // 
            this.btnReadCoils.Location = new System.Drawing.Point(221, 44);
            this.btnReadCoils.Name = "btnReadCoils";
            this.btnReadCoils.Size = new System.Drawing.Size(75, 23);
            this.btnReadCoils.TabIndex = 12;
            this.btnReadCoils.Text = "Read";
            this.btnReadCoils.UseVisualStyleBackColor = true;
            this.btnReadCoils.Click += new System.EventHandler(this.btnReadCoils_Click);
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
            // gbxWrite
            // 
            this.gbxWrite.Controls.Add(this.txtWriteAddress);
            this.gbxWrite.Controls.Add(this.btnWriteCoils);
            this.gbxWrite.Controls.Add(this.lblWriteAddress);
            this.gbxWrite.Location = new System.Drawing.Point(12, 100);
            this.gbxWrite.Name = "gbxWrite";
            this.gbxWrite.Size = new System.Drawing.Size(328, 81);
            this.gbxWrite.TabIndex = 20;
            this.gbxWrite.TabStop = false;
            this.gbxWrite.Text = "Write Multiple Coils (FC15)";
            // 
            // txtWriteAddress
            // 
            this.txtWriteAddress.Location = new System.Drawing.Point(35, 46);
            this.txtWriteAddress.Name = "txtWriteAddress";
            this.txtWriteAddress.Size = new System.Drawing.Size(71, 20);
            this.txtWriteAddress.TabIndex = 13;
            this.txtWriteAddress.Text = "0";
            // 
            // btnWriteCoils
            // 
            this.btnWriteCoils.Location = new System.Drawing.Point(227, 43);
            this.btnWriteCoils.Name = "btnWriteCoils";
            this.btnWriteCoils.Size = new System.Drawing.Size(75, 23);
            this.btnWriteCoils.TabIndex = 12;
            this.btnWriteCoils.Text = "Write";
            this.btnWriteCoils.UseVisualStyleBackColor = true;
            this.btnWriteCoils.Click += new System.EventHandler(this.btnWriteCoils_Click);
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
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(12, 272);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(328, 383);
            this.txtData.TabIndex = 21;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 662);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.gbxWrite);
            this.Controls.Add(this.gbxReadCoils);
            this.Controls.Add(this.gbxConfIp);
            this.Name = "frmMain";
            this.Text = "WagoModbusNet  WinForm Example 04";
            this.gbxConfIp.ResumeLayout(false);
            this.gbxConfIp.PerformLayout();
            this.gbxReadCoils.ResumeLayout(false);
            this.gbxReadCoils.PerformLayout();
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
        private System.Windows.Forms.GroupBox gbxReadCoils;
        private System.Windows.Forms.TextBox txtReadCoilAddress;
        private System.Windows.Forms.Button btnReadCoils;
        private System.Windows.Forms.Label lblReadAddress;
        private System.Windows.Forms.GroupBox gbxWrite;
        private System.Windows.Forms.TextBox txtWriteAddress;
        private System.Windows.Forms.Button btnWriteCoils;
        private System.Windows.Forms.Label lblWriteAddress;
        private System.Windows.Forms.TextBox txtData;
    }
}

