using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger.Infrastructure.Utility.Extensions
{
    public static class JsonExtensions
    {
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Get all properties namesfrom the model present in the JSON.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="rawJson">The Json body request.</param>
        /// <returns>List of keys.</returns>
        public static HashSet<string> GetModelPropertiesNames<T>(this string rawJson)
        {
            if (string.IsNullOrWhiteSpace(rawJson))
            {
                return new HashSet<string>();
            }

            JObject json = JObject.Parse(rawJson);

            var jsonKeys = json.Properties().Select(key => key.Name).ToHashSet();

            if (!jsonKeys.Any())
            {
                return new HashSet<string>();
            }

            var modelProperties = typeof(T).GetProperties().Select(p => p.Name).ToHashSet();

            // Get only the properties name contained in the json.
            return modelProperties.Intersect(jsonKeys, StringComparer.InvariantCultureIgnoreCase).ToHashSet();
        }
        public static string GetPropertyFromJsonString(this string rawJson, string propertyName)
        {
            var jsonObject = JObject.Parse(rawJson);
            propertyName = propertyName.FirstCharToLower();
            if (jsonObject.SelectToken(propertyName) == null)
                return string.Empty;

            return jsonObject.Property(propertyName).ToString();
        }

        public static string AddPropertyInJsonString(this string rawJson, string propertyName, JToken propertyValue)
        {
            var jsonObject = JObject.Parse(rawJson);
            propertyName = propertyName.FirstCharToLower();
            if (jsonObject.SelectToken(propertyName) == null)
                jsonObject.Add(propertyName, propertyValue);
            rawJson = jsonObject.ToString();
            return rawJson;
        }

        public static string RemovePropertyFromJsonString(this string rawJson, string propertyName)
        {
            var jsonObject = JObject.Parse(rawJson);
            propertyName = propertyName.FirstCharToLower();
            if (jsonObject.SelectToken(propertyName) != null)
                jsonObject.Property(propertyName).Remove();
            rawJson = jsonObject.ToString();
            return rawJson;
        }

        public static bool PropertyExistsInJsonString(this string rawJson, string propertyName)
        {
            var jsonObject = JObject.Parse(rawJson);
            propertyName = propertyName.FirstCharToLower();
            var property = jsonObject.SelectToken(propertyName);
            return property != null;
        }

        public static string GetObjectFromJsonString(this string rawJson, string objectName)
        {
            var jsonObject = JObject.Parse(rawJson);
            objectName = objectName.FirstCharToLower();
            if (jsonObject.SelectToken(objectName) == null)
                return string.Empty;

            var propertyObject = jsonObject.Property(objectName);
            return propertyObject.Value.ToString();
        }
    }
}
