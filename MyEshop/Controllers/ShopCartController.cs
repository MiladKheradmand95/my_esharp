using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataLayer;

namespace MyEshop.Controllers
{
    public class ShopCartController : Controller
    {
        MyEshop_DBEntities db = new MyEshop_DBEntities();
        
        // GET: ShopCart
        public ActionResult ShowCart()
        {
            List<DataLayer.ShopCartItemViewModel> list = new List<DataLayer.ShopCartItemViewModel>();
            
            if (Session["ShopCart"]!=null)
            {
                List<DataLayer.ShopCartItem> listshop = (List<DataLayer.ShopCartItem>)Session["ShopCart"];
                foreach (var item in listshop)
                {
                    var product = db.Products.Where(p => p.ProductID == item.ProductId).Select(p => new
                    {
                        p.ImageName,
                        p.Title
                    }).Single();
                    list.Add(new ShopCartItemViewModel()
                    {
                        Count=item.Count,
                        ProductId=item.ProductId,
                        Title=product.Title,
                        ImageName=product.ImageName
                    });
                }
            }

            return PartialView(list);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Order()
        {
           
            return PartialView(getListOrder());
        }

        public ActionResult commandOrder(int id,int count)
        {
            List<ShopCartItem> list = Session["ShopCart"] as List<ShopCartItem>;
            int index = list.FindIndex(p => p.ProductId == id);
            if (count==0)
            {
                list.RemoveAt(index);
            }
            else
            {
                list[index].Count = count;
            }
            Session["ShopCart"] = list;
            return PartialView("Order", getListOrder());
        }
        List<ShowOrderViewModel> getListOrder()
        {
            List<ShowOrderViewModel> list = new List<ShowOrderViewModel>();

            if (Session["ShopCart"] != null)
            {
                List<ShopCartItem> listShop = Session["ShopCart"] as List<ShopCartItem>;

                foreach (var item in listShop)
                {
                    var product = db.Products.Where(p => p.ProductID == item.ProductId).Select(p => new
                    {
                        p.ImageName,
                        p.Title,
                        p.Price
                    }).Single();
                    list.Add(new ShowOrderViewModel()
                    {
                        Count = item.Count,
                        ProductId = item.ProductId,
                        Price = product.Price,
                        ImageName = product.ImageName,
                        Title = product.Title,
                        Sum = item.Count * product.Price
                    });
                }
            }
            return list;
        }

        [Authorize]
        public ActionResult Payment()
        {
            int UserId = db.Users.Single(u => u.UserName == User.Identity.Name).UserID;
            Order order = new Order()
            {
                UserID = UserId,
                Date = DateTime.Now,
                IsFinaly = false,
            };
            db.Orders.Add(order);
            var listDetials = getListOrder();
            foreach (var item in listDetials)
            {
                db.OrderDetails.Add(new OrderDetail()
                {
                    Count = item.Count,
                    OrderID = order.OrderID,
                    Price = item.Price,
                    ProductID = item.ProductId
                });
            }
            db.SaveChanges();
            
            //TODO: Online Payment

            return null;
        }
    }
}