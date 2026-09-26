using Fundo.LoanEngine.Application.Applications.SubmitApplication;
using Fundo.LoanEngine.WebApi.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Fundo.LoanEngine.WebApi.Endpoints;

public static class ApplicationEndpoints
{
    public static IEndpointRouteBuilder MapApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/applications")
            .WithTags("Applications");

        group.MapPost("/", Submit)
            .WithName("SubmitApplication")
            .Produces<SubmitApplicationResponse>(StatusCodes.Status200OK)
            .Produces<SubmitApplicationResponse>(StatusCodes.Status400BadRequest)
            .Produces<SubmitApplicationResponse>(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<IResult> Submit(
        SubmitApplicationRequest request,
        SubmitApplicationHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(request.ToCommand(), cancellationToken);
        var response = result.ToResponse();

        if (result.IsInvalid)
        {
            return Results.BadRequest(response);
        }

        return result.IsApproved ? Results.Ok(response) : Results.UnprocessableEntity(response);
    }
}