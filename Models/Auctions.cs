using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace beltExam.Models
{
    public class Auction{

        public int Id{get;set;}

        [Required(ErrorMessage="Product name is required")]
        [MinLength(3)]
        public string ProductName{get;set;}

        
        [Required(ErrorMessage="Description is required")]
        [MinLength(10)]
        public string ProductDescription{get;set;}

        [Required]
        [Range(1,1000000000)]
        public double StartingBid{get;set;}

        [Required]
        public DateTime EndDate{get;set;}
        public DateTime CreatedAt{get;set;}
        public DateTime UpdatedAt{get;set;}
        public int UserId{get;set;}
        public User host{get;set;}
        public List<Bidder> bidders{get;set;}
        public Auction(){
            bidders=new List<Bidder>();
            CreatedAt=DateTime.Now;
            UpdatedAt=DateTime.Now;
        }
    }
}