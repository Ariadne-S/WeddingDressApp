using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace website.Controllers
{
    [Route ("/image")] 
    public class ImageController : Controller 
    {
        readonly IDbConnection connection;
        readonly IDbTransaction dbTransaction;
        private object dbtranscation;

        public ImageController(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            this.connection = dbConnection;
            this.dbTransaction = dbTransaction;
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetImage(Guid imageId)
        {
            var sql = "SELECT FileName, FileExtension, FileData FROM Images Where ImageId = @ImageId";

            var image = await connection.QueryFirstOrDefaultAsync<ImageModel>(sql, new { ImageId = imageId }, dbTransaction);

            if (image == null) {
                return NotFound();
            }

            return File(image.FileData, $"image/{image.FileExtension}");
        }
    }
}