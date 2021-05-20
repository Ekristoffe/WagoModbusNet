using System;

namespace WagoModbusNet.Exceptions
{
    public class IpDnsException : Exception
    {
        public IpDnsException(string hostname) 
            : base("DNS error: Could not resolve Ip-Address for " + hostname) 
        {

        }
    }
}
