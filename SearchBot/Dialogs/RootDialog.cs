using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using SearchBot.Code;
using SearchBot.Models;

namespace SearchBot.Dialogs
{
    [LuisModel("7539cc24-135c-48ea-9eee-babc6f7a3fd0", "caa95fb30d004c1fbe26ee18c928ada9")]
    [Serializable]
    public class RootDialog: LuisDialog<object>
    {
        private const string EntityEntertainmentMediaFormat = "Entertainment.MediaFormat";

        private const string EntityEntertainmentMediaSource = "Entertainment.MediaSource";

        private const string EntityEntertainmentTitle = "Entertainment.Title";

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type '/help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Search")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"I'm analyzing your message: '{message.Text}'...");

            var searchQuery = new SearchQuery();

            EntityRecommendation searchTitleEntityRecommendation;
            if (result.TryFindEntity(EntityEntertainmentTitle, out searchTitleEntityRecommendation))
            {
                searchTitleEntityRecommendation.Type = "Title";
            }
            EntityRecommendation searchFormatEntityRecommendation;
            EntityRecommendation searchSourceEntityRecommendation;


            if (result.TryFindEntity(EntityEntertainmentTitle, out searchFormatEntityRecommendation))
            {
                searchFormatEntityRecommendation.Type = "Type";
            }
            if (result.TryFindEntity(EntityEntertainmentTitle, out searchSourceEntityRecommendation))
            {
                searchSourceEntityRecommendation.Type = "Source";
            }

            var searchFormDialog = new FormDialog<SearchQuery>(searchQuery, this.BuildSearchForm, FormOptions.PromptInStart, result.Entities);

            context.Call(searchFormDialog, this.ResumeAfterSearchFormDialog);
        }

        private async Task ResumeAfterSearchFormDialog(IDialogContext context, IAwaitable<SearchQuery> result)
        {
            try
            {
                var searchQuery = await result;

                var results = await this.GetResultsAsync(searchQuery);

                await context.PostAsync($"I found {results.Count()} result(s):");

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.List;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var result1 in results)
                {
                    HeroCard heroCard = new HeroCard()
                    {
                        Title = result1.Title,
                        Subtitle = result1.NumberOflikes!=null?$"{result1.NumberOflikes} likes. ":"" + $"{result1.FirstMessege}",
                        Images = new List<CardImage>()
                        {
                            new CardImage() { Url = result1.Image }
                        },
                        Buttons = new List<CardAction>()
                        {
                            new CardAction()
                            {
                                Title = "See more",
                                Type = ActionTypes.OpenUrl,
                                Value = result1.Link
                            }
                        }
                    };

                    resultMessage.Attachments.Add(heroCard.ToAttachment());
                }

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation.";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi! Try asking me things like 'How to tell a joke', 'search action video on YouTube' or 'What is sticker?'");

            context.Wait(this.MessageReceived);
        }

        private IForm<SearchQuery> BuildSearchForm()
        {
            OnCompletionAsyncDelegate<SearchQuery> processSearch = async (context, state) =>
            {
                var message = $"Searching for {state.Title}" + (!string.IsNullOrEmpty(state.Source)? $" in {state.Source} ...":" ...");
                await context.PostAsync(message);
            };
            return new FormBuilder<SearchQuery>()
                .Field(nameof(SearchQuery.Title), (state) => string.IsNullOrEmpty(state.Title))
                .OnCompletion(processSearch)
                .Build();
        }

        private async Task<IEnumerable<SearchResult>> GetResultsAsync(SearchQuery searchQuery)
        {
            var results = new List<SearchResult>();

             //results.AddRange(await YouTubeSearch.SearchAsync(searchQuery.Title, 2));
            results.AddRange(await LSLearningCenterSearch.SearchAsync(searchQuery.Title, 2));
            // Filling the hotels results manually just for demo purposes

            //results.Sort((h1, h2) => h1.Rating.CompareTo(h2.NumberOflikes));

            return results;
        }
    }
}