namespace WpfDataGrid.SQLiteOperation
{
    public class OrderDetail
    {
        public int Id { get; set; }
        /// <summary>
        /// номер заказа
        /// </summary>
        public int CustomerOrderId { get; set; }

        /// <summary>
        /// производитель
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// цветовая группа
        /// </summary>
        public string ColorGroup { get; set; }

        /// <summary>
        /// код цвета
        /// </summary>
        public string ColorCode { get; set; }

        /// <summary>
        /// название цвета
        /// </summary>
        public string ColorName { get; set; }

        /// <summary>
        /// комментарий
        /// </summary>
        public string Comment { get; set; }

        public CustomerOrder Order { get; set; }
    }
}
