# WagoModbusNet

# Description:
    WagoModbusNet provide easy to use Modbus-Master classes for TCP, UDP, RTU and ASCII.
    WagoModbusNet based on dot.net framework 2.0.
    WagoModbusNet.Masters do not throw any exception, all function returns a struct of type "wmnRet".
    For a list of supported function codes see "enum ModbusFunctionCodes".     

# Author: 
	WAGO Kontakttechnik GmbH & Co.KG

# Typical pitfall:
    With 750-3xx or 750-8xx
    You dial with a WAGO Ethernet controller.
	Try to set outputs - but nothing happens!
    WAGO Ethernet controller provide a "owner" policy for physical outputs.
    The "owner" could be CoDeSys-Runtime or Fieldbus-Master.
    Every time you download a PLC program the CoDeSys-Runtime becomes "owner" of physical outputs.
    Use tool "WAGO Ethernet Settings" and do "Reset File System",
	it is the easiest way to assign Modbus-Master as "owner".
    Alternatively you can "Login" with CoDeSys-IDE and perform "Reset(original)".

    With 750-8xxx
    By default there is no Modbus server available, you must use e!Cockpit to create your own Modbus server.
	
# Frequently Ask Questions(FAQ)
[Modbus]
1. Q001: What protocol types are supported?
    - ModbusTCP
    - ModbusUDP
    - ModbusRTU
    - ModbusASCII


2. Q002: What function code are supported?
    - fc1_ReadCoils 
    - fc2_ReadDiscreteInputs 
    - fc3_ReadHoldingRegisters
    - fc4_ReadInputRegisters
    - fc5_WriteSingleCoil
    - fc6_WriteSingleRegister
    - fc11_GetCommEventCounter
    - fc15_WriteMultipleCoils
    - fc16_WriteMultipleRegisters
    - fc22_MaskWriteRegister
    - fc23_ReadWriteMultipleRegisters
	- fc66_ReadBlock


[Usage]
1. Q101: What do I have to do to work with WagoModbusNet code class?
    - Variante1: Add class file "WagoModbusNet.cs" to your project 
    - Variante2: Add a reference to "WagoModbusNet.dll" to project
    - Add namespace "using WagoModbusNet;"
    - Enable network permission for your project 

2. Q102: What frame work version is required? 
    WagoModbusNet code class utilize net framework version 2.
    You can use it with all frameworks >= version 2.


[Hardware]
1. Q201: Try to set physical outputs, no error - but nothing happens!
    WAGO Ethernet controller provide a "owner" policy for physical outputs.
    The "owner" could be CoDeSys-Runtime or Fieldbus-Master.
    Every time you download a PLC program the CoDeSys-Runtime becomes "owner" of physical outputs.
    Use tool "WAGO Ethernet Settings" and do "Reset File System", it is the easiest way to assign Modbus-Master as "owner".
    Alternatively you can "Login" with CoDeSys-IDE and perform "Reset(original)".


[License]
1. Q901: Can I use, modify or extend code?
    YES, see below

    Copyright (c) 2019 WAGO Kontakttechnik GmbH & Co.KG
    Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
    and associated documentation files (the "Software"), to deal in the Software without restriction, 
    including without limitation the rights to use, copy, modify, merge, publish, distribute, 
    sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all copies or substantial 
    portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
    NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
    IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
    SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

# Release Note
	Version: 1.1.0.4 (01.08.2019)
	- Global cleanup and use the C# Coding Standards and Naming Conventions

	Version: 1.1.0.3 (23.07.2019)
	- BugFix: ModbusMasterUdp - add the use of dispose in Disconnect()
	- BugFix: ModbusMasterTcp - add the use of dispose in Disconnect()
	- BugFix: ModbusMasterRtu - add the use of dispose in Disconnect()
	- BugFix: ModbusMasterUdp - replace "private static ushort _transactionId" with 
					"private static ushort _globalTransactionId" and
					add "protected ushort _requestTransactionId" for local use

	Version: 1.1.0.2 (09.07.2019)
	- BugFix: ModbusMasterUdp - add the use of _connected in Connect()
	- BugFix: ModbusMasterUdp - check the status of _connected and the use of _autoConnect in Query()
	- BugFix: ModbusMasterUdp - add the use of _connected in Disconnect()
	- BugFix: ModbusMasterUdp - add Transaction-ID check in CheckResponse()
	- BugFix: ModbusMasterUdp - add UnitID(TCP/UDP) check in CheckResponse()
	- BugFix: ModbusMasterUdp - add Modbus-Function-Code check in CheckResponse()
	- BugFix: ModbusMasterRtu - add SlaveID(RTU/ASCII) check in CheckResponse()
	- BugFix: ModbusMasterRtu - add Modbus-Function-Code check in CheckResponse()
	- BugFix: ModbusMasterAscii - add SlaveID(RTU/ASCII) check in CheckResponse()
	- BugFix: ModbusMasterAscii - add Modbus-Function-Code check in CheckResponse()

	Version: 1.1.0.1 (10.02.2015)
	- BugFix: ModbusMasterTcp - receive buffer handling 

	Version: 1.1.0.0 (30.01.2015)
	- Add: FC66 ReadBlock, WAGO specific Modbus extension. 

	Version: 1.0.1.1 (06.02.2013)
	- BugFix: FC1 ReadCoils, returns multiple times the status of first 8 coils
	- BugFix: FC2 ReadDiscreteInputs, returns multiple times the status of first 8 coils 

	Version: 1.0.1.0 (09.01.2013)
	- Add FC11 ReadCommEventCounter 

	Version: 1.0.0.0 (03.01.2013)
	- Add FC22 MaskWriteRegister 
	- Extend wmnRet.Value returns now ModbusExceptionCode instead of static -400

	Version: 0.0.3.8 (02.10.2012)
	- Add wmnConvert function to convert write data into ushort[]

	Version: 0.0.3.7 (27.09.2012)
	- Published