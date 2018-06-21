using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using beltExam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace beltExam.Controllers
{
    public class HomeController : Controller
    {
        
        private LoginContext _context;
        
        public HomeController(LoginContext context){
            _context=context;

        }       
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpGet]
        [Route("redirectlogin")]
        public IActionResult redirectlogin()
        {
            return View("Index");
        }

        [HttpPost]
        [Route("validation")]
        public IActionResult Validation(User user,string Confirm)
        {
          
           if(ModelState.IsValid){
                if(user.Password==Confirm){

                ViewBag.error="";

                PasswordHasher<User> Hasher=new PasswordHasher<User>();
                user.Password=Hasher.HashPassword(user,user.Password);
                user.Wallet=1000;

                _context.Add(user);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.FirstName);

                return RedirectToAction("Success");
                }
                else{
                    ViewBag.error="Password & confirm password don't match!";
                    return View("Index");
                }

           }
           else{
               return View("Index");
           }

        }

        [HttpGet]
        [Route("success")]
        public IActionResult Success()
        {
            ViewData["Name"] = HttpContext.Session.GetString("UserName");

            int? ID=HttpContext.Session.GetInt32("UserId");

            ViewBag.id=ID;

            var v = TempData["error"];
            if (v== null || v.ToString()=="")
            {
                ViewBag.Error="";
            }
            else
            {
                ViewBag.Error=v;
            }

            return View("Success");
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(string lemail,string lpassword)
        {
            
            User ReturnedValue = _context.users.SingleOrDefault(user => user.Email == lemail);
            if(ReturnedValue != null && lpassword != null)
            {
                var Hasher = new PasswordHasher<User>();
                // Pass the user object, the hashed password, and the PasswordToCheck
                if(0 != Hasher.VerifyHashedPassword(ReturnedValue, ReturnedValue.Password,lpassword))
                {
                    ViewBag.loginerror="";
                    HttpContext.Session.SetString("UserName", ReturnedValue.FirstName);
                    HttpContext.Session.SetInt32("UserId", ReturnedValue.Id);
                    return RedirectToAction("Success");
                    
                }
                else
                {
                    ViewBag.loginerror="Something went wrong...Try again";
                    return View("Index");
                }
            }
            else{
                ViewBag.loginerror="Both fields are required!";
                return View("Index");
            }
            
        }

        [HttpGet]
        [Route("logout")]

        public IActionResult logout(){
        
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("CreateAuction")]
        public IActionResult CreateAuction(Auction auctions)
        {
            if(ModelState.IsValid)
            {
                TempData["Error"]="";
                int? ID=HttpContext.Session.GetInt32("UserId");
                auctions.UserId=(int)ID;
                if(auctions.EndDate<DateTime.Now)
                {
                    TempData["Error"]="End Date Must be in the Future";
                    return RedirectToAction("Success");
                }
                _context.Add(auctions);
                _context.SaveChanges();
            }
            else
            {
                TempData["Error"]="All the Fields are Required(Starting Bid Must be >0)";
                return RedirectToAction("Success");
            }
              return RedirectToAction("Dashboard");  
        }

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            
            var v = TempData["Error"];
            if (v== null || v.ToString()=="")
            {
                ViewBag.Error="";
            }
            else
            {
                ViewBag.Error=v;
            }
            List<Auction> allAuctions=_context.auction.Include(b=>b.bidders).Include(u=>u.host).ToList();
            ViewBag.Auctions=allAuctions;
            int? ID=HttpContext.Session.GetInt32("UserId");

            ViewBag.id=ID;
            Dictionary<int, double> dictionary = new Dictionary< int,double>();
            foreach(var aucti in allAuctions)
            {
                List<Bidder> bidders=_context.bidder.Include(bidder=>bidder.user).Include(bidder=>bidder.auctions).Where(b=>b.AuctionId==aucti.Id).OrderByDescending(b=>b.BidValue).ToList();
                var top=bidders.Take(1);
                foreach(var val in top)
                {
                    dictionary.Add(aucti.Id,val.BidValue);
                    if(DateTime.Now>aucti.EndDate)
                    {
                        if(val.user.Wallet>val.BidValue)
                        {
                            aucti.host.Wallet+=val.BidValue;
                            val.user.Wallet-=val.BidValue;
                            Auction ViewAuction=_context.auction.Include(a=>a.host).SingleOrDefault(a=>a.Id==aucti.Id);
                            _context.auction.Remove(ViewAuction);
                            _context.SaveChanges();

                        }
                        else
                        {
                            ViewBag.Error="Insufficient funds... Could not exchange money";
                        }
                    }
              }

                
            }
            List<Auction> allNewAuctions=_context.auction.Include(b=>b.bidders).Include(u=>u.host).OrderBy(e=>e.EndDate).ToList();
            ViewBag.Auctions=allNewAuctions;
            ViewBag.Tops=dictionary;
           
            
            return View();
        }

        [HttpPost]
        [Route("viewauction")]
        public IActionResult viewauction(int auction_id)
        {
            Auction ViewAuction=_context.auction.Include(a=>a.host).SingleOrDefault(a=>a.Id==auction_id);
            ViewBag.Auction=ViewAuction;
            List<Bidder> bidders=_context.bidder.Include(bidder=>bidder.user).Include(bidder=>bidder.auctions).Where(b=>b.AuctionId==auction_id).OrderByDescending(b=>b.BidValue).ToList();
            ViewBag.id=(int)HttpContext.Session.GetInt32("UserId");
            ViewBag.Top=bidders.Take(1);
            return View("ViewAuction");
        }

        [HttpPost]
        [Route("deleteauction")]
        public IActionResult deleteauction(int auction_id)
        {
            Auction ViewAuction=_context.auction.Include(a=>a.host).SingleOrDefault(a=>a.Id==auction_id);
            // List<Bidder> bidders=_context.bidder.Include(bidder=>bidder.user).Include(bidder=>bidder.auctions).Where(b=>b.AuctionId==auction_id).OrderByDescending(b=>b.BidValue).ToList();
            ViewBag.id=(int)HttpContext.Session.GetInt32("UserId");
            _context.auction.Remove(ViewAuction);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [Route("createBid")]
        public IActionResult createBid(int auction_id,double bids)
        {
                Bidder bidding =new Bidder();
                int ID=(int)HttpContext.Session.GetInt32("UserId");
                User person=_context.users.SingleOrDefault(a=>a.Id==ID);
                Auction ViewAuction=_context.auction.SingleOrDefault(a=>a.Id==auction_id);
                if(person.Wallet>bids)
                {
                        if(ViewAuction.StartingBid<bids)
                    {
                        bidding.BidValue=bids;
                        bidding.UserId=ID;
                        bidding.AuctionId=auction_id;
                        _context.Add(bidding);
                        _context.SaveChanges();
                    }
                    else{
                        TempData["error"]="Your bid must be greater than starting bid";
                    }
                }
                else
                {
                    TempData["error"]="You don't have enough money to place this bid";
                }
                return RedirectToAction("Dashboard");
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
