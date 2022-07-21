using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WpfDataGrid.SQLiteOperation
{
    public class Customer
    {

        public int Id { get; set; }

        /// <summary>
        /// код заказчика, если имеется
        /// </summary>
        public int CustomerCode { get; set; }
        /// <summary>
        /// имя заказчика
        /// </summary>
        [Required]
        public string CustomerName { get; set; }
        /// <summary>
        /// номер телефона
        /// </summary>
        [Phone]
        public string PhoneNumber { get; set; }

        public List<CustomerOrder> CustomerOrders { get; set; }
        public override string ToString()
        {
            return CustomerName;
        }
    }
}
