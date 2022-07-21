namespace WpfDataGrid.PaintPreparationProcess
{

    /// <summary>
    /// использованный компонент
    /// </summary>
    internal class WastePaintComponent
    {
        private string componentName;
        private float componentCost;
        public WastePaintComponent(string cName, float cCost)
        {
            componentName = cName;
            componentCost = cCost;
        }
        public string ComponentName { get { return componentName; } }
        public float ComponentCost { get { return componentCost; } }
    }
}
