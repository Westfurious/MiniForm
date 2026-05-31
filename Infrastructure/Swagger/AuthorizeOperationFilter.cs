using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MiniForm.Infrastructure.Swagger;

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorizeAttribute = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
            || context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true;

        var allowsAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
            || context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() == true;

        if (!hasAuthorizeAttribute || allowsAnonymous)
        {
            return;
        }

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", null, null),
                new List<string>()
            }
        });
    }
}
