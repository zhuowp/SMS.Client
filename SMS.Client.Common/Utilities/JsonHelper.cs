using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SMS.Client.Common.Utilities
{
    public static class JsonHelper
    {
        public static string ToJson(this object obj)
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
            };

            string jsonStr = JsonSerializer.Serialize(obj, serializerOptions);
            return jsonStr;
        }

        public static T FromJson<T>(this string json)
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };

            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static string ToJsonFile(this object obj, string filePath, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            string json = ToJson(obj);
            File.WriteAllText(filePath, json, encoding);
            return json;
        }

        public static T FromJsonFile<T>(this string filePath, Encoding encoding = null)
        {
            if (!File.Exists(filePath))
            {
                return default;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            string jsonStr = File.ReadAllText(filePath);
            T data = FromJson<T>(jsonStr);

            return data;
        }
    }
}
