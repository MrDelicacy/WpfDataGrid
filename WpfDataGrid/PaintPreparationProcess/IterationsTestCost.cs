using System.Collections.Generic;
using System.Linq;
using System.Data;
using WpfDataGrid.SQLiteOperation;
using System.Threading.Tasks;

namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// предоставляет информацию о добавленных и затраченных на тест компонентах
    /// </summary>
    internal class IterationsTestCost
    {
        /// <summary>
        /// возвращает таблицу добавленных и затраченных на тест компонентов для заказа с конкретным id
        /// </summary>
        public static async Task<DataTable> GetTestCostTableAsync(ConnectToPrismDB db, int orderId)
        {
            DataTable tb = new DataTable();

            IEnumerable<OrderIterationRow> itRows = db.OrderIterationRows.Where(o => o.CustomerOrderId == orderId).ToList();

            tb.Columns.Add("итерация", typeof(int));
            tb.Columns.Add("компонент", typeof(string));
            tb.Columns.Add("добавтенное количество", typeof(float));
            tb.Columns.Add("затраты на тест", typeof(float));
            tb.Columns.Add("абсолютное количество", typeof(float));

            foreach (OrderIterationRow itR in itRows)
            {
                DataRow newRow = tb.NewRow();
                newRow[0] = itR.IterationId;
                newRow[1] = itR.ComponentName;
                newRow[2] = itR.AddAmount;
                newRow[3] = itR.Test;
                newRow[4] = itR.AbsoluteAmount;
                tb.Rows.Add(newRow);
            }
            await Task.Delay(0);
            return tb;
        }
    }
}
