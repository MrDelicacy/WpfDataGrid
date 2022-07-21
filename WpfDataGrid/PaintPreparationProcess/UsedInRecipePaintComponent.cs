namespace WpfDataGrid.PaintPreparationProcess
{
    /// <summary>
    /// компонент, используемый в рецепте краски
    /// </summary>
    public class UsedInRecipePaintComponent:PaintComponent
    {

        public UsedInRecipePaintComponent(string name):base()
        {
            this.ComponentName = name;
        }
        public UsedInRecipePaintComponent() : base()
        {

        }
    }
}
