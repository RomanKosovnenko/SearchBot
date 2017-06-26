using System;
using Microsoft.Bot.Builder.FormFlow;

namespace SearchBot.Models
{
    [Serializable]
    public class SearchQuery
    {
        [Prompt("Please enter keyword:")]
        [Optional]
        public string Title { get; set; }

        [Prompt("Please enter where search?")]
        [Optional]
        public string Source { get; set; }

        [Prompt("What kind of information do you want?")]
        [Optional]
        public string Type { get; set; }


    }
}