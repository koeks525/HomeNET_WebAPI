using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HomeNetAPI.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Text;
using RestEase;

namespace HomeNetAPI.Services
{
    public class FirebaseMessagingService : IFirebaseMessagingService
    {

        public async Task<bool> SendFirebaseMessage(int id, string title, string message, string token, string key)
        {
           
            FirebaseInterface service = RestClient.For<FirebaseInterface>("https://fcm.googleapis.com");
            var data = new
            {
                to = token,
                data = new
                {
                    body = message,
                    title = title,
                    dataID = id,

                },
                priority = "high"
            };
            var serializedData = JsonConvert.SerializeObject(data);
            var sendTask = await service.SendFirebaseMessage(serializedData);
            if (sendTask.ResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //var responseData = JsonConvert.DeserializeObject(sendTask.StringContent);
                return true;
            } else
            {
               //Request could not be sent for some reason. 
                return false;
            }
        }
    }
}
