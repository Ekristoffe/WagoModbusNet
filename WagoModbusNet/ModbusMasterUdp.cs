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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WagoModbusNet
{
    public class ModbusMasterUdp : ModbusMaster
    {
        private static ushort _globalTransactionId = 4711;
        protected ushort _requestTransactionId;
        private ushort GlobalTransactionId
        {
            get
            {
                return ++_globalTransactionId;
            }
        }


        protected string _hostname = "";
        protected IPAddress _ip = null;
        public string Hostname
        {
            get { return _hostname; }
            set
            {
                _hostname = value;
                if (IPAddress.TryParse(value, out _ip) == false)
                {
                    /*//Sync name resolving would block up to 5 seconds
                     * IPHostEntry ipHostEntry = Dns.GetHostEntry(value);
                     *_ip = ipHostEntry.AddressList[0];
                     */
                    //Async name resolving will not block but needs also up to 5 seconds until it returns 
                    IAsyncResult _asyncResult = Dns.BeginGetHostEntry(value, null, null);
                    _asyncResult.AsyncWaitHandle.WaitOne(); // Wait until job is done - No chance to cancel request
                    IPHostEntry _ipHostEntry = null;
                    try
                    {
                        _ipHostEntry = Dns.EndGetHostEntry(_asyncResult); //EndGetHostEntry will wait for you if calling before job is done 
                    }
                    catch { }
                    if (_ipHostEntry != null)
                    {
                        _ip = _ipHostEntry.AddressList[0];
                    }
                    else
                        throw new Exceptions.IpDnsException(_hostname);
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

        protected bool _connected;
        public override bool Connected
        {
            get { return _connected; }
        }

        protected Socket _socket;
        public override void Connect()
        {
            //Create socket
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
            // Successful connected
            _connected = true;
        }

        public virtual void Connect(string hostname)
        {
            this.Hostname = hostname;
            Connect();
        }

        public virtual void Connect(string hostname, int port)
        {
            this.Hostname = hostname;
            _port = port;
            Connect();
        }

        public override void Disconnect()
        {
            //Close socket and free ressources 
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
        protected override byte[] Query(byte[] requestAdu)
        {
           byte[] _responsePdu = null;

            if (requestAdu[7] == (byte)Enums.ModbusFunctionCodes.Fc66_ReadBlock) // Check for FC66 - ReadBlock
            {
                int _readcount = ((requestAdu[10] << 8) | requestAdu[11]);
                if (_readcount > 750) //Check ReadCount exceed max UDP-Package size of 1500 byte 
                    throw new Exceptions.IpUdpException();
            }
            if ((_ip == null) || (_hostname == ""))
            {
                throw new Exceptions.IpDnsException(_hostname);
            }
            try
            {
                if (!_connected && _autoConnect)
                    Connect(); // Connect will succesful in any case because it just create a socket instance

                if (!_connected)
                    throw new Exceptions.NotConnectedException();

                // Send Request( synchron )             
                IPEndPoint _ipepRemote;
                _ipepRemote = new IPEndPoint(_ip, _port);
                _socket.SendTo(requestAdu, _ipepRemote);

                byte[] _receiveBuffer = new byte[1500]; //Receive buffer

                // Remote EndPoint to capture the identity of responding host.
                EndPoint _epRemote = (EndPoint)_ipepRemote;

                int _byteCount = _socket.ReceiveFrom(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, ref _epRemote);

                _responsePdu = CheckResponse(requestAdu[6], requestAdu[7], _receiveBuffer, _byteCount);
            }
            finally
            {
                if (_autoConnect)
                {
                    Disconnect();
                }
            }

            return _responsePdu;
        }

        protected virtual byte[] CheckResponse(byte requestUnitID, byte requestFcCode, byte[] responseBuffer, int responseBufferLength)
        {
            // Check minimal response length of 8 byte
            if (responseBufferLength < 8)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram. Did not receive minimal length of 8 bytes.");

            // Decode Transaction-ID
            ushort _responseTransactionId = (ushort)((ushort)responseBuffer[1] | (ushort)((ushort)(responseBuffer[0] << 8)));
            // Check Transaction-ID
            if (_responseTransactionId != _requestTransactionId)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, Slave ID doesn't match");

            // Decode act telegram lengh
            ushort _responsePduLength = (ushort)((ushort)responseBuffer[5] | (ushort)((ushort)(responseBuffer[4] << 8)));
            // Check all bytes received 
            if (responseBufferLength < _responsePduLength + 6)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, do not receive complied telegram");

            // Decode UnitID(TCP/UDP)
            byte _responseUnitID = responseBuffer[6];
            // Check UnitID(TCP/UDP)
            if (_responseUnitID != requestUnitID)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, Unit ID doesn't match");

            // Is response a "modbus exception response"
            if ((responseBuffer[7] & 0x80) > 0)
                throw Exceptions.ModbusException.GetModbusException(responseBuffer[8]);

            // Decode Modbus-Function-Code
            byte _responseFcCode = responseBuffer[7];
            // Check Modbus-Function-Code
            if (_responseFcCode != requestFcCode)
                throw new Exceptions.InvalidResponseTelegramException("Error: Invalid response telegram, Function Code doesn't match");

            // Strip ADU header and copy response PDU into output buffer 
            byte[] _responsePdu = new byte[_responsePduLength];
            for (int _index = 0; _index < _responsePduLength; _index++)
            {
                _responsePdu[_index] = responseBuffer[6 + _index];
            }
            return _responsePdu;
        }

        // Prepare request telegram
        protected override byte[] BuildRequestAdu(byte[] requestPdu)
        {
            byte[] _requestAdu = new byte[6 + requestPdu.Length]; // Contains the modbus request protocol data unit(PDU) togehther with additional information for ModbusTCP
            byte[] _help; // Used to convert ushort into bytes

            _requestTransactionId = this.GlobalTransactionId;
            _help = BitConverter.GetBytes(_requestTransactionId);
            _requestAdu[0] = _help[1];						// Transaction-ID -Hi
            _requestAdu[1] = _help[0];						// Transaction-ID -Lo
            _requestAdu[2] = 0x00;						    // Protocol-ID - allways zero
            _requestAdu[3] = 0x00;						    // Protocol-ID - allways zero
            _help = BitConverter.GetBytes(requestPdu.Length);
            _requestAdu[4] = _help[1];						// Number of bytes follows -Hi 
            _requestAdu[5] = _help[0];						// Number of bytes follows -Lo 
            // Copy request PDU
            for (int _index = 0; _index < requestPdu.Length; _index++)
            {
                _requestAdu[6 + _index] = requestPdu[_index];
            }
            return _requestAdu;
        }
    }
}
