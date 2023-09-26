using AngleSharp;
using AutoDataCollection.Interfaces;
using AutoDataCollection.Models;
using System.Text;

namespace AutoDataCollection.Services
{
    public class ParserLinks
    {
        private ParserSetting _parserSetting;

        public ParserLinks()
        {
            _parserSetting = new ParserSetting()
            {
                BaseUrl = "https://www.toy.ru",
                DelayRequest = 2000
            };
        }

        public async Task<List<Product>> Parse(int numberPage)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(_parserSetting.BaseUrl + "/catalog/boy_transport/" + "?PAGEN_5=" + numberPage);
            //Получаем ссылки на товары 
            var productItems = document.QuerySelectorAll(".card-preview .name").Select(x => x.GetAttribute("href")).ToList();
            //Освобождаем память 
            context.Dispose();

            var products = new List<Product>();

            foreach (var productItem in productItems)
            {
                //Задержка потока 
                Thread.Sleep(_parserSetting.DelayRequest);
                //Получаем данные о продукте
                Console.WriteLine("Получаем информацию о продукте");
                var product = await ParseProduct(productItem);
                //Добавляем в список продуктов
                products.Add(product);
                Console.WriteLine("Добавили в список продуктов");
            }
            Console.WriteLine("Получены все продукты с текущей страницы");
            return products;
        }

        public async Task<Product> ParseProduct(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(_parserSetting.BaseUrl + url);

            var product = new Product();

            var name = document.QuerySelector(".content-title").TextContent;
            //Название продукта
            product.Name = name;
            //Ссылка на продукт
            product.ProductUrl = _parserSetting.BaseUrl + url;

            var regionName = document.QuerySelector("a.nav-link span").TextContent.Trim();
            //Регион
            product.RegionName = regionName;
            //Список ссылок изображений 
            List<string> imageUrls = new List<string>();

            var imageUrlItems = document.QuerySelectorAll(".swiper-slide");
            foreach (var imageItem in imageUrlItems)
            {
                if (imageItem.QuerySelector(".img-fluid") != null)
                {
                    var image = imageItem.QuerySelector(".img-fluid").GetAttribute("src").ToString();
                    imageUrls.Add(image + ";");
                }
            }

            foreach (string imageUrl in imageUrls)
            {
                product.ImageUrl += imageUrl;
            }

            //Навигационный путь(Хлебные крошки) 
            StringBuilder sb = new StringBuilder();
            var naviChainItems = document.QuerySelectorAll(".breadcrumb-item");
            foreach (var naviChainItem in naviChainItems)
            {
                string NavigationChain = naviChainItem.QuerySelector(".breadcrumb-item a").TextContent.ToString();
                sb.Append(NavigationChain + "/");
            }
            product.NavigationChain = sb.ToString();
            //Проверка на наличие продукта
            if (document.QuerySelector(".not-in-stock-text") == null)
            {
                //Проверка на старую цену
                if (document.QuerySelector(".old-price") != null)
                {
                    var oldPrice = decimal.Parse(document.QuerySelector(".old-price").TextContent.Replace("₽", "").Replace(" ", ""));
                    product.OldPrice = oldPrice;
                    var price = decimal.Parse(document.QuerySelectorAll(".buy-block div").Skip(6).Take(1).FirstOrDefault().TextContent.Replace("₽", "").Replace(" ", ""));
                    product.Price = price;
                }
                else
                {
                    var currprice = decimal.Parse(document.QuerySelectorAll(".buy-block div").Skip(5).Take(1).FirstOrDefault().TextContent.Replace("₽", "").Replace(" ", ""));
                    product.Price = currprice;
                }
                product.Availability = "В наличии";
            }
            else
            {
                product.Availability = "Нет в наличии";
            }
            //Освобождаем память
            context.Dispose();
            Console.WriteLine("Информация о продукте получена");
            return product;
        }



    }

}
