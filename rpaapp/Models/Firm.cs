using System.ComponentModel.DataAnnotations;

namespace rpaapp.Models
{
    public class Firm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VAT { get; set; }
        public string Keyword { get; set; }
        public string Group { get; set; }
        public string Currency { get; set; }
        
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")] 
        public int? DueDate { get; set; }
        public bool isTrained { get; set; }
        public string ProcessTypeId { get; set; }
    }
}