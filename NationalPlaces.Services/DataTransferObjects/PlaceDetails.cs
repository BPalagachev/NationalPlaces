using NationalPlaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web;

namespace NationalPlaces.Services.DataTransferObjects
{
    [DataContract]
    public class PlaceDetailsDto
    {
        public static Expression<Func<Place, PlaceDetailsDto>> FromPlace
        {
            get
            {
                return place => new PlaceDetailsDto()
                {
                    Name = place.Name,
                    Url = place.Url,
                    PlaceIndentifierNumber = place.PlaceIndentifierNumber,
                    Group = place.Group,
                    Longitude = place.Longitude,
                    Latitude = place.Latitude,
                };
            }

        }


        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "placeIndentifier")]
        public int PlaceIndentifierNumber { get; set; }

        [DataMember(Name = "group")]
        public int Group { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }
    }
}