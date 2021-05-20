using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WagoModbusNet;

namespace WMN_WinFormExample03
{
    public partial class frmMain : Form
    {
        private ModbusMasterUdp mbmUdp;

        public frmMain()
        {
            InitializeComponent();
            cbxBoolValue.SelectedIndex = 0;            
            mbmUdp = new ModbusMasterUdp();
            mbmUdp.AutoConnect = true;
            mbmUdp.Port = Convert.ToUInt16(txtPort.Text);

        }

        private void btnReadBool_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            try
            {
                bool[] readData = mbmUdp.ReadCoils(0, 0x3000, 2);
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (0x3000 + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnReadWord_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            try
            {
                ushort[] readData = mbmUdp.ReadInputRegisters(0, 12298, 2);
                for (int i = 0; i < readData.Length; i++)
                {
                txtData.Text += "Address[" + (12298 + i).ToString() + "] Value: " + readData[i].ToString() + ";  \r\n";
                }
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnReadDINT_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            try
            {
                // To read one DINT(32bit) value from PLC, you have to read two register(16bit)  
                ushort[] readData = mbmUdp.ReadInputRegisters(0, 12308, 4);

                // Convert ushort[] into int[]
                int[] data = WagoModbusNet.Utilities.wmnConvert.ToInt32(readData);
                for (int i = 0; i < data.Length; i++)
                {
                    txtData.Text += "Address[" + (12308 + (2 * i)).ToString() + "] Value: " + data[i].ToString() + "; \r\n";
                }
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnReadUDINT_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            try
            {
                // To read one UDINT(32bit) value from PLC, you have to read two register(16bit)  
                ushort[] readData = mbmUdp.ReadInputRegisters(0, 12328, 4);

                // Convert ushort[] into uint[]
                uint[] data = WagoModbusNet.Utilities.wmnConvert.ToUInt32(readData);
                for (int i = 0; i < data.Length; i++)
                {
                    txtData.Text += "Address[" + (12328 + (2 * i)).ToString() + "] Value: " + data[i].ToString() + "; \r\n";
                }
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnReadReal_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            try
            {
                // To read one REAL(32bit) value from PLC, you have to read two register(16bit)  
                ushort[] readData = mbmUdp.ReadInputRegisters(0, 12348, 4);

                // Convert ushort[] into float[]
                float[] data = WagoModbusNet.Utilities.wmnConvert.ToSingle(readData);
                for (int i = 0; i < data.Length; i++)
                {
                    txtData.Text += "Address[" + (12348 + (2*i)).ToString() + "] Value: " + data[i].ToString() + "; \r\n";
                }
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnReadString_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            try
            {
                // To read a STRING(80) value from PLC, you have to read 40 register(16bit)  
                ushort[] readData = mbmUdp.ReadInputRegisters(0, 12388, 50);
               
                txtData.Text += "Address[" + 12388.ToString() + "] Value: " + WagoModbusNet.Utilities.wmnConvert.ToString(readData) + "; \r\n";

                readData = mbmUdp.ReadInputRegisters(0, 12438, 50);

                txtData.Text += "Address[" + 12438.ToString() + "] Value: " + WagoModbusNet.Utilities.wmnConvert.ToString(readData) + ";\r\n";
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnWriteBool_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text; 
            bool writeData = Convert.ToBoolean(cbxBoolValue.Text);
            try
            {
                mbmUdp.WriteSingleCoil(0, 12288, writeData);
                txtData.Text = "Successful executed";
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnWriteString_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            ushort[] writeData = WagoModbusNet.Utilities.wmnConvert.ToUInt16(txtStringValue.Text);
            try
            {
                mbmUdp.WriteMultipleRegisters(0, 12388, writeData);
                txtData.Text = "Successful executed";
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnWriteReal_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;             
            ushort[] writeData = WagoModbusNet.Utilities.wmnConvert.ToUInt16(Convert.ToSingle(txtRealValue.Text));
            //float[] values = { 1.42f, 9.81f, 3.14f }; 
            //ushort[] writeData = wmnConvert.ToUInt16(values);
            try
            {
                mbmUdp.WriteMultipleRegisters(0, 12348, writeData);
                txtData.Text = "Successful executed";
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnWriteDint_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            //ushort[] writeData = wmnConvert.ToUInt16(Convert.ToInt32(txtDintValue.Text));
            int[] values = { -14202, -98150, 31470 };
            ushort[] writeData = WagoModbusNet.Utilities.wmnConvert.ToUInt16(values);
            try
            {
                mbmUdp.WriteMultipleRegisters(0, 12308, writeData);
                txtData.Text = "Successful executed";
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

        private void btnWriteUdint_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            //ushort[] writeData = wmnConvert.ToUInt16(Convert.ToUInt32(txtUdintValue.Text));
            uint[] values = { 14202, 98150, 31470 };
            ushort[] writeData = WagoModbusNet.Utilities.wmnConvert.ToUInt16(values);
            try
            {
                mbmUdp.WriteMultipleRegisters(0, 12328, writeData);
                txtData.Text = "Successful executed";
            }
            catch (Exception ex)
            {
                txtData.Text = ex.Message;
            }
        }

 
    }
}