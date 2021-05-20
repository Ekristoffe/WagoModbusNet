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

namespace WagoModbusNet
{
    public class ModbusMasterAscii : ModbusMasterRtu
    {

        public ModbusMasterAscii()
        {
            Ascii = true;
        }

        protected override byte[] BuildRequestAdu(byte[] requestPdu)
        {
            // Check for unsupported function codes by transport layer ASCII        
            if (requestPdu[1] == (byte)Enums.ModbusFunctionCodes.Fc66_ReadBlock)
                throw new Exceptions.Modbus.IllegalFunctionException();

            // Decorate request pdu   
            byte[] _requestAdu = new byte[((requestPdu.Length + 1) * 2) + 3];  // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusASCII
            // Insert START_OF_FRAME_CHAR's
            _requestAdu[0] = 0x3A; // START_OF_FRAME_CHAR   

            // Convert nibbles to ASCII, insert nibbles into ADU and calculate LRC on the fly
            byte _val;
            byte _lrc = 0;
            for (int _index1 = 0, _index2 = 0; _index1 < (requestPdu.Length * 2); _index1++)
            {
                //Example : Byte = 0x5B converted to Char1 = 0x35 ('5') and Char2 = 0x42 ('B') 
                _val = ((_index1 % 2) == 0) ? _val = (byte)((requestPdu[_index2] >> 4) & 0x0F) : (byte)(requestPdu[_index2] & 0x0F);
                _requestAdu[1 + _index1] = (_val <= 0x09) ? (byte)(0x30 + _val) : (byte)(0x37 + _val);
                if ((_index1 % 2) != 0)
                {
                    _lrc += requestPdu[_index2];
                    _index2++;
                }
            }
            _lrc = (byte)(_lrc * (-1));
            // Convert LRC upper nibble to ASCII            
            _val = (byte)((_lrc >> 4) & 0x0F);
            // Insert ASCII coded upper LRC nibble into ADU
            _requestAdu[_requestAdu.Length - 4] = (_val <= 0x09) ? (byte)(0x30 + _val) : (byte)(0x37 + _val);
            // Convert LRC lower nibble to ASCII   
            _val = (byte)(_lrc & 0x0F);
            // Insert ASCII coded lower LRC nibble into ADU
            _requestAdu[_requestAdu.Length - 3] = (_val <= 0x09) ? (byte)(0x30 + _val) : (byte)(0x37 + _val);
            // Insert END_OF_FRAME_CHAR's
            _requestAdu[_requestAdu.Length - 2] = 0x0D; // END_OF_FRAME_CHAR1
            _requestAdu[_requestAdu.Length - 1] = 0x0A; // END_OF_FRAME_CHAR2

            return _requestAdu;
        }

        protected override byte[] CheckResponse(byte requestSlaveID, byte requestFcCode, byte[] responseBuffer, int responseBufferLength)
        {
            // Check minimal response length 
            if (responseBufferLength < 13)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, did not receive minimal length of 13 bytes");

            // Check "START_OF_FRAME_CHAR" and "END_OF_FRAME_CHAR's"
            if ((responseBuffer[0] != 0x3A) | (responseBuffer[responseBufferLength - 2] != 0x0D) | (responseBuffer[responseBufferLength - 1] != 0x0A))
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, could not find 'START_OF_FRAME_CHAR' or 'END_OF_FRAME_CHARs'");

            // Convert ASCII telegram to binary
            byte[] _buffer = new byte[(responseBufferLength - 3) / 2];
            byte _high, _low, _val;
            for (int _index = 0; _index < _buffer.Length; _index++)
            {
                //Example : Char1 = 0x35 ('5') and Char2 = 0x42 ('B') compressed to Byte = 0x5B
                _val = responseBuffer[(2 * _index) + 1];
                _high = (_val <= 0x39) ? (byte)(_val - 0x30) : (byte)(_val - 0x37);
                _val = responseBuffer[(2 * _index) + 2];
                _low = (_val <= 0x39) ? (byte)(_val - 0x30) : (byte)(_val - 0x37);
                _buffer[_index] = (byte)((byte)(_high << 4) | _low);
            }

            // Calculate LRC
            byte _lrc = 0;
            for (int _index = 0; _index < _buffer.Length - 1; _index++)
            {
                _lrc += _buffer[_index];
            }
            _lrc = (byte)(_lrc * (-1));

            // Check LRC
            if (_buffer[_buffer.Length - 1] != _lrc)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, LRC check failed");

            // Decode SlaveID(RTU/ASCII)
            byte responseSlaveID = _buffer[0];
            // Check SlaveID(RTU/ASCII)
            if (responseSlaveID != requestSlaveID)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, Slave ID doesn't match");

            // Is response a "modbus exception response"
            if ((_buffer[1] & 0x80) > 0)
                throw Exceptions.ModbusException.GetModbusException(_buffer[2]);

            // Decode Modbus-Function-Code
            byte responseFcCode = _buffer[1];
            // Check Modbus-Function-Code
            if (responseFcCode != requestFcCode)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, Function Code doesn't match");
 
            // Strip LRC and copy response PDU into output buffer 
            byte[] _responsePdu = new byte[_buffer.Length - 1];
            for (int _index = 0; _index < _responsePdu.Length; _index++)
            {
                _responsePdu[_index] = _buffer[_index];
            }
            return _responsePdu;
        }
    }
}
