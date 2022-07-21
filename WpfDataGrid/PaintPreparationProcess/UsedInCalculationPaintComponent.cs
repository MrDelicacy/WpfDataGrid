namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// компонент, используемый в расчете рецепта краски
    /// </summary>
    public class UsedInCalculationPaintComponent:PaintComponent
    {

        private float addAmount;
        private float percentageAmount;


        public delegate void AddAmountHandler(object sender, float addAmount);


        /// <summary>
        /// происходит при изменении добавляемого количества
        /// </summary>
        public event AddAmountHandler AddAmountNotify;


        public UsedInCalculationPaintComponent()
        {
            addAmount = 0;
        }
        public UsedInCalculationPaintComponent(string name) : base()
        {
            ComponentName = name;
            addAmount = 0;
            percentageAmount = 0;
        }


        /// <summary>
        /// добавленное количество
        /// </summary>
        public float AddAmount
        {
            get { return addAmount; }
            set
            { 
                addAmount = value;
                AddAmountNotify?.Invoke(this, addAmount);
                OnPropertyChanged("AddAmount");
            }
        }


        /// <summary>
        /// процентное соотношение
        /// </summary>
        public float PercentageAmount
        {
            get { return percentageAmount; }
            set
            {
                percentageAmount = value;
                OnPropertyChanged("PercentageAmount");
            }
        }
    }
}
