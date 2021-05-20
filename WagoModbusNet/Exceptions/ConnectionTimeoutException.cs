using System;

namespace WagoModbusNet.Exceptions
{
    public class ConnectionTimeoutException : Exception
    {
        public ConnectionTimeoutException() 
            : base("TIMEOUT-ERROR: Timeout expired while trying to connect.")
        {

        }
    }
}
