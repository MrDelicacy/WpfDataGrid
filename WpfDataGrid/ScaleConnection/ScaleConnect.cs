using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;

namespace WpfDataGrid.ScaleConnection
{
    internal class ScaleConnect
    {
        private bool _continue = false;
        private readonly SerialPort _serialPort;

        IUpgradeString upgradeString;
        public delegate void ScaleStringHandler(string scaleStr);
        public event ScaleStringHandler ScaleStringNotify;
        public ScaleConnect(string portName)
        {
            try
            {
                //новый объект SerialPort с настройками по умолчанию.
                _serialPort = new SerialPort();
                _serialPort.PortName = portName;
                _serialPort.BaudRate = 1200;
                _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), "Odd", true);
                _serialPort.DataBits = 7;
                _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One", true);
                _serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), "None", true);
                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;
            }
            catch (Exception ex)
            {
                MessageBox.Show("конструктор"+ex.Message);
            }
            upgradeString = new StandartUpgradeString();
        }
        public async void StartConnection()
        {
            try
            {

                if (!_serialPort.IsOpen) { _serialPort.Open(); }

                _continue = true;
                await Task.Run(()=>Read());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void StopConnection()
        {
            try
            {
                if (_serialPort != null)
                {
                    _continue = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void CloseConnection()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
        }
        private void Read()
        {
            while (_continue)
            {
                try
                {
                    if (_serialPort != null)
                    {
                        string mess = _serialPort.ReadLine();
                        string resultTxt = upgradeString.Upgrade(mess);
                        ScaleStringNotify?.Invoke(resultTxt);
                    }
                }
                catch (TimeoutException) { }
                {
                   // MessageBox.Show(ex.Message);
                }
            }
            return;
        }

    }
}
