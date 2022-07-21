using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using WpfDataGrid.PaintPreparationProcess;

namespace WpfDataGrid.SecondaryWindows
{
    /// <summary>
    /// Логика взаимодействия для RecipeCatalog.xaml
    /// </summary>
    public partial class RecipeCatalogWindow : Window
    {
        public PaintRecipe recipe;
        public PaintRecipe newRecipe { get; set; }
        public RecipeCatalogWindow()
        {
            InitializeComponent();
            LoadRecipeList();
        }
        public void LoadRecipeList()
        {
            //string fullPath = Path.Get("RecipeCatalog.xml");
            XDocument xdoc = XDocument.Load("RecipeCatalog.xml");

            var catalog = xdoc.Element("Catalog")?
                .Elements("Recipe")
                .Select(r => new
                {
                    RecipeNumber = r.Element("RecipeNumber")?.Value,
                    Manufacturer = r.Element("Manufacturer")?.Value,
                    ColorGroup = r.Element("ColorGroup")?.Value,
                    ColorCode = r.Element("ColorCode")?.Value
                });
            if (catalog !=null)
            {
                recipeList.ItemsSource = catalog;
            }
        }
        public void LoadRecipeList(string param)
        {

            XDocument xdoc = XDocument.Load("RecipeCatalog.xml");

            var result1 = xdoc.Element("Catalog")?
                .Elements("Recipe").Where(r=>r.Element("RecipeNumber")?.Value==param)
                .Select(r => new
                {
                    RecipeNumber = r.Element("RecipeNumber")?.Value,
                    Manufacturer = r.Element("Manufacturer")?.Value,
                    ColorGroup = r.Element("ColorGroup")?.Value,
                    ColorCode = r.Element("ColorCode")?.Value
                });
            var result2 = xdoc.Element("Catalog")?
                .Elements("Recipe").Where(r => r.Element("Manufacturer")?.Value == param)
                .Select(r => new
                {
                    RecipeNumber = r.Element("RecipeNumber")?.Value,
                    Manufacturer = r.Element("Manufacturer")?.Value,
                    ColorGroup = r.Element("ColorGroup")?.Value,
                    ColorCode = r.Element("ColorCode")?.Value
                });
            var result3 = xdoc.Element("Catalog")?
                .Elements("Recipe").Where(r => r.Element("ColorGroup")?.Value == param)
                .Select(r => new
                {
                    RecipeNumber = r.Element("RecipeNumber")?.Value,
                    Manufacturer = r.Element("Manufacturer")?.Value,
                    ColorGroup = r.Element("ColorGroup")?.Value,
                    ColorCode = r.Element("ColorCode")?.Value
                });
            var result4 = xdoc.Element("Catalog")?
                .Elements("Recipe").Where(r => r.Element("ColorCode")?.Value == param)
                .Select(r => new
                {
                    RecipeNumber = r.Element("RecipeNumber")?.Value,
                    Manufacturer = r.Element("Manufacturer")?.Value,
                    ColorGroup = r.Element("ColorGroup")?.Value,
                    ColorCode = r.Element("ColorCode")?.Value
                });
            result1 = result1.Union(result2);
            result1 = result1.Union(result3);
            result1 = result1.Union(result4);
            if (result1 != null)
            {
                recipeList.ItemsSource = result1;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_SearchParameter.Text))
                LoadRecipeList(txt_SearchParameter.Text);
        }
        private void GetPaintRecipe()
        {
            recipe = new PaintRecipe();
            char[] ch = new char[] { ',', '=' };
            string[] str = recipeList.SelectedItem.ToString().Trim(' ').Split(ch);
            string recipeNumber = str[1].Trim(' ');
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("RecipeCatalog.xml");
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                // обход всех узлов в корневом элементе
                foreach (XmlElement xnode in xRoot)
                {

                    // обходим все дочерние узлы элемента Recipe
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        if (childnode.Name == "RecipeNumber")
                        {
                            if (childnode.InnerText == recipeNumber)
                            {
                                for (int i = 0; i < xnode.ChildNodes[4].ChildNodes.Count; i += 2)
                                {
                                    UsedInRecipePaintComponent comp = new UsedInRecipePaintComponent(xnode.ChildNodes[4].ChildNodes[i].InnerText);
                                    comp.AbsoluteAmount = float.Parse(xnode.ChildNodes[4].ChildNodes[i + 1].InnerText);
                                    if (comp.ComponentName == "pазб." || comp.ComponentName == "thin.")
                                        recipe.Thinner = comp;
                                    else
                                        recipe.RecipeComponents.Add(comp);
                                }
                            }
                        }
                    }

                }
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GetPaintRecipe();
            UseProportionWindow window = new UseProportionWindow(recipe,true);
            if (window.ShowDialog() == true)
            {
                newRecipe = window.newRecipe;
                this.DialogResult = true;
            }
        }
    }
}
