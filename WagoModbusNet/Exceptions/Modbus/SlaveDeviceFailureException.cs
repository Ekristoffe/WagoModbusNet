namespace WagoModbusNet.Exceptions.Modbus
{
    public class SlaveDeviceFailureException : ModbusException
    {
        private const Enums.ModbusExceptionCodes _code = Enums.ModbusExceptionCodes.SLAVE_DEVICE_FAILURE;
        private const string _name = "Slave Device Failure";
        private const string _meaning = "Unrecoverable error occurred while slave was attempting to perform requested action.";
        public SlaveDeviceFailureException() : base(_code, _name, _meaning) { }
    }
}
