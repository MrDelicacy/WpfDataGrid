namespace WpfDataGrid.SQLiteOperation
{
    public class TestWeightInfo
    {
        public int Id { get; set; }
        public int IterationId { get; set; }
        /// <summary>
        /// вес тары
        /// </summary>
        public float Tare { get; set; }

        /// <summary>
        /// показывает вес тары и смеси предыдущей итерации
        /// </summary>
        public float PreviousIterationBrutto { get; set; }
        /// <summary>
        /// смесь + тара до теста
        /// </summary>
        public float TestBefore { get; set; }
        /// <summary>
        /// смесь + тара после теста
        /// </summary>
        public float TestAfter { get; set; }
        /// <summary>
        /// количество смеси, потраченное на тест
        /// </summary>
        public float Test { get; set; }

        /// <summary>
        /// Id заказа
        /// </summary>
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
    }
}
