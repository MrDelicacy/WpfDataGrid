using System;
using System.Collections.ObjectModel;


namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// считает пропорцию компонентов
    /// </summary>
    public class CalculateProportion
    {
        public static void Calculate(ObservableCollection<UsedInCalculationPaintComponent> components, UsedInCalculationPaintComponent thinner,out float totalWeight)
        {
            float summ = 0;
            foreach (UsedInCalculationPaintComponent c in components)
                summ += c.AbsoluteAmount;

            foreach (UsedInCalculationPaintComponent c in components)
            {
                c.PercentageAmount = c.AbsoluteAmount / summ * 100;
            }

            thinner.PercentageAmount = thinner.AbsoluteAmount / summ * 100;
            totalWeight = (float)Math.Round(summ+thinner.AbsoluteAmount,2);
        }
        public static PaintRecipe Calculate(PaintRecipe standartRecipe, float totalWeight)
        {
            PaintRecipe newRecipe = new PaintRecipe();
            double summ = 0;
            double abs;
            foreach (UsedInRecipePaintComponent c in standartRecipe.RecipeComponents)
                summ += c.AbsoluteAmount;

            foreach (UsedInRecipePaintComponent c in standartRecipe.RecipeComponents)
            {
                abs = c.AbsoluteAmount / summ * totalWeight;
                newRecipe.RecipeComponents.Add(new UsedInRecipePaintComponent(c.ComponentName) { AbsoluteAmount = (float)Math.Round(abs, 2) });                
            }

            abs= standartRecipe.Thinner.AbsoluteAmount / summ * totalWeight;
            newRecipe.Thinner.AbsoluteAmount = (float)Math.Round(abs, 2);

            return newRecipe;
        }
    }
}
