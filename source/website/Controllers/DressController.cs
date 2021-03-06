using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Dapper;
using Website.DatabaseModels;
using DressType = Website.Models.DressType;
using AutomaticModelStateValidation;
using System.Security.Cryptography;

namespace Website.Controllers
{
    [Route("/dress")]
    public class DressController : Controller
    {
        readonly IDbConnection connection;
        readonly IDbTransaction dbTransaction;

        public DressController(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            this.connection = dbConnection;
            this.dbTransaction = dbTransaction;
        }

        [HttpGet("")]
        public IActionResult Home(DressesFilterModel model)
        {
            string sql =
                @"SELECT d.DressId, d.DressName, d.DressWebpage, d.Price, d.ProductDescription, d.DressType, d.DressApproval, d.Rating, d.ShopId, d.CreatedBy, d.Deleted, i.ImageId
                    FROM Dresses d
                    FULL OUTER JOIN DressImages di on d.DressId = di.DressId
                    FULL OUTER JOIN Images i on di.ImageId = i.ImageId
                    WHERE d.DressType = @DressType 
                    AND (d.Deleted <> 1 OR d.DressId = @DeletedDressId)
                    AND (i.ImageId is null
                        OR i.ImageId = (
                            Select top 1 di2.ImageId
                            From DressImages di2
                            Where di2.DressId = di.DressId
                            Order By di2.[Favourite], di2.[SequentialId]))
                    Order By d.[SequentialId]";

            var dresses =
                connection
                .Query<DressItemsQueryModel>(
                    sql,
                    new {
                        model.DressType,
                        model.DeletedDressId
                    }, dbTransaction)
                .Select(dress => {        
                    return new DressItem() {
                        DressId = dress.DressId,
                        Name = dress.DressName,
                        Price = dress.Price.ToString("C"),
                        Shop = "Need to do",
                        Description = dress.ProductDescription,
                        ImageId = dress.ImageId,
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
                    Images = null,
                    DressType = null,
                    Url = model.WebpageUrl
                };
            return View("GetNewDress", responseModel);
        }

        private DatabaseModels.DressType MapDressType(DressType modelDressType)
        {
            switch (modelDressType) {
                case DressType.Bride:
                    return DatabaseModels.DressType.Bride;
                case DressType.BridesMaid:
                    return DatabaseModels.DressType.BridesMaid;
            }
            return DatabaseModels.DressType.Bride;
        }


        [HttpPost("save")]
        [AutoValidateModel(nameof(NewDress))]
        public async Task<IActionResult> SaveDress(SaveDressModel model)
        {
            var dressId = Guid.NewGuid();
            foreach (var image in model.Images) {

                var imageId = Guid.NewGuid();
                var imageFileName = image.FileName;
                var imageName = System.IO.Path.GetFileName(image.FileName);
                var imageExtension = System.IO.Path.GetExtension(image.FileName);
                var imageContent = image.OpenReadStream().ReadFullyToArray();
                var imageHash = GetByteArrayHash(imageContent);

                var result = await connection.QueryFirstOrDefaultAsync<Guid?>("Select ImageId From Images Where Hash = @Hash", new { Hash = imageHash }, dbTransaction);

                if (result == null) {
                    await connection.ExecuteAsync(
                        @"Insert Images (ImageId, FileName, FileExtension, FileData, Hash) 
                    values (@ImageId, @FileName, @FileExtension, @FileData, @Hash)",
                        new Images() {
                            ImageID = imageId,
                            FileName = imageName,
                            FileExtension = imageExtension,
                            FileData = imageContent,
                            Hash = imageHash,
                        },
                        dbTransaction
                    );
                } else {
                    imageId = result.Value;
                }

                await connection.ExecuteAsync(
                    @"Insert DressImages (DressId, ImageId, Favourite) 
                    values (@DressId, @ImageId, @Favourite)",
                    new DressImages {
                        DressId = dressId,
                        ImageID = imageId,
                        Favourite = false
                    },
                    dbTransaction
                );
            }

            await connection.ExecuteAsync(
                @"Insert Dresses(DressId, DressName, DressWebpage, Price, ProductDescription, DressType, DressApproval, Rating, ShopId, WeddingId, CreatedBy, CreatedAt, ModifiedBy, ModifiedAt, Deleted, DeletedAt) 
                       values (@DressId, @DressName, @DressWebpage, @Price, @ProductDescription, @DressType, @DressApproval, @Rating, @ShopId, @WeddingId, @CreatedBy, @CreatedAt, @ModifiedBy, @ModifiedAt, @Deleted, @DeletedAt)",
                    new DressSaveModel() {
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
                        CreatedBy = Guid.Empty, //curent user
                        CreatedAt = DateTimeOffset.Now,
                        ModifiedBy = Guid.Empty,
                        ModifiedAt = DateTimeOffset.Now,
                        Deleted = false,
                        DeletedAt = null
                    },
                    dbTransaction
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
            var sqlDressImages =
                    @"SELECT i.ImageId, di.Favourite
                    FROM DressImages di
                    JOIN Images i on di.ImageId = i.ImageId
                    WHERE di.DressId = @DressId
                    ORDER BY di.SequentialId";

            var dressImages = 
                connection
                .Query<DressImageModel>(
                sqlDressImages, new { DressId = dressId }, dbTransaction)
                .ToList();

            var dress =
                connection
                .QueryFirst<DressQueryModel>(
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, DressApproval, Rating, ShopId, CreatedBy FROM Dresses WHERE DressId = @DressId",
                    new { DressId = dressId }, dbTransaction);

            var image = dressImages.FirstOrDefault(x => x.Favourite);
            if (image == null && dressImages.Count > 0) {
                image = dressImages[0];
            }
            var primaryImageId = image == null ? null : new Guid?(image.ImageId);

            var model = new DressDetailsModel() {
                Name = dress.DressName,
                DressWebpage = dress.DressWebpage,
                Price = dress.Price.ToString("C"),
                Shop = dress.ShopId,
                Description = dress.ProductDescription,
                PrimaryImageId = primaryImageId,
                Images = dressImages.Select(x => x.ImageId).ToList(),
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
            var dress =
                await connection
                .QueryFirstAsync<DressQueryModel>(
                    "Select DressId, DressName, DressWebpage, Price, ProductDescription, DressType, ShopId, ImageId, ModifiedBy, ModifiedAt FROM Dresses WHERE DressId = @DressId",
                    new { DressId = dressId }, dbTransaction);

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
                },
                dbTransaction

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
                }, dbTransaction);

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
                }, dbTransaction);
            return RedirectToAction(nameof(GetDressDetails));
        }

    }
}
