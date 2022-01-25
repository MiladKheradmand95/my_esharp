using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLayer;
using System.Web;
using WebMarkupMin.AspNet4.Mvc;

namespace MyEshop.Controllers
{
    [MinifyHtml]
    public class ShopController : ApiController
    {
        // GET: api/Shop
        public int Get()
        {
            List<ShopCartItem> list = new List<ShopCartItem>();
            var sessions = HttpContext.Current.Session;
            if (sessions["ShopCart"] !=null)
            {
                list = sessions["ShopCart"] as List<ShopCartItem>;
            }
            return list.Sum(l => l.Count);
        }

        // GET: api/Shop/5
        public int Get(int id)
        {
            List<ShopCartItem> list = new List<ShopCartItem>();
            var sessions = HttpContext.Current.Session;
            if (sessions["ShopCart"]!=null)
            {
                list=sessions["ShopCart"] as List<ShopCartItem>;
            }
            if (list.Any(l=>l.ProductId==id))
            {
                int index = list.FindIndex(p => p.ProductId == id);
                list[index].Count += 1;
            }
            else
            {
                list.Add(new ShopCartItem()
                {
                    ProductId = id,
                    Count = 1
                });
            }
            sessions["ShopCart"] = list;
            return Get();
        }


    }
}
