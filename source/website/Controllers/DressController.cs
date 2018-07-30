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
            var dresses =
                connection
                .Query<DressEntity>(
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, RecommendedBy, DressApproval, Rating, ShopId, ImageId FROM Dresses")
                .Select(dress =>
                {
                    return new DressItem()
                    {
                        Id = dress.DressId,
                        Name = dress.DressName,
                        Price = dress.Price.ToString("C"),
                        Shop = "Need to do",
                        Description = dress.ProductDescription,
                        Image = "Need to do",
                        RecommendedBy = "To do",
                        Approval = dress.DressApproval.ToString(),
                        Rating = "2",
                    };
                })
                .ToList();

            var model = new DressIndexModel() {
                DressType = DressType.Bride,
                Dresses = dresses
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
            var dressId = Guid.NewGuid();
            //var x = ModelState;
            await connection.ExecuteAsync(
                @"Insert Dresses(DressId, DressName, DressWebpage, Price, ProductDescription, DressType, RecommendedBy, DressApproval, Rating, ShopId, WeddingId, ImageId) 
                       values (@DressId, @DressName, @DressWebpage, @Price, @ProductDescription, @DressType, @RecommendedBy, @DressApproval, @Rating, @ShopId, @WeddingId, @ImageId)",
                    new DressEntity()
                    {
                        DressId = dressId,
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
            return RedirectToAction(nameof(GetDressDetails), new { Id = dressId });
        }

        [HttpGet("{dressId}")]
        public async Task<IActionResult> GetDressDetails(Guid dressId)
        {
            var dress =
                connection
                .Query<DressEntity>(
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, RecommendedBy, DressApproval, Rating, ShopId, ImageId FROM Dresses WHERE DressId = @DressId", new { DressId = dressId })
                .Single();

            var model = new DressDetailsModel()
            {
                Name = dress.DressName,
                DressWebpage = dress.DressWebpage,
                Price = dress.Price.ToString("C"),
                Shop = "Need to do",
                Description = dress.ProductDescription,
                Image = "Need to do",
                Recommendation = "To do",
                Comments = new List<string>() {
                    "Love Love Love!",
                    "So pretty!",
                },
                Approval = dress.DressApproval.ToString(),
                Rating = "2",
                DressType = DressType.Bride,
            };
            return View(model);
        }

        [HttpGet("{dressId}/edit")]
        public async Task<IActionResult> EditDressDetails(Guid dressId)
        {
            var dresses =
                await connection
                .QueryAsync<DressEntity>(
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, ShopId, ImageId FROM Dresses WHERE DressId = @DressId",
                    new { DressId = dressId });
            var dress = dresses.Single();

            var model = new EditDressDetailsModel()
            {
                DressId = dress.DressId,
                DressName = dress.DressName,
                DressWebpage = dress.DressWebpage,
                Price = dress.Price.ToString("C"),
                Shop = "Need to do",
                ProductDescription = dress.ProductDescription,
                Image = "Need to do",
                DressType = DressType.Bride,
            };
            return View(model);
        }

        [HttpPost("{dressId}")]
        [AutoValidateModel(nameof(EditDressDetails))]
        public async Task<IActionResult> UpdateDressDetails(EditDressDetailsModel model)
        {
            var sql =
                @"UPDATE Dresses SET
                DressName = @DressName,
                DressWebpage = @DressWebpage,
                Price = @Price,
                ProductDescription = @ProductDescription,
                DressType = @DressType,
                ShopId = @ShopId,
                ImageId = @ImageId
                WHERE DressId = @DressId";

            await connection.ExecuteAsync(sql,
                new
                {
                    model.DressId,
                    model.DressName,
                    model.DressWebpage,
                    model.Price,
                    model.ProductDescription,
                    DressType = MapDressType(model.DressType.Value),
                    ShopID = model.Shop,
                    ImageID = model.Image,
                }

            );
            return RedirectToAction(nameof(GetDressDetails));
        }
        
    }
}
