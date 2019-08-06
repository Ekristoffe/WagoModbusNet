----------------------------------------------------------------------------
--- WagoModbusNet - README.txt                                       -------
----------------------------------------------------------------------------

Description:
    WagoModbusNet provide easy to use Modbus-Master classes for TCP, UDP, RTU and ASCII.
    WagoModbusNet based on dot.net framework 2.0.
    WagoModbusNet.Masters do not throw any exception, all function returns a struct of type "wmnRet".
    For a list of supported function codes see "enum ModbusFunctionCodes".     

Author: WAGO Kontakttechnik GmbH & Co.KG

Typical pitfall:
    With 750-3xx or 750-8xx
    You dial with a WAGO Ethernet controller. Try to set outputs - but nothing happens!
    WAGO Ethernet controller provide a "owner" policy for physical outputs.
    The "owner" could be CoDeSys-Runtime or Fieldbus-Master.
    Every time you download a PLC program the CoDeSys-Runtime becomes "owner" of physical outputs.
    Use tool "WAGO Ethernet Settings" and do "Reset File System", it is the easiest way to assign Modbus-Master as "owner".
    Alternatively you can "Login" with CoDeSys-IDE and perform "Reset(original)".

    With 750-8xxx
    By default there is no Modbus server available, you must use e!Cockpit to create your own Modbus server.

License:
    Copyright (c) 2012 WAGO Kontakttechnik GmbH & Co.KG

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