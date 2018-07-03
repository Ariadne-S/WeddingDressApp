using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Dapper;
using Website.Entities;
using DressType = Website.Models.DressType;
using AutomaticModelStateValidation;

namespace Website.Controllers
{
    [Route("/dress")]
    public class DressController : Controller
    {
        IDbConnection connection;

        public DressController(IDbConnection dbConnection)
        {
            this.connection = dbConnection;
        }

        [HttpGet("")]
        public IActionResult Home()
        {
            var model = new DressIndexModel() {
                DressType = DressType.Bride,
                Dresses = new List<DressItem>()
                {
                    new DressItem() {
                        Name = "Lilli White",
                        Price = "$2300",
                        Shop = "Amy Loves Beads",
                        Description = "Feel dreamy in this feminine chiffon A-line gown with an illusion V-neckline, lace appliqués on the sleeves and sheer back, and a dropped waistline. This gown is completed with a chapel length train.",
                        Image = null,
                    },
                    new DressItem() {
                        Name = "Sea Pearl",
                        Price = "$3800",
                        Shop = "Sunkissed Brides",
                        Description = "Lovely lace cap sleeves and a subtle v-neckline. The back of this dress is an eyecatcher!",
                        Image = null,
                    },
                    new DressItem() {
                        Name = "Snow Drop",
                        Price = null,
                        Shop = "Ivory and Seashells",
                        Description = "Hand constructed A-line silhouette, V-neck dress with plunge detail and chapel train. Tulle and lace beaded with pearls and crystals.",
                        Image = null,
                    },
                }
              
            };
            return View(model);
        }

        [HttpPost("new")]
        public IActionResult NewDress(AddDressUrlModel model)
        {
            // TODO: Scrapes website
            var responseModel = new GetNewDressModel()
            {
                Name = "",
                Price = "",
                Shop = "",
                Description = "",
                Image = null,
                DressType = null,
                Url = model.WebpageUrl
            };
            return View("GetNewDress", responseModel);
        }

        private Website.Entities.DressType MapDressType(Website.Models.DressType modelDressType)
        {
            switch (modelDressType)
            {
                case DressType.Bride:
                    return Website.Entities.DressType.Bride;
                case DressType.BridesMaid:
                    return Website.Entities.DressType.BridesMaid;
            }
            return Website.Entities.DressType.Bride;
        }


        [HttpPost("save")]
        [AutoValidateModel(nameof(NewDress))]
        public async Task<IActionResult> SaveDress(SaveDressModel model)
        {
            //var x = ModelState;
            await connection.ExecuteAsync(
                @"Insert Dresses(DressId, DressName, DressWebpage, Price, ProductDescription, DressType, RecommendedBy, DressApproval, Rating, ShopId, WeddingId, ImageId) 
                       values (@DressId, @DressName, @DressWebpage, @Price, @ProductDescription, @DressType, @RecommendedBy, @DressApproval, @Rating, @ShopId, @WeddingId, @ImageId)",
                    new DressEntity()
                    {
                        DressId = Guid.NewGuid(),
                        DressName = "",
                        DressWebpage = model.Url,
                        Price = 0,
                        ProductDescription = "",
                        DressType = MapDressType(model.DressType.Value),
                        RecommendedBy = Guid.Empty, //currentuser
                        DressApproval = DressApproval.NeedsApproval,
                        Rating = null,
                        ShopId = Guid.NewGuid(),
                        WeddingId = Guid.Empty,
                        ImageId = Guid.Empty
                    }

                );
            return RedirectToAction(nameof(GetDressDetails), new {Id = Guid.Empty});
        }

        [HttpGet("{id}")]
        public IActionResult GetDressDetails(Guid id)
        {
            var model = new DressDetailsModel()
            {
                Name = "Lilli White",
                Price = "$2300",
                Shop = "Amy Loves Beads",
                Description = "Feel dreamy in this feminine chiffon A-line gown with an illusion V-neckline, lace appliqués on the sleeves and sheer back, and a dropped waistline. This gown is completed with a chapel length train.",
                Image = null,
                Recommendation = "Kayla",
                Comments = new List<string>() {
                    "Love Love Love!",
                    "So pretty!",
                },
                Approval = "Yes",
                DressType = DressType.Bride,
            };
            return View(model);
        }

        [HttpGet("{id}/edit")]
        public IActionResult EditDressDetails(Guid id)
        {
            var model = new EditDressDetailsModel()
            {
                Name = "Lilli White",
                Price = "$2300",
                Shop = "Amy Loves Beads",
                Description = "Feel dreamy in this feminine chiffon A-line gown with an illusion V-neckline, lace appliqués on the sleeves and sheer back, and a dropped waistline. This gown is completed with a chapel length train.",
                Image = null,
                DressType = DressType.Bride,
            };
            return View(model);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateDress(Guid id)
        {
            /*
            await connection.ExecuteAsync(
                @"Insert Dress(DressId, DressName, DressWebpage, Price, ProductDescription, DressType. RecommendedBy, Approval, Rating, ShopId, ImageId) 
                       values (@DressId, @DressName, @DressWebpage, @Price, @ProductDescription, @DressType, @RecommendedBy, @Rating, @ShopId, @ImageId)",
                    new DressEntity()
                    {
                        DressId = Guid.NewGuid(),
                        DressName = "",
                        DressWebpage = model.WebpageUrl,
                        Price = 0,
                        ProductDescription = "",
                        DressType = null,
                        RecommendedBy = Guid.Empty, //currentuser
                        DressApproval = DressApproval.NeedsApproval,
                        Rating

                    }

                );
            */
            return Redirect("/dress");
            //remember to redirect to shop if we need to set up a new shop otherwise go to dress. (To do later)
        }
    }
}
