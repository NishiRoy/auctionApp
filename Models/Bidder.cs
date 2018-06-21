using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace beltExam.Models
{
    public class Bidder{

        public int Id{get;set;}
        public int UserId{get;set;}
        public User user{get;set;}
        public int AuctionId{get;set;}
        public Auction auctions{get;set;}

        public double BidValue{get;set;}
        public DateTime CreatedAt{get;set;}
        public DateTime UpdatedAt{get;set;}

        public Bidder(){
            CreatedAt=DateTime.Now;
            UpdatedAt=DateTime.Now;
        }
    }
}