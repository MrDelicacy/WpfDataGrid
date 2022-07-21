using System.Collections.ObjectModel;

namespace WpfDataGrid.PaintPreparationProcess
{
    public class PaintRecipe
    {
       public PaintRecipe()
        {
            RecipeComponents = new ObservableCollection<UsedInRecipePaintComponent>();
            Thinner = new UsedInRecipePaintComponent("Thinner");
        }
        public ObservableCollection<UsedInRecipePaintComponent> RecipeComponents { get; set; }
        public UsedInRecipePaintComponent Thinner { get; set; }
    }
}
