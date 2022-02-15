using System.ComponentModel.DataAnnotations;

namespace rpaapp.Models
{
    public class Txt
    {
        public int Id { get; set; }
        public Guid DocId {get; set;}
        public bool isReviewed { get; set; }
        public bool isDownloaded { get; set;}
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
        [Display(Name = "Payment number")]
        public string ReferenceNumber { get; set; }
        public int? ProcessTypeId { get; set; }
        #nullable enable
        private ProcessType? _ProcessType;
        public ProcessType? ProcessType { get { return _ProcessType ?? (_ProcessType = new ProcessType());} set { _ProcessType = value;}}
    }
}