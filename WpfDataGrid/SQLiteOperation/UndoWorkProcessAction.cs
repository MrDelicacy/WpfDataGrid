namespace WpfDataGrid.SQLiteOperation
{
    public class UndoWorkProcessAction
    {
        public int Id { get; set; }
        /// <summary>
        /// номер итерации
        /// </summary>
        public int IterationId { get; set; }
        /// <summary>
        /// показывает источник изменений в расчете рецепта
        /// </summary>
        public int IndexSourceChanges { get; set; }
        /// <summary>
        /// изменения в  в расчете рецепта
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// Id заказа
        /// </summary>
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
    }
}
