using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public interface IKeyRepository
    {
        Key AddKey(Key newKey);
        Key GetKey(String keyName);
        List<Key> GetKeys();
        Key RemoveKey(String keyName);
    }
}
