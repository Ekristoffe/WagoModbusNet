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
using System.IO.Ports;

namespace WagoModbusNet
{
    public class ModbusMasterRtu : ModbusMaster
    {

        public ModbusMasterRtu()
        {
        }

        private string _portName = "COM1"; // Name of serial interface like "COM23" 
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

        private SerialPort _serialPort; // The serial interface instance
        public override void Connect()
        {
            if (_connected)
                Disconnect();

            //Create instance
            _serialPort = new SerialPort(_portName, _baudrate, _parity, _databits, _stopbits);
            _serialPort.Handshake = _handshake;
            _serialPort.WriteTimeout = _timeout;
            _serialPort.ReadTimeout = _timeout;
            _serialPort.Open();
            _connected = true;
        }

        public virtual void Connect(string portname, int baudrate, Parity parity, int databits, StopBits stopbits, Handshake handshake)
        {
            //Copy settings into private members
            _portName = portname;
            _baudrate = baudrate;
            _parity = parity;
            _databits = databits;
            _stopbits = stopbits;
            _handshake = handshake;

            //Create instance
            Connect();
        }

        public override void Disconnect()
        {
            //Close port and free ressources
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
               ((IDisposable)_serialPort).Dispose();
//                _serialPort.Dispose();
                _serialPort = null;
            }
            _connected = false;
        }

        // TODO: inspect implementation. Should not keep around a buffer as a field.
        // Receive response helpers
        //private byte[] _responseBuffer = new byte[512];
        //private int _responseBufferLength;

        // Send request and and wait for response 
        protected override byte[] Query(byte[] requestAdu)
        {
            byte[] _responsePdu = null;
            if (!_connected)
                throw new Exceptions.NotConnectedException();

            // Send Request( synchron ) 
            _serialPort.Write(requestAdu, 0, requestAdu.Length);
            // Receive response helpers
            byte[] _responseBuffer = new byte[512];
            _responseBuffer.Initialize();
            int _responseBufferLength = 0;
            _serialPort.ReadTimeout = _timeout;
            int tmpTimeout = 50; // Milliseconds
            if (_baudrate < 9600)
                tmpTimeout = (int)((10000 / _baudrate) + 50);
            
            // TODO: Check if we can't instead of waiting for a timeout, check the data received and compare with the espected size
            try
            {
                // Read all data until a timeout exception is arrived
                do
                {
                    _responseBuffer[_responseBufferLength] = (byte)_serialPort.ReadByte();
                    _responseBufferLength++;

                    // Change receive timeout after first received byte
                    if (_serialPort.ReadTimeout != tmpTimeout)
                        _serialPort.ReadTimeout = tmpTimeout;

                } while (true);
            }
            catch (TimeoutException)
            {
                ; // Expected exception to know "All data received" 
            }
            finally
            {
                // Check Response
                if (_responseBufferLength == 0)
                    // TODO: This is a direct replacement. It may not be the appropriate Exception type.
                    throw new TimeoutException("Timeout error: Did not receive response whitin specified 'Timeout'.");
                else
                    _responsePdu = CheckResponse(requestAdu[0], requestAdu[1], _responseBuffer, _responseBufferLength);
            }
            return _responsePdu;
        }

        protected virtual byte[] CheckResponse(byte requestSlaveID, byte requestFcCode, byte[] responseBuffer, int responseBufferLength)
        {
            // Check minimal response length 
            if (responseBufferLength < 5)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram. Did not receive minimal length of 5 bytes.");

            // Decode SlaveID(RTU/ASCII)
            byte _responseSlaveID = responseBuffer[0];
            // Check SlaveID(RTU/ASCII)
            if (_responseSlaveID != requestSlaveID)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, Slave ID doesn't match");

            // Is response a "modbus exception response"
            if ((responseBuffer[1] & 0x80) > 0)
                throw Exceptions.ModbusException.GetModbusException(responseBuffer[2]);

            // Decode Modbus-Function-Code
            byte _responseFcCode = responseBuffer[1];
            // Check Modbus-Function-Code
            if (_responseFcCode != requestFcCode)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, Function Code doesn't match");

            // Check CRC
            byte[] _crc16 = Utilities.CRC16.CalcCRC16(responseBuffer, responseBufferLength - 2);
            if ((responseBuffer[responseBufferLength - 2] != _crc16[0]) | (responseBuffer[responseBufferLength - 1] != _crc16[1]))
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, CRC16-check failed");

            // Strip ADU header and copy response PDU into output buffer 
            byte[] _responsePdu = new byte[responseBufferLength - 2];
            for (int _index = 0; _index < responseBufferLength - 2; _index++)
                _responsePdu[_index] = responseBuffer[_index];

            return _responsePdu;
        }

        protected override byte[] BuildRequestAdu(byte[] requestPdu)
        {
            // Check for unsupported function codes by transport layer RTU and ASCII        
            if (requestPdu[1] == (byte)Enums.ModbusFunctionCodes.Fc66_ReadBlock)
                throw new Exceptions.Modbus.IllegalFunctionException();

            // Decorate request pdu                
            byte[] _requestAdu = new byte[requestPdu.Length + 2];  // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusRTU

            // Copy request PDU
            for (int _index = 0; _index < requestPdu.Length; _index++)
                _requestAdu[_index] = requestPdu[_index];

            // Calc CRC16
            byte[] crc16 = Utilities.CRC16.CalcCRC16(_requestAdu, _requestAdu.Length - 2);

            // Append CRC
            _requestAdu[_requestAdu.Length - 2] = crc16[0];
            _requestAdu[_requestAdu.Length - 1] = crc16[1];

            return _requestAdu;
        }
    }
}
