/*
Description:    
    WagoModbusNet provide easy to use Modbus-Master classes for TCP, UDP, RTU and ASCII.
    WagoModbusNet based on dot.net framework 2.0.
    WagoModbusNet.Masters do not throw any exception, all function returns a struct of type 'wmnRet'.
    For a list of supported function codes see 'enum ModbusFunctionCodes'.    
  
Version: 1.2.0.0 (11.07.2019)
   
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
    Copyright (c) WAGO Kontakttechnik GmbH & Co.KG 2019

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

namespace WagoModbusNet
{
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

        public abstract void Connect();

        public abstract void Disconnect();

        /// <summary>
        /// FC1 - Read Coils
        /// WAGO coupler and controller do not differ between FC1 and FC2
        /// Digital outputs utilize a offset of 256. First coil start at address 256.
        /// Address 0 and follows returning status of digital inputs modules 
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress"></param>
        /// <param name="readCount"></param>
        /// <returns>bool[] readData</returns>
        public bool[] ReadCoils(byte id, ushort readAddress, ushort readCount)
        {
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            byte[] _responsePdu = SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc1_ReadCoils, readAddress, readCount, 0, 0, null); // Response PDU

            // Strip PDU header and convert data into bool[]
            bool[] _readData = new bool[readCount];
            for (int _index1 = 0, _index2 = 0; _index1 < readCount; _index1++)
            {
                _readData[_index1] = ((_responsePdu[_index2 + 3] & (byte)(0x01 << (_index1 % 8))) > 0) ? true : false;
                _index2 = (_index1 + 1) / 8;
            }

            return _readData;
        }

        /// <summary>
        /// FC2 - Read Discrete Inputs
        /// WAGO coupler and controller do not differ between FC1 and FC2
        /// Address 0 and follows returning status of digital inputs modules
        /// Digital outputs utilize a offset of 256. First coil start at address 256.         
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress"></param>
        /// <param name="readCount"></param>
        /// <returns>bool[] readData</returns>
        public bool[] ReadDiscreteInputs(byte id, ushort readAddress, ushort readCount)
        {
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            byte[] _responsePdu = SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc2_ReadDiscreteInputs, readAddress, readCount, 0, 0, null); // Response PDU

            // Strip PDU header and convert data into bool[]
            bool[] _readData = new bool[readCount];
            for (int _index1 = 0, _index2 = 0; _index1 < readCount; _index1++)
            {
                _readData[_index1] = ((_responsePdu[_index2 + 3] & (byte)(0x01 << (_index1 % 8))) > 0) ? true : false;
                _index2 = (_index1 + 1) / 8;
            }

            return _readData;
        }

        /// <summary>
        /// FC3 - Read Holding Registers
        /// WAGO coupler and controller do not differ between FC3 and FC4
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress"></param>
        /// <param name="readCount"></param>
        /// <returns>ushort[] readData</returns>
        public ushort[] ReadHoldingRegisters(byte id, ushort readAddress, ushort readCount)
        {
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            byte[] _responsePdu = SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc3_ReadHoldingRegisters, readAddress, readCount, 0, 0, null); // Response PDU

            // Strip PDU header and convert data into ushort[]
            byte[] _tmp = new byte[2];
            int _count = (_responsePdu[2] / 2);
            ushort[] _readData = new ushort[_count];
            for (int _index = 0; _index < _count; _index++)
            {
                _tmp[0] = _responsePdu[4 + (2 * _index)];
                _tmp[1] = _responsePdu[3 + (2 * _index)];
                _readData[_index] = BitConverter.ToUInt16(_tmp, 0);
            }

            return _readData;
        }

        /// <summary>
        /// FC4 - Read Input Registers
        /// WAGO coupler and controller do not differ between FC3 and FC4
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress"></param>
        /// <param name="readCount"></param>
        /// <returns>ushort[] readData</returns>
        public ushort[] ReadInputRegisters(byte id, ushort readAddress, ushort readCount)
        {
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            byte[] _responsePdu = SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc4_ReadInputRegisters, readAddress, readCount, 0, 0, null); // Response PDU

            // Strip PDU header and convert data into ushort[]
            byte[] _tmp = new byte[2];
            int _count = (_responsePdu[2] / 2);
            ushort[] _readData = new ushort[_count];
            for (int _index = 0; _index < _count; _index++)
            {
                _tmp[0] = _responsePdu[4 + (2 * _index)];
                _tmp[1] = _responsePdu[3 + (2 * _index)];
                _readData[_index] = BitConverter.ToUInt16(_tmp, 0);
            }

            return _readData;
        }

        /// <summary>
        /// FC5 - Write Single Coil
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public void WriteSingleCoil(byte id, ushort writeAddress, bool writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] _writeData = new byte[1];
            _writeData[0] = (writeData) ? (byte)0xFF : (byte)0x00;
            SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc5_WriteSingleCoil, 0, 0, writeAddress, 1, _writeData);
        }

        /// <summary>
        /// FC6 - Write Single Register
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public void WriteSingleRegister(byte id, ushort writeAddress, ushort writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] _writeData = BitConverter.GetBytes(writeData);
            SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc6_WriteSingleRegister, 0, 0, writeAddress, 1, _writeData);
        }

        /// <summary>
        /// FC11 - Get Comm Event Counter
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="status"> return 0 for ready to process next requst or 0xFFFF when device busy</param>
        /// <returns>ushort eventCount (number of successful processed Modbus-Requests)</returns>
        public ushort GetCommEventCounter(byte id, out ushort status)
        {
            status = 0;
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            byte[] _responsePdu = SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc11_GetCommEventCounter, 0, 0, 0, 0, null); // Response PDU

            // Strip PDU header and convert data into ushort[]
            byte[] _tmp = new byte[2];
            // Extract status
            _tmp[0] = _responsePdu[3];
            _tmp[1] = _responsePdu[2];
            status = BitConverter.ToUInt16(_tmp, 0);
            // Extract eventCount
            _tmp[0] = _responsePdu[5];
            _tmp[1] = _responsePdu[4];
            ushort _eventCount = BitConverter.ToUInt16(_tmp, 0);

            return _eventCount;
        }

        /// <summary>
        /// FC15 - Write Multiple Coils
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public void WriteMultipleCoils(byte id, ushort writeAddress, bool[] writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] _writeData = ((writeData.Length % 8) == 0) ? new byte[writeData.Length / 8] : new byte[(writeData.Length / 8) + 1];
            for (int _index1 = 0, _index2 = 0; _index1 < writeData.Length; _index1++)
            {
                if ((_index1 > 0) && ((_index1 % 8) == 0)) _index2++;
                if (writeData[_index1]) _writeData[_index2] = (byte)(_writeData[_index2] | (byte)(0x01 << (_index1 % 8)));
            }

            SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc15_WriteMultipleCoils, 0, 0, writeAddress, (ushort)writeData.Length, _writeData);
        }

        /// <summary>
        /// FC16 - Write Multiple Registers
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public void WriteMultipleRegisters(byte id, ushort writeAddress, ushort[] writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] _writeData = new byte[writeData.Length * 2];
            byte[] _tmp;
            for (int _index1 = 0, _index2 = 0; _index1 < writeData.Length; _index1++)
            {
                _tmp = BitConverter.GetBytes(writeData[_index1]);
                _writeData[_index2] = _tmp[1];
                _writeData[_index2 + 1] = _tmp[0];
                _index2 += 2;
            }

            SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc16_WriteMultipleRegisters, 0, 0, writeAddress, (ushort)writeData.Length, _writeData);
        }

        /// <summary>
        /// FC22 - Mask Write Register
        /// Modify single bits in a register
        /// Result = (CurrentContent AND andMask) OR (orMask AND (NOT andMask))
        /// If the orMask value is zero, the result is simply the logical ANDing of the current contents and andMask. 
        /// If the andMask value is zero, the result is equal to the orMask value.
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="writeAddress"></param>
        /// <param name="andMask"></param>
        /// <param name="orMask"></param>
        /// <returns></returns>
        public void MaskWriteRegister(byte id, ushort writeAddress, ushort andMask, ushort orMask)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] _writeData = new byte[4];
            byte[] _tmp;
            _tmp = BitConverter.GetBytes(andMask);
            _writeData[0] = _tmp[0];
            _writeData[1] = _tmp[1];
            _tmp = BitConverter.GetBytes(orMask);
            _writeData[2] = _tmp[0];
            _writeData[3] = _tmp[1];

            SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc22_MaskWriteRegister, 0, 0, writeAddress, 4, _writeData);
        }

        /// <summary>
        /// FC23 - Read Write Multiple Registers
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress"></param>
        /// <param name="readCount"></param>
        /// <param name="writeAddress"></param>
        /// <param name="writeData"></param>
        /// <returns>ushort[] readData</returns>
        public ushort[] ReadWriteMultipleRegisters(byte id, ushort readAddress, ushort readCount, ushort writeAddress, ushort[] writeData)
        {
            // Convert data to write into array of byte with the correct byteorder
            byte[] _writeBuffer = new byte[writeData.Length * 2];
            byte[] _tmp;
            for (int _index1 = 0, _index2 = 0; _index1 < writeData.Length; _index1++)
            {
                _tmp = BitConverter.GetBytes(writeData[_index1]);
                _writeBuffer[_index2] = _tmp[1];
                _writeBuffer[_index2 + 1] = _tmp[0];
                _index2 += 2;
            }

            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            byte[] _responsePdu = SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc23_ReadWriteMultipleRegisters, readAddress, readCount, writeAddress, (ushort)writeData.Length, _writeBuffer); // Response PDU

            // Strip PDU header and convert data into ushort[]
            byte[] _buf = new byte[2];
            int _count = (_responsePdu[2] / 2);
            ushort[] _readData = new ushort[_count];
            for (int _index = 0; _index < _count; _index++)
            {
                _buf[0] = _responsePdu[4 + (2 * _index)];
                _buf[1] = _responsePdu[3 + (2 * _index)];
                _readData[_index] = BitConverter.ToUInt16(_buf, 0);
            }

            return _readData;
        }

        /// <summary>
        /// FC66 - ReadBlock
        /// WAGO specific Modbus extension for reading up to 32000 registers with one request.
        /// Only supported for transport layer TCP and UDP
        /// </summary>
        /// <param name="id">Unit-Id or Slave-Id depending underlaying transport layer</param>
        /// <param name="readAddress"></param>
        /// <param name="readCount"></param>
        /// <returns>ushort[] readData</returns>
        public ushort[] ReadBlock(byte id, ushort readAddress, ushort readCount)
        {
            // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
            byte[] _responsePdu = SendModbusRequest(id, Enums.ModbusFunctionCodes.Fc66_ReadBlock, readAddress, readCount, 0, 0, null); // Response PDU

            // Strip PDU header and convert data into ushort[]
            byte[] _tmp = new byte[2];
            int _count = (ushort)(_responsePdu[3] | (_responsePdu[2] << 8));

            ushort[] _readData = new ushort[_count];
            for (int _index = 0; _index < _count / 2; _index++)
            {
                _tmp[0] = _responsePdu[5 + (2 * _index)];
                _tmp[1] = _responsePdu[4 + (2 * _index)];
                _readData[_index] = BitConverter.ToUInt16(_tmp, 0);
            }

            return _readData;
        }

        // Build common part of modbus request, decorate it with transport layer specific header, send request and get response PDU back 
        public byte[] SendModbusRequest(byte id, Enums.ModbusFunctionCodes functionCode, ushort readAddress, ushort readCount, ushort writeAddress, ushort writeCount, byte[] writeData)
        {
            // Build common part of modbus request
            byte[] _requestPdu = BuildRequestPDU(id, functionCode, readAddress, readCount, writeAddress, writeCount, writeData); // Request PDU

            // Decorate common part of modbus request with transport layer specific header
            byte[] _requestAdu = BuildRequestAdu(_requestPdu); // Request ADU

            // Send modbus request and return response 
            byte[] _responsePdu = Query(_requestAdu);
            return _responsePdu;
        }


        // Decorate common part of modbus request with transport layer specific header
        protected abstract byte[] BuildRequestAdu(byte[] requestPdu);

        // Send modbus request transport layer specific and return response PDU
        protected abstract byte[] Query(byte[] requestAdu);

        // Build common part of modbus request
        private byte[] BuildRequestPDU(byte id, Enums.ModbusFunctionCodes functionCode, ushort readAddress, ushort readCount, ushort writeAddress, ushort writeCount, byte[] writeData)
        {
            byte[] _help; // Used to convert ushort into bytes
            byte[] _requestPdu = null;

            switch (functionCode)
            {
                case Enums.ModbusFunctionCodes.Fc1_ReadCoils:
                case Enums.ModbusFunctionCodes.Fc2_ReadDiscreteInputs:
                case Enums.ModbusFunctionCodes.Fc3_ReadHoldingRegisters:
                case Enums.ModbusFunctionCodes.Fc4_ReadInputRegisters:
                case Enums.ModbusFunctionCodes.Fc66_ReadBlock:
                    _requestPdu = new byte[6];
                    // Build request header 
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = (byte)functionCode;         // Modbus-Function-Code
                    _help = BitConverter.GetBytes(readAddress);
                    _requestPdu[2] = _help[1];					// Start read address -Hi
                    _requestPdu[3] = _help[0];					// Start read address -Lo
                    _help = BitConverter.GetBytes(readCount);
                    _requestPdu[4] = _help[1];				    // Number of coils or register to read -Hi
                    _requestPdu[5] = _help[0];				    // Number of coils or register to read -Lo  
                    break;

                case Enums.ModbusFunctionCodes.Fc5_WriteSingleCoil:
                    _requestPdu = new byte[6];
                    // Build request header 
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = 0x05;                       // Modbus-Function-Code: fc5_WriteSingleCoil
                    _help = BitConverter.GetBytes(writeAddress);
                    _requestPdu[2] = _help[1];					// Address of coil to force -Hi
                    _requestPdu[3] = _help[0];					// Address of coil to force -Lo
                    // Copy data
                    _requestPdu[4] = writeData[0];				// Output value -Hi  ( 0xFF or 0x00 )
                    _requestPdu[5] = 0x00;				        // Output value -Lo  ( const: 0x00  ) 
                    break;

                case Enums.ModbusFunctionCodes.Fc6_WriteSingleRegister:
                    _requestPdu = new byte[6];
                    // Build request header 
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = 0x06;                       // Modbus-Function-Code: fc6_WriteSingleRegister
                    _help = BitConverter.GetBytes(writeAddress);
                    _requestPdu[2] = _help[1];					// Address of register to force -Hi
                    _requestPdu[3] = _help[0];					// Address of register to force -Lo
                    _requestPdu[4] = writeData[1];				// Output value -Hi  
                    _requestPdu[5] = writeData[0];				// Output value -Lo  
                    break;

                case Enums.ModbusFunctionCodes.Fc11_GetCommEventCounter:
                    _requestPdu = new byte[2];
                    // Build request header 
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = 0x0B;                       // Modbus-Function-Code: fc11_GetCommEventCounter
                    break;

                case Enums.ModbusFunctionCodes.Fc15_WriteMultipleCoils:
                    byte _byteCount = (byte)(writeCount / 8);
                    if ((writeCount % 8) > 0)
                    {
                        _byteCount += 1;
                    }
                    _requestPdu = new byte[7 + _byteCount];
                    // Build request header
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = 0x0F;                       // Modbus-Function-Code: fc15_WriteMultipleCoils
                    _help = BitConverter.GetBytes(writeAddress);
                    _requestPdu[2] = _help[1];					// Start address of coils to force -Hi
                    _requestPdu[3] = _help[0];					// Start address of coils to force -Lo
                    _help = BitConverter.GetBytes(writeCount);
                    _requestPdu[4] = _help[1];				    // Number of coils to write -Hi 
                    _requestPdu[5] = _help[0];				    // Number of coils to write -Lo  
                    _requestPdu[6] = _byteCount;				    // Number of bytes to write                    
                    // Copy data
                    for (int _index = 0; _index < _byteCount; _index++)
                    {
                        _requestPdu[7 + _index] = writeData[_index];
                    }
                    break;

                case Enums.ModbusFunctionCodes.Fc16_WriteMultipleRegisters:
                    _requestPdu = new byte[7 + (writeCount * 2)];
                    // Build request header 
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = 0x10;                       // Modbus-Function-Code: fc16_WriteMultipleRegisters
                    _help = BitConverter.GetBytes(writeAddress);
                    _requestPdu[2] = _help[1];					// Start address of coils to force -Hi
                    _requestPdu[3] = _help[0];					// Start address of coils to force -Lo
                    _help = BitConverter.GetBytes(writeCount);
                    _requestPdu[4] = _help[1];				    // Number of register to write -Hi 
                    _requestPdu[5] = _help[0];				    // Number of register to write -Lo  
                    _requestPdu[6] = (byte)(writeCount * 2);		// Number of bytes to write                    
                    // Copy data
                    for (int _index = 0; _index < (writeCount * 2); _index++)
                    {
                        _requestPdu[7 + _index] = writeData[_index];
                    }
                    break;


                case Enums.ModbusFunctionCodes.Fc22_MaskWriteRegister:
                    _requestPdu = new byte[8];
                    // Build request header 
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = 0x16;                       // Modbus-Function-Code: fc22_MaskWriteRegister
                    _help = BitConverter.GetBytes(writeAddress);
                    _requestPdu[2] = _help[1];					// Address of register to force -Hi
                    _requestPdu[3] = _help[0];					// Address of register to force -Lo
                    _requestPdu[4] = writeData[1];				// And_Mask -Hi  
                    _requestPdu[5] = writeData[0];				// And_Mask -Lo  
                    _requestPdu[6] = writeData[3];				// Or_Mask -Hi  
                    _requestPdu[7] = writeData[2];				// Or_Mask -Lo  
                    break;

                case Enums.ModbusFunctionCodes.Fc23_ReadWriteMultipleRegisters:
                    _requestPdu = new byte[11 + (writeCount * 2)];
                    // Build request header 
                    _requestPdu[0] = id;                         // ID: SlaveID(RTU/ASCII) or UnitID(TCP/UDP)
                    _requestPdu[1] = 0x17;                       // Modbus-Function-Code: fc23_ReadWriteMultipleRegisters
                    _help = BitConverter.GetBytes(readAddress);
                    _requestPdu[2] = _help[1];					// Start read address -Hi
                    _requestPdu[3] = _help[0];					// Start read address -Lo
                    _help = BitConverter.GetBytes(readCount);
                    _requestPdu[4] = _help[1];				    // Number of register to read -Hi
                    _requestPdu[5] = _help[0];				    // Number of register to read -Lo           
                    _help = BitConverter.GetBytes(writeAddress);
                    _requestPdu[6] = _help[1];				    // Start write address -Hi
                    _requestPdu[7] = _help[0];				    // Start write address -Lo
                    _help = BitConverter.GetBytes(writeCount);
                    _requestPdu[8] = _help[1];				    // Number of register to write -Hi
                    _requestPdu[9] = _help[0];				    // Number of register to write -Lo
                    _requestPdu[10] = (byte)(writeCount * 2);     // Number of bytes to write
                    // Copy data
                    for (int _index = 0; _index < (writeCount * 2); _index++)
                    {
                        _requestPdu[11 + _index] = writeData[_index];
                    }
                    break;
            } // switch
            return _requestPdu;
        }
    }
}
