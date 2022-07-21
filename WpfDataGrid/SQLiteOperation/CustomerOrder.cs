using System;
using System.Collections.Generic;

namespace WpfDataGrid.SQLiteOperation
{
    /// <summary>
    /// заказ клиента
    /// </summary>
    public class CustomerOrder
    {
        /// <summary>
        /// Id заказа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id заказчика
        /// </summary>
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

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

        public List<WorkProcessAction> WorkProcessActions { get; set; }
        public List<UndoWorkProcessAction> UndoWorkProcessActions { get; set; }
        public List<OrderIterationRow> OrderIterations { get; set; }
        public OrderDetail OrderDetail { get; set; }
        public List<TestWeightInfo> TestWeightInfoes { get; set; }
        public CompletedWork CompletedWork { get; set; }
    }
}
