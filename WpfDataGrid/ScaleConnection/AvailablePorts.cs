using System.Collections.Generic;
using System.IO.Ports;

namespace WpfDataGrid.ScaleConnection
{
    internal static class AvailablePorts
    {
        public static void SetPortName(List<string> ports)
        {
            foreach (string s in SerialPort.GetPortNames())
            {
                ports.Add(s);
            }
        }
    }
}
