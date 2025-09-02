

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Invoice_Payment_Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyApi.Models;
using Stripe;
using Stripe.Checkout;

namespace MyApi.Controllers
{

    [ApiController]
    [Route("webhook")]
    public class WebhookController : Controller
    {

        private readonly MyDbContext _context;

        public WebhookController(MyDbContext context)
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

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string endpointSecret = Startup.webhookKey;
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var sessionIntent = stripeEvent.Data.Object as Session;
                    if (sessionIntent.PaymentStatus == "paid")
                    {
                        //Console.WriteLine("A successful payment for {0} was made.", sessionIntent.AmountTotal);
                        var account = await GetAccountFromNumber(sessionIntent.Metadata["accountNumber"]);
                        var invoice = await GetLastUnpaidInvoice(account.accountid);

                        if (invoice.invoicedate.ToString() == sessionIntent.Metadata["invoiceDate"])
                        {
                            invoice.status = "Paid";
                            await _context.SaveChangesAsync();
                        }



                    }

                }
                else
                {
                }
                return Ok();
            }
            catch (StripeException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }




};


