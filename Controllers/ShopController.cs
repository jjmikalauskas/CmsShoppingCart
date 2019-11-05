using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmsShoppingCart.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            List<CategoryVM> catVMList;
            using (Db db = new Db())
            {
                catVMList = db.Category.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList(); 

            }
            return PartialView(catVMList);
        }

        // GET: /shop/category/name
        public ActionResult Category(string name)
        {
            List<ProductVM> prodVMList;

            using (Db db = new Db())
            {
                CategoryDTO categoryDTO = db.Category.Where(x => x.Slug == name).FirstOrDefault();
                int catId = categoryDTO.Id;

                prodVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;

            }

            return View(prodVMList); 
        }

        // GET: /shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductVM model;
            ProductDTO dto;
            int prodId;

            using (Db db = new Db())
            {
                if (!db.Products.Any(x => x.Slug==name))
                {
                    return RedirectToAction("Index", "Shop");
                }
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();
                prodId = dto.Id;
                model = new ProductVM(dto);                
            }

            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + prodId + "/Gallery/Thumbs"))
                   .Select(fn => Path.GetFileName(fn));

            return View("ProductDetails", model); 
        }

    }
}