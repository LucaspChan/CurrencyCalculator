namespace fixer.Models
{
    public class TaxInput
    {
        public DateTime InvoiceDate { get; set; } = DateTime.Today;
        public double PreTaxAmount { get; set; } = 123.45;
        public string PreTaxCurrency { get; set; } = "EUR";
        public string PaymentCurrency { get; set; } = "USD";


    }
}
