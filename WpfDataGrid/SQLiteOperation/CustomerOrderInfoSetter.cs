using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using WpfDataGrid.ViewModel;

namespace WpfDataGrid.SQLiteOperation
{
    internal class CustomerOrderInfoSetter
    {
        private static string conn;
        public CustomerOrderInfoSetter()
        {

        }

        /// <summary>
        /// возвращает  коллекцию строк с информацией о заказах клиентов
        /// </summary>
        /// <param name="orderId">id заказа</param>
        /// <param name="customer">имя заказчика</param>
        /// <param name="manufacturer">производитель</param>
        /// <param name="colorgroup">цветовая группа</param>
        /// <param name="colorcode">код краски</param>
        /// <param name="rowsсount">количество возвращаемых строк</param>
        public static List<CustomerOrderInfo> FastSearchQuery(string orderId, string customer, string manufacturer, string colorgroup, string colorcode, int rowsсount)
        {

            conn = "Data Source=PrismDB.db";
            List<CustomerOrderInfo> orderInfos = new List<CustomerOrderInfo>();

            string sqlQuery = @"SELECT CustomerOrders.Id, CustomerOrders.CustomerId, CustomerOrders.OrderDate, CustomerOrders.RelatedOrderId, CustomerOrders.Status,
	                            Customers.Id, Customers.CustomerName, OrderDetails.Manufacturer, OrderDetails.ColorGroup,
	                            OrderDetails.ColorCode, OrderDetails.ColorName, OrderDetails.Comment
                                FROM [CustomerOrders], Customers, OrderDetails
                                WHERE CustomerOrders.Id=OrderDetails.CustomerOrderId 
                                AND CustomerOrders.CustomerId=Customers.Id";
            if (!String.IsNullOrEmpty(orderId))
                sqlQuery += @" AND CustomerOrders.Id=@orderId";

            if (!String.IsNullOrEmpty(customer))
                sqlQuery += @" AND Customers.CustomerName=@customerName";

            if (!String.IsNullOrEmpty(manufacturer))
                sqlQuery += @" AND OrderDetails.Manufacturer=@manufacturer";

            if (!String.IsNullOrEmpty(colorgroup))
                sqlQuery += @" AND OrderDetails.ColorGroup=@colorgroup";

            if (!String.IsNullOrEmpty(colorcode))
                sqlQuery += @" AND OrderDetails.ColorCode=@colorcode";

            sqlQuery += @" ORDER BY CustomerOrders.Id DESC LIMIT " + rowsсount.ToString();


            using (var connection = new SQLiteConnection(conn))
            {
                DataTable dt = new DataTable();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, connection);
                    SQLiteCommandBuilder cmb = new SQLiteCommandBuilder(adapter);
                    adapter.SelectCommand.Parameters.Add("@orderId", DbType.Int32).Value = orderId;
                    adapter.SelectCommand.Parameters.Add("@customerName", DbType.String).Value = customer;
                    adapter.SelectCommand.Parameters.Add("@manufacturer", DbType.String).Value = manufacturer;
                    adapter.SelectCommand.Parameters.Add("@colorgroup", DbType.String).Value = colorgroup;
                    adapter.SelectCommand.Parameters.Add("@colorcode", DbType.String).Value = colorcode;

                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dt = ds.Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        CustomerOrderInfo oRow = new CustomerOrderInfo();
                        oRow.OrderDetail = new OrderDetail();
                        oRow.OrderDetail.CustomerOrderId = Convert.ToInt32(dt.Rows[i].ItemArray[0]);
                        oRow.CustomerId= Convert.ToInt32(dt.Rows[i].ItemArray[1]);
                        oRow.OrderDate = DateTime.Parse(dt.Rows[i].ItemArray[2].ToString());
                        oRow.RelatedOrderId= Convert.ToInt32(dt.Rows[i].ItemArray[3]);
                        oRow.Status=dt.Rows[i].ItemArray[4].ToString();
                        oRow.CustomerId=Convert.ToInt32(dt.Rows[i].ItemArray[5]);
                        oRow.CustomerName = dt.Rows[i].ItemArray[6].ToString();
                        oRow.OrderDetail.Manufacturer = dt.Rows[i].ItemArray[7].ToString();
                        oRow.OrderDetail.ColorGroup = dt.Rows[i].ItemArray[8].ToString();
                        oRow.OrderDetail.ColorCode = dt.Rows[i].ItemArray[9].ToString();
                        oRow.OrderDetail.ColorName = dt.Rows[i].ItemArray[10].ToString();
                        oRow.OrderDetail.Comment = dt.Rows[i].ItemArray[11].ToString();
                        orderInfos.Add(oRow);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return orderInfos;
        }
    }
}
