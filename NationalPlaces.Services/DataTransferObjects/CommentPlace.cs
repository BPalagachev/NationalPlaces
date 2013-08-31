using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NationalPlaces.Services.DataTransferObjects
{
    [DataContract]
    public class CommentPlace
    {
        [DataMember(Name="identifier")]
        public int PlaceIndentifierNumber { get; set; }

        [DataMember(Name="content")]
        public string Content { get; set; }

        [DataMember(Name = "coordstoken")]
        public string LocationToken { get; set; }
    }
}