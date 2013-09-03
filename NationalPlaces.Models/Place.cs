using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace NationalPlaces.Models
{
    [DataContract]
    public class Place
    {
        [BsonIgnore]
        private ICollection<BsonObjectId> users;

        private ICollection<Comment> comments;

        [BsonConstructor]
        public Place()
        {
            this.users = new HashSet<BsonObjectId>();
            this.comments = new HashSet<Comment>();
        }

        public BsonObjectId Id { get; set; }

        [DataMember]
        [Required, MaxLength(300)]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        [Required]
        public int PlaceIndentifierNumber { get; set; }

        [DataMember]
        [Required]
        public int Group { get; set; }


        [DataMember]
        [Required]
        public double Longitude { get; set; }

        [DataMember]
        [Required]
        public double Latitude { get; set; }

        [DataMember]
        [Required]
        public string Information { get; set; }



        [DataMember]
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
