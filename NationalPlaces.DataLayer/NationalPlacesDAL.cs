using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalPlaces.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace NationalPlaces.DataLayer
{
    public static class NationalPlacesDAL
    {
        private static string connectionString = "mongodb://localhost";
        private static MongoClient client = new MongoClient(connectionString);
        private static MongoServer  server = client.GetServer();
        private static MongoDatabase database = server.GetDatabase("placesDb");

        public static IQueryable<T> Get<T>(string collectionName)
        {
            var collection = database.GetCollection<T>(collectionName);
            var query = collection.AsQueryable<T>();
            return query;
        }
        
        public static void Add<T>(T item, string collectionName)
        {
            var collection = database.GetCollection<T>(collectionName);
            collection.Insert(item);
        }

        public static void SaveEntity<T>(T item, string collectionName)
        {
            var collection = database.GetCollection<T>(collectionName);
            collection.Save(item);
        }
    }
}
