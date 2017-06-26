using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Bot.Connector;
using SearchBot.Models;

namespace SearchBot.Code
{
    public static class YouTubeSearch
    {
        private static YouTubeService _youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyCyNBVdMBMa4SR9UNDznWYLbxM1E-qCM6A",
            ApplicationName = "SearchBot"
        });

        public static async Task<List<SearchResult>> SearchAsync(string key, int maxCountOfResults)
        {
            var searchListRequest = _youtubeService.Search.List("snippet");
            searchListRequest.Q = key; // Replace with your search term.
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<SearchResult> videos = new List<SearchResult>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(new SearchResult() {
                            Link = "https://www.youtube.com/watch?v=" + searchResult.Id.VideoId,
                            Title = searchResult.Snippet.Title,
                            Type = "video",
                            FirstMessege = searchResult.Snippet.Description,
                            Image = searchResult.Snippet.Thumbnails.High?.Url ??
                                    searchResult.Snippet.Thumbnails.Default__.Url
                        });
                        break;
                }
                if(videos.Count == maxCountOfResults) break;
            }
            return videos;
        }
    }
}