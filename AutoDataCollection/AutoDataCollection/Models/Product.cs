namespace AutoDataCollection.Models
{
    public class Product
    {

        public string RegionName { get; set; } //Название региона

        public string NavigationChain { get; set; } //Хлебные крошки

        public string Name { get; set; } //Название товара

        public decimal Price { get; set; } //Цена

        public decimal OldPrice { get; set; } //Цена старая

        public string Availability { get; set; } //Наличие

        public string ProductUrl { get; set; } //Ссылка на товар

        public string ImageUrl { get; set; } //Ссылки на картинки 
    }
}
