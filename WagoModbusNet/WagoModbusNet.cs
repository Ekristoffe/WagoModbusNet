/*
 * Description:    
 *     WagoModbusNet provide easy to use Modbus-Master classes for TCP, UDP, RTU and ASCII.
 *     WagoModbusNet based on dot.net framework 2.0.
 *     WagoModbusNet.Masters do not throw any exception, all function returns a struct of type 'wmnRet'.
 *     For a list of supported function codes see 'enum ModbusFunctionCodes'.    
 * 
 * Version: 1.1.0.4 (01.08.2019)
 * 
 * Author: WAGO Kontakttechnik GmbH & Co.KG
 * 
 * Contact: support@wago.com
 * 
 * Typical pitfal:
 *     You dial with a WAGO ethernet controller. Try to set outputs - but nothing happens!
 *     WAGO ethernet controller provide a "owner" policy for physical outputs.
 *     The "owner" could be CoDeSys-Runtime or Fieldbus-Master.
 *     Every time you download a PLC program the CoDeSys-Runtime becomes "owner" of physical outputs.
 *     Use tool "Ethernet-Settings.exe" and "format" and "extract" filesystem is easiest way to assign Modbus-Master as "owner".
 *     Alternativly you can "Login" with CoDeSys-IDE and perform "Reset(original)".
 * 
 * License:
 *     Copyright (c) WAGO Kontakttechnik GmbH & Co.KG 2013 
 * 
 *     Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
 *     and associated documentation files (the "Software"), to deal in the Software without restriction, 
 *     including without limitation the rights to use, copy, modify, merge, publish, distribute, 
 *     sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
 *     furnished to do so, subject to the following conditions:
 * 
 *     The above copyright notice and this permission notice shall be included in all copies or substantial 
 *     portions of the Software.
 * 
 *     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
 *     NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
 *     IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 *     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 *     SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO.Ports;

namespace WagoModbusNet
{
    #region Enums

    // public enum ModbusFunctionCodes
    public enum ModbusFunctionCodes : byte
    {
        Fc1_ReadCoils = 1,
        Fc2_ReadDiscreteInputs = 2,
        Fc3_ReadHoldingRegisters = 3,
        Fc4_ReadInputRegisters = 4,
        Fc5_WriteSingleCoil = 5,
        Fc6_WriteSingleRegister = 6,
        Fc11_GetCommEventCounter = 11,
        Fc15_WriteMultipleCoils = 15,
        Fc16_WriteMultipleRegisters = 16,
        Fc22_MaskWriteRegister = 22,
        Fc23_ReadWriteMultipleRegisters = 23,
        Fc66_ReadBlock = 66
    };

    // public enum ModbusExceptionCodes
     public enum ModbusExceptionCodes : byte
    {
        // Modbus specified exception codes 
        Ec1_IllegalFunction = 1,
        Ec2_IllegalDataAddress = 2,
        Ec3_IllegalDataValue = 3,
        Ec4_SlaveDeviceFailure = 4,
        Ec5_Acknowledge = 5,
        Ec6_SlaveDeviceBusy = 6,
        Ec7_NegativeAcknowledge = 7,
        Ec8_MemoryParityError = 8,
        Ec10_GatewayPathUnavailable = 10,
        Ec11_GatewayTargetDeviceFailedToRespond = 11,
    };

    // WAGO specified error offset to group errors 
    // 
    // 100: "Receive-Error: Timeout expired while 'Waiting for slave response ...'"
    // 101: "Connection-Error: Timeout expired while 'Try to connect ...'"
    public enum wmnErrorOffset : int
    {
        /* >0 ==> remote slave respond with a Modbus-Exception */
        Success = 0,             // Indicate a succesful execution 
        TimeoutError = -100,     // Timeout expired (OnConnect or OnReceive)
        ParameterError = -200,   // Unexpected usage of this class
        DotNetException = -300,  // A dot net exception was catched, see 'Text' for Details
        OtherError = -500        // All other, see 'Text' for Details  
    }

    /// <summary>
    /// Provide a numeric return value and a textual description of execution state.
    /// Value == 0: indicates success,
    /// Value &gt; 0 : Modbus-Exception-Code, received from remote slave,
    /// Value &lt; 0 : Internal Error, see Text for details.
    /// </summary> 
    public struct wmnRet
    {
        public int Value;      // Numeric retval, ==0 success, >0 ModbusException, <0 Internal error 
        public string Text;    // Textual description about whats going wrong

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
        /* Problem, This DLL support Modbus ASCII:
         * Intervals of up to one second may elapse between characters within the message. 
         * Unless the user has configured a longer timeout, an interval greater than 1 second means an error has occurred. 
         * Some Wide-Area-Network application may require a timeout in the 4 to 5 second range.
        */
        protected int _timeout = 1000; // Milliseconds
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public abstract bool Connected { get; }

        public abstract wmnRet Connect();

        public abstract void Disconnect();

        /// <summary>
        /// FC1 - Read Coils
        /// WAGO coupler and controller do not differ between FC1 and FC2
        /// Digital outputs utilze a offset of 256. First coil start at address 256.
        /// Address 0 and follows returning status of digital inputs modules 
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress">ushort: </param>
        /// <param name="readCount">ushort: </param>
        /// <param name="readData">out bool[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet ReadCoils(byte id, ushort readAddress, ushort readCount, out bool[] readData)          
        {
            readData = null;
            byte[] responsePdu; // Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet wmnReceiveRet = SendModbusRequest(id, ModbusFunctionCodes.Fc1_ReadCoils, readAddress, readCount, 0, 0, null, out responsePdu);
            if (wmnReceiveRet.Value == 0)
            {
                // Strip PDU header and convert data into bool[]
                readData = new bool[readCount];
                for (int index1 = 0, index2 = 0; index1 < readCount; index1++)
                {
                    readData[index1] = ((responsePdu[index2 + 3] & (byte)(0x01 << (index1 % 8))) > 0) ? true : false;
                    index2 = (index1 + 1) / 8;
                }               
            }
            return wmnReceiveRet;
        }

        /// <summary>
        /// FC2 - Read Discrete Inputs
        /// WAGO coupler and controller do not differ between FC1 and FC2
        /// Address 0 and follows returning status of digital inputs modules
        /// Digital outputs utilze a offset of 256. First coil start at address 256.         
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress">ushort: </param>
        /// <param name="readCount">ushort: </param>
        /// <param name="readData">out bool[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet ReadDiscreteInputs(byte id, ushort readAddress, ushort readCount, out bool[] readData)
        {
            readData = null;
            byte[] responsePdu; // Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet wmnReceiveRet = SendModbusRequest(id, ModbusFunctionCodes.Fc2_ReadDiscreteInputs, readAddress, readCount, 0, 0, null, out responsePdu);
            if (wmnReceiveRet.Value == 0)
            {
                // Strip PDU header and convert data into bool[]
                readData = new bool[readCount];
                for (int index1 = 0, index2 = 0; index1 < readCount; index1++)
                {
                    readData[index1] = ((responsePdu[index2 + 3] & (byte)(0x01 << (index1 % 8))) > 0) ? true : false;
                    index2 = (index1 + 1) / 8;
                }
            }
            return wmnReceiveRet;
        }

        /// <summary>
        /// FC3 - Read Holding Registers
        /// WAGO coupler and controller do not differ between FC3 and FC4
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress">ushort: </param>
        /// <param name="readCount">ushort: </param>
        /// <param name="readData">out ushort[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet ReadHoldingRegisters(byte id, ushort readAddress, ushort readCount, out ushort[] readData)
        {
            readData = null;
            byte[] responsePdu; // Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet wmnReceiveRet = SendModbusRequest(id, ModbusFunctionCodes.Fc3_ReadHoldingRegisters, readAddress, readCount, 0, 0, null, out responsePdu);
            if (wmnReceiveRet.Value == 0)
            {
                // Strip PDU header and convert data into ushort[]
                byte[] tmp = new byte[2];
                int count = (responsePdu[2] / 2);
                readData = new ushort[count];
                for (int index = 0; index < count; index++)
                {
                    tmp[0] = responsePdu[4 + (2 * index)];
                    tmp[1] = responsePdu[3 + (2 * index)];
                    readData[index] = BitConverter.ToUInt16(tmp, 0);
                }
            }
            return wmnReceiveRet;
        }

        /// <summary>
        /// FC4 - Read Input Registers
        /// WAGO coupler and controller do not differ between FC3 and FC4
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress">ushort: </param>
        /// <param name="readCount">ushort: </param>
        /// <param name="readData">out ushort[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet ReadInputRegisters(byte id, ushort readAddress, ushort readCount, out ushort[] readData)
        {
            readData = null;
            byte[] responsePdu; // Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet wmnReceiveRet = SendModbusRequest(id, ModbusFunctionCodes.Fc4_ReadInputRegisters, readAddress, readCount, 0, 0, null, out responsePdu);
            if (wmnReceiveRet.Value == 0)
            {
                // Strip PDU header and convert data into ushort[]
                byte[] tmp = new byte[2];
                int count = (responsePdu[2] / 2);
                readData = new ushort[count];
                for (int index = 0; index < count; index++)
                {
                    tmp[0] = responsePdu[4 + (2 * index)];
                    tmp[1] = responsePdu[3 + (2 * index)];
                    readData[index] = BitConverter.ToUInt16(tmp, 0);
                }
            }
            return wmnReceiveRet;
        }

        /// <summary>
        /// FC5 - Write Single Coil
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress">ushort: </param>
        /// <param name="writeData">bool: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet WriteSingleCoil(byte id, ushort writeAddress, bool writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeBuffer = new byte[1];
            writeBuffer[0] = (writeData) ? (byte)0xFF : (byte)0x00;
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.Fc5_WriteSingleCoil, 0, 0, writeAddress, 1, writeBuffer, out responsePdu);
        }

        /// <summary>
        /// FC6 - Write Single Register
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress">ushort: </param>
        /// <param name="writeData">bool: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet WriteSingleRegister(byte id, ushort writeAddress, ushort writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeBuffer = BitConverter.GetBytes(writeData);
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.Fc6_WriteSingleRegister, 0, 0, writeAddress, 1, writeBuffer, out responsePdu);
        }

        /// <summary>
        /// FC11 - Get Comm Event Counter
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="status">out ushort: return 0 for ready to process next requst or 0xFFFF when device busy</param>
        /// <param name="eventCount">out ushort: number of successful processed Modbus-Requests</param>
        /// <returns>wmnRet: </returns>
        public wmnRet GetCommEventCounter(byte id, out ushort status, out ushort eventCount)
        {
            status = 0;
            eventCount = 0;
            byte[] responsePdu; // Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet wmnReceiveRet = SendModbusRequest(id, ModbusFunctionCodes.Fc11_GetCommEventCounter, 0, 0, 0, 0, null, out responsePdu);
            if (wmnReceiveRet.Value == 0)
            {
                // Strip PDU header and convert data into ushort[]
                byte[] tmp = new byte[2];
                // Extract status
                tmp[0] = responsePdu[3];
                tmp[1] = responsePdu[2];
                status = BitConverter.ToUInt16(tmp, 0);
                // Extract eventCount
                tmp[0] = responsePdu[5];
                tmp[1] = responsePdu[4];
                eventCount = BitConverter.ToUInt16(tmp, 0);
            }
            return wmnReceiveRet;
        }

        /// <summary>
        /// FC15 - Write Multiple Coils
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress">ushort: </param>
        /// <param name="writeData">bool[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet WriteMultipleCoils(byte id, ushort writeAddress, bool[] writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeBuffer = ((writeData.Length % 8) == 0) ? new byte[writeData.Length / 8] : new byte[(writeData.Length / 8) + 1];
            for (int index1 = 0, index2 = 0; index1 < writeData.Length; index1++)
            {
                if ((index1 > 0) && ((index1 % 8) == 0)) index2++;
                if (writeData[index1]) writeBuffer[index2] = (byte)(writeBuffer[index2] | (byte)(0x01 << (index1 % 8)));
            }
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.Fc15_WriteMultipleCoils, 0, 0, writeAddress, (ushort)writeBuffer.Length, writeBuffer, out responsePdu);
        }

        /// <summary>
        /// FC16 - Write Multiple Registers
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress">ushort: </param>
        /// <param name="writeData">ushort[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet WriteMultipleRegisters(byte id, ushort writeAddress, ushort[] writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeBuffer = new byte[writeData.Length * 2];
            byte[] tmp;
            for (int index1 = 0, index2 = 0; index1 < writeData.Length; index1++)
            {
                tmp = BitConverter.GetBytes(writeData[index1]);
                writeBuffer[index2] = tmp[1];
                writeBuffer[index2 + 1] = tmp[0];
                index2 += 2;
            }
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.Fc16_WriteMultipleRegisters, 0, 0, writeAddress, (ushort)writeData.Length , writeBuffer, out responsePdu); 
        }

        /// <summary>
        /// FC22 - Mask Write Register
        /// Modify single bits in a register
        /// Result = (CurrentContent AND andMask) OR (orMask AND (NOT andMask))
        /// If the orMask value is zero, the result is simply the logical ANDing of the current contents and andMask. 
        /// If the andMask value is zero, the result is equal to the orMask value.
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress">ushort: </param>
        /// <param name="andMask">ushort: </param>
        /// <param name="orMask">ushort: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet MaskWriteRegister(byte id, ushort writeAddress, ushort andMask, ushort orMask)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeBuffer = new byte[4];
            byte[] tmp;
            tmp = BitConverter.GetBytes(andMask);
            writeBuffer[0] = tmp[0];
            writeBuffer[1] = tmp[1];
            tmp = BitConverter.GetBytes(orMask);
            writeBuffer[2] = tmp[0];
            writeBuffer[3] = tmp[1];
            byte[] responsePdu = null; // Response PDU
            return SendModbusRequest(id, ModbusFunctionCodes.Fc22_MaskWriteRegister, 0, 0, writeAddress, 4, writeBuffer, out responsePdu);
        }

        /// <summary>
        /// FC23 - Read Write Multiple Registers
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress">ushort: </param>
        /// <param name="readCount">ushort: </param>
        /// <param name="writeAddress">ushort: </param>
        /// <param name="writeData">ushort[]: </param>
        /// <param name="readData">out ushort[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet ReadWriteMultipleRegisters(byte id, ushort readAddress, ushort readCount, ushort writeAddress, ushort[] writeData, out ushort[] readData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] writeBuffer = new byte[writeData.Length * 2];
            byte[] tmp;
            for (int index1 = 0, index2 = 0; index1 < writeData.Length; index1++)
            {
                tmp = BitConverter.GetBytes(writeData[index1]);
                writeBuffer[index2] = tmp[1];
                writeBuffer[index2 + 1] = tmp[0];
                index2 += 2;
            }

            readData = null;
            byte[] responsePdu; // Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet wmnReceiveRet = SendModbusRequest(id, ModbusFunctionCodes.Fc23_ReadWriteMultipleRegisters, readAddress, readCount, writeAddress, (ushort)writeData.Length, writeBuffer, out responsePdu);
            if (wmnReceiveRet.Value == 0)
            {
                // Strip PDU header and convert data into ushort[]
                tmp = new byte[2];
                int count = (responsePdu[2] / 2);
                readData = new ushort[count];
                for (int index = 0; index < count; index++)
                {
                    tmp[0] = responsePdu[4 + (2 * index)];
                    tmp[1] = responsePdu[3 + (2 * index)];
                    readData[index] = BitConverter.ToUInt16(tmp, 0);
                }
            }
            return wmnReceiveRet;
        }

        /// <summary>
        /// FC66 - ReadBlock
        /// WAGO specific Modbus extension for reading up to 32000 registers with one request.
        /// Only supported for transport layer TCP and UDP
        /// </summary>
        /// <param name="id">byte: Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress">ushort: </param>
        /// <param name="readCount">ushort: </param>
        /// <param name="readData">out ushort[]: </param>
        /// <returns>wmnRet: </returns>
        public wmnRet ReadBlock(byte id, ushort readAddress, ushort readCount, out ushort[] readData)
        {
            readData = null;
            byte[] responsePdu; // Response PDU
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            wmnRet wmnReceiveRet = SendModbusRequest(id, ModbusFunctionCodes.Fc66_ReadBlock, readAddress, readCount, 0, 0, null, out responsePdu);
            if (wmnReceiveRet.Value == 0)
            {
                // Strip PDU header and convert data into ushort[]
                byte[] tmp = new byte[2];
                int count = (ushort)(responsePdu[3] | (responsePdu[2] << 8));

                readData = new ushort[count];
                for (int index = 0; index < count / 2; index++)
                {
                    tmp[0] = responsePdu[5 + (2 * index)];
                    tmp[1] = responsePdu[4 + (2 * index)];
                    readData[index] = BitConverter.ToUInt16(tmp, 0);
                }
            }
            return wmnReceiveRet;
        }

        // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
        public wmnRet SendModbusRequest(byte id, ModbusFunctionCodes functionCode, ushort readAddress, ushort readCount, ushort writeAddress, ushort writeCount, byte[] writeData, out byte[] responsePdu)
        {
            responsePdu = null;
            byte[] requestPdu; // Request PDU
            // Build common part of modbus request
            wmnRet wmnReceiveRet = BuildRequestPDU(id, functionCode, readAddress, readCount, writeAddress, writeCount, writeData, out requestPdu);
            if (wmnReceiveRet.Value != 0)
            {
                return wmnReceiveRet;
            }
            byte[] requestAdu; // Request ADU
            // Decorate common part of modbus request with transport layer specific header
            wmnReceiveRet = BuildRequestAdu(requestPdu, out requestAdu);
            if (wmnReceiveRet.Value != 0)
            {
                return wmnReceiveRet;
            }
            // Send modbus request and return response 
            return Query(requestAdu, out responsePdu);
        }


        // Decorate common part of modbus request with transport layer specific header
        protected abstract wmnRet BuildRequestAdu(byte[] requestPdu, out byte[] requestAdu);

        // Send modbus request transport layer specific and return response PDU
        protected abstract wmnRet Query(byte[] requestAdu, out byte[] responsePdu);

        // Build common part of modbus request
        private wmnRet BuildRequestPDU(byte id, ModbusFunctionCodes functionCode, ushort readAddress, ushort readCount, ushort writeAddress, ushort writeCount, byte[] writeData, out byte[] requestPdu)
        {            
            byte[] tmp; // Used to convert ushort into bytes
            requestPdu = null;

            switch (functionCode)
            {
                case ModbusFunctionCodes.Fc1_ReadCoils:
                case ModbusFunctionCodes.Fc2_ReadDiscreteInputs:
                case ModbusFunctionCodes.Fc3_ReadHoldingRegisters:
                case ModbusFunctionCodes.Fc4_ReadInputRegisters:
                case ModbusFunctionCodes.Fc66_ReadBlock:
                    requestPdu = new byte[6];
                    // Build request header 
                    requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code
                    tmp = BitConverter.GetBytes(readAddress);
                    requestPdu[2] = tmp[1];					    // Start read address -Hi
                    requestPdu[3] = tmp[0];					    // Start read address -Lo
                    tmp = BitConverter.GetBytes(readCount);
                    requestPdu[4] = tmp[1];				        // Number of coils or register to read -Hi
                    requestPdu[5] = tmp[0];				        // Number of coils or register to read -Lo  
                    break;

                case ModbusFunctionCodes.Fc5_WriteSingleCoil:
                    requestPdu = new byte[6];
                    // Build request header 
                    requestPdu[0] = id;                          // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code: fc5_WriteSingleCoil
                    tmp = BitConverter.GetBytes(writeAddress);
                    requestPdu[2] = tmp[1];					    // Address of coil to force -Hi
                    requestPdu[3] = tmp[0];					    // Address of coil to force -Lo
                    // Copy data
                    requestPdu[4] = writeData[0];			    // Output value -Hi  ( 0xFF or 0x00 )
                    requestPdu[5] = 0x00;				        // Output value -Lo  ( const: 0x00  ) 
                    break;

                case ModbusFunctionCodes.Fc6_WriteSingleRegister:
                    requestPdu = new byte[6];
                    // Build request header 
                    requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code: fc6_WriteSingleRegister
                    tmp = BitConverter.GetBytes(writeAddress);
                    requestPdu[2] = tmp[1];					    // Address of register to force -Hi
                    requestPdu[3] = tmp[0];					    // Address of register to force -Lo
                    requestPdu[4] = writeData[1];				// Output value -Hi  
                    requestPdu[5] = writeData[0];				// Output value -Lo  
                    break;

                case ModbusFunctionCodes.Fc11_GetCommEventCounter:
                    requestPdu = new byte[2];
                    // Build request header 
                    requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code: fc11_GetCommEventCounter
                    break;

                case ModbusFunctionCodes.Fc15_WriteMultipleCoils:
                    byte byteCount = (byte)(writeCount / 8);
                    if ((writeCount % 8) > 0)
                    {
                        byteCount += 1;
                    }
                    requestPdu = new byte[7 + byteCount];
                    // Build request header
                    requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code: fc15_WriteMultipleCoils
                    tmp = BitConverter.GetBytes(writeAddress);
                    requestPdu[2] = tmp[1];					    // Start address of coils to force -Hi
                    requestPdu[3] = tmp[0];					    // Start address of coils to force -Lo
                    tmp = BitConverter.GetBytes(writeCount);
                    requestPdu[4] = tmp[1];				        // Number of coils to write -Hi 
                    requestPdu[5] = tmp[0];				        // Number of coils to write -Lo  
                    requestPdu[6] = byteCount;				    // Number of bytes to write                    
                    // Copy data
                    for (int index = 0; index < byteCount; index++)
                    {
                        requestPdu[7 + index] = writeData[index];
                    }
                    break;

                case ModbusFunctionCodes.Fc16_WriteMultipleRegisters:
                    requestPdu = new byte[7 + (writeCount * 2)];
                    // Build request header 
                    requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code: fc16_WriteMultipleRegisters
                    tmp = BitConverter.GetBytes(writeAddress);
                    requestPdu[2] = tmp[1];				        // Start address of coils to force -Hi
                    requestPdu[3] = tmp[0];					    // Start address of coils to force -Lo
                    tmp = BitConverter.GetBytes(writeCount);
                    requestPdu[4] = tmp[1];				        // Number of register to write -Hi 
                    requestPdu[5] = tmp[0];				        // Number of register to write -Lo  
                    requestPdu[6] = (byte)(writeCount * 2);		// Number of bytes to write                    
                    // Copy data
                    for (int index = 0; index < (writeCount * 2); index++)
                    {
                        requestPdu[7 + index] = writeData[index];
                    }
                    break;


                case ModbusFunctionCodes.Fc22_MaskWriteRegister:
                    requestPdu = new byte[8];
                    // Build request header 
                    requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code: fc22_MaskWriteRegister
                    tmp = BitConverter.GetBytes(writeAddress);
                    requestPdu[2] = tmp[1];					    // Address of register to force -Hi
                    requestPdu[3] = tmp[0];					    // Address of register to force -Lo
                    requestPdu[4] = writeData[1];				// And_Mask -Hi  
                    requestPdu[5] = writeData[0];				// And_Mask -Lo  
                    requestPdu[6] = writeData[3];				// Or_Mask -Hi  
                    requestPdu[7] = writeData[2];				// Or_Mask -Lo  
                    break;

                case ModbusFunctionCodes.Fc23_ReadWriteMultipleRegisters:
                    requestPdu = new byte[11 + (writeCount * 2)];
                    // Build request header 
                    requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code: fc23_ReadWriteMultipleRegisters
                    tmp = BitConverter.GetBytes(readAddress);
                    requestPdu[2] = tmp[1];					    // Start read address -Hi
                    requestPdu[3] = tmp[0];					    // Start read address -Lo
                    tmp = BitConverter.GetBytes(readCount);
                    requestPdu[4] = tmp[1];				        // Number of register to read -Hi
                    requestPdu[5] = tmp[0];				        // Number of register to read -Lo           
                    tmp = BitConverter.GetBytes(writeAddress);
                    requestPdu[6] = tmp[1];				        // Start write address -Hi
                    requestPdu[7] = tmp[0];				        // Start write address -Lo
                    tmp = BitConverter.GetBytes(writeCount);
                    requestPdu[8] = tmp[1];				        // Number of register to write -Hi
                    requestPdu[9] = tmp[0];				        // Number of register to write -Lo
                    requestPdu[10] = (byte)(writeCount * 2);    // Number of bytes to write
                    // Copy data
                    for (int index = 0; index < (writeCount * 2); index++)
                    {
                        requestPdu[11 + index] = writeData[index];
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
        private static ushort _globalTransactionId = 4711;
        protected string _hostname = "";
        protected int _port = 502;
        protected bool _autoConnect;
        protected bool _connected;
        protected Socket _socket;
        protected IPAddress _ip = null;
        protected ushort _requestTransactionId;


        private ushort GlobalTransactionId
        {
            get
            {
                return ++_globalTransactionId;
            }
        }
        
        public string Hostname
        {
            get { return _hostname; }
            set 
            { 
                _hostname = value;
                if (IPAddress.TryParse(value, out _ip) == false)
                {
                    /*// Sync name resolving would block up to 5 seconds
                     * IPHostEntry hst = Dns.GetHostEntry(value);
                     *_ip = hst.AddressList[0];
                     */
                    // Async name resolving will not block but needs also up to 5 seconds until it returns 
                    IAsyncResult ar = Dns.BeginGetHostEntry(value, null, null);
                    ar.AsyncWaitHandle.WaitOne(); // Wait until job is done - No chance to cancel request
                    IPHostEntry iphe = null;
                    try
                    {
                        iphe = Dns.EndGetHostEntry(ar); // EndGetHostEntry will wait for you if calling before job is done 
                    }
                    catch { }
                    if (iphe != null)
                    {
                        _ip = iphe.AddressList[0];
                    }
                }            
            }           
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }           
        }

        public bool AutoConnect
        {
            get { return _autoConnect; }
            set { _autoConnect = value; }
        }

        public ModbusMasterUdp()
        {
        }

        public ModbusMasterUdp(string hostname)
            : this()
        {
            this.Hostname = hostname;
        }

        public ModbusMasterUdp(string hostname, int port)
            : this()
        {
            this.Hostname = hostname;
            this.Port = port;            
        }

        public override bool Connected
        {
            get { return _connected; }
        }


        public override wmnRet Connect() 
        {
            // Create socket
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
            // Successful connected
            _connected = true;
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

        public override void Disconnect()
        {
            // Close socket and free ressources 
            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    _socket.Close();
                }
                ((IDisposable)_socket).Dispose();
                _socket = null;
            }
            _connected = false;
        }

        // Send request and and wait for response
        protected override wmnRet Query(byte[] requestAdu, out byte[] responsePdu)
        {
            responsePdu = null;
            if (requestAdu[7] == (byte)ModbusFunctionCodes.Fc66_ReadBlock) // Check for FC66 - ReadBlock
            {
                int _readcount = ((requestAdu[10] << 8) | requestAdu[11]);
                if (_readcount > 750) // Check ReadCount exceed max UDP-Package size of 1500 byte 
                    return new wmnRet(-310, "UDP error: ReadCount exceed max UDP-Package size of 1500 byte" );
            }
            if ((_ip == null) || (_hostname == ""))
            {
                return new wmnRet(-301 ,"DNS error: Could not resolve Ip-Address for " + _hostname );
            }
            if (!_connected && _autoConnect)
            {
                Connect(); // Connect will succesful in any case because it just create a socket instance
            }
            if (!_connected)
            {
                return new wmnRet(-500, "Error: 'Not connected, call Connect()' ");
            }
            wmnRet wmnReceiveRet = new wmnRet(0, "");
            try
            {

                // Send Request( synchron )
                IPEndPoint ipepRemote;
                try
                {
                    ipepRemote = new IPEndPoint(_ip, _port);

                    _socket.SendTo(requestAdu, ipepRemote);

                    byte[] receiveBuffer = new byte[1500]; // Receive buffer

                    // Remote EndPoint to capture the identity of responding host.
                    EndPoint epRemote = (EndPoint)ipepRemote;

                    int byteCount = _socket.ReceiveFrom(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, ref epRemote);

                    wmnReceiveRet = CheckResponse(requestAdu[6], requestAdu[7], receiveBuffer, byteCount, out responsePdu);
                }
                catch (Exception e)
                {
                    wmnReceiveRet = new wmnRet(-300, "NetException: " + e.Message );
                }
            }
            finally
            {
                if (_autoConnect)
                {
                    Disconnect();
                }
            }
            return wmnReceiveRet;
        }

        protected virtual wmnRet CheckResponse(byte requestUnitID, byte requestFcCode, byte[] responseBuffer, int responseBufferLength, out byte[] responsePdu)
        {
            responsePdu = null;
            // Check minimal response length of 8 byte
            if (responseBufferLength < 8)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, do not receive minimal length of 8 byte");
            }
            // Decode Transaction-ID
            ushort responseTransactionId = (ushort)((ushort)responseBuffer[1] | (ushort)((ushort)(responseBuffer[0] << 8)));
            // Check Transaction-ID
            if (responseTransactionId != _requestTransactionId)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, Transaction ID doesn't match");
            }
            // Decode act telegram lengh
            ushort responsePduLength = (ushort)((ushort)responseBuffer[5] | (ushort)((ushort)(responseBuffer[4] << 8)));
            // Check all bytes received 
            if (responseBufferLength < responsePduLength + 6)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, do not receive complied telegram");
            }
            // Decode UnitID(TCP/UDP)
            byte responseUnitID = responseBuffer[6];
            // Check UnitID(TCP/UDP)
            if (responseUnitID != requestUnitID)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, Unit ID doesn't match");
            }
            // Is response a "modbus exception response"
            if ((responseBuffer[7] & 0x80) > 0)
            {
                return new wmnRet((int)responseBuffer[8], "Modbus exception received: " + ((ModbusExceptionCodes)responseBuffer[8]).ToString());
            }
            // Decode Modbus-Function-Code
            byte responseFcCode = responseBuffer[7];
            // Check Modbus-Function-Code
            if (responseFcCode != requestFcCode)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, Function Code doesn't match");
            }
            // Strip ADU header and copy response PDU into output buffer 
            responsePdu = new byte[responsePduLength];
            for (int index = 0; index < responsePduLength; index++)
            {
                responsePdu[index] = responseBuffer[6 + index];
            }
            return new wmnRet(0, "Successful executed");
        }

         // Prepare request telegram
        protected override wmnRet BuildRequestAdu(byte[] requestPdu, out byte[] requestAdu)
        {
            requestAdu = new byte[6 + requestPdu.Length]; // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusTCP
            byte[] tmp; // Used to convert ushort into bytes
            _requestTransactionId = this.GlobalTransactionId;
            tmp = BitConverter.GetBytes(_requestTransactionId);
            requestAdu[0] = tmp[1];						// Transaction-ID -Hi
            requestAdu[1] = tmp[0];						// Transaction-ID -Lo
            requestAdu[2] = 0x00;						    // Protocol-ID - allways zero
            requestAdu[3] = 0x00;						    // Protocol-ID - allways zero
            tmp = BitConverter.GetBytes(requestPdu.Length);
            requestAdu[4] = tmp[1];						// Number of bytes follows -Hi 
            requestAdu[5] = tmp[0];						// Number of bytes follows -Lo 
            // Copy request PDU
            for (int index = 0; index < requestPdu.Length; index++)
            {
                requestAdu[6 + index] = requestPdu[index];
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

        private ManualResetEvent _mreConnectTimeout = new ManualResetEvent(false);
        public override wmnRet Connect()
        {
            if (_connected)
                Disconnect();

            if ((_ip == null) || (_hostname == ""))
            {
                return new wmnRet(-301, "DNS error: Could not resolve Ip-Address for " + _hostname);
            }

            // Create client socket
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
            // Reset timer
            _mreConnectTimeout.Reset();
            try
            {
                // Call async Connect 
                _socket.BeginConnect(new IPEndPoint(_ip, _port), new AsyncCallback(OnConnect), _socket);
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
                    _socket.Close(); // Implizit .EndConnect free ressources 
                    _socket = null;
                    return new wmnRet(-101, "TIMEOUT-ERROR: Timeout expired while 'Try to connect ...'");
                }
            }
            catch (Exception e)
            {
                return new wmnRet(-300, "NetException: " + e.Message);
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                Socket _socket = ar.AsyncState as Socket;
                if (_socket != null)
                {
                    _socket.EndConnect(ar);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _mreConnectTimeout.Set();  // Wake up waiting threat to go further
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
            // Close socket and free ressources 
            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    _socket.Close();
                }
                ((IDisposable)_socket).Dispose();
                _socket = null;
            }
            _connected = false;
        }

        // Send request and and wait for response
        protected override wmnRet Query(byte[] requestAdu, out byte[] responsePdu)
        {
            responsePdu = null;  // Assign null to make compiler silent
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
                _socket.Send(requestAdu, 0, requestAdu.Length, SocketFlags.None);

                byte[] receiveBuffer = new byte[65000]; // Receive buffer
                int rxCountTotal = 0;
                int responsePduLen = 0; 

                // Try to receive complied response
                do
                {
                    int rxCountActual = _socket.Receive(receiveBuffer, rxCountTotal, (receiveBuffer.Length - rxCountTotal), SocketFlags.None);                    
                    if (rxCountActual == 0)
                        break;
                    rxCountTotal += rxCountActual;
                    // Extract response length information
                    responsePduLen = (int)(receiveBuffer[4] << 8 | receiveBuffer[5]);
                }while (rxCountTotal < responsePduLen + 6);

                return CheckResponse(requestAdu[6], requestAdu[7], receiveBuffer, rxCountTotal, out responsePdu);
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

        private SerialPort _serialPort;     // The serial interface instance
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

        protected bool _connected;
        public override bool Connected
        {
            get { return _connected; }
        }

        public override wmnRet Connect()
        {
            if (_connected) Disconnect();
            try
            {
                 // Create instance
                _serialPort = new SerialPort(_portName, _baudrate, _parity, _databits, _stopbits);
                _serialPort.Handshake = _handshake;
                _serialPort.WriteTimeout = _timeout;
                _serialPort.ReadTimeout = _timeout;
            }
            catch(Exception e)
            {
                // Could not create instance of SerialPort class
                return new wmnRet(-300, "NetException: " + e.Message);
            }
            try
            {
                _serialPort.Open();
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
            // Copy settings into private members
            _portName = portname;
            _baudrate = baudrate;
            _parity = parity;
            _databits = databits;
            _stopbits = stopbits;
            _handshake = handshake;
            // Create instance
            return this.Connect();
        }

        public override void Disconnect()
        {
            // Close port and free ressources
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
               ((IDisposable)_serialPort).Dispose();
                // _serialPort.Dispose();
                _serialPort = null;
            }
            _connected = false;
        }

        // Send request and and wait for response 
        protected override wmnRet Query(byte[] requestAdu, out byte[] responsePdu)
        {
            responsePdu = null;
            if (!_connected)
            {                 
                return new wmnRet(-500, "Error: 'Not connected' ");
            }                      
            try
            {
                // Send Request( synchron ) 
                _serialPort.Write(requestAdu, 0, requestAdu.Length);
            }
            catch (Exception e)
            {
                return new wmnRet(-300, "NetException: " + e.Message);
            }

            byte[] responseBuffer = new byte[512];
            responseBuffer.Initialize();

            int responseBufferLength = 0;
            _serialPort.ReadTimeout = _timeout;
            int tmpTimeout = 50; // Milliseconds
            if (_baudrate < 9600)
            {
                tmpTimeout = (int)((10000/_baudrate)+50); 
            }
            wmnRet wmnReceiveRet;
            try
            {
                // Read all data until a timeout exception is arrived
                do
                {
                    responseBuffer[responseBufferLength] = (byte)_serialPort.ReadByte();
                    responseBufferLength++;
                    // Change receive timeout after first received byte
                    if (_serialPort.ReadTimeout != tmpTimeout)
                    {
                        _serialPort.ReadTimeout = tmpTimeout;
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
                if (responseBufferLength == 0)
                {
                    wmnReceiveRet =  new wmnRet(-102, "Timeout error: Do not receive response whitin specified 'Timeout' ");
                }
                else
                {
                    wmnReceiveRet = CheckResponse(requestAdu[0], requestAdu[1], responseBuffer, responseBufferLength, out responsePdu);
                }
            }
            return wmnReceiveRet;
        }

        protected virtual wmnRet CheckResponse(byte requestSlaveID, byte requestFcCode, byte[] responseBuffer, int responseBufferLength, out byte[] responsePdu)
        {
            responsePdu = null;
            // Check minimal response length 
            if (responseBufferLength < 5)
            {                
                return new wmnRet(-500, "Error: Invalid response telegram, do not receive minimal length of 5 byte");
            }
            // Decode SlaveID(RTU/ASCII)
            byte responseSlaveID = responseBuffer[0];
            // Check SlaveID(RTU/ASCII)
            if (responseSlaveID != requestSlaveID)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, Slave ID doesn't match");
            }
            // Is response a "modbus exception response"
            if ((responseBuffer[1] & 0x80) > 0)
            {                
                return new wmnRet((int)responseBuffer[2], "Modbus exception received: " + ((ModbusExceptionCodes)responseBuffer[2]).ToString());
            }
            // Decode Modbus-Function-Code
            byte responseFcCode = responseBuffer[1];
            // Check Modbus-Function-Code
            if (responseFcCode != requestFcCode)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, Function Code doesn't match");
            }
            // Check CRC
            byte[] crc16 = CRC16.CalcCRC16(responseBuffer, responseBufferLength - 2);
            if ((responseBuffer[responseBufferLength - 2] != crc16[0]) | (responseBuffer[responseBufferLength - 1] != crc16[1]))
            {               
                return new wmnRet(-501, "Error: Invalid response telegram, CRC16-check failed");
            }
            // Strip ADU header and copy response PDU into output buffer 
            responsePdu = new byte[responseBufferLength - 2];
            for (int index = 0; index < responseBufferLength - 2; index++)
            {
                responsePdu[index] = responseBuffer[index];
            }
            return new wmnRet(0, "Successful executed");
        }

        protected override wmnRet BuildRequestAdu(byte[] requestPdu, out byte[] requestAdu)       
        {
            // Check for unsupported function codes by transport layer RTU and ASCII        
            if (requestPdu[1] == (byte)ModbusFunctionCodes.Fc66_ReadBlock)
            {
                requestAdu = null;
                return new wmnRet(-205, "Error: Unsupported FunctionCode for transport layer RTU");
            }
            // Decorate request pdu                
            requestAdu = new byte[requestPdu.Length +2];  // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusRTU
            // Copy request PDU
            for (int index = 0; index < requestPdu.Length; index++)
            {
                requestAdu[index] = requestPdu[index];
            }
            // Calc CRC16
            byte[] crc16 = CRC16.CalcCRC16(requestAdu, requestAdu.Length-2);
            // Append CRC
            requestAdu[requestAdu.Length - 2] = crc16[0];
            requestAdu[requestAdu.Length - 1] = crc16[1];

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
        
        protected override wmnRet BuildRequestAdu(byte[] requestPdu, out byte[] requestAdu)        
        {
            // Check for unsupported function codes by transport layer ASCII        
            if (requestPdu[1] == (byte)ModbusFunctionCodes.Fc66_ReadBlock)
            {
                requestAdu = null;
                return new wmnRet(-206, "Error: Unsupported FunctionCode for transport layer ASCII");
            }
            // Decorate request pdu   
            requestAdu = new byte[((requestPdu.Length + 1) * 2) + 3];  // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusASCII
            // Insert ":"
            requestAdu[0] = 0x3A;                   // ":"

            // Convert nibbles to ASCII, insert nibbles into ADU and calculate LRC on the fly
            byte tmp;
            byte lrc = 0;
            for (int index1 = 0, index2 = 0; index1 < (requestPdu.Length * 2); index1++)
            {
                // Example : Byte = 0x5B converted to Char1 = 0x35 ('5') and Char2 = 0x42 ('B') 
                tmp = ((index1 % 2) == 0) ? tmp = (byte)((requestPdu[index2] >> 4) & 0x0F) : (byte)(requestPdu[index2] & 0x0F);
                requestAdu[1 + index1] = (tmp <= 0x09) ? (byte)(0x30 + tmp) : (byte)(0x37 + tmp);
                if ((index1 % 2) != 0)
                {
                    lrc += requestPdu[index2];
                    index2++;
                }
            }
            lrc = (byte)(lrc * (-1));
            // Convert LRC upper nibble to ASCII            
            tmp = (byte)((lrc >> 4) & 0x0F);
            // Insert ASCII coded upper LRC nibble into ADU
            requestAdu[requestAdu.Length - 4] = (tmp <= 0x09) ? (byte)(0x30 + tmp) : (byte)(0x37 + tmp);
            // Convert LRC lower nibble to ASCII   
            tmp = (byte)(lrc & 0x0F);
            // Insert ASCII coded lower LRC nibble into ADU
            requestAdu[requestAdu.Length - 3] = (tmp <= 0x09) ? (byte)(0x30 + tmp) : (byte)(0x37 + tmp);
            // Insert "CRLF"
            requestAdu[requestAdu.Length - 2] = 0x0D; // "CR"
            requestAdu[requestAdu.Length - 1] = 0x0A; // "LF"

            return new wmnRet(0, "Successful executed");                  
        }

        protected override wmnRet CheckResponse(byte requestSlaveID, byte requestFcCode, byte[] responseBuffer, int responseBufferLength, out byte[] responsePdu)
        {
            responsePdu = null;
            // Check minimal response length 
            if (responseBufferLength < 13)
            {                
                return new wmnRet(-501, "Error: Invalid response telegram, do not receive minimal length of 13 byte");
            }
            // Check ":" and "CRLF"
            if ((responseBuffer[0] != 0x3A) | (responseBuffer[responseBufferLength - 2] != 0x0D) | (responseBuffer[responseBufferLength-1] != 0x0A))
            {
                return new wmnRet(-501, "Error: Invalid response telegram, could not find ':' or 'CRLF");
            }
            // Convert ASCII telegram to binary
            byte[] buffer = new byte[(responseBufferLength - 3) / 2];
            byte high, low, tmp;
            for (int index = 0; index < buffer.Length; index++)
            {
                // Example : Char1 = 0x35 ('5') and Char2 = 0x42 ('B') compressed to Byte = 0x5B
                tmp = responseBuffer[(2 * index) + 1];
                high = (tmp <= 0x39) ? (byte)(tmp - 0x30) : (byte)(tmp - 0x37);
                tmp = responseBuffer[(2 * index) + 2];
                low = (tmp <= 0x39) ? (byte)(tmp - 0x30) : (byte)(tmp - 0x37);
                buffer[index] = (byte)((byte)(high << 4) | low);
            }

            // Calculate LRC
            byte lrc = 0;
            for (int index = 0; index < buffer.Length - 1; index++)
            {
                lrc += buffer[index];
            }
            lrc = (byte)(lrc * (-1));

            // Check LRC
            if (buffer[buffer.Length - 1] != lrc)
            {
                return new wmnRet(-501, "Error: Invalid response telegram, LRC check failed"); 
            }
            // Decode SlaveID(RTU/ASCII)
            byte responseSlaveID = buffer[0];
            // Check SlaveID(RTU/ASCII)
            if (responseSlaveID != requestSlaveID)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, Slave ID doesn't match");
            }
            // Is response a "modbus exception response"
            if ((buffer[1] & 0x80) > 0)
            {
                return new wmnRet((int)buffer[2], "Modbus exception received: " + ((ModbusExceptionCodes)buffer[2]).ToString());
            }
            // Decode Modbus-Function-Code
            byte responseFcCode = buffer[1];
            // Check Modbus-Function-Code
            if (responseFcCode != requestFcCode)
            {
                return new wmnRet(-500, "Error: Invalid response telegram, Function Code doesn't match");
            }
            // Strip LRC and copy response PDU into output buffer 
            responsePdu = new byte[buffer.Length - 1];
            for (int index = 0; index < responsePdu.Length; index++)
            {
                responsePdu[index] = buffer[index];
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
                for (int index = 0; index < length; ++index)
                {
                   idx = high ^ buffer[index];
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
                for (int index = 0; index < length; ++index)
                {
                    idx = high ^ buffer[index];
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
        // Convert data from ushort[] into float[]
        public static float[] ToSingle(ushort[] buffer)
        {            
            byte[] tmp = new byte[4];
            float[] outData = new float[buffer.Length / 2];
            for (int index = 0; index < outData.Length; index++)
            {
                tmp[2] = (byte)(buffer[(index * 2) + 1] & 0xFF);
                tmp[3] = (byte)(buffer[(index * 2) + 1] >> 8);
                tmp[0] = (byte)(buffer[index * 2] & 0xFF);
                tmp[1] = (byte)(buffer[index * 2] >> 8);
                outData[index] = BitConverter.ToSingle(tmp, 0);
            }
            return outData;
        }

        // Convert data from ushort[] into Int32[]
        public static int[] ToInt32(ushort[] buffer)
        {
            byte[] tmp = new byte[4];
            int[] outData = new int[buffer.Length / 2];
            for (int index = 0; index < outData.Length; index++)
            {
                tmp[2] = (byte)(buffer[(index * 2) + 1] & 0xFF);
                tmp[3] = (byte)(buffer[(index * 2) + 1] >> 8);
                tmp[0] = (byte)(buffer[index * 2] & 0xFF);
                tmp[1] = (byte)(buffer[index * 2] >> 8);
                outData[index] = BitConverter.ToInt32(tmp, 0);
            }
            return outData;
        }

        // Convert data from ushort[] into UInt32[]
        public static uint[] ToUInt32(ushort[] buffer)
        {
            byte[] tmp = new byte[4];
            uint[] outData = new uint[buffer.Length / 2];
            for (int index = 0; index < outData.Length; index++)
            {
                tmp[2] = (byte)(buffer[(index * 2) + 1] & 0xFF);
                tmp[3] = (byte)(buffer[(index * 2) + 1] >> 8);
                tmp[0] = (byte)(buffer[index * 2] & 0xFF);
                tmp[1] = (byte)(buffer[index * 2] >> 8);
                outData[index] = BitConverter.ToUInt32(tmp, 0);
            }
            return outData;
        }

        // Convert data from ushort[] into string
        public static string ToString(ushort[] buffer)
        {
            byte[] tmp = new byte[buffer.Length * 2];
            int count = 0;
            for (int index1 = 0, index2 = 0; index1 < buffer.Length; index1++)
            {
                tmp[index2] = (byte)(buffer[index1] & 0xFF);
                if (tmp[index2] == 0x00) { count = index2; break; }
                tmp[index2 + 1] = (byte)(buffer[index1] >> 8);
                if (tmp[index2 + 1] == 0x00) { count = index2 + 1; break; }
                index2 += 2;                              
            }
            return Encoding.ASCII.GetString(tmp, 0, count);
        }

        // Convert data from string into ushort[]
        public static ushort[] ToUInt16(string txt)
        {
            byte[] tmp = Encoding.ASCII.GetBytes(txt);
            int count = tmp.Length;
            ushort[] outData = new ushort[(count / 2) + 1];
            for (int index = 0; index < tmp.Length; index++)
            {
                outData[index / 2] = (index % 2 == 0) ? (ushort)(tmp[index]) : (ushort)(outData[index / 2] | tmp[index] << 8);
            }             
            return outData;
        }

        // Convert data from float into ushort[]
        public static ushort[] ToUInt16(float value)
        {           
            ushort[] outData = new ushort[2];
            byte[] tmp = BitConverter.GetBytes(value);
            for (int index = 0; index < 4; index++)
            {
                outData[index / 2] = (index % 2 == 0) ? (ushort)(tmp[index]) : (ushort)(outData[index / 2] | tmp[index] << 8);
            }
            return outData;
        }


        // Convert data from float[] into ushort[]
        public static ushort[] ToUInt16(float[] values)
        {
            ushort[] outData = new ushort[values.Length *2];
            int index = 0;
            foreach (float value in values)
            {
                byte[] tmp = BitConverter.GetBytes(value);                              
                outData[index] = (ushort)(tmp[0] | (tmp[1] << 8));
                outData[index + 1] = (ushort)(tmp[2] | (tmp[3] << 8));
                index += 2;                
            }
            return outData;
        }

        // Convert data from Int32 into ushort[]
        public static ushort[] ToUInt16(int value)
        {
            ushort[] outData = new ushort[2];
            byte[] tmp = BitConverter.GetBytes(value);
            for (int index = 0; index < 4; index++)
            {
                outData[index / 2] = (index % 2 == 0) ? (ushort)(tmp[index]) : (ushort)(outData[index / 2] | tmp[index] << 8);
            }
            return outData;
        }

        // Convert data from Int32[] into ushort[]
        public static ushort[] ToUInt16(int[] values)
        {
            ushort[] outData = new ushort[values.Length * 2];
            int index = 0;
            foreach (int value in values)
            {
                byte[] tmp = BitConverter.GetBytes(value);
                outData[index] = (ushort)(tmp[0] | (tmp[1] << 8));
                outData[index + 1] = (ushort)(tmp[2] | (tmp[3] << 8));
                index += 2;
            }
            return outData;
        }

       // Convert data from Int32 into ushort[]
        public static ushort[] ToUInt16(uint value)
        {
            ushort[] outData = new ushort[2];
            byte[] tmp = BitConverter.GetBytes(value);
            for (int index = 0; index < 4; index++)
            {
                outData[index / 2] = (index % 2 == 0) ? (ushort)(tmp[index]) : (ushort)(outData[index / 2] | tmp[index] << 8);
            }
            return outData;
        }

        // Convert data from Int32[] into ushort[]
        public static ushort[] ToUInt16(uint[] values)
        {
            ushort[] outData = new ushort[values.Length * 2];
            int index = 0;
            foreach (uint value in values)
            {
                byte[] tmp = BitConverter.GetBytes(value);
                outData[index] = (ushort)(tmp[0] | (tmp[1] << 8));
                outData[index + 1] = (ushort)(tmp[2] | (tmp[3] << 8));
                index += 2;
            }
            return outData;
        }

    }



     #endregion            

   
}
