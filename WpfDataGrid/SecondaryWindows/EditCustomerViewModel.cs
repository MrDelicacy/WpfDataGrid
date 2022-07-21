using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WpfDataGrid.SQLiteOperation;
using WpfDataGrid.ViewModel;

namespace WpfDataGrid.SecondaryWindows
{
    internal class EditCustomerViewModel : ViewModelBase
    {
        private IEnumerable<Customer> customers;
        private Customer selectedCustomer;
        private RelayCommand saveCustomerChanges;
        public EditCustomerViewModel()
        {
            using (ConnectToPrismDB db = new ConnectToPrismDB())
            {
                db.Customers.Load();
                customers = db.Customers.Local.ToBindingList().OrderBy(c => c.CustomerName);
                SelectedCustomer = customers.FirstOrDefault();
            }
        }
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
        /// проверяет, есть ли изменения в данных клиента, если есть вносит изменения в бд
        /// </summary>
        public RelayCommand SaveCustomerChanges
        {
            get
            {
                return saveCustomerChanges ??
                    (saveCustomerChanges = new RelayCommand((o) =>
                    { 
                        using (ConnectToPrismDB db = new ConnectToPrismDB())
                        {
                            foreach (var cust in Customers)
                            {
                                var result = db.Customers.Where(c => c.Id == cust.Id).FirstOrDefault();
                                if (result.CustomerName != cust.CustomerName || result.PhoneNumber != cust.PhoneNumber || result.CustomerCode != cust.CustomerCode)
                                {
                                    result.CustomerName = cust.CustomerName;
                                    result.PhoneNumber = cust.PhoneNumber;
                                    result.CustomerCode = cust.CustomerCode;
                                }
                            }
                            db.SaveChanges();
                        }
                    }));
            }
        }


        public Customer SelectedCustomer
        {
            get { return selectedCustomer; }
            set
            {
                selectedCustomer = value;
                OnPropertyChanged("SelectedCustomer");
            }
        }

    }
}
