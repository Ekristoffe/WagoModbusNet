using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using WagoModbusNet;

namespace WMN_WinFormExample02
{
    public partial class frmMain : Form
    {
        private ModbusMasterRtu mbm;
        
        public frmMain()
        {
            InitializeComponent(); 

            //Init combobox Ports
            cbxPort.Items.AddRange(SerialPort.GetPortNames());
            cbxPort.SelectedIndex = 0;
            //Init combobox Baudrate
            cbxBaudrate.Items.AddRange(new string[] { "300", "600", "1200", "2400", "9600", "14400", "19200", "38400" , "57600", "115200", "128000" });
            cbxBaudrate.SelectedIndex = 4;
            //Init combobox Databits
            cbxDatabits.Items.AddRange(new string[] { "5", "6", "7", "8" });
            cbxDatabits.SelectedIndex = 3;
            //Init combobox Stopbits
            cbxStopbits.DataSource = Enum.GetValues(typeof(StopBits));
            cbxStopbits.SelectedIndex = 1;
            //Init combobox Parity
            cbxParity.DataSource = Enum.GetValues(typeof(Parity));
            cbxParity.SelectedIndex = 0;
            //Init combobox Handshake
            cbxHandshake.DataSource = Enum.GetValues(typeof(Handshake));
            cbxHandshake.SelectedIndex = 0;

            //Create modbus master instance 
            mbm = new ModbusMasterRtu();
        }

 

        private void btnConnect_Click(object sender, EventArgs e)
        {
            mbm.Portname = cbxPort.SelectedItem.ToString();
            mbm.Baudrate = Convert.ToInt32(cbxBaudrate.SelectedItem);
            mbm.Databits = Convert.ToInt32(cbxDatabits.SelectedItem);
            mbm.Parity = (Parity)Enum.Parse(typeof(Parity), cbxParity.SelectedItem.ToString());
            mbm.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbxStopbits.SelectedItem.ToString());
            mbm.Handshake = (Handshake)Enum.Parse(typeof(Handshake), cbxHandshake.SelectedItem.ToString());
            mbm.Connected = true;
            if (mbm.Connected)
            {
                gbxSlave.Enabled = true;
                //MessageBox .Show( "Connected with " + mbRtu.Portname);
            }
            else
            {
                gbxSlave.Enabled = false;
                //MessageBox.Show("Could not open port: " + mbRtu.Portname);
            }            
        }


  

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            mbm.Disconnect();
            gbxSlave.Enabled = false;
        }


        private void btnRead_Click(object sender, EventArgs e)
        {
            ushort[] readData;
            txtData.Text = "";
            wmnRet ret = mbm.ReadInputRegisters( Convert.ToByte(txtSlaveId.Text), 
                                                    Convert.ToUInt16(txtReadAddress.Text), 
                                                    Convert.ToUInt16(txtReadCount.Text), 
                                                    out readData);
            if (ret.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text = txtData.Text + "Address[" + (Convert.ToUInt16(txtReadAddress.Text) + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = ret.Text;
            }
        }

        private void btnWriteRegister_Click(object sender, EventArgs e)
        {
            ushort[] writeData = new ushort[1];
            writeData[0] = Convert.ToUInt16(txtWriteValue.Text);
            wmnRet ret = mbm.WriteMultipleRegisters(Convert.ToByte(txtSlaveId.Text), 
                                                    Convert.ToUInt16(txtWriteAddress.Text),                                                   
                                                    writeData);
            txtData.Text = ret.Text; ;
        }
    }


}