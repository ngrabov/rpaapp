using System.ComponentModel.DataAnnotations;

namespace rpaapp.Models
{
    public enum Status
    {
        Waiting, Ready, Confirmed, Archived
    }
    public class Document 
    {
        public int Id { get; set; }
        public Guid fguid { get; set; }
        public string fname { get; set; }
        public string pdfname { get; set; }
        public long fsize { get; set; }
        public string writername { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy.}")] 
        public DateTime uploaded { get; set; }       
    }
}