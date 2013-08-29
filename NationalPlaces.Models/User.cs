using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NationalPlaces.Models
{
    public class User
    {
        [BsonConstructor]
        public User()
        {
            this.VisitedPlaces = new List<int>();
        }

        public BsonObjectId Id { get; set; }

        [Required, MinLength(6), MaxLength(30)]
        public string UserName { get; set; }

        [Required, MinLength(6), MaxLength(30)]
        public string NickName { get; set; }

        [Required, StringLength(40)]
        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        public IList<int> VisitedPlaces { get; set; }
    }
}
