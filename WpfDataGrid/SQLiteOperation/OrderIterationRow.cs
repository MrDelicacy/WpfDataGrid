namespace WpfDataGrid.SQLiteOperation
{
    public class OrderIterationRow
    {
        public int Id {get;set;}

        /// <summary>
        /// номер итерации
        /// </summary>
        public int IterationId { get; set; }

        /// <summary>
        /// название компонента
        /// </summary>
        public string ComponentName { get; set; }

        /// <summary>
        /// добавленное количество
        /// </summary>
        public float AddAmount { get; set; }

        /// <summary>
        /// процент компонента всмеси
        /// </summary>
        public float AbsoluteAmount { get; set; }

        /// <summary>
        /// показывает источник изменений в расчете рецепта
        /// </summary>
        public int IndexSourceChanges { get; set; }

        /// <summary>
        /// затраты компонента на тест
        /// </summary>
        public float Test { get; set; }

        /// <summary>
        /// Id заказа
        /// </summary>
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
    }
}
