using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Dapper;
using Website.Entities;
using DressType = Website.Models.DressType;
using AutomaticModelStateValidation;
using System.Security.Cryptography;

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
        public IActionResult Home(DressesFilterModel model)
        {
            var dresses =
                connection
                .Query<DressEntity>(
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, DressApproval, Rating, ShopId, CreatedBy, ImageId, Deleted " +
                    "FROM Dresses " +
                    "Where DressType = @DressType " +
                    "And (Deleted <> 1 OR DressId = @DeletedDressId)",
                    new {
                        model.DressType,
                        model.DeletedDressId
                    })
                .Select(dress => {        
                    return new DressItem() {
                        DressId = dress.DressId,
                        Name = dress.DressName,
                        Price = dress.Price.ToString("C"),
                        Shop = "Need to do",
                        Description = dress.ProductDescription,
                        Image = "Need to do",
                        CreatedBy = Guid.Empty,
                        Approval = dress.DressApproval.ToString(),
                        Rating = "2",
                        Deleted = dress.Deleted
                    };
                })
                .ToList();

            var viewModel = new DressIndexModel() {
                DressType = DressType.Bride,
                Dresses = dresses
            };
            return View(viewModel);
        }

        [HttpGet("new")]
        [HttpPost("new")]
        public IActionResult NewDress(AddDressUrlModel model)
        {
            // TODO: Scrapes website
            var responseModel =
                new GetNewDressModel() {
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

        private Entities.DressType MapDressType(DressType modelDressType)
        {
            switch (modelDressType) {
                case DressType.Bride:
                    return Entities.DressType.Bride;
                case DressType.BridesMaid:
                    return Entities.DressType.BridesMaid;
            }
            return Entities.DressType.Bride;
        }


        [HttpPost("save")]
        [AutoValidateModel(nameof(NewDress))]
        public async Task<IActionResult> SaveDress(SaveDressModel model)
        {
            var dressId = Guid.NewGuid();
            var imageId = Guid.NewGuid();

            //var x = ModelState;
            await connection.ExecuteAsync(
                @"Insert Dresses(DressId, DressName, DressWebpage, Price, ProductDescription, DressType, DressApproval, Rating, ShopId, WeddingId, ImageId, CreatedBy, CreatedAt, ModifiedBy, ModifiedAt, Deleted, DeletedAt) 
                       values (@DressId, @DressName, @DressWebpage, @Price, @ProductDescription, @DressType, @DressApproval, @Rating, @ShopId, @WeddingId, @ImageId, @CreatedBy, @CreatedAt, @ModifiedBy, @ModifiedAt, @Deleted, @DeletedAt)",
                    new DressEntity() {
                        DressId = dressId,
                        DressName = model.Name,
                        DressWebpage = model.Url,
                        Price = model.Price.Value,
                        ProductDescription = model.Description,
                        DressType = MapDressType(model.DressType.Value),
                        DressApproval = DressApproval.NeedsApproval,
                        Rating = null,
                        ShopId = Guid.Empty, 
                        WeddingId = Guid.Empty,
                        ImageId = imageId,
                        CreatedBy = Guid.Empty, //curent user
                        CreatedAt = DateTimeOffset.Now,
                        ModifiedBy = Guid.Empty,
                        ModifiedAt = DateTimeOffset.Now,
                        Deleted = false,
                        DeletedAt = null
                    }
                );

            // Insert Into
            var image = model.Image;
            var imageFileName = image.FileName;
            var imageName = System.IO.Path.GetFileName(image.FileName);
            var imageExtension = System.IO.Path.GetExtension(image.FileName);
            var imageContent = image.OpenReadStream().ReadFullyToArray();
            var imageHash = GetByteArrayHash(imageContent);

            await connection.ExecuteAsync(
                @"Insert Images (ImageId, FileName, FileExtension, FileData, Hash) 
                    values (@ImageId, @FileName, @FileExtension, @FileData, @Hash)",
                new Images() {
                    ImageID = imageId,
                    FileName = imageName,
                    FileExtension = imageExtension,
                    FileData = imageContent,
                    Hash = imageHash,
                }
            );

            await connection.ExecuteAsync(
                @"Insert DressImages (DressId, ImageId, Favourite) 
                    values (@DressId, @ImageId, @Favourite)",
                new DressImages{
                    DressId = dressId,
                    ImageID = imageId,
                    Favourite = false
                }
            );

            return RedirectToAction(nameof(GetDressDetails), new { dressId });
        }

        private string GetByteArrayHash(byte[] byteArray)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider()) {
                return Convert.ToBase64String(sha1.ComputeHash(byteArray));
            }
        }

        [HttpGet("{dressId}")]
        public async Task<IActionResult> GetDressDetails(Guid dressId)
        {
            var dress =
                connection
                .Query<DressEntity>(
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, DressApproval, Rating, ShopId, ImageId, CreatedBy FROM Dresses WHERE DressId = @DressId", new { DressId = dressId })
                .Single();

            var model = new DressDetailsModel() {
                Name = dress.DressName,
                DressWebpage = dress.DressWebpage,
                Price = dress.Price.ToString("C"),
                Shop = dress.ShopId,
                Description = dress.ProductDescription,
                Image = "Need to do",
                Comments = new List<string>() {
                    "Love Love Love!",
                    "So pretty!",
                },
                Approval = dress.DressApproval.ToString(),
                Rating = dress.Rating,
                CreatedBy = dress.CreatedBy, //current user
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
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, ShopId, ImageId, ModifiedBy, ModifiedAt FROM Dresses WHERE DressId = @DressId",
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
                ModifiedBy = Guid.Empty, //current user
                ModifiedAt = DateTimeOffset.Now
            };
            return View(model);
        }

        [HttpPost("{dressId}")]
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
                ImageId = @ImageId,
                ModifiedBy = @ModifiedBy,
                ModifiedAt = @ModifiedAt
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
                    ModifiedBy = Guid.Empty,
                    ModifiedAt = DateTimeOffset.Now
                }

            );
            return RedirectToAction(nameof(GetDressDetails));
        }


        [HttpPost("{dressId}/delete")]
        public async Task<IActionResult> DeleteDressDetails(Guid dressId)
        {
            var sql =
                @"UPDATE Dresses SET
                Deleted = @Deleted,
                DeletedAt = @DeletedAt
                WHERE DressId = @DressId";

            await connection.ExecuteAsync(sql,
                new {
                    DressId = dressId,
                    Deleted = true,
                    DeletedAt = DateTimeOffset.Now
                });

            return RedirectToAction(nameof(Home), new { DeletedDressId = dressId });
        }

        [HttpPost("{dressId}/UndoDelete")]
        public async Task<IActionResult> UndoDelete (Guid dressId)
        {
            var sql =
                @"UPDATE Dresses SET
                Deleted = @Deleted
                WHERE DressId = @DressId";

            await connection.ExecuteAsync(sql,
                new {
                    DressId = dressId,
                    Deleted = false,
                });
            return RedirectToAction(nameof(GetDressDetails));
        }

    }
}
