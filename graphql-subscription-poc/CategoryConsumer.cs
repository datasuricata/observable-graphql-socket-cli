using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Reactive.Linq;

namespace graphql_subscription_poc
{
    public class CategoryConsumer
    {
        private readonly IGraphQLClient _client;
        private readonly ILogger<CategoryConsumer> _logger;
        public CategoryConsumer(IGraphQLClient client, ILogger<CategoryConsumer> logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Subscribe()
        {
            var categoryJoinedRequest = new GraphQLRequest
            {
                Query = @"
                    subscription{
                      onCategoryCreate{
                        active
                        createdAt
                        id
                        name
                        priority
                      }
                    }"
            };

            IObservable<GraphQLResponse<dynamic>> stream =
                _client.CreateSubscriptionStream<dynamic>(categoryJoinedRequest);

            IDisposable subscription = stream.Subscribe(response =>
            {
                _logger.LogInformation((string)JsonConvert.SerializeObject(response.Data));
            });

            while (true) { }

            //subscription.Dispose();
        }
    }
}