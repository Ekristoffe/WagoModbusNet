using System;

namespace WagoModbusNet.Exceptions
{
    public class ModbusException : Exception
    {
        public byte Code { get; protected set; }
        public string Name { get; protected set; }
        public string Meaning { get; protected set; }

        public ModbusException(byte code, string name, string meaning) 
            : base() 
        {
            Code = code;
            Name = name;
            Meaning = meaning;
        }

        public ModbusException(Enums.ModbusExceptionCodes code, string name, string meaning) : this((byte)code, name, meaning) { }
        public ModbusException(string message) : base(message) { }
        public ModbusException(string message, Exception innerException) : base(message, innerException) { }
        
        public static ModbusException GetModbusException(byte exceptionCode)
        {
            Enums.ModbusExceptionCodes code = (Enums.ModbusExceptionCodes)exceptionCode;
            return GetModbusException(code);
        }

        public static ModbusException GetModbusException(Enums.ModbusExceptionCodes code)
        {
            switch (code)
            {
                case Enums.ModbusExceptionCodes.ILLEGAL_FUNCTION:
                    return new Modbus.IllegalFunctionException();
                case Enums.ModbusExceptionCodes.ILLEGAL_DATA_ADDRESS:
                    return new Modbus.IllegalDataAddressException();
                case Enums.ModbusExceptionCodes.ILLEGAL_DATA_VALUE:
                    return new Modbus.IllegalDataValueException();
                case Enums.ModbusExceptionCodes.SLAVE_DEVICE_FAILURE:
                    return new Modbus.SlaveDeviceFailureException();
                case Enums.ModbusExceptionCodes.ACKNOWLEDGE:
                    return new Modbus.AcknowledgeException();
                case Enums.ModbusExceptionCodes.SLAVE_DEVICE_BUSY:
                    return new Modbus.SlaveDeviceBusyException();
                case Enums.ModbusExceptionCodes.NEGATIVE_ACKNOWLEDGE:
                    return new Modbus.NegativeAcknowledgeException();
                case Enums.ModbusExceptionCodes.MEMORY_PARITY_ERROR:
                    return new Modbus.MemoryParityErrorException();
                case Enums.ModbusExceptionCodes.GATEWAY_PATH_UNAVAILABLE:
                    return new Modbus.GatewayPathUnavailableException();
                case Enums.ModbusExceptionCodes.GATEWAY_TARGET_DEVICE_FAILED_TO_RESPOND:
                    return new Exceptions.Modbus.GatewayTargetDeviceFailedToRespondException();
                default: // If the code can't be cast, it will fall through to here.
                    return new ModbusException(string.Format("Unspecified Modbus Exception Code = {0}", (byte)code));
            }
        }

        public override string Message
        {
            get
            {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                if (Code != null && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Meaning))
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    return string.Format("Modbus Exception Code {0}: {1}. {2}", Code, Name, Meaning);
                else 
                    return base.Message;
            }
        }
    }
}
