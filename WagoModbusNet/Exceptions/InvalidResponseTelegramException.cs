using System;

namespace WagoModbusNet.Exceptions
{
    public class InvalidResponseTelegramException : Exception
    {
        public InvalidResponseTelegramException(string message) 
            : base(message)
        {

        }
    }
}
