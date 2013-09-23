using MongoDB.Bson;
using NationalPlaces.DataLayer;
using NationalPlaces.Models;
using NationalPlaces.Services.Attributes;
using NationalPlaces.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ValueProviders;

namespace NationalPlaces.Services.Controllers
{
    public class UsersController : BaseApiController
    {
        private static readonly string SessionKeyChars = "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
        private static Random rand = new Random();
        private const int SessionKeyLength = 50;

        [ActionName("register")]
        [HttpPost]
        public HttpResponseMessage Register(UserRegisterDto registerDto)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                if (ModelState.IsValid && registerDto != null)
                {
                    var existingUser = NationalPlacesDAL.Get<User>("UsersInformation")
                        .FirstOrDefault(x => x.UserName == registerDto.UserName.ToLower() ||
                                             x.NickName.ToLower() == registerDto.NickName.ToLower());
                    if (existingUser != null)
                    {
                        throw new InvalidOperationException("User name or nickname is already taken");
                    }

                    var newUser = UserRegisterDto.CreateUser(registerDto);
                    NationalPlacesDAL.Add(newUser, "UsersInformation");

                    newUser.SessionKey = this.GenerateSessionKey(newUser.Id.Value.Pid);
                    NationalPlacesDAL.SaveEntity(newUser, "UsersInformation");
                   

                    var loginInforrmation = UserLoggedInDto.FromUser.Compile()(newUser);

                    var response = Request.CreateResponse(HttpStatusCode.Created, loginInforrmation);
                    return response;
                }
                else
                {
                    var errors = String.Join("\n ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    var errorMessage = string.Format("User input was not validated:\n {0}", errors);
                    throw new ArgumentException(errorMessage);
                }
            });

            return operationResult;
        }

        [ActionName("login")]
        public HttpResponseMessage Login(UserLogInDto loginInformation)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                if (ModelState.IsValid && loginInformation != null)
                {
                    var existingUser = NationalPlacesDAL.Get<User>("UsersInformation")
                        .FirstOrDefault(x =>
                            x.UserName == loginInformation.UserName.ToLower()
                            && x.AuthCode == loginInformation.AuthCode);

                    if (loginInformation == null || existingUser == null)
                    {
                        throw new InvalidOperationException("User or password is incorrent");
                    }

                    if (existingUser.SessionKey == null)
                    {
                        existingUser.SessionKey = this.GenerateSessionKey(existingUser.Id.Value.Pid);
                        NationalPlacesDAL.SaveEntity(existingUser, "UsersInformation");
                    }

                    var loginInforrmation = UserLoggedInDto.FromUser.Compile()(existingUser);

                    var response = Request.CreateResponse(HttpStatusCode.Created, loginInforrmation);
                    return response;
                }
                else
                {
                    var errors = String.Join("\n ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    var errorMessage = string.Format("User input was not validated:\n {0}", errors);
                    throw new ArgumentException(errorMessage);
                }
            });

            return operationResult;
        }

        [ActionName("logout")]
        [HttpPut]
        public HttpResponseMessage Logout([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = NationalPlacesDAL.Get<User>("UsersInformation").FirstOrDefault(x => x.SessionKey == sessionKey);
                if (user != null)
                {
                    user.SessionKey = null;
                    NationalPlacesDAL.SaveEntity(user, "UsersInformation");
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    throw new InvalidOperationException("User or password is incorrect.");
                }
            });

            return operationResult;
        }


        private string GenerateSessionKey(short userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }
            return skeyBuilder.ToString();
        }
    }
}
