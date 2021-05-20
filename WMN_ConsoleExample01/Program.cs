using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using WagoModbusNet;

namespace WMN_ConsoleExample01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the test IP address...");
            string ipAddress = Console.ReadLine();
            ModbusMasterUdp mbTcp = new ModbusMasterTcp(ipAddress, 502);
            int count = 1000; // Number of modbus request to process
            Stopwatch sw1 = new Stopwatch();
            Console.WriteLine("Press any key to run test...");
            Console.ReadKey();
            Console.WriteLine("Test: " + count.ToString() + " ReadInputRegisters at IP address: " + ipAddress);
            sw1.Start();
            Console.WriteLine("\r\n");
            try
            {
                mbTcp.Connect();        
                ushort[] data;
                for (int i = 0; i < count; i++)
                {
                    data = mbTcp.ReadInputRegisters(0, 12288, 100);
                    Console.Write("#");
                }
                mbTcp.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - Message: " + ex.ToString());
            }
            Console.WriteLine("\r\n");
            sw1.Stop();
            Console.WriteLine("Typical cycletime: "+ sw1.ElapsedMilliseconds/count  + " mSec");
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
