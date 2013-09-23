using NationalPlaces.DataLayer;
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
using System.Web.Providers.Entities;

namespace NationalPlaces.Services.Controllers
{
    public class PlacesController : BaseApiController
    {

        [HttpGet]
        [ActionName("getall")]
        public IEnumerable<PlaceDto> AllPlaces()
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var allPlaces = NationalPlacesDAL
                    .Get<NationalPlaces.Models.Place>("PlaceInformation")
                    .Select(PlaceDto.FromPlace);

                return allPlaces;

            });

            return operationResult;
        }

        [ActionName("details")]
        [HttpGet]
        public PlaceDetailsDto Details(int identifier)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var place = NationalPlacesDAL
                    .Get<NationalPlaces.Models.Place>("PlaceInformation")
                    .Where(x => x.PlaceIndentifierNumber == identifier)
                    .Select(PlaceDetailsDto.FromPlace)
                    .FirstOrDefault();

                if (place == null )
                {
                    throw new ArgumentException("This place is not registered in the database");
                }

                return place;

            });

            return operationResult;
        }

        [ActionName("getcomments")]
        [HttpGet]
        public IEnumerable<CommentDto> GetComments(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey,
            int identifier)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = NationalPlacesDAL.Get<NationalPlaces.Models.User>("UsersInformation")
                .FirstOrDefault(x => x.SessionKey == sessionKey);
                if (user == null)
                {
                    throw new InvalidOperationException("You need to be logged in to view comments.");
                }

                var place = NationalPlacesDAL
                    .Get<NationalPlaces.Models.Place>("PlaceInformation")
                    .Where(x => x.PlaceIndentifierNumber == identifier)
                    .FirstOrDefault();

                if (place == null)
                {
                    throw new ArgumentException("place not found");
                }

                List<CommentDto> comments = new List<CommentDto>();

                foreach (var comment in place.Comments)
                {
                    if (comment.UserNickName == user.NickName)
                    {
                        comments.Add(new CommentDto()
                        {
                            Author = comment.UserNickName,
                            Content = comment.Text
                        });
                    }
                    
                }
                comments.Reverse();

                return comments;

            });

            return operationResult;
        }

        [ActionName("myplaces")]
        [HttpGet]
        public IEnumerable<int> MyPlaces([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = NationalPlacesDAL.Get<NationalPlaces.Models.User>("UsersInformation")
                    .FirstOrDefault(x => x.SessionKey == sessionKey);
                if (user != null)
                {
                    var visitedPlaces = user.VisitedPlaces;
                    return visitedPlaces;
                }
                else
                {
                    throw new InvalidOperationException("User or password is incorrect.");
                }
            });

            return operationResult;
        }

        [ActionName("visit")]
        [HttpPost]
        public HttpResponseMessage VisitPlace([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey,
            VititPlaceDto placeToVisit)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = NationalPlacesDAL.Get<NationalPlaces.Models.User>("UsersInformation")
                    .FirstOrDefault(x => x.SessionKey == sessionKey);

                if (user == null)
                {
                    throw new InvalidOperationException("User or password is incorrect.");
                }
                if (placeToVisit == null)
                {
                    throw new InvalidOperationException("Token Validation failed");

                }

                double longitude = 0;
                double latitude = 0;
                DecryptCoordinateToken(placeToVisit.CoordsToken, user.AuthCode, ref longitude, ref latitude);

                // parse coordinates
                // get places by coordinates
                var placeToVIsit = GetNearPlaces(longitude, latitude)
                                        .Where(x=>x.PlaceIndentifierNumber==placeToVisit.PlaceId)
                                        .Select(x => x.PlaceIndentifierNumber)
                                        .FirstOrDefault();
                if (placeToVIsit == 0)
                {
                    throw new InvalidOperationException("This place is not near you!");
                }

                user.VisitedPlaces.Add(placeToVIsit);

                NationalPlacesDAL.SaveEntity(user, "UsersInformation");

                return Request.CreateResponse(HttpStatusCode.OK);
            });

            return operationResult;
        }

        [ActionName("comment")]
        [HttpPost]
        public HttpResponseMessage CommentPlace([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey, CommentPlace comment)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = NationalPlacesDAL.Get<NationalPlaces.Models.User>("UsersInformation")
                    .FirstOrDefault(x => x.SessionKey == sessionKey);

                if (user == null)
                {
                    throw new InvalidOperationException("User or password is incorrect.");
                }

                double longitude = 0;
                double latitude = 0;
                DecryptCoordinateToken(comment.LocationToken, user.AuthCode, ref longitude, ref latitude);
                var avaiablePlaces = GetNearPlaces(longitude, latitude);
                if (avaiablePlaces == null || avaiablePlaces.Count() == 0)
                {
                    throw new InvalidOperationException("There are no places near by.");
                }

                var placeToComment = avaiablePlaces.Where(x => x.PlaceIndentifierNumber == comment.PlaceIndentifierNumber).FirstOrDefault();
                if (placeToComment == null)
                {
                    throw new InvalidOperationException("You cant comment this place. It is not near you.");
                }

                var newComment = new NationalPlaces.Models.Comment()
                {
                    UserNickName = user.NickName,
                    Text = comment.Content
                };

                placeToComment.Comments.Add(newComment);
                NationalPlacesDAL.SaveEntity(placeToComment, "PlaceInformation");

                return Request.CreateResponse(HttpStatusCode.OK);
            });

            return operationResult;
        }


        private IEnumerable<NationalPlaces.Models.Place> GetNearPlaces(double longitude, double latitude)
        {
            var scale = Math.PI / 180;
            var earthRadius = 6371;

            var allPlaces = NationalPlacesDAL.Get<NationalPlaces.Models.Place>("PlaceInformation").ToList()
                .Where(x => Math.Acos(Math.Sin(x.Latitude * scale) * Math.Sin(latitude * scale)
                    + Math.Cos(x.Latitude * scale) * Math.Cos(latitude * scale) * Math.Cos(x.Longitude * scale - longitude * scale)) * earthRadius < 3);
            return allPlaces;
        }

        private static void DecryptCoordinateToken(string token, string userSha1, ref double longitude, ref double latitude)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < token.Length; i++)
            {
                var currentChar = token[i] ^ userSha1[i % userSha1.Length];
                sb.Append((char)currentChar);
            }

            var coortinates = sb.ToString().Split(';');
            longitude = 0;
            latitude = 0;

            var longitudeParsed = double.TryParse(coortinates[1], out longitude);
            var latitudeParse = double.TryParse(coortinates[0], out latitude);

            if (!longitudeParsed || !latitudeParse)
            {
                throw new ArgumentException("Coordinates not excepted");
            }
        }

    }
}
