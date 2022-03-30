using rpaapp.Models;
using System.ComponentModel.DataAnnotations;
namespace rpaapp.Models;

public class MinDate : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var txt = (Txt)validationContext.ObjectInstance;
        return (txt.InvoiceDate <= txt.InvoiceDueDate) ? ValidationResult.Success
         : new ValidationResult("Invoice date should be earlier or equal to invoice due date.");
    }
}