namespace rpaapp.Models
{
    public class Txt
    {
        public int Id { get; set; }
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public string Group { get; set;}
        public string State { get; set; }
        public string VAT { get; set; }
        public string Currency { get; set; }
        public string BillingGroup { get; set; }
        public string IBAN { get; set; }
        public string VATobligation { get; set; }
        public string InvoiceNumber1 { get; set; }
        public string InvoiceNumber2 { get; set; }
        public DateTime InvoiceDate1 { get; set; }
        public DateTime InvoiceDate2 { get; set; }
        public DateTime InvoiceDueDate1 { get; set; }
        public DateTime InvoiceDueDate2 { get; set; }
        public double Bruto1 { get; set; }
        public double Bruto2 { get; set; }
        public double Neto1 { get; set; }
        public double Neto2 { get; set; }
        public string ReferenceNumber1 { get; set; }
        public string ReferenceNumber2 { get; set; }
    }
}