using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeNetAPI.Models;

namespace HomeNetAPI.Repository
{
    public class KeyRepository : IKeyRepository
    {
        private HomeNetContext homeNetContext;
        public KeyRepository(HomeNetContext homeNetContext)
        {
            this.homeNetContext = homeNetContext;
        }

        public Key AddKey(Key newKey)
        {
            var addedKey = homeNetContext.Keys.Add(newKey);
            homeNetContext.SaveChanges();
            return addedKey.Entity;
        }

        public Key GetKey(string keyName)
        {
            var selectedKey = homeNetContext.Keys.FirstOrDefault(k => k.Name == keyName);
            return selectedKey;
        }

        public List<Key> GetKeys()
        {
            return homeNetContext.Keys.ToList();
        }

        public Key RemoveKey(string keyName)
        {
            var selectedKey = homeNetContext.Keys.FirstOrDefault(key => key.Name == keyName);
            selectedKey.IsDeleted = 1;
            homeNetContext.SaveChanges();
            return selectedKey;
        }
    }
}
