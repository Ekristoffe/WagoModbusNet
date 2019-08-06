using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WagoModbusNet;

namespace WMN_WinFormExample04
{
    public partial class frmMain : Form
    {

        private ModbusMasterUdp mbmUdp;

        public frmMain()
        {
            InitializeComponent();
            mbmUdp = new ModbusMasterUdp();
            mbmUdp.AutoConnect = true;
            mbmUdp.Port = Convert.ToUInt16(txtPort.Text);
        }

        private void btnWriteCoils_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            bool[] writeData = new bool[32];
            writeData[0] = true;
            writeData[1] = false;
            writeData[2] = true;
            writeData[3] = false;
            writeData[4] = true;
            writeData[5] = false;
            writeData[6] = true;
            writeData[7] = false;
            writeData[8] = true;
            writeData[9] = true;
            writeData[10] = true;
            writeData[11] = true;
            writeData[12] = true;
            writeData[13] = true;
            writeData[14] = true;
            writeData[15] = true;
            writeData[16] = false;
            writeData[17] = false;
            writeData[18] = false;
            writeData[19] = false;
            writeData[20] = false;
            writeData[21] = false;
            writeData[22] = false;
            writeData[23] = false;
            writeData[24] = true;
            writeData[25] = true;
            writeData[26] = true;
            writeData[27] = true;
            writeData[28] = false;
            writeData[29] = false;
            writeData[30] = false;
            writeData[31] = false;
            wmnRet _wmnReceiveRet = mbmUdp.WriteMultipleCoils(0, 0, writeData);
            txtData.Text = _wmnReceiveRet.Text;
        }

        private void btnReadCoils_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            bool[] readData;
            ushort readAddr = Convert.ToUInt16(txtReadCoilAddress.Text);
            wmnRet _wmnReceiveRet = mbmUdp.ReadCoils(0, readAddr, 32, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
        }




    }
}