using System.ComponentModel.DataAnnotations;

namespace rpaapp.Models
{
    public class Txt
    {
        public int Id { get; set; }
        public Document Document { get { return _document ?? (_document = new Document()); }  set { _document = value; } }
        private Document _document { get; set; }
        public bool isReviewed { get; set; }
        public string pngNames { get; set; }
        public string Name { get; set; }
        public string Group { get; set;}
        public string State { get; set; }
        public string VAT { get; set; }
        public string Currency { get; set; }
        public string BillingGroup { get; set; }
        public string IBAN { get; set; }
        public string VATobligation { get; set; }
        public string InvoiceNumber { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")] 
        public DateTime InvoiceDate { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")] 
        public DateTime InvoiceDueDate { get; set; }
        public double Bruto { get; set; }
        public double Neto { get; set; }
        public string ReferenceNumber { get; set; }
    }
}