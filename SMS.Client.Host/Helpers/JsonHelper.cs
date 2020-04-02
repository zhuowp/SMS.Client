using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SMS.Client.Host.Helpers
{
    public static class JsonHelper
    {
        public static string ToJson(this object o, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = dateTimeFormat;
            string json = JsonConvert.SerializeObject(o, Formatting.None, timeFormat);
            return json;
        }

        public static T FromJson<T>(this string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    return default(T);
                }

                T data = JsonConvert.DeserializeObject<T>(json);
                return data;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static string ToJsonFile(this object o, string filePath, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            string json = JsonConvert.SerializeObject(o);
            File.WriteAllText(filePath, json, encoding);
            return json;
        }

        public static T FromJsonFile<T>(this string filePath, Encoding encoding = null)
        {
            if (!File.Exists(filePath))
            {
                return default(T);
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            try
            {
                string fileContent = File.ReadAllText(filePath);
                T data = JsonConvert.DeserializeObject<T>(fileContent);
                return data;
            }
            catch //(System.Exception ex)
            {
                return default(T);
            }
        }
    }
}
