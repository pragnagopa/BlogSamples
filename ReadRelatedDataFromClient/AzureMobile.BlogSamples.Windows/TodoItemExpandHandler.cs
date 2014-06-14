using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureMobile.BlogSamples
{
    public class TodoItemExpandHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage>
        SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool requestToTodoTable = request.RequestUri.PathAndQuery
                .StartsWith("/tables/todoItem", StringComparison.OrdinalIgnoreCase)
                    && request.Method == HttpMethod.Get;
            if (requestToTodoTable)
            {
                UriBuilder builder = new UriBuilder(request.RequestUri);
                string query = builder.Query;
                if (!query.Contains("$expand"))
                {
                    if (string.IsNullOrEmpty(query))
                    {
                        query = string.Empty;
                    }
                    else
                    {
                        query = query + "&";
                    }

                    query = query + "$expand=items";
                    builder.Query = query.TrimStart('?');
                    request.RequestUri = builder.Uri;
                }
            }

            var result = await base.SendAsync(request, cancellationToken);
            return result;
        }
    }
}
