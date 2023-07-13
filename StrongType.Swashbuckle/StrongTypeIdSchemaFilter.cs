using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StrongType.Swashbuckle;
public class StrongTypeIdSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (StrongTypeIdHelper.IsStrongTypeId(context.Type, out var underlyingType) == false)
            return;
        if (schema.Type != "object")
            return;

        var hasExample = schema.Example != null;
        if (underlyingType == typeof(Guid))
        {
            schema.Type = "string";
            schema.Format = "uuid";
            if (hasExample == false)
                schema.Example = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        }
        else if (underlyingType == typeof(string)) 
        {
            schema.Type = "string";
        }
        else if (underlyingType == typeof(int))
        {
            schema.Type = "integer";
            if (hasExample == false)
                schema.Example = new OpenApiString("1505");
        }
        else if (underlyingType == typeof(decimal))
        {
            schema.Type = "number";
            if (hasExample == false)
                schema.Example = new OpenApiString("1505.85");
        }
        else if (underlyingType == typeof(double))
        {
            schema.Type = "number";
            if (hasExample == false)
                schema.Example = new OpenApiString("1505.85");
        }
    }
}
