using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDataGrid.PaintPreparationProcess
{
    interface ICalculateProportion
    {
        void Calculate(ObservableCollection<UsedInCalculationPaintComponent> components, PaintComponent thinnner);
    }
}
