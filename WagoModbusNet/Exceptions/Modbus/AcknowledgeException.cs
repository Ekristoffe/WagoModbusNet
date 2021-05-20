namespace WagoModbusNet.Exceptions.Modbus
{
    public class AcknowledgeException : ModbusException
    {
        private const Enums.ModbusExceptionCodes _code = Enums.ModbusExceptionCodes.ACKNOWLEDGE;
        private const string _name = "Acknowledge";
        private const string _meaning = "Slave has accepted request and is processing it, but a long duration of time is required. This response is returned to prevent a timeout error from occurring in the master. Master can next issue a Poll Program Complete message to determine whether processing is completed.";

        public AcknowledgeException() : base(_code, _name, _meaning) { }
    }
}
