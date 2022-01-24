using System.ComponentModel.DataAnnotations;
namespace rpaapp.Models;

public class Pdf
{
    public int Id { get; set; }
    public string fname { get; set; }
    public Guid guid { get; set; }
    public long fsize { get; set; }
    public bool isDownloaded { get; set; }
    public bool isUploaded { get; set; }
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy. H:mm:ss}")] 
    public DateTime uploaded { get; set; }
    public string writername { get {return Writer.FirstName + " " + Writer.LastName;}}

    private Writer _writer;
    public Writer Writer { get { return _writer ?? (_writer = new Writer()); }  set { _writer = value; }  }
}