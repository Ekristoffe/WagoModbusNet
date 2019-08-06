using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WagoModbusNet;

namespace WMN_WinFormExample01
{
    public partial class frmMain : Form
    {
        //private ModbusMasterUdp mbm;
        private ModbusMasterTcp mbm;
        
        public frmMain()
        {
            InitializeComponent();
            //mbm = new ModbusMasterUdp();
            mbm = new ModbusMasterTcp();
        }
 
        private void btnRead_Click(object sender, EventArgs e)
        {
            mbm.Hostname = txtHost.Text;
            mbm.Port = Convert.ToUInt16(txtPort.Text);
            txtData.Text = "";
            ReadHoldingRegister();
            //ReadInputRegister();
            //ReadDiscreteInputs();
            //ReadCoils();
            //ReadWriteMultipleRegisters();
            //GetCommEventCounter();
            //ReadBlock();
        }

        private void GetCommEventCounter()
        {
            ushort status;
            ushort eventCount;
            wmnRet ret = mbm.GetCommEventCounter( Convert.ToByte(txtUnitId.Text),
                                                    out status,
                                                    out eventCount);
            if (ret.Value == 0)
            {
                txtData.Text += "Status := " + status.ToString() + "; EventCount := " + eventCount.ToString() + "; \r\n";
            }
            else
            {
                txtData.Text = ret.Text;
            }            
        }

        private void ReadWriteMultipleRegisters()
        {
            ushort[] readData;
            ushort[] writeData = new ushort[2];
            writeData[0] = Convert.ToUInt16(txtWriteValue.Text);
            writeData[1] = 2222;
            wmnRet ret = mbm.ReadWriteMultipleRegisters(Convert.ToByte(txtUnitId.Text),
                                                            Convert.ToUInt16(txtReadAddress.Text),
                                                            Convert.ToUInt16(txtReadCount.Text),
                                                            Convert.ToUInt16(txtWriteAddress.Text),
                                                            writeData,
                                                            out readData);
            if (ret.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (Convert.ToUInt16(txtReadAddress.Text) + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = ret.Text;
            }
        }



        private void ReadDiscreteInputs()
        {
            bool[] readData;           
            wmnRet ret = mbm.ReadDiscreteInputs( Convert.ToByte(txtUnitId.Text),
                                                    Convert.ToUInt16(txtReadAddress.Text),
                                                    Convert.ToUInt16(txtReadCount.Text),
                                                    out readData);
            if (ret.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (Convert.ToUInt16(txtReadAddress.Text) + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = ret.Text;
            }
        }

        private void ReadCoils()
        {
            bool[] readData;
            wmnRet ret = mbm.ReadCoils(  Convert.ToByte(txtUnitId.Text),
                                            Convert.ToUInt16(txtReadAddress.Text),
                                            Convert.ToUInt16(txtReadCount.Text),
                                            out readData);
            if (ret.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (Convert.ToUInt16(txtReadAddress.Text) + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = ret.Text;
            }
        }


        private void ReadInputRegister()
        {
            ushort[] readData;           
            wmnRet ret = mbm.ReadInputRegisters(Convert.ToByte(txtUnitId.Text),
                                                    Convert.ToUInt16(txtReadAddress.Text),
                                                    Convert.ToUInt16(txtReadCount.Text),
                                                    out readData);
            if (ret.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (Convert.ToUInt16(txtReadAddress.Text) + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = ret.Text;
            }
        }


        private void ReadHoldingRegister()
        {
            ushort[] readData;
            wmnRet ret = mbm.ReadHoldingRegisters(Convert.ToByte(txtUnitId.Text),
                                                    Convert.ToUInt16(txtReadAddress.Text),
                                                    Convert.ToUInt16(txtReadCount.Text),
                                                    out readData);
            if (ret.Value == 0)
            {
                for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (Convert.ToUInt16(txtReadAddress.Text) + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }
            }
            else
            {
                txtData.Text = ret.Text;
            }
        }

        private void ReadBlock()
        {
            ushort[] readData;
            wmnRet ret = mbm.ReadBlock(Convert.ToByte(txtUnitId.Text),
                                                    Convert.ToUInt16(txtReadAddress.Text),
                                                    Convert.ToUInt16(txtReadCount.Text),
                                                    out readData);
            if (ret.Value == 0)
            {
                /*for (int i = 0; i < readData.Length; i++)
                {
                    txtData.Text += "Address[" + (Convert.ToUInt16(txtReadAddress.Text) + i).ToString() + "] Value: " + readData[i].ToString() + "; \r\n";
                }*/
                txtData.Text = "Successful executed";
            }
            else
            {
                txtData.Text = ret.Text;
            }
        }


        private void btnWriteRegister_Click(object sender, EventArgs e)
        {
            mbm.Hostname = txtHost.Text;
            mbm.Port = Convert.ToUInt16(txtPort.Text);
            WriteMultipleRegisters();
            //WriteSingleRegister();
            //WriteSingleCoil();
            //WriteMultipleCoils();
            //MaskWriteRegister();
        }

        private void MaskWriteRegister()
        {
            ushort andMask = Convert.ToUInt16(txtWriteValue.Text);
            //ushort andMask = 0x0000;
            ushort orMask = 0x0000;
            wmnRet ret = mbm.MaskWriteRegister(Convert.ToByte(txtUnitId.Text),
                                                  Convert.ToUInt16(txtWriteAddress.Text),
                                                  andMask, 
                                                  orMask);
            txtData.Text = ret.Text;
        }

        private void WriteMultipleCoils()
        {
            bool[] writeData = new bool[36];
            writeData[ 0] = true;
            writeData[ 1] = false;
            writeData[ 2] = false;
            writeData[ 3] = false; 
            writeData[ 4] = false;
            writeData[ 5] = false;
            writeData[ 6] = false;
            writeData[ 7] = false;

            writeData[8] = true;
            writeData[9] = true;
            writeData[10] = true;
            writeData[11] = true; 
            writeData[12] = true;
            writeData[13] = true;
            writeData[14] = true;
            writeData[15] = false;

            writeData[16] = true;
            writeData[17] = true;
            writeData[18] = true;
            writeData[19] = true; 
            writeData[20] = false;
            writeData[21] = false;
            writeData[22] = false;
            writeData[23] = false;

            writeData[24] = true;
            writeData[25] = true;
            writeData[26] = false;
            writeData[27] = false; 
            writeData[28] = true;
            writeData[29] = true;
            writeData[30] = false;
            writeData[31] = false;

            writeData[32] = false;
            writeData[33] = true;
            writeData[34] = false;
            writeData[35] = true; 
            wmnRet ret = mbm.WriteMultipleCoils(Convert.ToByte(txtUnitId.Text),
                                                   Convert.ToUInt16(txtWriteAddress.Text),
                                                   writeData);
            txtData.Text = ret.Text;
        }

        private void WriteMultipleRegisters()
        {
            ushort[] writeData = new ushort[1];
            writeData[0] = Convert.ToUInt16(txtWriteValue.Text);
            wmnRet ret = mbm.WriteMultipleRegisters(Convert.ToByte(txtUnitId.Text),
                                                        Convert.ToUInt16(txtWriteAddress.Text),
                                                        writeData);
            txtData.Text = ret.Text;
        }

        private void WriteSingleRegister()
        {
            ushort writeData = Convert.ToUInt16(txtWriteValue.Text);
            wmnRet ret = mbm.WriteSingleRegister(Convert.ToByte(txtUnitId.Text),
                                                    Convert.ToUInt16(txtWriteAddress.Text),
                                                    writeData);
            txtData.Text = ret.Text;
        }

        private void WriteSingleCoil()
        {
            bool writeData = Convert.ToBoolean(txtWriteValue.Text);
            wmnRet ret = mbm.WriteSingleCoil(Convert.ToByte(txtUnitId.Text),
                                                Convert.ToUInt16(txtWriteAddress.Text),
                                                writeData);
            txtData.Text = ret.Text;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            wmnRet ret = mbm.Connect(txtHost.Text, Convert.ToUInt16(txtPort.Text));
            if (ret.Value != 0)
            {
                txtData.Text = ret.Text;
            }
            else
                txtData.Text = "Connected";
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            mbm.Disconnect();
            txtData.Text = "Disconnected";
        }

        private void txtHost_TextChanged(object sender, EventArgs e)
        {

        }

   
    }
}