using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WagoModbusNet;
using System.Threading;

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

        // The ThreadProc method is called when the thread starts.
        // It loops ten times, writing to the console and yielding 
        // the rest of its time slice each time, and then ends.
        public void ThreadProc(object obj)
        {
            ModbusMasterUdp mbmUdpt = new ModbusMasterUdp();
            mbmUdpt.AutoConnect = true;
            mbmUdpt.Port = 502;
            mbmUdpt.Hostname = (string)obj;

            bool[] readData;
            wmnRet _wmnReceiveRet = mbmUdpt.ReadCoils(0, 0x3000, 2, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    Console.WriteLine(mbmUdpt.Hostname + ":" + mbmUdpt.Port.ToString() + " Address[" + (0x3000 + i).ToString() + "] Value: " + readData[i].ToString() + ";");
                }
            }
            else
            {
                Console.WriteLine(_wmnReceiveRet.Text);
            }
        }

        private void btnReadBool_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            bool[] readData;
            wmnRet _wmnReceiveRet = mbmUdp.ReadCoils(0, 0x3000, 2, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (0x3000 + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
            Thread t1 = new Thread(new ParameterizedThreadStart(ThreadProc));
            Thread t2 = new Thread(new ParameterizedThreadStart(ThreadProc));
            t1.Start(txtHost.Text);
            t2.Start("192.168.1.1");
        }

        private void btnReadWord_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text; 
            ushort[] readData;
            wmnRet _wmnReceiveRet = mbmUdp.ReadInputRegisters(0, 12298, 2, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (12298 + i).ToString() + "] Value: " + readData[i].ToString() + ";  \r\n";
                }
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
        }

        private void btnReadDINT_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            ushort[] readData;
            // To read one DINT(32bit) value from PLC, you have to read two register(16bit)  
            wmnRet _wmnReceiveRet = mbmUdp.ReadInputRegisters(0, 12308, 4, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                // Convert ushort[] into int[]
                int[] data = wmnConvert.ToInt32(readData);
                for (int i = 0; i < data.Length; i++)
                {
                    txtData.Text += "Address[" + (12308 + (2 * i)).ToString() + "] Value: " + data[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
        }

        private void btnReadUDINT_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            ushort[] readData;
            // To read one UDINT(32bit) value from PLC, you have to read two register(16bit)  
            wmnRet _wmnReceiveRet = mbmUdp.ReadInputRegisters(0, 12328, 4, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                // Convert ushort[] into uint[]
                uint[] data = wmnConvert.ToUInt32(readData);
                for (int i = 0; i < data.Length; i++)
                {
                    txtData.Text += "Address[" + (12328 + (2 * i)).ToString() + "] Value: " + data[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
        }

        private void btnReadReal_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text; 
            ushort[] readData;           
            // To read one REAL(32bit) value from PLC, you have to read two register(16bit)  
            wmnRet _wmnReceiveRet = mbmUdp.ReadInputRegisters(0, 12348, 4, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                // Convert ushort[] into float[]
                float[] data = wmnConvert.ToSingle(readData);
                for (int i = 0; i < data.Length; i++)
                {
                    txtData.Text += "Address[" + (12348 + (2*i)).ToString() + "] Value: " + data[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
        }

        private void btnReadString_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            ushort[] readData;
            // To read a STRING(80) value from PLC, you have to read 40 register(16bit)  
            wmnRet _wmnReceiveRet = mbmUdp.ReadInputRegisters(0, 12388, 50, out readData);
            if (_wmnReceiveRet.Value == 0)
            {                
                txtData.Text += "Address[" + 12388.ToString() + "] Value: " + wmnConvert.ToString(readData) + "; \r\n";
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
            _wmnReceiveRet = mbmUdp.ReadInputRegisters(0, 12438, 50, out readData);
            if (_wmnReceiveRet.Value == 0)
            {
                txtData.Text += "Address[" + 12438.ToString() + "] Value: " + wmnConvert.ToString(readData) + ";\r\n";
            }
            else
            {
                txtData.Text = _wmnReceiveRet.Text;
            }
        }

        private void btnWriteBool_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text; 
            bool writeData = Convert.ToBoolean(cbxBoolValue.Text);
            wmnRet _wmnReceiveRet = mbmUdp.WriteSingleCoil(0, 12288, writeData);
            txtData.Text = _wmnReceiveRet.Text;
        }

        private void btnWriteString_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            ushort[] writeData = wmnConvert.ToUInt16(txtStringValue.Text);
            wmnRet _wmnReceiveRet = mbmUdp.WriteMultipleRegisters(0, 12388, writeData);
            txtData.Text = _wmnReceiveRet.Text;
        }

        private void btnWriteReal_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;             
            ushort[] writeData = wmnConvert.ToUInt16(Convert.ToSingle(txtRealValue.Text));
            //float[] values = { 1.42f, 9.81f, 3.14f }; 
            //ushort[] writeData = wmnConvert.ToUInt16(values);
            wmnRet _wmnReceiveRet = mbmUdp.WriteMultipleRegisters(0, 12348, writeData);
            txtData.Text = _wmnReceiveRet.Text;
        }

        private void btnWriteDint_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            //ushort[] writeData = wmnConvert.ToUInt16(Convert.ToInt32(txtDintValue.Text));
            int[] values = { -14202, -98150, 31470 };
            ushort[] writeData = wmnConvert.ToUInt16(values);
            wmnRet _wmnReceiveRet = mbmUdp.WriteMultipleRegisters(0, 12308, writeData);
            txtData.Text = _wmnReceiveRet.Text;
        }

        private void btnWriteUdint_Click(object sender, EventArgs e)
        {
            txtData.Text = "";
            mbmUdp.Hostname = txtHost.Text;
            //ushort[] writeData = wmnConvert.ToUInt16(Convert.ToUInt32(txtUdintValue.Text));
            uint[] values = { 14202, 98150, 31470 };
            ushort[] writeData = wmnConvert.ToUInt16(values);
            wmnRet _wmnReceiveRet = mbmUdp.WriteMultipleRegisters(0, 12328, writeData);
            txtData.Text = _wmnReceiveRet.Text;
        }

 
    }
}