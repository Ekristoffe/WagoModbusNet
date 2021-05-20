namespace WagoModbusNet.Exceptions.Modbus
{
    public class IllegalDataAddressException : ModbusException
    {
        private const Enums.ModbusExceptionCodes _code = Enums.ModbusExceptionCodes.ILLEGAL_DATA_ADDRESS;
        private const string _name = "Illegal Data Address";
        private const string _meaning = "Data address of some or all the required entities are not allowed or do not exist in slave";

        public IllegalDataAddressException() : base(_code, _name, _meaning) { }
    }
}
