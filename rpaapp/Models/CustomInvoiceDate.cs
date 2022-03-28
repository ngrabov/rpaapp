using System;
using System.ComponentModel.DataAnnotations;

namespace rpaapp.Models
{
    public class CustomInvoiceDate : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            return dateTime <= DateTime.Now; 
        }
    }
}