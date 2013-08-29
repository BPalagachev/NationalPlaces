using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace NationalPlaces.Models
{
    public class Place
    {
        [BsonIgnore]
        private ICollection<BsonObjectId> users;

        [BsonIgnore]
        private ICollection<Comment> comments;

        [BsonConstructor]
        public Place()
        {
            this.users = new HashSet<BsonObjectId>();
            this.comments = new HashSet<Comment>();
        }

        public BsonObjectId Id { get; set; }

        [Required, MaxLength(300)]
        public string Name { get; set; }

        public string Url { get; set; }
        
        [Required]
        public int PlaceIndentifierNumber { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double Latitude { get; set; }

        public virtual ICollection<Comment> Comments
        {
            get
            {
                return this.comments;
            }
            set
            {
                this.comments = value;
            }
        }

        public virtual ICollection<BsonObjectId> Users
        {
            get
            {
                return this.users;
            }
            set
            {
                this.users = value;
            }
        }
    }
}
