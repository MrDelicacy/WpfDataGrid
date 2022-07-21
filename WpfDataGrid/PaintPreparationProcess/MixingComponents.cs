using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// содержит информацию о компонентах краски и методы работы сними. Использует две коллекции компонентов, 
    /// для расчета пропорционального соотношения компонентов  и хранения текущей пропорции.
    /// </summary>
    public class MixingComponents
    {
        /// <summary>
        /// коллекция компонентов краски, используемая для расчета рецепта
        /// </summary>
        public ObservableCollection<UsedInCalculationPaintComponent> components { get; set; }
        /// <summary>
        /// коллекция компонентов краски, используемая в рецепте
        /// </summary>
        private List<UsedInRecipePaintComponent> recipeComponents { get; set; }
        /// <summary>
        /// расчитывает пропорцию компонентов
        /// </summary>
        private ICalculateProportion calculateProportion;
        /// <summary>
        /// разбавитель в расчете рецепта
        /// </summary>
        private UsedInCalculationPaintComponent thinnerCalculation;
        /// <summary>
        /// разбавитель в рецепте краски
        /// </summary>
        private UsedInRecipePaintComponent thinnerRecipe;

        public MixingComponents()
        {
            components = new ObservableCollection<UsedInCalculationPaintComponent>();
            recipeComponents = new List<UsedInRecipePaintComponent>();
            components.CollectionChanged += this.OnCollectionChanged;
            calculateProportion = new CalculateProportionWithOutThin();
            thinnerCalculation = new UsedInCalculationPaintComponent("Thinner");
            thinnerRecipe = new UsedInRecipePaintComponent("Thinner");
            thinnerCalculation.AddAmountNotify += ChangeAddAmountComponent;
        }
        /// <summary>
        /// вызывается при добавлении компонента краски в коллекцию "components"
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (UsedInCalculationPaintComponent newItem in e.NewItems)
                    {                     
                        newItem.AddAmountNotify += ChangeAddAmountComponent;
                        recipeComponents.Add( new UsedInRecipePaintComponent(newItem.ComponentName));
                    }
                }
            }
            if (e.OldItems != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (UsedInCalculationPaintComponent oldItem in e.OldItems)
                    {
                        oldItem.AddAmountNotify -= ChangeAddAmountComponent;
                        recipeComponents.RemoveAt(components.IndexOf(oldItem));
                    }
                }
            }
        }
        /// <summary>
        /// показывает компонент краски "разбавитель" в расчете рецепта краски
        /// </summary>
        public UsedInCalculationPaintComponent ThinnerCalculation 
        {
            get { return thinnerCalculation; }
            set { thinnerCalculation = value; } 
        }
        ///// <summary>
        ///// показывает "разбавитель" рецепта краски
        ///// </summary>
        //public UsedInRecipePaintComponent ThinnerRecipe
        //{
        //    get { return thinnerRecipe; }
        //    set { thinnerRecipe = value; }
        //}
        /// <summary>
        /// изменяет добавляемое количество в компонент, используемый в расчете рецепта
        /// и пересчитывает пропорцию
        /// </summary>
        /// <param name="sender">компонент</param>
        /// <param name="am">добавленное количество</param>
        private void ChangeAddAmountComponent(object sender, float am)
        {

                UsedInCalculationPaintComponent paintComponent = sender as UsedInCalculationPaintComponent;
                if (paintComponent.ComponentName == "Thinner")
                {
                    if (components.Count > 0)
                    {
                        thinnerCalculation.AbsoluteAmount = am + thinnerRecipe.AbsoluteAmount;
                        calculateProportion.Calculate(components, ThinnerCalculation);
                    }
                }
                else
                {
                    paintComponent.AbsoluteAmount = am + recipeComponents[components.IndexOf(paintComponent)].AbsoluteAmount;
                    calculateProportion.Calculate(components, ThinnerCalculation);
                }
        }
        public void SetRecipePaintComponents()
        {
            for(int i=0; i< recipeComponents.Count; i++)
            {
                recipeComponents[i].AbsoluteAmount = components[i].AbsoluteAmount;
                recipeComponents[i].PercentageAmount = components[i].PercentageAmount;
                components[i].AbsoluteAmount = 0;
                components[i].AddAmount = 0;
            }
            thinnerRecipe.AbsoluteAmount = thinnerCalculation.AbsoluteAmount;
            thinnerRecipe.PercentageAmount = thinnerRecipe.PercentageAmount;
            thinnerCalculation.AbsoluteAmount = 0;
            thinnerCalculation.AddAmount = 0;
        }
    }
}
