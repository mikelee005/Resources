using Braintree;
using BraintreeProcessing;
using adr.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace adr.Web.Controllers
{
    public class CartController : BaseController
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        public IBraintreeConfiguration config = new BraintreeConfiguration();

        public static readonly TransactionStatus[] transactionSuccessStatuses = { TransactionStatus.AUTHORIZED, TransactionStatus.AUTHORIZING, TransactionStatus.SETTLED, TransactionStatus.SETTLING, TransactionStatus.SETTLEMENT_CONFIRMED, TransactionStatus.SETTLEMENT_PENDING, TransactionStatus.SUBMITTED_FOR_SETTLEMENT };

        public BraintreeGateway gateway = new BraintreeGateway
        {
            Environment = Braintree.Environment.SANDBOX,
            MerchantId = ConfigService.GetBT_MerchantId(),
            PublicKey = ConfigService.GetBT_PublicKey(),
            PrivateKey = ConfigService.GetBT_PrivateKey()
        };

        public ActionResult CreditCard()
        {
            CardViewModel vm = new CardViewModel();

            var clientToken = gateway.ClientToken.generate();
            vm.clientToken = clientToken;

            return View(vm);
        }

        public ActionResult Create()
        {
            Decimal amount;

            CardViewModel vm = new CardViewModel();

            var clientToken = gateway.ClientToken.generate();
            vm.clientToken = clientToken;

            try
            {
                amount = Convert.ToDecimal(Request["amount"]);
            }
            catch (FormatException ex)
            {
                throw ex;
            }

            var nonce = Request["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                return RedirectToAction("Show", new { id = transaction.Id });
            }
            else if (result.Transaction != null)
            {
                return RedirectToAction("Show", new { id = result.Transaction.Id });
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }
                vm.errorMessages = errorMessages;
                return RedirectToAction("CreditCard");
            }

        }

        public ActionResult Show(String id)
        {
            CardViewModel vm = new CardViewModel();

            var clientToken = gateway.ClientToken.generate();
            vm.clientToken = clientToken;

            Transaction transaction = gateway.Transaction.Find(id);

            if (transactionSuccessStatuses.Contains(transaction.Status))
            {
                vm.status = true;
            }
            else
            {
                vm.status = false;
            };

            vm.transaction = transaction;

            return View(vm);
        }
    }
}