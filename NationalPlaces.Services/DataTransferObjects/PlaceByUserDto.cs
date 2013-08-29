using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NationalPlaces.Services.DataTransferObjects
{
    public class PlaceByUserDto
    {
        public MongoDB.Bson.BsonObjectId Id { get; set; }

        public int PlaceIndentifierNumber { get; set; }

        public string Name { get; set; }

        public DateTime VisitDate { get; set; }
    }
}