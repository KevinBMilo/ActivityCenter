using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [Display(Name = " ")]
        [MinLength(2)]
        [RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage="Please use only letters and spaces.")]
        public string Name {get;set;}

        [Required]
        [Display(Name = " ")]
        [EmailAddress]
        public string Email {get;set;}

        [DataType(DataType.Password)]
        [Required]
        [Display(Name = " ")]
        [MinLength(8)]
        public string Password {get;set;}
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public List<Act> CreatedActs {get;set;}
        public List<Going> Goings {get;set;}
    }

    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string LoginEmail {get; set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string LoginPassword { get; set; }
    }
    public class Act
    {
        [Key]
        public int ActId {get;set;}

        [Required]
        [Display(Name = " ")]
        [MinLength(5, ErrorMessage="Do it right!")]
        public string ActName {get;set;}

        [Required]
        [Display(Name = " ")]
        [MinLength(5, ErrorMessage="Do it right!")]
        public string ActContent {get;set;}

        [Required]
        [Display(Name = " ")]
        [Range(1, int.MaxValue, ErrorMessage="Do it right!")]
        public int ActDurationOne {get;set;}
        [Required]
        [Display(Name = " ")]
        public string ActDurationTwo {get;set;}

        [DataType(DataType.Date)]
        [Required]
        [Display(Name = " ")]
        [AfterToday]
        public DateTime ActDate {get;set;}


        [DataType(DataType.Time)]
        [Required]
        [Display(Name = " ")]
        public string ActTime {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int UserId { get; set; }
        public User Creator {get;set;}
        public List<Going> Goings {get;set;}
        public Act()
                {
                    Goings = new List<Going>();
                    CreatedAt = DateTime.Now;
                    UpdatedAt = DateTime.Now;
                }

    }
    
    public class Going
    {
        [Key]
        public int GoingId {get;set;}
        public int UserId {get;set;}
        public int ActId {get;set;}
        public User User {get;set;}
        public Act Act {get;set;}

        
    }
    public class AfterToday : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            value = (DateTime)value;
            if ((DateTime)value > DateTime.Now)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Date must be set after today.");
            }
        }
    }
    

}