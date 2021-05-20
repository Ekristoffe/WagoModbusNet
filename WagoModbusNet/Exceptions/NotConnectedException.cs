using System;

namespace WagoModbusNet.Exceptions
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException()
            : base("Not Connected")
        {

        }
    }
}
