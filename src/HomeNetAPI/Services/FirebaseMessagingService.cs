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

namespace HomeNetAPI.Services
{
    public class FirebaseMessagingService : IFirebaseMessagingService
    {
        //Source: https://stackoverflow.com/questions/37750451/send-http-post-message-in-asp-net-core-using-httpclient-postasjsonasync 
        //Source: https://firebase.google.com/docs/cloud-messaging/server 
        //Source: https://code.msdn.microsoft.com/windowsapps/How-to-use-HttpClient-to-b9289836/view/SourceCode#content 
        public async Task<bool> SendFirebaseMessage(string title, string message, string token, string key)
        {
            String link = "https://fcm.googleapis.com/fcm/send";
            Uri firebaseUri = new Uri("link");
            HttpClient client = new HttpClient();
            client.BaseAddress = firebaseUri;
            client.DefaultRequestHeaders.Add("Content-Type:", "application/json");
            client.DefaultRequestHeaders.Add("Authorization:", $"key= {key}");
            FirebaseMessage firebaseMessage = new FirebaseMessage();
            
            firebaseMessage.to = token;
            firebaseMessage.data = new Data()
            {
                title = title,
                message = message,
                dateSent = DateTime.Now.ToString(),
            };
            String serializer = JsonConvert.SerializeObject(firebaseMessage);
            HttpResponseMessage response = await client.PostAsync(link, new StringContent(serializer, Encoding.UTF8, "application/json"));
            String fromServer = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return true;
            } else
            {
                return false;
            }   
        }
    }
}
