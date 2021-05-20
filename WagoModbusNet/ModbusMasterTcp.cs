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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WagoModbusNet
{
    public class ModbusMasterTcp : ModbusMasterUdp
    {
        public ModbusMasterTcp()
        {
        }

        public ModbusMasterTcp(string hostname)
            : this()
        {
            this.Hostname = hostname;
        }

        public ModbusMasterTcp(string hostname, int port)
            : this()
        {
            this.Hostname = hostname;
            _port = port;
        }

        private ManualResetEvent _mreConnectTimeout = new ManualResetEvent(false);
        public override void Connect()
        {
            if (_connected)
                Disconnect();
            
            if ((_ip == null) || (_hostname == ""))
            {
                throw new Exceptions.IpDnsException(_hostname);
            }

            // Create client socket
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, _timeout);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout);
            // Reset timer
            _mreConnectTimeout.Reset();

            // Call async Connect 
            _socket.BeginConnect(new IPEndPoint(_ip, _port), new AsyncCallback(OnConnect), _socket);
            // Stay here until connection established or timeout expires
            if (_mreConnectTimeout.WaitOne(_timeout, false))
            {
                // Successful connected
                _connected = true;
                return;
            }
            else
            {
                // Timeout expired 
                _connected = false;
                _socket.Close(); // Implizit .EndConnect free ressources 
                _socket = null;
                throw new Exceptions.ConnectionTimeoutException();
            }

        }

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

        public override void Connect(string hostname)
        {
            this.Hostname = hostname;
            Connect();
        }

        public override void Connect(string hostname, int port)
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
                _socket.Close();
                _socket = null;
            }
            _connected = false;
        }

        // Send request and and wait for response
        protected override byte[] Query(byte[] requestAdu)
        {
            byte[] _responsePdu = null;  //Assign null to make compiler silent
            try
            {
                if (!_connected && _autoConnect)
                    Connect();

                if (!_connected)
                    throw new Exceptions.NotConnectedException();

                // Send request sync
                _socket.Send(requestAdu, 0, requestAdu.Length, SocketFlags.None);

                byte[] _receiveBuffer = new byte[65000]; //Receive buffer
                int _rxCountTotal = 0;
                int _responsePduLen = 0;

                // Try to receive complied response
                do
                {
                    int _rxCountActual = _socket.Receive(_receiveBuffer, _rxCountTotal, (_receiveBuffer.Length - _rxCountTotal), SocketFlags.None);
                    if (_rxCountActual == 0)
                        break;
                    _rxCountTotal += _rxCountActual;
                    // Extract response length information
                    _responsePduLen = (int)(_receiveBuffer[4] << 8 | _receiveBuffer[5]);
                } while (_rxCountTotal < _responsePduLen + 6);

                _responsePdu = CheckResponse(requestAdu[6], requestAdu[7], _receiveBuffer, _rxCountTotal);
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
    }
}
