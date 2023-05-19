using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using API.Models;
using System.Collections.Generic;

namespace API.Data
{
    public class ProductContext
    {

        public ProductContext(IConfiguration configuration) {

            var cliente = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = cliente.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Products = database.GetCollection<Product>(configuration["DatabaseSettings:CollectionName"]);
            SeedData(Products);
        }

        public IMongoCollection<Product> Products { get; }

        private static void SeedData(IMongoCollection<Product> coleccion) { 
            
            bool existProd=  coleccion.Find(p => true).Any();
            if (!existProd) {
                coleccion.InsertManyAsync(getPreconfiguredProducts());
            }
        }

        private static IEnumerable<Product> getPreconfiguredProducts() {

            return new List<Product>()
            {
                new Product()
                {
                    Name = "IPhone X",
                    Description = "Desarrollado por Apple INC Y EXPORTADO",
                    ImageFile = "product-1.png",
                    Price = "950.00",
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Name = "Samsung 10",
                    Description = "Desarrollado por Samsung.",
                    ImageFile = "product-2.png",
                    Price = "840.00",
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Name = "Huawei Plus",
                    Description = "Desarrollado por Huawei en Argentina.",
                    ImageFile = "product-3.png",
                    Price = "650.00",
                    Category = "Smart Phones"
                },
                new Product()
                {
                    Name = "Xiaomi Mi 9",
                    Description = "Desarrollado y ensamblando en Tierra del Fuego por Xiaomi.",
                    ImageFile = "product-4.png",
                    Price = "470.00",
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Name = "LG G7 ThinQ EndofCourse",
                    Description = "Nuevo LG Desarrollado en Cordoba.",
                    ImageFile = "product-6.png",
                    Price = "240.00",
                    Category = "Smart Phone"
                }
            };
            
        }

    }
}
