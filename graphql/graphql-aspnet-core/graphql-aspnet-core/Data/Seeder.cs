using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace graphql_aspnet_core.Data
{
    public static class Seeder
    {
        public static void Seed(CustomersEntitiesDataContext context, int count, string rootPath)
        {
            var addresses = context.Customers.Select(x => x.Address).ToList();
            var cities = context.Customers.Select(x => x.City).ToList();
            var companynames = context.Customers.Select(x => x.CompanyName).ToList();
            var contactnames = context.Customers.Select(x => x.ContactName).ToList();
            var contacttitles = context.Customers.Select(x => x.ContactTitle).ToList();
            var countries = context.Customers.Select(x => x.Country).ToList();
            var faxes = context.Customers.Select(x => x.Fax).ToList();
            var phones = context.Customers.Select(x => x.Phone).ToList();
            var postalcodes = context.Customers.Select(x => x.PostalCode).ToList();
            var random = new Random();
            var lineOfText = "";

            var filestream = new System.IO.FileStream(rootPath + "/cities.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            var file = new System.IO.StreamReader(filestream, System.Text.Encoding.ASCII, true, 128);
            context.Database.ExecuteSqlCommand("delete from Customers");

            while ((lineOfText = file.ReadLine()) != null)
            {
                cities.Add(lineOfText);
            }

            for (int i = 0; i < 50000; i++)
            {
                context.Customers.Add(new graphql_aspnet_core.Data.Entities.Customer
                {
                    Address = addresses[random.Next(0, addresses.Count - 1)],
                    City = cities[random.Next(0, 1000)],
                    CompanyName = companynames[random.Next(0, companynames.Count - 1)] + " " + cities[random.Next(0, cities.Count - 1)],
                    ContactName = contactnames[random.Next(0, contactnames.Count - 1)],
                    ContactTitle = contacttitles[random.Next(0, contacttitles.Count - 1)],
                    Country = countries[random.Next(0, countries.Count - 1)],
                    Fax = faxes[random.Next(0, faxes.Count - 1)],
                    Phone = phones[random.Next(0, phones.Count - 1)],
                    PostalCode = phones[random.Next(0, phones.Count - 1)],
                    CustomerID = Seeder.RandomString(15, random)
                });
            }

 

            context.SaveChanges();
        }

        public static string RandomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
