using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ServerMon.Helpers.Filters
{
    public class DefaultHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Api-Key",
                In = ParameterLocation.Header,
                Required = true,
                Example = new OpenApiString("GUID")
            });
        }
    }
}