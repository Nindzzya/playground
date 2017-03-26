using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AccountsTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Core.CoreAssets database = new Core.CoreAssets();
        public List<Core.Account> accountsList= new List<Core.Account>();
        Core.Account tempAccount = new Core.Account();
        Core.Account activeAccount = new Core.Account();
        ObservableCollection<Core.Account> accountsCollection;
        Core.Transaction tempTransaction = new Core.Transaction();
        public MainPage()
        {
            this.InitializeComponent();
            accountListLv.ItemsSource = database.Account.ToList();
        }
        public void newAccount(Core.Account account)
        {
            database.Account.Add(account);
            database.SaveChanges();
            accountListLv.ItemsSource = database.Account.ToList();
        }
        public string getDateTag()
        {
            var now = DateTime.Now.ToLocalTime();
            string tag = now.Day.ToString() + now.Month.ToString() + now.Year.ToString() + now.Hour.ToString() + now.Minute.ToString()+now.Second.ToString();
            return tag;
        }

        private void newAccount_Click(object sender, RoutedEventArgs e)
        {
            tempAccount.Name = accountNameTb.Text;
            tempAccount.Id = "A" + getDateTag();
            newAccount(tempAccount);
            tempAccount = new Core.Account();
        }

        private void accountSelectionCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(accountSelectionCb.SelectedIndex)
            {
                case 0:
                    tempAccount.AccountType= Core.AccountType.Wallet;
                    break;
                case 1:
                    tempAccount.AccountType = Core.AccountType.Card;
                    break;
            }
        }

        private void accountListLv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (accountListLv.SelectedItem != null)
            {
                activeAccount = accountListLv.SelectedItem as Core.Account;
                transLv.ItemsSource = new ObservableCollection<Core.Transaction>(activeAccount.TransactionsAList);
            }
        }

        private void tagSelectionCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tagSelectionCb.SelectedIndex)
            {
                case 0:
                    tempTransaction.Tag = Core.Tags.Online;
                    break;
                case 1:
                    tempTransaction.Tag = Core.Tags.Shopping;
                    break;
                case 2:
                    tempTransaction.Tag = Core.Tags.Fuel;
                    break;
                case 3:
                    tempTransaction.Tag = Core.Tags.Bills;
                    break;
                case 4:
                    tempTransaction.Tag = Core.Tags.Fees;
                    break;
                case 5:
                    tempTransaction.Tag = Core.Tags.Salary;
                    break;
                case 6:
                    tempTransaction.Tag = Core.Tags.CustomMonthlyIncome;
                    break;

            }
        }

        private void payTypeCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (payTypeCb.SelectedIndex)
            {
                case 0:
                    tempTransaction.PayType = Core.PaymentType.Card;
                    break;
                case 1:
                    tempTransaction.PayType = Core.PaymentType.Cash;
                    break;
                case 2:
                    tempTransaction.PayType = Core.PaymentType.Internet;
                    break;
                case 3:
                    tempTransaction.PayType = Core.PaymentType.ShoppingWallet;
                    break;
            }
        }

        private void directionCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (directionCb.SelectedIndex)
            {
                case 0:
                    tempTransaction.Direction = Core.TransactionDirection.Expense;
                    break;
                case 1:
                    tempTransaction.Direction = Core.TransactionDirection.Income;
                    break;
            }
        }

        private void addTransBtn_Click(object sender, RoutedEventArgs e)
        {
            tempTransaction.Id = "T" + getDateTag();
            tempTransaction.Title = titleTb.Text;
            tempTransaction.Description = descTb.Text;
            tempTransaction.PriceAmount = double.Parse(priceTb.Text);
            //activeAccount.TransactionList.Add(tempTransaction);
            activeAccount.TransactionsAList.Add(tempTransaction);
            database.Account.Attach(activeAccount);
            database.Entry(activeAccount).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            database.SaveChanges(true);
            tempTransaction = new Core.Transaction();
            transLv.ItemsSource = new ObservableCollection<Core.Transaction>(activeAccount.TransactionsAList);

            //  transLv.ItemsSource = activeAccount.TransactionsAList;
        }

        private void datePickerTrans_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            tempTransaction.CreationDate = datePickerTrans.Date.Date;
        }
    }
}
