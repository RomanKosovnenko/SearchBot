using SearchBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace SearchBot.Code
{
    public static class LSLearningCenterSearch
    {
        public static async Task<List<SearchResult>> SearchAsync(string key, int maxCountOfResults)
        {
            List<SearchResult> results = new List<SearchResult>();
            string url = "https://lcservice.lizard.net.ua/_api/courses/items";

            string jsonstring = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                jsonstring = reader.ReadToEnd();
            }
            JArray jobject = JArray.Parse(jsonstring, new JsonLoadSettings());
            foreach (var course in jobject)
            {
                if (course["name"].Value<string>().Contains(key.Trim(key.Where(Char.IsPunctuation).Distinct().ToArray())))
                {
                    results.Add(new SearchResult()
                    {
                        Title = course["name"].Value<string>(),
                        Type = "cource",
                        FirstMessege = course["descript"].Value<string>(),
                        Image = course["MainImgUrl"].Value<string>(),
                        Link = "https://lc.lizard.net.ua/coursescollection/" + course["internalID"].Value<string>()
                    });
                }
            }
            Console.WriteLine(jsonstring);
            Console.WriteLine(jsonstring);
            Console.WriteLine(jsonstring);
            Console.WriteLine(jsonstring);
            Console.WriteLine(jsonstring); Console.WriteLine(jsonstring);






            return new List<SearchResult>();
        }
    }
}