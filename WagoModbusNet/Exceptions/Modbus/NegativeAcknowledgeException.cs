namespace WagoModbusNet.Exceptions.Modbus
{
    public class NegativeAcknowledgeException : ModbusException
    {
        private const Enums.ModbusExceptionCodes _code = Enums.ModbusExceptionCodes.NEGATIVE_ACKNOWLEDGE;
        private const string _name = "Negative Acknowledge";
        private const string _meaning = "Slave cannot perform the programming functions. Master should request diagnostic or error information from slave.";

        public NegativeAcknowledgeException() : base(_code, _name, _meaning) { }
    }
}
