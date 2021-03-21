using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DotnetChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            WebRequest request = WebRequest.Create("https://coderbyte.com/api/challenges/json/json-cleaning");

            request.ContentType = "application/json; charset=utf-8";

            WebResponse response = request.GetResponse();

            // Console.WriteLine(response.ToString());

            string dataString = "";

            using (var stream = new StreamReader(response.GetResponseStream()))
            {
                dataString = stream.ReadToEnd();
            }

            JObject data = JObject.Parse(dataString);

            Console.WriteLine("==================================== Raw Object ============================");
            Console.WriteLine(data);

            Console.WriteLine("==================================== Output ============================");
            Console.WriteLine(CleanJObject(data));

            response.Close();
        }




        public static JObject CleanJObject(JObject MyJObject)
        {
            var invalidKeys = new List<string>();

            foreach (var x in MyJObject)
            {
                string name = x.Key;
                JToken value = x.Value;

                if (value.GetType() == typeof(JObject))
                {
                    CleanJObject(((JObject) value));
                }
                else if (value.GetType() == typeof(JValue))
                {
                    // the object is a JValue
                    if(string.IsNullOrEmpty(value.ToString()) || value.ToString().Equals("-") ||
                        value.ToString().  Equals("N/A"))
                    {
                        invalidKeys.Add(name);
                    }
                }
                else if (value.GetType() == typeof(JArray))
                {
                    var invalItems = new List<JToken>();

                    // the object is a JArray
                    foreach(var item in value)
                    {
                        string stringValue = item.ToString();

                        if (string.IsNullOrEmpty(stringValue) || stringValue.ToString().Equals("-") ||
                        stringValue.ToString().  Equals("N/A"))
                        {
                            invalItems.Add(item);
                        }
                    }

                    foreach(var item in invalItems)
                    {
                        ((JArray) value).Remove(item);
                    }
                }

            }

            foreach(var key in invalidKeys)
            {
                    MyJObject.Remove(key);
            }

            return MyJObject;
        }
    }
}
