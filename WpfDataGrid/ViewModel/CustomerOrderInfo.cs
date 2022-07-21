using System;
using WpfDataGrid.SQLiteOperation;

namespace WpfDataGrid.ViewModel
{
    /// <summary>
    /// предоставляет информацию о заказах в таблице последних заказов
    /// </summary>
    internal class CustomerOrderInfo
    {
        /// <summary>
        /// детали заказа
        /// </summary>
        public OrderDetail OrderDetail { get; set; }

        /// <summary>
        /// Id заказчика
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// имя заказчика
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// дата заказа
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Id связанного заказа
        /// </summary>
        public int RelatedOrderId { get; set; }
        /// <summary>
        /// статус заказа
        /// </summary>
        public string Status { get; set; }

    }
}
