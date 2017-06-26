using System;

namespace SearchBot.Models
{
    [Serializable]
    public class SearchResult
    {
        public string Title { get; set; }

        public int? Rating { get; set; }

        public int? NumberOflikes { get; set; }

        public string Type { get; set; }

        public string FirstMessege { get; set; }

        public string Image { get; set; }

        public string Link { get; set; }
    }
}