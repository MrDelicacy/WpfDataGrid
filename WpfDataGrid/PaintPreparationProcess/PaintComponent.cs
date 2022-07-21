using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// Описывает компонент приготавливаемой краски
    /// </summary>
    public class PaintComponent:INotifyPropertyChanged
    {

        private string componentName;
        private float absoluteAmount;


        public PaintComponent()
        {
            componentName = "";
            absoluteAmount = 0;
        }


        /// <summary>
        /// название компонента
        /// </summary>
        public string ComponentName 
        { 
            get { return componentName; }
            set { componentName = value; }
        }


        /// <summary>
        /// абсолютное количество
        /// </summary>
        public float AbsoluteAmount
        {
            get { return absoluteAmount; }
            set 
            { 
                absoluteAmount = value;
                OnPropertyChanged("AbsoluteAmount");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
