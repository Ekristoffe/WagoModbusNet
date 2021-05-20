using System;

namespace WagoModbusNet.Exceptions
{
    public class IpUdpException : Exception
    {
        public IpUdpException()
            : base("UDP error: ReadCount exceed max UDP-Package size of 1500 byte")
        {

        }
    }
}
