using System;
using System.Collections.Generic;
using System.Windows;
using WpfDataGrid.ScaleConnection;

namespace WpfDataGrid.ViewModel
{
    internal class RightPanelElementsViewModel:ViewModelBase
    {
        protected ScaleConnect scaleConnect = null;
        private string selectedPort;
        private string scaleString;
        private string scaleLable;
        private string scaleConnectionButtonText;
        private bool scaleButtonEnabled;
        private bool buttonScaleConnectionState;
        private bool stepBackActivity;
        private bool stepForwardActivity;
        private bool cancelIterationActivity;

        private RelayCommand scaleConnectionCommand;

        public RightPanelElementsViewModel()
        {
            Ports = new List<string>();
            AvailablePorts.SetPortName(Ports);

            scaleConnectionButtonText = "подключить";
            scaleLable = "весы отключены";
            ScaleString = "0";

            StepBackActivity = false;
            StepForwardActivity = false;
            CancelIterationActivity = false;
        }


        #region подключение весов
        /******************************************************************/


        /// <summary>
        /// отображает выбранный COM порт
        /// </summary>
        public string SelectedPort
        {
            get { return selectedPort; }
            set
            {
                selectedPort = value;
                ScaleButtonEnabled = true;
                OnPropertyChanged("SelectedPort");
            }
        }
        /// <summary>
        /// список доступных COM портов
        /// </summary>
        public List<string> Ports { get; set; }


        /// <summary>
        /// команда подключения/отключения весов
        /// </summary>
        public RelayCommand ScaleConnectionCommand
        {
            get
            {
                return scaleConnectionCommand ??
                    (scaleConnectionCommand = new RelayCommand(obj =>
                    {
                        if (selectedPort != null)
                        {
                            if (ButtonScaleConnectionState)
                            {
                                scaleConnect = new ScaleConnect(selectedPort);
                                scaleConnect.StartConnection();
                                scaleConnect.ScaleStringNotify += SetScaleString;
                                ScaleConnectionButtonText = "отключить";
                                ScaleLable = "весы подключены";
                            }
                            else
                            {
                                scaleConnect.StopConnection();
                                scaleConnect.ScaleStringNotify -= SetScaleString;
                                ScaleConnectionButtonText = "подключить";
                                ScaleLable = "весы отключены";
                            }
                        }
                        else MessageBox.Show("Выберите COM порт");
                    }));
            }
        }

        /// <summary>
        /// устанавливает строку полученую от весов в текстбокс на правой панели
        /// </summary>
        protected void SetScaleString(string str)
        {
            try
            {
                //значение устанавливаемое в readout должно поступать с весов
                ScaleString = str;
                Readout.ReadoutAmount = str;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// строка, получаемая с весов
        /// </summary>
        public string ScaleString
        {
            get { return scaleString; }
            set
            {
                scaleString = value;
                OnPropertyChanged("ScaleString");
            }
        }


        /// <summary>
        /// управляет текстом о состоянии подключения весов
        /// </summary>
        public string ScaleLable
        {
            get { return scaleLable; }
            set
            {
                scaleLable = value;
                OnPropertyChanged("ScaleLable");
            }
        }


        /// <summary>
        /// отображает текст на кнопке подключения весов
        /// </summary>
        public string ScaleConnectionButtonText
        {
            get { return scaleConnectionButtonText; }
            set
            {
                scaleConnectionButtonText = value;
                OnPropertyChanged("ScaleConnectionButtonText");
            }
        }


        /// <summary>
        /// управляет активностью кнопки подключения весов
        /// </summary>
        public bool ScaleButtonEnabled
        {
            get { return scaleButtonEnabled; }
            set
            {
                scaleButtonEnabled = value;
                OnPropertyChanged("ScaleButtonEnabled");
            }
        }


        /// <summary>
        /// показывает состояние подключения весов
        /// </summary>
        public bool ButtonScaleConnectionState
        {
            get { return buttonScaleConnectionState; }
            set
            {
                buttonScaleConnectionState = value;
                OnPropertyChanged("ButtonScaleConnectionState");
            }
        }
        #endregion


        /// <summary>
        /// устанавливает активность кнопки "шаг назад"
        /// </summary>
        public bool StepBackActivity
        {
            get { return stepBackActivity; }
            set
            {
                stepBackActivity = value;
                OnPropertyChanged("StepBackActivity");
            }

        }


        /// <summary>
        /// устанавливает активность кнопки "шаг вперед"
        /// </summary>
        public bool StepForwardActivity
        {
            get { return stepForwardActivity; }
            set
            {
                stepForwardActivity = value;
                OnPropertyChanged("StepForwardActivity");
            }
        }


        /// <summary>
        /// устанавливает активность кнопки "отмена итерация"
        /// </summary>
        public bool CancelIterationActivity
        {
            get { return cancelIterationActivity; }
            set
            {
                cancelIterationActivity = value;
                OnPropertyChanged("CancelIterationActivity");
            }
        }

    }
}
