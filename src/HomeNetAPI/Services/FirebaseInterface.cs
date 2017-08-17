using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.ViewModels;

namespace HomeNetAPI.Services
{
    [Header("Content-Type", "application/json")]
    [Header("Authorization", "key=AAAATv-qp_M:APA91bEO9NlvgWWhoFa1OFBbMGnAxFYxw6Pfue353yYyYDg25bJGrRQ8bUe2sirS1KkjSZZgBLMi_sSV23ysbTb16qi48jKqgNktDYG8rl996q-Pm5drZv1Ym4S4Dzyh30P0KPrkoOQ1")]
    [Header("Sender", "id=339296823283")]
    [AllowAnyStatusCode]
    public interface FirebaseInterface
    {
        //Source: https://fcm.googleapis.com/fcm/send 
        [Post("/fcm/send")]
        Task<Response<String>> SendFirebaseMessage([Body] String newObject);



    }
}
