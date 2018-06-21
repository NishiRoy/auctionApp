using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace beltExam.Models
{
    public class User{
        public int Id{get;set;}

        [Required(ErrorMessage="First name is required")]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z]+$",ErrorMessage="Only Alphabets Required")]
        public string FirstName{get;set;}

        [Required]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z]+$",ErrorMessage="Only Alphabets Required")]
        public string LastName{get;set;}

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email{get;set;}

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8),MaxLength(20)]
        public string Password{get;set;}

        public double Wallet{get;set;}

        public List<Auction> auctions{get;set;}
        public List<Bidder> bidders{get;set;}

        public DateTime CreatedAt{get;set;}
        public DateTime UpdatedAt{get;set;}

        public User(){
            CreatedAt=DateTime.Now;
            UpdatedAt=DateTime.Now;
        }
    }
}