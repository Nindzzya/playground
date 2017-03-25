using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace AccountsTest.Core
{
   public class CoreAssets:DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Accounts.db");
        }

    }
    #region Declarations
    public class Transaction
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public TransactionDirection Direction { get; set; }
        public Tags Tag { get; set; }
        public PaymentType PayType { get; set; }
        public double PriceAmount { get; set; }
        //add currency
        //public Transaction(string id,string title, string desc, Tags tag, PaymentType payType, double price, DateTime creationDate)
        //{
        //    Id = id;
        //    Title = title;
        //    Description = desc;
        //    Tag = tag;
        //    PayType = payType;
        //    PriceAmount = price;
        //    CreationDate = creationDate;
        //}

    }
    public class Account
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public AccountType AccountType { get; set; }
        public List<Transaction> Transactions = new List<Transaction>();
        //public Account(string id, string name, AccountType type)
        //{
        //    Id = id;
        //    Name = name;
        //    AccountType = type;
        //}
    }

    public enum PaymentType
    {
        Card,
        Cash,
        Internet,
        ShoppingWallet
    }
    public enum Tags
    {
        Online,
        Shopping,
        Fuel,
        Bills,
        Fees,
        Salary,
        CustomMonthlyIncome
    }
    public enum AccountType
    {
        Card,
        Wallet
    }

    public enum TransactionDirection
    {
        Expense,
        Income
    }
    #endregion
}
