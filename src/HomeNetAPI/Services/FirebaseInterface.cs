using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.ViewModels;

namespace HomeNetAPI.Services
{
    [Header("Content-Type", "application/json")]
    public interface FirebaseInterface
    {
        //Source: https://fcm.googleapis.com/fcm/send 

        [Post("/fcm/send")]
        Task<FirebaseMessage> SendFirebaseMessage([Header("Authorization")] String key, [Body] FirebaseMessage message);



    }
}
