using fixer.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace fixer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaxController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TaxController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost(Name = "GetTaxs")]
        [Route("GetTaxes")]
        public IActionResult Get(TaxInput input)
        {
            TaxResult taxresult = new TaxResult();
            try
            {
                //would add error messages for inproper fields
                if (input != null)
                {
                    var invoiceDate = input.InvoiceDate.ToString("yyyy-MM-dd");
                    var authKey = _configuration.GetValue<string>("FixerKey");

                    if (string.IsNullOrEmpty(invoiceDate) || string.IsNullOrEmpty(input.PreTaxCurrency) || string.IsNullOrEmpty(input.PaymentCurrency))
                    {
                        throw new Exception();
                    }
                    var client = new RestClient("http://data.fixer.io/api/" 
                        + invoiceDate + "?access_key=" 
                        + authKey + "&base="
                        + input.PreTaxCurrency + "&symbols="
                        + input.PaymentCurrency);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                    if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        FixerAPIResponse? exchangeData = Newtonsoft.Json.JsonConvert.DeserializeObject<FixerAPIResponse>(response.Content);
                        if(exchangeData != null && exchangeData.success)
                        {
                            taxresult.PreTaxTotal.CurrencyCode = input.PaymentCurrency;
                            //pretax
                            double rate = 1;
                            if (input.PaymentCurrency == "CAD")
                            {
                                rate = exchangeData.rates.CAD;
                            }
                            else if (input.PaymentCurrency == "USD")
                            {
                                rate = exchangeData.rates.USD;
                            }
                            else if (input.PaymentCurrency == "EUR")
                            {
                                rate = exchangeData.rates.EUR;
                            }
                            taxresult.ExchangeRate = rate;
                            taxresult.PreTaxTotal.TotalAmount = Math.Round((input.PreTaxAmount * rate), 2);
                            //tax by currency code
                            taxresult.TaxAmount.CurrencyCode = input.PaymentCurrency;
                            if(taxresult.TaxAmount.CurrencyCode == "CAD")
                            {
                                taxresult.TaxAmount.TotalAmount = taxresult.PreTaxTotal.TotalAmount * 0.11;
                            }
                            else if (taxresult.TaxAmount.CurrencyCode == "USD")
                            {
                                taxresult.TaxAmount.TotalAmount = taxresult.PreTaxTotal.TotalAmount * 0.10;
                            }
                            else if (taxresult.TaxAmount.CurrencyCode == "EUR")
                            {
                                taxresult.TaxAmount.TotalAmount = taxresult.PreTaxTotal.TotalAmount * 0.09;
                            }
                            taxresult.TaxAmount.TotalAmount = Math.Round(taxresult.TaxAmount.TotalAmount, 2);
                            //sum total
                            taxresult.GrandTotal.TotalAmount = Math.Round((taxresult.PreTaxTotal.TotalAmount + taxresult.TaxAmount.TotalAmount), 2);
                            taxresult.GrandTotal.CurrencyCode = input.PaymentCurrency;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                return StatusCode(500);
            }

            return Ok(JsonConvert.SerializeObject(taxresult));
        }
    }
}