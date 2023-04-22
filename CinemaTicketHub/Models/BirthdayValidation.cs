using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CinemaTicketHub.Models
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                var today = DateTime.Today;
                var age = today.Year - date.Year;
                if (date > today.AddYears(-age))
                    age--;
                if (age < 18)
                {
                    string script = "<script>alert('Bạn chưa đủ 18 tuổi');</script>";
                    HttpContext.Current.Response.Write(script);
                    return new ValidationResult("");
                }
            }
            return ValidationResult.Success;
        }
    }
}