using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NationalPlaces.Services.DataTransferObjects
{
    [DataContract]
    public class VititPlaceDto
    {
        [DataMember(Name="coordstoken")]
        public string CoordsToken { get; set; }
        [DataMember(Name="placeId")]
        public int PlaceId { get; set; }
    }
}