using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.NewtonsoftJson;
using GraphQL.Types;
using graphql_aspnet_core.Models.GraphQL;
using Microsoft.AspNetCore.Mvc;

namespace graphql_aspnet_core.Controllers
{
    [Route("[controller]")]
    public class GraphQLController : Controller
    {
        private readonly IDocumentExecuter _documentExecuter;

        private readonly ISchema _schema;

        private readonly IGraphQLSerializer _serializer;

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter, IGraphQLSerializer serializer)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
            _serializer = serializer;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            if (query == null) {
                throw new ArgumentNullException(nameof(query));

            }
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Variables = query.Variables != null ? ((GraphQLSerializer)_serializer).Deserialize<Inputs>(query.Variables.ToString()) : null
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

            return new GraphQLActionResult(result);
        }
    }
}