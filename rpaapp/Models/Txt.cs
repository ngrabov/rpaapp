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
        public string ClientCode { get; set; }
        public string Group { get; set;}
        public string State { get; set; }
        public string VAT { get; set; }
        public int? InvoiceTypeId { get; set; }
        public string Currency { get; set; }
        public string InvoiceNumber { get; set; }
        
        [DataType(DataType.Date)]
        [CustomInvoiceDate(ErrorMessage ="Invoice date should be earlier or equal to today's date.")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")] 
        [MinDate]
        public DateTime InvoiceDate { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")] 
        public DateTime InvoiceDueDate { get; set; }
        public double Bruto { get; set; }
        public double Neto { get; set; }
        [Display(Name = "Order number")]
        public string ReferenceNumber { get; set; }
        public string PaymentReference { get; set; }
        public int? ProcessTypeId { get; set; }
        [Display(Name = "Person in charge")]
        public int? PersonInChargeId { get; set; }
    }
}