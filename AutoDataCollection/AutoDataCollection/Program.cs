using AngleSharp;
using AutoDataCollection.Models;
using AutoDataCollection.Services;
using CsvHelper;
using System.Globalization;
using System.Text;

//AngleSharp
var config = Configuration.Default.WithDefaultLoader();
var context = BrowsingContext.New(config);
//Получаем страницу сайта
var document = await context.OpenAsync("https://www.toy.ru/catalog/boy_transport/");
//CSV
using var fs = new StreamWriter("Products.csv", false, Encoding.UTF8);
using var csvWriter = new CsvWriter(fs, CultureInfo.CurrentCulture);
//Создаем заголовки для полей в файле "Products.csv"
csvWriter.WriteHeader<Product>();
csvWriter.NextRecord();

//Количество страниц
var countPages = int.Parse(document.QuerySelectorAll(".pagination a").Skip(3).FirstOrDefault().TextContent);
//Основной класс
ParserLinks parser = new ParserLinks();
for (int i = 1; i <= countPages; i++)
{
    //Получаем ископые результаты с сайта
    var products = await parser.Parse(i);
    //Записываем результаты в файл
    csvWriter.WriteRecords(products);
}




