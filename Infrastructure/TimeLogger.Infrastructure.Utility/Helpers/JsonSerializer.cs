using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Helpers
{
    //public class JsonSerializer : IJsonSerializer
    //{
    //    public T Deserialize<T>(string jsonString)
    //    {
    //        return JsonConvert.DeserializeObject<T>(jsonString);
    //    }

    //    public Task<T> DeserializeAsync<T>(string jsonString)
    //    {
    //        return Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(jsonString));
    //    }

    //    public string Serialize(object obj)
    //    {
    //        return JsonConvert.SerializeObject(obj);
    //    }
    //    public Task<string> SerializeAsync(object obj)
    //    {
    //        return Task.Factory.StartNew(() => JsonConvert.SerializeObject(obj));

    //    }
    //}

    public static class JsonSerializer
    {
        public static T Deserialize<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static Task<T> DeserializeAsync<T>(string jsonString)
        {
            return Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(jsonString));
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static Task<string> SerializeAsync(object obj)
        {
            return Task.Factory.StartNew(() => JsonConvert.SerializeObject(obj));

        }
    }
}
