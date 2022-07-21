using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfDataGrid.SQLiteOperation;

namespace WpfDataGrid.PaintPreparationProcess
{
    internal class MixingComponentsTabBase:INotifyPropertyChanged
    {

        private OrderDetail orderDetail;
        public OrderDetail OrderDetail
        {
            get { return orderDetail; }
            set { orderDetail = value; }
        }
        /// <summary>
        /// заказчик
        /// </summary>
        public string CustomerName { get; set; }


        public int iterationId;
        /// <summary>
        /// итерация
        /// </summary>
        public int IterationId
        {
            get { return iterationId; }
            set
            {
                iterationId = value;
                OnPropertyChanged("IterationId");
            }
        }


        public MixingComponentsTabBase()
        {
            IterationId = 0;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
