namespace WagoModbusNet.Exceptions.Modbus
{
    public class IllegalDataValueException : ModbusException
    {
        private const Enums.ModbusExceptionCodes _code = Enums.ModbusExceptionCodes.ILLEGAL_DATA_VALUE;
        private const string _name = "Illegal Data Value";
        private const string _meaning = "Value is not accepted by slave.";

        public IllegalDataValueException() : base(_code, _name, _meaning) { }
    }
}
