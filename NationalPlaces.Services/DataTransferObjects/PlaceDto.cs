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
    public class PlaceDto
    {
        public static Expression<Func<Place, PlaceDto>> FromPlace
        {
            get
            {
                return place => new PlaceDto()
                {
                    Name = place.Name,
                    Url = place.Url,
                    PlaceIndentifierNumber = place.PlaceIndentifierNumber,
                    Group =place.Group,
                    Longitude = place.Longitude,
                    Latitude = place.Latitude,
                    Comments = place.Comments.Select(comment => new CommentDto()
                    {
                        Author = comment.UserNickName,
                        Content = comment.Text
                    })
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

        [DataMember]
        public virtual IEnumerable<CommentDto> Comments { get; set; }
    }
}