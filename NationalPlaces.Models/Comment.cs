using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace NationalPlaces.Models
{
    public class Comment
    {
        public string UserNickName { get; set; }

        public string Text { get; set; }
    }
}
