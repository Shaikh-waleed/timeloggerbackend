using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TimeLogger.Infrastructure.Utility.Extensions
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, string> ToKeyValue(this object data)
        {
            if (data == null)
                return null;

            JToken token = data as JToken;
            if (token == null)
                return ToKeyValue(JObject.FromObject(data));

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue();
                    if (childContent != null)
                        contentData = contentData.Concat(childContent).ToDictionary(k => k.Key, v => v.Value);
                }
                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
                return null;

            var value = jValue?.Type == JTokenType.Date ?
                            jValue?.ToString("o", CultureInfo.InvariantCulture) :
                            jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }
    }
}
