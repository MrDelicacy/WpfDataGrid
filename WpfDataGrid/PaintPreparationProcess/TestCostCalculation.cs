using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// пересчитывает абсолютное количество компонентов краски с учетом затрат на тест
    /// </summary>
    static class TestCostCalculation
    {

        /// <summary>
        /// считает, сколько конкретного компонента потрачено на тест
        /// </summary>
        /// <param name="components">миксы</param>
        /// <param name="thinner">разбавитель</param>
        /// <param name="testAmount">количество смеси, потраченное на тест</param>
        public static List<WastePaintComponent> CalculateWasteComponents(ObservableCollection<UsedInCalculationPaintComponent> components, PaintComponent thinner, float testAmount)
        {
            List<WastePaintComponent>wastePaintComponents = new List<WastePaintComponent>();

            //устанавливаем в абсолютное количество всех компонентов абсолютное количество разбавителя
            float absoluteSumm = thinner.AbsoluteAmount;
            //добавляем абсолютное количество всех остальных компонентов краски
            foreach (UsedInCalculationPaintComponent c in components)
                absoluteSumm += c.AbsoluteAmount;
            foreach (UsedInCalculationPaintComponent c in components)
            {
                //отнять из абсолютного количества затраты на тест
                float test = c.AbsoluteAmount / absoluteSumm * testAmount;
                c.AbsoluteAmount -= test;
                wastePaintComponents.Add(new WastePaintComponent(thinner.ComponentName, test));
            }
            float testThinner = thinner.AbsoluteAmount / absoluteSumm * testAmount;
            thinner.AbsoluteAmount -= testThinner;
            wastePaintComponents.Add(new WastePaintComponent(thinner.ComponentName, testThinner));
            return wastePaintComponents;
        }
    }
}
