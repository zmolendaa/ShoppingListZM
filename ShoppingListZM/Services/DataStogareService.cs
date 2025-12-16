using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using ShoppingListZM.Models;

namespace ShoppingListZM.Services
{
    public class DataStorageService
    {
        private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "products.xml");
        private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(List<Product>));

        public void SaveProducts(ObservableCollection<Product> products)
        {
            try
            {
                using (var writer = new StreamWriter(_filePath))
                {
                    _xmlSerializer.Serialize(writer, products.ToList());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisu XML: {ex.Message}");
            }
        }

        public ObservableCollection<Product> LoadProducts()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    using (var reader = new StreamReader(_filePath))
                    {
                        var loadedList = (List<Product>)_xmlSerializer.Deserialize(reader);
                        return new ObservableCollection<Product>(loadedList);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd ładowania XML: {ex.Message}");
            }
            return new ObservableCollection<Product>();
        }
    }
}
