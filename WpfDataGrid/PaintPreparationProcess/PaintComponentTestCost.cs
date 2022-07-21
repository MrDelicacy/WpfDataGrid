using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDataGrid.PaintPreparationProcess
{
    public class PaintComponentTestCost
    {
        /// <summary>
        ///  коллекция компонентов краски и их затраты на тест в конкретной итерации
        /// </summary>
        public List<WastePaintComponent> wasteComponents;
        public PaintComponentTestCost()
        {
            wasteComponents = new List<WastePaintComponent>();
        }
        /// <summary>
        /// добавляет коипонент в коллекцию компонентов
        /// </summary>
        /// <param name="cName">
        /// название компонента
        /// </param>
        /// <param name="cCost">
        /// количество, затраченное на тест
        /// </param>
        public void AddWastePaintComponent(string cName, float cCost)
        {
            wasteComponents.Add(new WastePaintComponent(cName, cCost));
        }
    }
}
