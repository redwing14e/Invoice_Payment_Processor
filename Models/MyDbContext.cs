using System;
using Microsoft.EntityFrameworkCore;

//Postgres database
namespace MyApi.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Account> accounts { get; set; }
        public DbSet<Invoice> invoices { get; set; }
    }

    //Accounts Table 
    public class Account
    {
        public int accountid { get; set; }
        public string accountnumber { get; set; }
        public string customername { get; set; }
    }

    //Invoices Table
    public class Invoice
    {
        public int invoiceid { get; set; }
        public int accountid { get; set; }
        public DateOnly invoicedate { get; set; }
        public float amountdue { get; set; }
        public string status { get; set; }
    }

}