using NationalPlaces.Models;
using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace NationalPlaces.Services.DataTransferObjects
{
    [DataContract]
    public class UserLoggedInDto
    {
        public static Expression<Func<User, UserLoggedInDto>> FromUser
        {
            get
            {
                return user => new UserLoggedInDto()
                {
                    NickName = user.NickName,
                    SessionKey = user.SessionKey
                };
            }

        }

        [DataMember(Name = "displayName")]
        public string NickName { get; set; }

        [DataMember(Name = "sessionKey")]
        public string SessionKey { get; set; }
    }
}