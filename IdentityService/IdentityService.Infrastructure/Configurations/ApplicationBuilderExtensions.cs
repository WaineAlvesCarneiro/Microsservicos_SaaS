using Microsoft.AspNetCore.Builder;

namespace IdentityService.Infrastructure.Configurations;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de autenticação v1");
            c.DocumentTitle = "API de autenticação";
        });

        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}