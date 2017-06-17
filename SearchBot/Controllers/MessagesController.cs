using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Google.Apis.Services;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Google.Apis.YouTube.v3;

namespace SearchBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                #region YouTube Search
                //todo перекинуть в отдельное место
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyCyNBVdMBMa4SR9UNDznWYLbxM1E-qCM6A",
                    ApplicationName = "SearchBot"
                });
                

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = activity.Text ?? "cats"; // Replace with your search term.
                searchListRequest.MaxResults = 5;

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<string> videos = new List<string>();

                // Add each result to the appropriate list, and then display the lists of
                // matching videos, channels, and playlists.
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            videos.Add("https://www.youtube.com/watch?v="+searchResult.Id.VideoId);
                            break;
                    }
                }
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return


                // return our reply to the user
                Activity reply = activity.CreateReply($"This is what I found by the word {activity.Text} on YouTube");
                await connector.Conversations.ReplyToActivityAsync(reply);
                foreach (var video in videos)
                {
                    reply = activity.CreateReply(video);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }

                #endregion
            }
            else
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply = HandleSystemMessage(activity);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                return message.CreateReply($"Hi,{message.From.Name}! Welcome to bots wrold");
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}