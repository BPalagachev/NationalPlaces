using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NationalPlaces.Services.DataTransferObjects
{
    [DataContract]
    public class CommentDto
    {
        [DataMember(Name="authorname")]
        public string Author { get; set; }

        [DataMember(Name="content")]
        public string Content { get; set; }
    }
}
