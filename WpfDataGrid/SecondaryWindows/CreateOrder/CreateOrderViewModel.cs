using System.Collections.Generic;
using System.Data.Entity;
using System.Xml;
using WpfDataGrid.SQLiteOperation;
using WpfDataGrid.ViewModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace WpfDataGrid.SecondaryWindows.CreateOrder
{
    /// <summary>
    /// модель отображения окна создания заказа
    /// </summary>
    public class CreateOrderViewModel:ViewModelBase
    {
        private IEnumerable<Customer> customers;
        private IEnumerable<Manufacturer> manufacturers;
        private Manufacturer selectedManufacturer;
        private Customer selectedCustomer;
        private XmlElement selectedColorGroup;
        private int selectedManufacturerIndex;
        private int selectedColorGroupIndex;
        private string colorCode;
        private string colorName;
        private string comment;
        private float tare;
        private bool canCreateOrder;
        private RelayCommand addNewManufacturerCommand;
        private RelayCommand addNewCustomerCommand;

        /// <summary>
        /// список заказчиков
        /// </summary>
        public IEnumerable<Customer> Customers
        {
            get { return customers; }
            set
            {
                customers = value;
                OnPropertyChanged("Customers");
            }
        }
        /// <summary>
        /// выбранный заказчик
        /// </summary>
        [Required]
        public Customer SelectedCustomer
        {
            get { return selectedCustomer; }
            set 
            { 
                selectedCustomer = value;
                CanCreateOrder = ValidateFields();
                OnPropertyChanged("SelectedCustomer");
            }
        }
        /// <summary>
        /// список производителей
        /// </summary>
        public IEnumerable<Manufacturer> Manufacturers
        {
            get { return manufacturers; }
            set
            {
                manufacturers = value;
                OnPropertyChanged("Manufacturers");
            }
        }
        /// <summary>
        /// выбранный производитель
        /// </summary>
        [Required]
        public Manufacturer SelectedManufacturer
        {
            get { return selectedManufacturer; }
            set
            {
                selectedManufacturer = value;
                CanCreateOrder = ValidateFields();
                OnPropertyChanged("SelectedManufacturer");
            }
        }

        /// <summary>
        /// индекс выбранного производителя
        /// </summary>
        public int SelectedManufacturerIndex
        {
            get { return selectedManufacturerIndex; }
            set
            {
                selectedManufacturerIndex = value;
                OnPropertyChanged("SelectedManufacturerIndex");
            }
        }
        /// <summary>
        /// цветовая группа, используется для передачи значения в расчет рецепта
        /// </summary>
        [Required]
        public string ColorGroup { get; set; }
        /// <summary>
        /// выбранная цветовая группа
        /// </summary>
        [Required]
        public XmlElement SelectedColorGroup
        {
            get { return selectedColorGroup; }
            set
            {
                selectedColorGroup = value;
                if(selectedColorGroup!=null)
                    ColorGroup = selectedColorGroup.ChildNodes[0].InnerText;
                else
                    SelectedColorGroupIndex = -1;
                CanCreateOrder = ValidateFields();
                OnPropertyChanged("SelectedColorGroup");
            }
        }

        /// <summary>
        /// индекс выбранной цветовой группы
        /// </summary>
        public int SelectedColorGroupIndex
        {
            get { return selectedColorGroupIndex; }
            set
            {
                selectedColorGroupIndex = value;
                OnPropertyChanged("SelectedColorGroupIndex");
            }
        }
        [Required]
        public string ColorCode
        {
            get { return colorCode; }
            set
            {
                colorCode = value;
                CanCreateOrder = ValidateFields();
                OnPropertyChanged("ColorCode");
            }
        }

        public string ColorName
        {
            get { return colorName; }
            set
            {
                colorName = value;
                OnPropertyChanged("ColorName");
            }
        }

        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public float Tare
        {
            get { return tare; }
            set
            {
                tare = value;
                OnPropertyChanged("Tare");
            }
        }
        /// <summary>
        /// управляет активностью кнопки "Ok"
        /// </summary>
        public bool CanCreateOrder
        {
            get { return canCreateOrder; }
            set
            {
                canCreateOrder = value;
                OnPropertyChanged("CanCreateOrder");
            }
        }
        public CreateOrderViewModel()
        {
            using (ConnectToPrismDB db = new ConnectToPrismDB())
            {
                db.Customers.Load();
                customers = db.Customers.Local.ToBindingList().OrderBy(c=>c.CustomerName);
                db.Manufacturers.Load();
                manufacturers = db.Manufacturers.Local.ToBindingList().OrderBy(m => m.ManufacturerName);
            }
            SelectedManufacturerIndex = -1;
            SelectedColorGroupIndex = -1;
        }


        /// <summary>
        /// устанавливает данные заказа в модель отображения
        /// </summary>
        /// <param name="detail">детали заказа</param>
        public void SetOrderDetail(OrderDetail detail)
        {
            int i = 0;
            foreach(Manufacturer m in  Manufacturers)
            {
                if (m.ManufacturerName == detail.Manufacturer)
                    break;
                else i++;
            }
            SelectedManufacturerIndex = i;
            i = 0;
            using (var reader = XmlReader.Create("ColorNameIndexes.xml"))
            {
               while( reader.ReadToFollowing("color"))
                {
                    if (reader.GetAttribute(0) == detail.ColorGroup)
                        break;
                    else i++;
                }
            }
            SelectedColorGroupIndex = i;
            ColorName = detail.ColorName;
            ColorCode = detail.ColorCode;
        }

        /// <summary>
        /// команда добавления нового заказчика
        /// </summary>
        public RelayCommand AddNewCustomerCommand
        {
            get
            {
                return addNewCustomerCommand ??
                    (addNewCustomerCommand = new RelayCommand((o) =>
                    {
                        AddCustomerWindow addCustomer = new AddCustomerWindow(new Customer());
                        if (addCustomer.ShowDialog() == true)
                        {
                            using (ConnectToPrismDB db = new ConnectToPrismDB())
                            {
                                Customer customer = addCustomer.NewCustomer;
                                db.Customers.Add(customer);
                                db.SaveChanges();
                                db.Customers.Load();
                                Customers = db.Customers.Local.ToBindingList().OrderBy(c=> c.CustomerName);
                            }
                        }
                    }));
            }
        }
        /// <summary>
        /// команда добавления нового производителя
        /// </summary>
        public RelayCommand AddNewManufacturerCommand
        {
            get
            {
                return addNewManufacturerCommand ??
                    (addNewManufacturerCommand = new RelayCommand((o) =>
                    {
                        AddManufacturerWindow addManuf = new AddManufacturerWindow(new Manufacturer());
                        if (addManuf.ShowDialog() == true)
                        {
                            using (ConnectToPrismDB db = new ConnectToPrismDB())
                            {
                                Manufacturer manufacturer = addManuf.NewManufacturer;
                                db.Manufacturers.Add(manufacturer);
                                db.SaveChanges();
                                db.Manufacturers.Load();
                                Manufacturers = db.Manufacturers.Local.ToBindingList().OrderBy(m=>m.ManufacturerName);
                            }
                        }
                    }));
            }
        }
        /// <summary>
        /// проверяет заполнены ли необходимые поля
        /// </summary>
        /// <returns>true, если поля заполнены</returns>
        public bool ValidateFields()
        {
            var context = new ValidationContext(this);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, true);

        }
    }
}
