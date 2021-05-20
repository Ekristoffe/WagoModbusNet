namespace WagoModbusNet.Exceptions.Modbus
{
    public class IllegalFunctionException : ModbusException
    {
        private const Enums.ModbusExceptionCodes _code = Enums.ModbusExceptionCodes.ILLEGAL_FUNCTION;
        private const string _name = "Illegal Function";
        private const string _meaning = "Function code received in the query is not recognized or allowed by slave.";

        public IllegalFunctionException() : base(_code, _name, _meaning) { }
    }
}
