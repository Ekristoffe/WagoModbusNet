/*
Description:    
    WagoModbusNet provide easy to use Modbus-Master classes for TCP, UDP, RTU and ASCII.
    WagoModbusNet based on dot.net framework 2.0.
    WagoModbusNet.Masters do not throw any exception, all function returns a struct of type 'wmnRet'.
    For a list of supported function codes see 'enum ModbusFunctionCodes'.    
  
Version: 1.0.1.0 (09.01.2013)
   
Author: WAGO Kontakttechnik GmbH & Co.KG
  
Contact: support@wago.com
 
Typical pitfal:
    You dial with a WAGO ethernet controller. Try to set outputs - but nothing happens!
    WAGO ethernet controller provide a "owner" policy for physical outputs.
    The "owner" could be CoDeSys-Runtime or Fieldbus-Master.
    Every time you download a PLC program the CoDeSys-Runtime becomes "owner" of physical outputs.
    Use tool "Ethernet-Settings.exe" and "format" and "extract" filesystem is easiest way to assign Modbus-Master as "owner".
    Alternativly you can "Login" with CoDeSys-IDE and perform "Reset(original)".
     
License:
    Copyright (c) WAGO Kontakttechnik GmbH & Co.KG 2013 

    Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
    and associated documentation files (the "Software"), to deal in the Software without restriction, 
    including without limitation the rights to use, copy, modify, merge, publish, distribute, 
    sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial 
    portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
    NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO.Ports;

namespace WagoModbusNet
{
    #region Enums

    public enum ModbusFunctionCodes : byte
    {
        fc1_ReadCoils = 1,
        fc2_ReadDiscreteInputs = 2,
        fc3_ReadHoldingRegisters = 3,
        fc4_ReadInputRegisters = 4,
        fc5_WriteSingleCoil = 5,
        fc6_WriteSingleRegister = 6,
        fc11_GetCommEventCounter = 11,
        fc15_WriteMultipleCoils = 15,
        fc16_WriteMultipleRegisters = 16,
        fc22_MaskWriteRegister = 22,
        fc23_ReadWriteMultipleRegisters = 23
    };

    public enum ModbusExceptionCodes : byte
    {
        // Modbus specified exception codes 
        ec1_ILLEGAL_FUNCTION = 1,
        ec2_ILLEGAL_DATA_ADDRESS = 2,
        ec3_ILLEGAL_DATA_VALUE = 3,
        ec4_SLAVE_DEVICE_FAILURE = 4,
        ec5_ACKNOWLEDGE = 5,
        ec6_SLAVE_DEVICE_BUSY = 6,
        ec8_MEMORY_PARITY_ERROR = 8,
        ec10_GATEWAY_PATH_UNAVAILABLE = 10,
        ec11_GATEWAY_TARGET_DEVICE_FAILED_TO_RESPOND = 11,
    };

    // WAGO specified error offset to group errors 
    //
    // 100: "Receive-Error: Timeout expired while 'Waiting for slave response ...'"
    // 101: "Connection-Error: Timeout expired while 'Try to connect ...'"
    public enum wmnErrorOffset : int
    {
        /* >0 ==> remote slave respond with a Modbus-Exception */
        wmnSUCCESS = 0,            // Indicate a succesful execution 
        wmnTIMEOUT_ERROR = -100,   // Timeout expired (OnConnect or OnReceive)
        wmnPARAMETER_ERROR = -200, // Unexpected usage of this class
        wmnDOTNET_EXCEPTION = -300,// A dot net exception was catched, see 'Text' for Details
        wmnOTHER_ERROR = -500      // All other, see 'Text' for Details  
    }

    ///  <summary>
    /// Provide a numeric return value and a textual description of execution state.
    ///     Value == 0: indicates success,
    ///     Value &gt; 0 : Modbus-Exception-Code, received from remote slave,
    ///     Value &lt; 0 : Internal Error, see Text for details.
    /// </summary> 
    public struct wmnRet
    {
        public int Value;      //Numeric retval, ==0 success, >0 ModbusException, <0 Internal error 
        public string Text;    //Textual description about whats going wrong

        public wmnRet( int value, string text)
        {
            Value = value;
            Text = text;
        }
    }

    #endregion

    #region  ModbusMaster
    public abstract class ModbusMaster
    {
        protected int _timeout = 500; 
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// FC1 - Read Coils
        /// WAGO coupler and controller do not differ between FC1 and FC2
        /// Digital outputs utilze a offset of 256. First coil start at address 256.
        /// Address 0 and follows returning status of digital inputs modules 
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="readCount"></param>
        /// <param name="data"></param>
        /// <returns>wmnRet</returns>
        public wmnRet ReadCoils(byte id, ushort startAddress, ushort readCount, out bool[] data)          
        {
            data = null;
            byte[] responsePdu; //Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet ret = SendModbusRequest(id, ModbusFunctionCodes.fc1_ReadCoils, startAddress, readCount, 0, 0, null, out responsePdu);
            if (ret.Value == 0)
            {
                //Strip PDU header and convert data into bool[]
                data = new bool[readCount];
                for (int i = 0, k = 0; i < readCount; i++)
                {
                    data[i] = ((responsePdu[k + 3] & (byte)(0x01 << (i % 8))) > 0) ? true : false;
                    k = (i + 1) / 8;
                }               
            }
            return ret;
        }

        /// <summary>
        /// FC2 - Read Discrete Inputs
        /// WAGO coupler and controller do not differ between FC1 and FC2
        /// Address 0 and follows returning status of digital inputs modules
        /// Digital outputs utilze a offset of 256. First coil start at address 256.         
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="readCount"></param>
        /// <param name="data"></param>
        /// <returns>wmnRet</returns>
        public wmnRet ReadDiscreteInputs(byte id, ushort startAddress, ushort readCount, out bool[] data)
        {
            data = null;
            byte[] responsePdu; //Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet ret = SendModbusRequest(id, ModbusFunctionCodes.fc2_ReadDiscreteInputs, startAddress, readCount, 0, 0, null, out responsePdu);
            if (ret.Value == 0)
            {
                //Strip PDU header and convert data into bool[]
                data = new bool[readCount];
                for (int i = 0, k = 0; i < readCount; i++)
                {
                    data[i] = ((responsePdu[k + 3] & (byte)(0x01 << (i % 8))) > 0) ? true : false;
                    k = (i + 1) / 8;
                }              
            }
            return ret;
        }
        
        /// <summary>
        /// FC3 - Read Holding Registers
        /// WAGO coupler and controller do not differ between FC3 and FC4
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="readCount"></param>
        /// <param name="data"></param>
        /// <returns>wmnRet</returns>
        public wmnRet ReadHoldingRegisters(byte id, ushort startAddress, ushort readCount, out ushort[] data)
        {
            data = null;
            byte[] responsePdu; //Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet ret = SendModbusRequest(id, ModbusFunctionCodes.fc3_ReadHoldingRegisters, startAddress, readCount, 0, 0, null, out responsePdu);
            if (ret.Value == 0)
            {
                //Strip PDU header and convert data into ushort[]
                byte[] tmp = new byte[2];
                int count = (responsePdu[2] / 2);
                data = new ushort[count];
                for (int i = 0; i < count; i++)
                {
                    tmp[0] = responsePdu[4 + (2 * i)];
                    tmp[1] = responsePdu[3 + (2 * i)];
                    data[i] = BitConverter.ToUInt16(tmp, 0);
                }              
            }
            return ret;
        }
        
        /// <summary>
        /// FC4 - Read Input Registers
        /// WAGO coupler and controller do not differ between FC3 and FC4
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress">     </param>
        /// <param name="readCount">      </param>
        /// <param name="data">out ushort[]</param>
        /// <returns>wmnRet</returns>
        public wmnRet ReadInputRegisters(byte id, ushort startAddress, ushort readCount, out ushort[] data)
        {
            data = null;
            byte[] responsePdu; //Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet ret = SendModbusRequest(id, ModbusFunctionCodes.fc4_ReadInputRegisters, startAddress, readCount, 0, 0, null, out responsePdu); 
            if (ret.Value == 0)
            {
                  //Strip PDU header and convert data into ushort[]
                byte[] tmp = new byte[2];
                int count = (responsePdu[2]/2);
                data = new ushort[count];
                for (int i = 0; i < count; i++)
                {
                    tmp[0] = responsePdu[4 + (2*i)];
                    tmp[1] = responsePdu[3 + (2*i)];                    
                    data[i] = BitConverter.ToUInt16(tmp, 0);
                }              
            }
            return ret;
        }

         /// <summary>
        /// FC5 - Write Single Coil
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="data"></param>
        /// <returns>wmnRet</returns>
        public wmnRet WriteSingleCoil(byte id, ushort startAddress, bool data)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeData = new byte[1];
            writeData[0] = (data) ? (byte)0xFF : (byte)0x00;
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.fc5_WriteSingleCoil, 0, 0, startAddress, 1, writeData, out responsePdu);
        }
        
        /// <summary>
        /// FC6 - Write Single Register
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="data"></param>
        /// <returns>wmnRet</returns>
        public wmnRet WriteSingleRegister(byte id, ushort startAddress, ushort data)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeData = BitConverter.GetBytes(data);
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.fc6_WriteSingleRegister, 0, 0, startAddress, 1, writeData, out responsePdu);
        }

        /// <summary>
        /// FC11 - Get Comm Event Counter
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="status"> return 0 for ready to process next requst or 0xFFFF when device busy</param>
        /// <param name="eventCount">number of successful processed Modbus-Requests</param>
        /// <returns>wmnRet</returns>
        public wmnRet GetCommEventCounter(byte id, out ushort status, out ushort eventCount)
        {
            status = 0;
            eventCount = 0;
            byte[] responsePdu; //Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet ret = SendModbusRequest(id, ModbusFunctionCodes.fc11_GetCommEventCounter, 0, 0, 0, 0, null, out responsePdu);
            if (ret.Value == 0)
            {
                //Strip PDU header and convert data into ushort[]
                byte[] tmp = new byte[2];
                //Extract status
                tmp[0] = responsePdu[3];
                tmp[1] = responsePdu[2];
                status = BitConverter.ToUInt16(tmp, 0);
                //Extract eventCount
                tmp[0] = responsePdu[5];
                tmp[1] = responsePdu[4];
                eventCount = BitConverter.ToUInt16(tmp, 0);
            }
            return ret;
        }

        /// <summary>
        /// FC15 - Write Multiple Coils
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="data"></param>
        /// <returns>wmnRet</returns>
        public wmnRet WriteMultipleCoils(byte id, ushort startAddress, bool[] data)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeData = ((data.Length % 8) == 0) ? new byte[data.Length/8] : new byte[(data.Length/8)+1];
            for (int i = 0, k = 0; i < data.Length; i++)
            {
                if ((i > 0) && ((i % 8) == 0)) k++;
                if (data[i]) writeData[k] = (byte)(writeData[k] | (byte)(0x01 << (i % 8)));                
            }
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.fc15_WriteMultipleCoils, 0, 0, startAddress, (ushort)data.Length, writeData, out responsePdu);
        }
    
        /// <summary>
        /// FC16 - Write Multiple Registers
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="writeCount"></param>
        /// <param name="data"></param>
        /// <returns>wmnRet</returns>
        public wmnRet WriteMultipleRegisters(byte id, ushort startAddress, ushort[] data)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeData = new byte[data.Length * 2];
            byte[] tmp; 
            for (int i = 0, k=0; i < data.Length; i++)
            { 
                tmp = BitConverter.GetBytes(data[i]);
                writeData[k] = tmp[1];
                writeData[k + 1] = tmp[0];
                k +=2;
            } 
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.fc16_WriteMultipleRegisters, 0, 0, startAddress, (ushort)data.Length , writeData, out responsePdu); 
        }

        /// <summary>
        /// FC22 - Mask Write Register
        /// Modify single bits in a register
        /// Result = (CurrentContent AND andMask) OR (orMask AND (NOT andMask))
        /// If the orMask value is zero, the result is simply the logical ANDing of the current contents and andMask. 
        /// If the andMask value is zero, the result is equal to the orMask value.
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="startAddress"></param>
        /// <param name="andMask"></param>
        /// <param name="orMask"></param>
        /// <returns>wmnRet</returns>
        public wmnRet MaskWriteRegister(byte id, ushort startAddress, ushort andMask, ushort orMask)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeData = new byte[4];
            byte[] tmp;
            tmp = BitConverter.GetBytes(andMask);
            writeData[0] = tmp[0];
            writeData[1] = tmp[1];
            tmp = BitConverter.GetBytes(orMask);
            writeData[2] = tmp[0];
            writeData[3] = tmp[1];           
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.fc22_MaskWriteRegister, 0, 0, startAddress, 4, writeData, out responsePdu);
        }
        
        /// <summary>
        /// FC23 - Read Write Multiple Registers
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress"></param>
        /// <param name="readCount"></param>
        /// <param name="writeAddress"></param>
        /// <param name="writeData"></param>
        /// <param name="readData"></param>
        /// <returns>wmnRet</returns>
        public wmnRet ReadWriteMultipleRegisters(byte id, ushort readAddress, ushort readCount, ushort writeAddress, ushort[] writeData, out ushort[] readData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeBuffer = new byte[writeData.Length * 2];
            byte[] tmp;
            for (int i = 0, k = 0; i < writeData.Length; i++)
            {
                tmp = BitConverter.GetBytes(writeData[i]);
                writeBuffer[k] = tmp[1];
                writeBuffer[k + 1] = tmp[0];
                k += 2;
            }                         
            readData = null;
            byte[] responsePdu; //Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet ret = SendModbusRequest(id, ModbusFunctionCodes.fc23_ReadWriteMultipleRegisters, readAddress, readCount, writeAddress, (ushort)writeData.Length, writeBuffer, out responsePdu);
            if (ret.Value == 0)
            {
                //Strip PDU header and convert data into ushort[]
                byte[] buf = new byte[2];
                int count = (responsePdu[2] / 2);
                readData = new ushort[count];
                for (int i = 0; i < count; i++)
                {
                    buf[0] = responsePdu[4 + (2 * i)];
                    buf[1] = responsePdu[3 + (2 * i)];
                    readData[i] = BitConverter.ToUInt16(buf, 0);
                } 
            }
            return ret;
        }



        // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
        public wmnRet SendModbusRequest(byte id, ModbusFunctionCodes functionCode, ushort readAddress, ushort readCount, ushort writeAddress, ushort writeCount, byte[] writeData, out byte[] responsePdu)
        {
            responsePdu = null;
            byte[] reqPdu; // Request PDU
            // Build common part of modbus request
            wmnRet ret = BuildRequestPDU(id, functionCode, readAddress, readCount, writeAddress, writeCount, writeData, out reqPdu);
            if (ret.Value != 0)
            {
                return ret;
            }
            byte[] reqAdu; // Request ADU
            // Decorate common part of modbus request with transport layer specific header
            ret = BuildRequestAdu(reqPdu, out reqAdu);
            if (ret.Value != 0)
            {
                return ret;
            }
            // Send modbus request and return response 
            return Query(reqAdu, out responsePdu);
        }


        // Decorate common part of modbus request with transport layer specific header
        protected abstract wmnRet BuildRequestAdu(byte[] reqPdu, out byte[] reqAdu);

        // Send modbus request transport layer specific and return response PDU
        protected abstract wmnRet Query(byte[] reqAdu, out byte[] respPdu);

        // Build common part of modbus request
        private wmnRet BuildRequestPDU(byte id, ModbusFunctionCodes functionCode, ushort readAddress, ushort readCount, ushort writeAddress, ushort writeCount, byte[] writeData, out byte[] reqPdu)
        {            
            byte[] help; // Used to convert ushort into bytes
            reqPdu = null;

            switch (functionCode)
            {
                case ModbusFunctionCodes.fc1_ReadCoils:
                case ModbusFunctionCodes.fc2_ReadDiscreteInputs:
                case ModbusFunctionCodes.fc3_ReadHoldingRegisters:
                case ModbusFunctionCodes.fc4_ReadInputRegisters:
                    reqPdu = new byte[6];
                    // Build request header 
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = (byte)functionCode;         // Modbus-Function-Code
                    help = BitConverter.GetBytes(readAddress);
                    reqPdu[2] = help[1];					// Start read address -Hi
                    reqPdu[3] = help[0];					// Start read address -Lo
                    help = BitConverter.GetBytes(readCount);
                    reqPdu[4] = help[1];				    // Number of coils or register to read -Hi
                    reqPdu[5] = help[0];				    // Number of coils or register to read -Lo  
                    break;

                case ModbusFunctionCodes.fc5_WriteSingleCoil:
                    reqPdu = new byte[6];
                    // Build request header 
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = 0x05;                       // Modbus-Function-Code: fc5_WriteSingleCoil
                    help = BitConverter.GetBytes(writeAddress);
                    reqPdu[2] = help[1];					// Address of coil to force -Hi
                    reqPdu[3] = help[0];					// Address of coil to force -Lo
                    // Copy data
                    reqPdu[4] = writeData[0];				// Output value -Hi  ( 0xFF or 0x00 )
                    reqPdu[5] = 0x00;				        // Output value -Lo  ( const: 0x00  ) 
                    break;

                case ModbusFunctionCodes.fc6_WriteSingleRegister:
                    reqPdu = new byte[6];
                    // Build request header 
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = 0x06;                       // Modbus-Function-Code: fc6_WriteSingleRegister
                    help = BitConverter.GetBytes(writeAddress);
                    reqPdu[2] = help[1];					// Address of register to force -Hi
                    reqPdu[3] = help[0];					// Address of register to force -Lo
                    reqPdu[4] = writeData[1];				// Output value -Hi  
                    reqPdu[5] = writeData[0];				// Output value -Lo  
                    break;

                case ModbusFunctionCodes.fc11_GetCommEventCounter:
                    reqPdu = new byte[2];
                    // Build request header 
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = 0x0B;                       // Modbus-Function-Code: fc11_GetCommEventCounter
                    break;

                case ModbusFunctionCodes.fc15_WriteMultipleCoils:
                    byte byteCount = (byte)(writeCount / 8);
                    if ((writeCount % 8) > 0)
                    {
                        byteCount += 1;
                    }
                    reqPdu = new byte[7 + byteCount];
                    // Build request header
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = 0x0F;                       // Modbus-Function-Code: fc15_WriteMultipleCoils
                    help = BitConverter.GetBytes(writeAddress);
                    reqPdu[2] = help[1];					// Start address of coils to force -Hi
                    reqPdu[3] = help[0];					// Start address of coils to force -Lo
                    help = BitConverter.GetBytes(writeCount);
                    reqPdu[4] = help[1];				    // Number of coils to write -Hi 
                    reqPdu[5] = help[0];				    // Number of coils to write -Lo  
                    reqPdu[6] = byteCount;				    // Number of bytes to write                    
                    // Copy data
                    for (int i = 0; i < byteCount; i++)
                    {
                        reqPdu[7 + i] = writeData[i];
                    }
                    break;

                case ModbusFunctionCodes.fc16_WriteMultipleRegisters:
                    reqPdu = new byte[7 + (writeCount * 2)];
                    // Build request header 
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = 0x10;                       // Modbus-Function-Code: fc16_WriteMultipleRegisters
                    help = BitConverter.GetBytes(writeAddress);
                    reqPdu[2] = help[1];					// Start address of coils to force -Hi
                    reqPdu[3] = help[0];					// Start address of coils to force -Lo
                    help = BitConverter.GetBytes(writeCount);
                    reqPdu[4] = help[1];				    // Number of register to write -Hi 
                    reqPdu[5] = help[0];				    // Number of register to write -Lo  
                    reqPdu[6] = (byte)(writeCount * 2);		// Number of bytes to write                    
                    // Copy data
                    for (int i = 0; i < (writeCount * 2); i++)
                    {
                        reqPdu[7 + i] = writeData[i];
                    }
                    break;


                case ModbusFunctionCodes.fc22_MaskWriteRegister:
                    reqPdu = new byte[8];
                    // Build request header 
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = 0x16;                       // Modbus-Function-Code: fc22_MaskWriteRegister
                    help = BitConverter.GetBytes(writeAddress);
                    reqPdu[2] = help[1];					// Address of register to force -Hi
                    reqPdu[3] = help[0];					// Address of register to force -Lo
                    reqPdu[4] = writeData[1];				// And_Mask -Hi  
                    reqPdu[5] = writeData[0];				// And_Mask -Lo  
                    reqPdu[6] = writeData[3];				// Or_Mask -Hi  
                    reqPdu[7] = writeData[2];				// Or_Mask -Lo  
                    break;

                case ModbusFunctionCodes.fc23_ReadWriteMultipleRegisters:
                    reqPdu = new byte[11 + (writeCount * 2)];
                    // Build request header 
                    reqPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    reqPdu[1] = 0x17;                       // Modbus-Function-Code: fc23_ReadWriteMultipleRegisters
                    help = BitConverter.GetBytes(readAddress);
                    reqPdu[2] = help[1];					// Start read address -Hi
                    reqPdu[3] = help[0];					// Start read address -Lo
                    help = BitConverter.GetBytes(readCount);
                    reqPdu[4] = help[1];				    // Number of register to read -Hi
                    reqPdu[5] = help[0];				    // Number of register to read -Lo           
                    help = BitConverter.GetBytes(writeAddress);
                    reqPdu[6] = help[1];				    // Start write address -Hi
                    reqPdu[7] = help[0];				    // Start write address -Lo
                    help = BitConverter.GetBytes(writeCount);
                    reqPdu[8] = help[1];				    // Number of register to write -Hi
                    reqPdu[9] = help[0];				    // Number of register to write -Lo
                    reqPdu[10] = (byte)(writeCount * 2);     // Number of bytes to write
                    // Copy data
                    for (int i = 0; i < (writeCount * 2); i++)
                    {
                        reqPdu[11 + i] = writeData[i];
                    }
                    break;
            }
            return new wmnRet(0, "Successful executed");
        }
    }
    #endregion

    #region  ModbusMasterUdp

    public class ModbusMasterUdp : ModbusMaster 
    {
        private static ushort _transactionId = 4711;
        private ushort TransactionId
        {
            get { _transactionId += 1; return _transactionId; }
        }
        
        protected string _hostname = "";
        public string Hostname
        {
            get { return _hostname; }
            set 
            { 
                _hostname = value;
                if (IPAddress.TryParse(value, out _ip) == false)
                {
                    /*//Sync name resolving would block up to 5 seconds
                     * IPHostEntry hst = Dns.GetHostEntry(value);
                     *_ip = hst.AddressList[0];
                     */
                    //Async name resolving will not block but needs also up to 5 seconds until it returns 
                    IAsyncResult ar = Dns.BeginGetHostEntry(value, null, null);
                    ar.AsyncWaitHandle.WaitOne(); // Wait until job is done - No chance to cancel request
                    IPHostEntry iphe = null;
                    try
                    {
                        iphe = Dns.EndGetHostEntry(ar); //EndGetHostEntry will wait for you if calling before job is done 
                    }
                    catch { }
                    if (iphe != null)
                    {
                        _ip = iphe.AddressList[0];
                    }
                }            
            }           
        }

        protected int _port = 502;
        public int Port
        {
            get { return _port; }
            set { _port = value; }           
        }

        protected bool _autoConnect;
        public bool AutoConnect
        {
            get { return _autoConnect; }
            set { _autoConnect = value; }
        }

        public ModbusMasterUdp()
        {
        }

        public ModbusMasterUdp(string hostname): this()
        {
            this.Hostname = hostname;
        }

        public ModbusMasterUdp(string hostname, int port): this()
        {
            this.Hostname = hostname;
            this.Port = port;            
        }

        protected bool _connected;
        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (value)
                {
                    _connected = (Connect().Value == 0) ? true : false;
                }
                else
                {
                    Disconnect();
                }
            }
        }


        public virtual wmnRet Connect() 
        {
            //Create socket
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
            _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
            return new wmnRet(0, "Successful executed"); 
        }

        public virtual wmnRet Connect(string hostname)
        {
            this.Hostname = hostname;
            return Connect();
        }

        public virtual wmnRet Connect(string hostname, int port)
        {
            this.Hostname = hostname;
            _port = port;
            return Connect();
        }

        public virtual void Disconnect()
        {
            //Close socket
            if (_sock != null)
            {
                _sock.Close();
                _sock = null;
            }
           
        }
        protected Socket _sock;
        protected IPAddress _ip = null;

        // Send request and and wait for response
        protected override wmnRet Query(byte[] reqAdu, out byte[] respPdu)
        {
            respPdu = null;
            if (_ip == null)
            {
                return new wmnRet(-301 ,"DNS error: Could not resolve Ip-Address for " + _hostname );
            }
            if (!_connected)
            {
                Connect(); // Connect will succesful in any case because it just create a socket instance
            }
            try
            {

                // Send Request( synchron )             
                IPEndPoint ipepRemote;             
                try
                {
                    ipepRemote = new IPEndPoint(_ip, _port);   
                    _sock.SendTo(reqAdu, ipepRemote);
                }
                catch (Exception e)
                {                    
                    return new wmnRet(-300, "NetException: " + e.Message );
                }
 
                byte[] tmpBuf = new byte[255]; //Receive buffer
                try
                {
                    // Remote EndPoint to capture the identity of responding host.                    
                    EndPoint epRemote = (EndPoint)ipepRemote;

                    int byteCount = _sock.ReceiveFrom(tmpBuf, 0, tmpBuf.Length, SocketFlags.None, ref epRemote);

                    return CheckResponse(tmpBuf, byteCount, out respPdu);                    
                }
                catch (Exception e)
                {
                    return new wmnRet(-300, "NetException: " + e.Message);
                }                
            }
            finally
            {
                if (_autoConnect)
                {
                    Disconnect();
                }
            }
        }

        protected virtual wmnRet CheckResponse(byte[] respRaw, int respRawLength, out byte[] respPdu)
        {
            respPdu = null;
            // Check minimal response length of 8 byte
            if (respRawLength < 8)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, do not receive minimal length of 8 byte");
            }
            //Decode act telegram lengh
            ushort respPduLength = (ushort)((ushort)respRaw[5] | (ushort)((ushort)(respRaw[4] << 8)));
            // Check all bytes received 
            if (respRawLength < respPduLength + 6)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, do not receive complied telegram");
            }
            // Is response a "modbus exception response"
            if ((respRaw[7] & 0x80) > 0)
            {
                return new wmnRet((int)respRaw[8], "Modbus exception received: " + ((ModbusExceptionCodes)respRaw[8]).ToString());
            }
            // Strip ADU header and copy response PDU into output buffer 
            respPdu = new byte[respPduLength];
            for (int i = 0; i < respPduLength; i++)
            {
                respPdu[i] = respRaw[6 + i];
            }
            return new wmnRet(0, "Successful executed");
        }

         // Prepare request telegram
        protected override wmnRet BuildRequestAdu(byte[] reqPdu, out byte[] reqAdu)
        {
            reqAdu = new byte[6 + reqPdu.Length]; // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusTCP
            byte[] help; // Used to convert ushort into bytes

            help = BitConverter.GetBytes(this.TransactionId);
            reqAdu[0] = help[1];						// Transaction-ID -Hi
            reqAdu[1] = help[0];						// Transaction-ID -Lo
            reqAdu[2] = 0x00;						    // Protocol-ID - allways zero
            reqAdu[3] = 0x00;						    // Protocol-ID - allways zero
            help = BitConverter.GetBytes(reqPdu.Length);
            reqAdu[4] = help[1];						// Number of bytes follows -Hi 
            reqAdu[5] = help[0];						// Number of bytes follows -Lo 
            // Copy request PDU
            for (int i = 0; i < reqPdu.Length; i++)
            {
                reqAdu[6 + i] = reqPdu[i];
            }
            return new wmnRet(0, "Successful executed");
        }
    }
    #endregion


    #region  ModbusMasterTcp

    public class ModbusMasterTcp : ModbusMasterUdp
    {
       
        public ModbusMasterTcp()
        {
        }

        public ModbusMasterTcp(string hostname): this()
        {
            this.Hostname = hostname;
        }

        public ModbusMasterTcp(string hostname, int port): this()
        {
            this.Hostname = hostname;
            _port = port;            
        }

        public override wmnRet Connect()
        {
            if (_connected) Disconnect(); 
                        
            // Create client socket
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
            _sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
            // Reset timer
            _mreConnectTimeout.Reset();
            try
            {
                // Call async Connect 
                _sock.BeginConnect(new IPEndPoint(_ip, _port), new AsyncCallback(OnConnect), _sock);
                // Stay here until connection established or timeout expires
                if (_mreConnectTimeout.WaitOne(_timeout, false))
                {
                    // Successful connected
                    _connected = true;
                    return new wmnRet(0, "Successful executed");
                }
                else
                {
                    // Timeout expired 
                    _connected = false;
                    _sock.Close(); // Implizit .EndConnect free ressources 
                    _sock = null;
                    return new wmnRet( -101, "TIMEOUT-ERROR: Timeout expired while 'Try to connect ...'"); 
                }
            }
            catch (Exception e)
            {
                return new wmnRet(-300, "NetException: " + e.Message);
            }   
        }

        private ManualResetEvent _mreConnectTimeout = new ManualResetEvent(false);

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                Socket s = ar.AsyncState as Socket;
                if (s != null)
                {
                    s.EndConnect(ar);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _mreConnectTimeout.Set();  //Wake up waiting threat to go further
            }
        }

        public override wmnRet Connect(string hostname)
        {
            this.Hostname = hostname;
            return Connect();
        }

        public override wmnRet Connect(string hostname, int port)
        {
            this.Hostname = hostname;
            _port = port;
            return Connect();
        }

        public override void Disconnect()
        {
            //Close socket and free ressources 
            if (_sock != null)
            {
                _sock.Close();
                _sock = null;
            }
            _connected = false;
        }

        // Send request and and wait for response
        protected override wmnRet Query(byte[] reqAdu, out byte[] respPdu)
        {
            respPdu = null;  //Assign null to make compiler silent
            if (_ip == null)
            {
                return new wmnRet(-301, "DNS error: Could not resolve Ip-Address for " + _hostname );
            }            
            try
            {
                if (!_connected && _autoConnect)
                {
                    Connect();               
                }
                if (!_connected)
                {                   
                    return new wmnRet(-500, "Error: 'Not connected, call Connect()' ");
                }
                // Send request sync
                _sock.Send(reqAdu, 0, reqAdu.Length, SocketFlags.None);

                byte[] tmpBuf = new byte[255]; //Receive buffer

                // Try to receive response 
                int byteCount = _sock.Receive(tmpBuf, 0, tmpBuf.Length, SocketFlags.None);

                return CheckResponse(tmpBuf, byteCount, out respPdu);  
            }
            catch (Exception e)
            {
                return new wmnRet(-300, "NetException: " + e.Message);
            }
            finally
            {
                if (_autoConnect)
                {
                    Disconnect();
                }
            }
        }
    }
    #endregion

        
    #region  ModbusMasterRtu

    public class ModbusMasterRtu : ModbusMaster
    {

        public ModbusMasterRtu()
        {
        }

        private SerialPort _sp;             // The serial interface instance
        private string _portName = "COM1";  // Name of serial interface like "COM23" 
        public string Portname
        {
            get { return _portName; }
            set { _portName = value; }
        }

        private int _baudrate = 9600;
        public int Baudrate
        {
            get { return _baudrate; }
            set { _baudrate = value; }
        }
        private int _databits = 8;
        public int Databits
        {
            get { return _databits; }
            set { _databits = value; }
        }
        private Parity _parity = Parity.None;
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }
        private StopBits _stopbits = StopBits.One;
        public StopBits StopBits
        {
            get { return _stopbits; }
            set { _stopbits = value; }
        }
        private Handshake _handshake = Handshake.None;
        public Handshake Handshake
        {
            get { return _handshake; }
            set { _handshake = value; }
        }

        // Receive response helpers        
        private byte[] _respRaw = new byte[512];
        private int _respRawLength; 

        protected bool _connected;
        public bool Connected
        {
            get { return _connected; }
            set
            {
                if (value)
                {
                    _connected = (Connect().Value == 0) ? true : false;
                }
                else
                {
                    Disconnect();
                }
            }
        }

        public virtual wmnRet Connect()
        {
            if (_connected) Disconnect();
            try
            {
                 //Create instance
                _sp = new SerialPort(_portName, _baudrate, _parity, _databits, _stopbits);
                _sp.Handshake = _handshake;
                _sp.WriteTimeout = _timeout;
                _sp.ReadTimeout = _timeout;
            }
            catch(Exception e)
            {
                // Could not create instance of SerialPort class
                return new wmnRet(-300, "NetException: " + e.Message);
            }
            try
            {
                _sp.Open();
            }
            catch(Exception e)
            {
                // Could not open serial port
                return new wmnRet( -300, "NetException: " + e.Message);
            }
            _connected = true;
            return new wmnRet(0, "Successful executed");            
        }


        public virtual wmnRet Connect(string portname, int baudrate, Parity parity, int databits, StopBits stopbits, Handshake handshake)
        {
            //Copy settings into private members
            _portName = portname;
            _baudrate = baudrate;
            _parity = parity;
            _databits = databits;
            _stopbits = stopbits;
            _handshake = handshake;
            //Create instance
            return this.Connect();
        }

        public virtual void Disconnect()
        {
            if (_sp != null)
            {
                _sp.Close();
                _sp = null;
            }
            _connected = false;
        }

        // Send request and and wait for response 
        protected override wmnRet Query(byte[] reqAdu, out byte[] respPdu)
        {
             respPdu = null;
             if (!_connected)
             {                 
                 return new wmnRet(-500, "Error: 'Not connected' ");
             }                      
             try
             {
                 // Send Request( synchron ) 
                 _sp.Write(reqAdu, 0, reqAdu.Length);
             }
             catch (Exception e)
             {
                 return new wmnRet(-300, "NetException: " + e.Message);
             }             
             _respRaw.Initialize();
             _respRawLength = 0;
             _sp.ReadTimeout = _timeout;
             int tmpTimeout = 50; // 50 ms
             if (_baudrate < 9600)
             {
                tmpTimeout = (int)((10000/_baudrate)+50); 
             }
             wmnRet ret;
             try
             {
                 // Read all data until a timeout exception is arrived
                 do
                 {
                     _respRaw[_respRawLength] = (byte)_sp.ReadByte();
                     _respRawLength++;
                     // Change receive timeout after first received byte
                     if (_sp.ReadTimeout != tmpTimeout)
                     {
                         _sp.ReadTimeout = tmpTimeout;
                     }
                 }
                 while (true);
             }
             catch(TimeoutException)
             {
                 ; // Thats what we are waiting for to know "All data received" 
             }
             catch (Exception e)
             {
                 // Something other happens 
                 return new wmnRet(-300, "NetException: " + e.Message); ;
             }
             finally
             {
                 // Check Response
                 if (_respRawLength == 0)
                 {
                     ret =  new wmnRet(-102, "Timeout error: Do not receive response whitin specified 'Timeout' ");
                 }
                 else
                 {
                     ret = CheckResponse(_respRaw, _respRawLength, out respPdu);
                 }
             }
             return ret;
        }

        protected virtual wmnRet CheckResponse(byte[] respRaw, int respRawLength, out byte[] respPdu)
        {
            respPdu = null;
            // Check minimal response length 
            if (respRawLength < 5)
            {                
                return new wmnRet(-500, "Error: Invalid response telegram, do not receive minimal length of 5 byte");
            }
            // Is response a "modbus exception response"
            if ((respRaw[1] & 0x80) > 0)
            {                
                return new wmnRet((int)respRaw[2], "Modbus exception received: " + ((ModbusExceptionCodes)respRaw[2]).ToString());                    
            }
            // Check CRC
            byte[] crc16 = CRC16.CalcCRC16(respRaw, respRawLength - 2);
            if ((respRaw[respRawLength - 2] != crc16[0]) | (respRaw[respRawLength - 1] != crc16[1]))
            {               
                return new wmnRet(-501, "Error: Invalid response telegram, CRC16-check failed");
            }
            // Strip ADU header and copy response PDU into output buffer 
            respPdu = new byte[respRawLength - 2];
            for (int i = 0; i < respRawLength - 2; i++)
            {
                respPdu[i] = respRaw[i];
            }
            return new wmnRet(0, "Successful executed");     
        }

         protected override wmnRet BuildRequestAdu(byte[] reqPdu, out byte[] reqAdu)       
         {
             reqAdu = new byte[reqPdu.Length +2];  // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusRTU
             // Copy request PDU
             for (int i = 0; i < reqPdu.Length; i++)
             {
                 reqAdu[i] = reqPdu[i];
             }
             // Calc CRC16
             byte[] crc16 = CRC16.CalcCRC16(reqAdu, reqAdu.Length-2);
             // Append CRC
             reqAdu[reqAdu.Length - 2] = crc16[0];
             reqAdu[reqAdu.Length - 1] = crc16[1];

             return new wmnRet(0, "Successful executed");
         }
    }
    #endregion


    #region  ModbusMasterAscii

    public class ModbusMasterAscii : ModbusMasterRtu
    {

        public ModbusMasterAscii()
        { 
        
        }        
        
        protected override wmnRet BuildRequestAdu(byte[] reqPdu, out byte[] reqAdu)        
        {
            reqAdu = new byte[((reqPdu.Length + 1) * 2) + 3];  // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusASCII
            // Insert START_OF_FRAME_CHAR's
            reqAdu[0] = 0x3A;                   // START_OF_FRAME_CHAR   
            
            // Convert nibbles to ASCII, insert nibbles into ADU and calculate LRC on the fly
            byte val;
            byte lrc = 0;
            for (int ii = 0, jj = 0; ii < (reqPdu.Length*2); ii++)
            {
                //Example : Byte = 0x5B converted to Char1 = 0x35 ('5') and Char2 = 0x42 ('B') 
                val = ((ii % 2) == 0) ? val = (byte)((reqPdu[jj] >> 4) & 0x0F) : (byte)(reqPdu[jj] & 0x0F);
                reqAdu[1 + ii] = (val <= 0x09) ? (byte)(0x30 + val) : (byte)(0x37 + val);
                if ((ii % 2) != 0)
                {
                    lrc += reqPdu[jj]; 
                    jj++;                    
                }                
            }
            lrc = (byte)(lrc * (-1));
            // Convert LRC upper nibble to ASCII            
            val = (byte)((lrc >> 4) & 0x0F);
            // Insert ASCII coded upper LRC nibble into ADU
            reqAdu[reqAdu.Length - 4] = (val <= 0x09) ? (byte)(0x30 + val) : (byte)(0x37 + val);
            // Convert LRC lower nibble to ASCII   
            val = (byte)(lrc & 0x0F);
            // Insert ASCII coded lower LRC nibble into ADU
            reqAdu[reqAdu.Length - 3] = (val <= 0x09) ? (byte)(0x30 + val) : (byte)(0x37 + val); 
            // Insert END_OF_FRAME_CHAR's
            reqAdu[reqAdu.Length - 2] = 0x0D;   // END_OF_FRAME_CHAR1
            reqAdu[reqAdu.Length - 1] = 0x0A;   // END_OF_FRAME_CHAR2

            return new wmnRet(0, "Successful executed");                  
        }

        protected override wmnRet CheckResponse(byte[] respRaw, int respRawLength, out byte[] respPdu)
        {
            respPdu = null;
            // Check minimal response length 
            if (respRawLength < 13)
            {                
                return new wmnRet(-501, "Error: Invalid response telegram, do not receive minimal length of 13 byte");
            }
            // Check "START_OF_FRAME_CHAR" and "END_OF_FRAME_CHAR's"
            if ((respRaw[0] != 0x3A) | (respRaw[respRawLength - 2] != 0x0D) | (respRaw[respRawLength-1] != 0x0A))
            {
                return new wmnRet(-501, "Error: Invalid response telegram, could not find 'START_OF_FRAME_CHAR' or 'END_OF_FRAME_CHARs'");
            }
            // Convert ASCII telegram to binary
            byte[] buffer = new byte[(respRawLength - 3) / 2];
            byte high, low, val;
            for (int i = 0; i < buffer.Length; i++)
            {                   
                //Example : Char1 = 0x35 ('5') and Char2 = 0x42 ('B') compressed to Byte = 0x5B
                val = respRaw[(2*i)+1];
                high = (val <= 0x39) ? (byte)(val - 0x30) : (byte)(val - 0x37);
                val = respRaw[(2*i)+2];
                low = (val <= 0x39) ? (byte)(val - 0x30) : (byte)(val - 0x37);
                buffer[i] = (byte)((byte)(high << 4) | low);                                            
            }
            // Calculate LRC
            byte lrc = 0;
            for (int i = 0; i < buffer.Length-1; i++)
            {
                lrc += buffer[i];
            }
            lrc = (byte)(lrc *(-1));
            // Check LRC
            if (buffer[buffer.Length-1] != lrc)
            {
                return new wmnRet(-501, "Error: Invalid response telegram, LRC check failed"); 
            }                                
            // Is response a "modbus exception response"
            if ((buffer[1] & 0x80) > 0)
            {
                return new wmnRet((int)respRaw[2], "Modbus exception received: " + ((ModbusExceptionCodes)buffer[2]).ToString());  
            }
            // Strip LRC and copy response PDU into output buffer 
            respPdu = new byte[buffer.Length - 1];
            for (int i = 0; i < respPdu.Length; i++)
            {
                respPdu[i] = buffer[i];
            }
            return new wmnRet(0, "Successful executed");                               
        }
    }
    #endregion


    #region  Utilities

    /// <summary>
    /// Calculate CRC16
    /// </summary>
    internal static class CRC16
    {
        public static ushort CalculateCRC16(byte[] buffer,  int length)
        {
            ushort crc = 0;
            if (buffer != null)
            {
                byte high = 0xFF;
                byte low = 0xFF;
                int idx;
                for (int i = 0; i < length; ++i)
                {
                   idx = high ^ buffer[i];
                   high = (byte)(low ^ CRCHi[idx]);
                   low = CRCLo[idx];
                }
                crc = (ushort)((ushort)((high << 8) | (ushort)low));
            }
            return crc;
        }

        public static byte[] CalcCRC16(byte[] buffer, int length)
        {
            byte[] crc = new byte[2];
            if (buffer != null)
            {
                byte high = 0xFF;
                byte low = 0xFF;
                int idx;
                // Loop over ADU package (without CRC-Fields) 
                for (int i = 0; i < length; ++i)
                {
                    idx = high ^ buffer[i];
                    high = (byte)(low ^ CRCHi[idx]);
                    low = CRCLo[idx];
                }               
                crc[0] = high;
                crc[1] = low;
            }
            return crc;
        }

        private static readonly byte[] CRCHi = 
        { 
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
	        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	        0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
	        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	        0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
	        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	        0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
	        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
	        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	        0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
	        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	        0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	        0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
	        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
	        0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
	        0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
	        0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
	        0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
            };

        private static readonly byte[] CRCLo = 
        { 
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
	        0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
	        0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
	        0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
	        0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
	        0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
	        0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
	        0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
	        0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
	        0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
	        0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
	        0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
	        0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
	        0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
	        0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
	        0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
	        0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
	        0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
	        0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
	        0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
	        0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
	        0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
	        0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
	        0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
	        0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
	        0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        };       
       
     }


    public static class wmnConvert 
    {
        //Convert data from ushort[] into float[]
        public static float[] ToSingle(ushort[] buffer)
        {            
            byte[] tmp = new byte[4];
            float[] outData = new float[buffer.Length / 2];
            for (int i = 0; i < outData.Length; i++)
            {
                tmp[2] = (byte)(buffer[(i * 2) + 1] & 0xFF);
                tmp[3] = (byte)(buffer[(i * 2) + 1] >> 8);
                tmp[0] = (byte)(buffer[i * 2] & 0xFF);
                tmp[1] = (byte)(buffer[i * 2] >> 8);
                outData[i] = BitConverter.ToSingle(tmp, 0);
            }
            return outData;
        }

        //Convert data from ushort[] into Int32[]
        public static int[] ToInt32(ushort[] buffer)
        {
            byte[] tmp = new byte[4];
            int[] outData = new int[buffer.Length / 2];
            for (int i = 0; i < outData.Length; i++)
            {
                tmp[2] = (byte)(buffer[(i * 2) + 1] & 0xFF);
                tmp[3] = (byte)(buffer[(i * 2) + 1] >> 8);
                tmp[0] = (byte)(buffer[i * 2] & 0xFF);
                tmp[1] = (byte)(buffer[i * 2] >> 8);
                outData[i] = BitConverter.ToInt32(tmp, 0);
            }
            return outData;
        }

        //Convert data from ushort[] into UInt32[]
        public static uint[] ToUInt32(ushort[] buffer)
        {
            byte[] tmp = new byte[4];
            uint[] outData = new uint[buffer.Length / 2];
            for (int i = 0; i < outData.Length; i++)
            {
                tmp[2] = (byte)(buffer[(i * 2) + 1] & 0xFF);
                tmp[3] = (byte)(buffer[(i * 2) + 1] >> 8);
                tmp[0] = (byte)(buffer[i * 2] & 0xFF);
                tmp[1] = (byte)(buffer[i * 2] >> 8);
                outData[i] = BitConverter.ToUInt32(tmp, 0);
            }
            return outData;
        }

        //Convert data from ushort[] into string
        public static string ToString(ushort[] buffer)
        {
            byte[] tmp = new byte[buffer.Length * 2];
            int count = 0;
            for (int i = 0, k = 0; i < buffer.Length; i++)
            {
                tmp[k] = (byte)(buffer[i] & 0xFF);
                if (tmp[k] == 0x00) { count = k; break; }
                tmp[k+1] = (byte)(buffer[i] >> 8);
                if (tmp[k + 1] == 0x00) { count = k+1; break; }
                k +=2;                              
            }
            return Encoding.ASCII.GetString(tmp, 0, count);
        }

        //Convert data from string into ushort[]
        public static ushort[] ToUInt16(string txt)
        {
            byte[] tmp = Encoding.ASCII.GetBytes(txt);
            int count = tmp.Length;
            ushort[] outData = new ushort[(count / 2) + 1];
            for (int i = 0; i < tmp.Length; i++)
            {
                outData[i / 2] = (i % 2 == 0) ? (ushort)(tmp[i]) : (ushort)(outData[i / 2] | tmp[i] << 8);
            }             
            return outData;
        }

        //Convert data from float into ushort[]
        public static ushort[] ToUInt16(float value)
        {           
            ushort[] outData = new ushort[2];
            byte[] tmp = BitConverter.GetBytes(value);
            for (int i = 0; i < 4; i++)
            {
                outData[i / 2] = (i % 2 == 0) ? (ushort)(tmp[i]) : (ushort)(outData[i / 2] | tmp[i] << 8);
            }
            return outData;
        }


        //Convert data from float[] into ushort[]
        public static ushort[] ToUInt16(float[] values)
        {
            ushort[] outData = new ushort[values.Length *2];
            int k = 0;
            foreach (float value in values)
            {
                byte[] tmp = BitConverter.GetBytes(value);                              
                outData[k] = (ushort)(tmp[0] | (tmp[1] << 8));
                outData[k + 1] = (ushort)(tmp[2] | (tmp[3] << 8));
                k += 2;                
            }
            return outData;
        }

        //Convert data from Int32 into ushort[]
        public static ushort[] ToUInt16(int value)
        {
            ushort[] outData = new ushort[2];
            byte[] tmp = BitConverter.GetBytes(value);
            for (int i = 0; i < 4; i++)
            {
                outData[i / 2] = (i % 2 == 0) ? (ushort)(tmp[i]) : (ushort)(outData[i / 2] | tmp[i] << 8);
            }
            return outData;
        }

        //Convert data from Int32[] into ushort[]
        public static ushort[] ToUInt16(int[] values)
        {
            ushort[] outData = new ushort[values.Length * 2];
            int k = 0;
            foreach (int value in values)
            {
                byte[] tmp = BitConverter.GetBytes(value);
                outData[k] = (ushort)(tmp[0] | (tmp[1] << 8));
                outData[k + 1] = (ushort)(tmp[2] | (tmp[3] << 8));
                k += 2;
            }
            return outData;
        }

       //Convert data from Int32 into ushort[]
        public static ushort[] ToUInt16(uint value)
        {
            ushort[] outData = new ushort[2];
            byte[] tmp = BitConverter.GetBytes(value);
            for (int i = 0; i < 4; i++)
            {
                outData[i / 2] = (i % 2 == 0) ? (ushort)(tmp[i]) : (ushort)(outData[i / 2] | tmp[i] << 8);
            }
            return outData;
        }

        //Convert data from Int32[] into ushort[]
        public static ushort[] ToUInt16(uint[] values)
        {
            ushort[] outData = new ushort[values.Length * 2];
            int k = 0;
            foreach (uint value in values)
            {
                byte[] tmp = BitConverter.GetBytes(value);
                outData[k] = (ushort)(tmp[0] | (tmp[1] << 8));
                outData[k + 1] = (ushort)(tmp[2] | (tmp[3] << 8));
                k += 2;
            }
            return outData;
        }

    }



     #endregion            

   
}
