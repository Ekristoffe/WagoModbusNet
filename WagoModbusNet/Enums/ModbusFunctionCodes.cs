namespace WagoModbusNet.Enums
{
    public enum ModbusFunctionCodes : byte
    {
        Fc1_ReadCoils = 1,
        Fc2_ReadDiscreteInputs = 2,
        Fc3_ReadHoldingRegisters = 3,
        Fc4_ReadInputRegisters = 4,
        Fc5_WriteSingleCoil = 5,
        Fc6_WriteSingleRegister = 6,
        Fc11_GetCommEventCounter = 11,
        Fc15_WriteMultipleCoils = 15,
        Fc16_WriteMultipleRegisters = 16,
        Fc22_MaskWriteRegister = 22,
        Fc23_ReadWriteMultipleRegisters = 23,
        Fc66_ReadBlock = 66
    };
}
