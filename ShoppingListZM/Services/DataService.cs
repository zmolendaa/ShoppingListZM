using ShoppingListZM.Models;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace ShoppingListZM.Services;

public class DataService
{
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "data.xml");

    public ObservableCollection<Category> LoadData()
    {
        var categories = new ObservableCollection<Category>();
        if (!File.Exists(_filePath)) CreateDefaultData();

        try
        {
            var xml = XDocument.Load(_filePath);
            foreach (var catXml in xml.Root.Element("Categories").Elements("Category"))
            {
                var category = new Category { Name = catXml.Attribute("Name").Value };
                foreach (var prodXml in catXml.Element("Products").Elements("Product"))
                {
                    category.Products.Add(new Product
                    {
                        Name = prodXml.Attribute("Name")?.Value ?? "Produkt",
                        Unit = prodXml.Attribute("Unit")?.Value ?? "szt.",
                        Quantity = int.TryParse(prodXml.Attribute("Quantity")?.Value, out int q) ? q : 1,
                        IsPurchased = bool.TryParse(prodXml.Attribute("IsPurchased")?.Value, out bool p) && p
                    });
                }
                categories.Add(category);
            }
        }
        catch { }
        return categories;
    }

    public void SaveData(ObservableCollection<Category> categories)
    {
        var xml = new XDocument(new XElement("ShoppingList", new XElement("Categories",
            from c in categories
            select new XElement("Category", new XAttribute("Name", c.Name),
                new XElement("Products", from p in c.Products
                                         select new XElement("Product",
                    new XAttribute("Name", p.Name), new XAttribute("Unit", p.Unit),
                    new XAttribute("Quantity", p.Quantity), new XAttribute("IsPurchased", p.IsPurchased)
                ))
            )
        )));
        xml.Save(_filePath);
    }



    private void CreateDefaultData()
    {
        var defaultData = new XDocument(new XElement("ShoppingList", new XElement("Categories",
            new XElement("Category", new XAttribute("Name", "Nabiał"), new XElement("Products",
                new XElement("Product", new XAttribute("Name", "Mleko"), new XAttribute("Unit", "szt."), new XAttribute("Quantity", "1"), new XAttribute("IsPurchased", "False"))
            ))
        )));
        defaultData.Save(_filePath);
    }
}