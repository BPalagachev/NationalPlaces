using NationalPlaces.DataLayer;
using NationalPlaces.Services.Attributes;
using NationalPlaces.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using System.Web.Providers.Entities;

namespace NationalPlaces.Services.Controllers
{
    public class PlacesController : BaseApiController
    {

        [HttpGet]
        public IEnumerable<NationalPlaces.Models.Place> AllPlaces([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var allPlaces = NationalPlacesDAL.Get<NationalPlaces.Models.Place>("PlaceInformation");
                return allPlaces;

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
            double longitude, double latitude)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = NationalPlacesDAL.Get<NationalPlaces.Models.User>("UsersInformation")
                    .FirstOrDefault(x => x.SessionKey == sessionKey);

                if (user == null)
                {
                    throw new InvalidOperationException("User or password is incorrect.");
                }

                // parse coordinates
                // get places by coordinates
                var avaiablePlaces = GetNearPlaces(longitude, latitude).Select(x=>x.PlaceIndentifierNumber);
                if (avaiablePlaces == null)
                {
                    throw new InvalidOperationException("There are no places near by.");
                }

                foreach (var place in avaiablePlaces)
                {
                    user.VisitedPlaces.Add(place);
                }

                NationalPlacesDAL.SaveEntity(user, "UsersInformation");

                return Request.CreateResponse(HttpStatusCode.OK);
            });

            return operationResult;
        }

        [ActionName("comment")]
        [HttpPost]
        public HttpResponseMessage CommentPlace([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey,
            double longitude, double latitude, CommentPlace comment)
        {
            var operationResult = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = NationalPlacesDAL.Get<NationalPlaces.Models.User>("UsersInformation")
                    .FirstOrDefault(x => x.SessionKey == sessionKey);

                if (user == null)
                {
                    throw new InvalidOperationException("User or password is incorrect.");
                }

                // parse coordinates
                // get places by coordinates
                var avaiablePlaces = GetNearPlaces(longitude, latitude);
                if (avaiablePlaces == null)
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

            var allPlaces = NationalPlacesDAL.Get<NationalPlaces.Models.Place>("PlaceInformation")
                .Where(x => Math.Acos(Math.Sin(x.Latitude * scale) * Math.Sin(latitude * scale)
                    + Math.Cos(x.Latitude * scale) * Math.Cos(latitude * scale) * Math.Cos(x.Longitude*scale - longitude*scale)) * earthRadius < 3);
            return allPlaces;
        }

    }
}
