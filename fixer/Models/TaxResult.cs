namespace fixer.Models
{
    public class TaxResult
    {
        public Amount PreTaxTotal { get; set; } = new Amount();
        public Amount TaxAmount { get; set; } = new Amount();
        public Amount GrandTotal { get; set; } = new Amount();
        public double ExchangeRate { get; set; }

    }

    public class Amount
    {
        public double TotalAmount { get; set; }

        public string CurrencyCode { get; set; } = String.Empty;
    }
}
