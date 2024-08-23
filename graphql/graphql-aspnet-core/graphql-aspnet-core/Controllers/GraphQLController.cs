﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.SystemTextJson;
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

        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            if (query == null) {
                throw new ArgumentNullException(nameof(query));

            }
            var inputs = new GraphQLSerializer().Deserialize<Inputs>(query.Variables.ToString());
            var executionOptions = new ExecutionOptions
            {
                Schema = _schema,
                Query = query.Query,
                Variables = inputs
            };

            var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);


            if (result.Errors?.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}