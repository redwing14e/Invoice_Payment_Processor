using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;
using Invoice_Payment_Processor;


namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/")]

    public class ApiController : Controller
    {
        private readonly MyDbContext _context;

        public ApiController(MyDbContext context)
        {
            _context = context;
        }

        private async Task<Models.Account> GetAccountFromNumber(string accountNumber)
        {
            return await _context.accounts.FromSql($"SELECT * FROM accounts WHERE AccountNumber={accountNumber}").FirstOrDefaultAsync(e => e.accountnumber == accountNumber);
        }

        private async Task<Models.Invoice> GetLastUnpaidInvoice(int accId)
        {
            return await _context.invoices.FromSql($"SELECT * FROM invoices WHERE AccountId={accId}").OrderBy(e => e.invoicedate).LastOrDefaultAsync(e => e.status == "Unpaid");
        }



        [HttpPost("Validate/{accountNumber?}/{invoiceDate?}")]
        public async Task<ActionResult<(float, string)>> GetAccount(string accountNumber, string invoiceDate)
        {
            //Console.WriteLine("Attempted to post with " + accountNumber + ", " + invoiceDate);
            var account = await GetAccountFromNumber(accountNumber);

            if (account == null)
            {
                Console.WriteLine("error cant find account");
                return NotFound("Invalid details");
            }

            var invoice = await GetLastUnpaidInvoice(account.accountid);

            if (invoice == null || invoice.invoicedate != DateOnly.Parse(invoiceDate))
            {
                Console.WriteLine("error cant find invoice");
                return NotFound("Invalid details");
            }
            //Console.WriteLine("validate amount "+invoice.amountdue);
            return Ok(new { amountDue = invoice.amountdue, customerName = account.customername });
        }


        [HttpPost("Pay/create-checkout-session/{accountNumber?}")]
        public  async Task<ActionResult> CreateCheckoutSession(string accountNumber)
        {
            //Console.WriteLine("attempted to create checkout with account number " + accountNumber);
            var account = await GetAccountFromNumber(accountNumber);
            var invoice = await GetLastUnpaidInvoice(account.accountid);
            //Console.WriteLine(invoice.amountdue * 100);

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                    UnitAmount = (long) (invoice.amountdue * 100), //set the price to the invoice price 
                    Currency = "cad",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Invoice",
                    },
                    },
                    Quantity = 1,
                },
                },
                Mode = "payment",
                UiMode = "embedded",
                RedirectOnCompletion = "never",
                //save the accountNumber and the invoiceDate for the webhook to use for post-payment processes 
                Metadata = new Dictionary<string, string> { { "accountNumber", accountNumber }, { "invoiceDate", invoice.invoicedate.ToString() } }
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return Json(new { clientSecret = session.ClientSecret });
        }

        
    }
}