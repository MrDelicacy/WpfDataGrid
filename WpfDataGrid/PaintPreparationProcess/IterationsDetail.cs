using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WpfDataGrid.SQLiteOperation;

namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// предоставляет информацию об пропорции компонентов на каждой итерации
    /// </summary>
    internal class IterationsDetail
    {
        /// <summary>
        /// возвращает таблицу итераций для заказа с конкретным id
        /// </summary>
        public static async Task<DataTable> GetIterationDetailTableAsync(ConnectToPrismDB db, int orderId)
        {
            DataTable tb = new DataTable();

            IEnumerable<OrderIterationRow> Rows = db.OrderIterationRows.Where(o => o.CustomerOrderId == orderId).ToList();
            int iterationsCount = Rows.Last().IterationId;
            tb.Columns.Add("итерация", typeof(int));
            foreach (OrderIterationRow o in Rows.Where(o => o.IterationId == iterationsCount).ToList())
                    tb.Columns.Add(o.ComponentName, typeof(float));

            
            for(int i=iterationsCount;i>=0;i--)
            {
                DataRow newRow = tb.NewRow();
                float summ = 0;

                foreach (OrderIterationRow o in Rows.Where(o => o.IterationId == i).ToList())
                {
                    if (o.ComponentName != "Thinner")
                        summ += o.AbsoluteAmount;
                }

                newRow["итерация"] = i;
                int j = 1;
                foreach (OrderIterationRow o in Rows.Where(o => o.IterationId == i).ToList())
                {
                    newRow[j] = o.AbsoluteAmount / summ * 100;
                    j++;
                }

                tb.Rows.Add(newRow);
            }
            await Task.Delay(0);
            return tb;
        }
    }
}
