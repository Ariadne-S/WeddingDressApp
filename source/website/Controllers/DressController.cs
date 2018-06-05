using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace Website.Controllers
{
    [Route("/dress")]
    public class DressController : Controller
    {
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
            return Redirect("/dress/465DD223-47D3-49F7-8397-42BC5D0D5928");
        }

        [HttpGet("{id}")]
        public IActionResult GetNewDress(Guid id)
        {
            var model = new GetNewDressModel();
            model.Url = "TODO";
            return View(model);
        }

        [HttpPost("{id}")]
        public IActionResult UpdateDress(Guid id)
        {
            return Redirect("/dress");
            //remember to redirect to shop if we need to set up a new shop otherwise go to dress. (To do later)
        }
    }
}
