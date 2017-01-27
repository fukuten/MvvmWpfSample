using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmWpfSample
{
    #region Model

    public class Customer
    {
        public int CustomerID { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public ObservableCollection<string> Hobbies { get; set; }

        public string NewHobby { get; set; }
    }

    public class CustomerRepository
    {
        private readonly List<Customer> _customers = new List<Customer>
        {
            new Customer
            {
                CustomerID = 1,
                FullName = "Dana Birkby",
                Phone = "394-555-0181",
                Hobbies = new ObservableCollection<string>(new string[] { "Swimming", "Movie" }),
                NewHobby = string.Empty
            },
            new Customer
            {
                CustomerID = 2,
                FullName = "Adriana Giorgi",
                Phone = "117-555-0119",
                Hobbies = new ObservableCollection<string>(new string[] { "Sports", "Video Game", "Sweets" }),
                NewHobby = string.Empty
            },
            new Customer
            {
                CustomerID = 3,
                FullName = "Wei Yu",
                Phone = "798-555-0118",
                Hobbies = new ObservableCollection<string>(),
                NewHobby = string.Empty
            }
        };

        public List<Customer> GetCustomers() => _customers;

        public void UpdateCustomer(Customer selectedCustomer)
        {
            if (!string.IsNullOrWhiteSpace(selectedCustomer.NewHobby))
            {
                selectedCustomer.Hobbies.Add(selectedCustomer.NewHobby);
                selectedCustomer.NewHobby = string.Empty;
            }

            Customer customerToChange = _customers.Single(c => c.CustomerID == selectedCustomer.CustomerID);
            customerToChange = selectedCustomer;
        }
    }

    #endregion

    #region ViewModel

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public class CustomerViewModel : ViewModelBase
    {
        private List<Customer> _customers;
        private Customer _currentCustomer;
        private CustomerRepository _repository;

        public CustomerViewModel()
        {
            _repository = new CustomerRepository();
            _customers = _repository.GetCustomers();

            WireCommands();
        }

        private void WireCommands()
            => UpdateCustomerCommand = new RelayCommand(UpdateCustomer);

        public RelayCommand UpdateCustomerCommand { get; private set; }

        public List<Customer> Customers => _customers;

        public Customer CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                if (_currentCustomer != value)
                {
                    _currentCustomer = value;
                    OnPropertyChanged(nameof(CurrentCustomer));
                    UpdateCustomerCommand.IsEnabled = true;
                }
            }
        }

        public void UpdateCustomer() => _repository.UpdateCustomer(CurrentCustomer);
    }

    #endregion

    #region Command

    public class RelayCommand : ICommand
    {
        private readonly Action _handler;
        private bool _isEnabled;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action handler)
        {
            _handler = handler;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter) => IsEnabled;

        public void Execute(object parameter) => _handler();
    }

    #endregion
}
