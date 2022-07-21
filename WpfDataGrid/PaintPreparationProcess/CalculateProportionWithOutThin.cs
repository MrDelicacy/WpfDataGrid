using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDataGrid.PaintPreparationProcess
{
    class CalculateProportionWithOutThin : ICalculateProportion
    {
        public void Calculate(ObservableCollection<UsedInCalculationPaintComponent> components, PaintComponent thinnner)
        {
            float summ = 0;
            foreach(UsedInCalculationPaintComponent c in components)
                summ += c.AbsoluteAmount;

            foreach (UsedInCalculationPaintComponent c in components)
            {
                c.PercentageAmount = c.AbsoluteAmount / summ * 100;
                thinnner.PercentageAmount = thinnner.AbsoluteAmount / summ * 100;
            }
        }
    }
}
