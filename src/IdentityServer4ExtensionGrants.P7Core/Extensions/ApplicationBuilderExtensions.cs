using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using P7Core.IRules;

namespace P7Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLowercaseRewriter(this IApplicationBuilder app)
        {
            app.UseRewriter(new RewriteOptions().Add(new RewriteLowerCaseRule()));
            return app;
        }
    }
}