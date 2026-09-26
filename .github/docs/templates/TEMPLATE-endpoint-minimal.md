
---

# 23. `docs/templates/TEMPLATE-endpoint-minimal.md`

```markdown
# TEMPLATE — Minimal API Endpoint

```csharp
namespace Fundo.WebApi.Endpoints;

public static class LoanApplicationEndpoints
{
    public static IEndpointRouteBuilder MapLoanApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/applications")
            .WithTags("Applications")
            .WithOpenApi();

        group.MapPost("/", Submit)
            .Produces<SubmitApplicationResponse>(StatusCodes.Status200OK)
            .Produces<SubmitApplicationResponse>(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    private static async Task<IResult> Submit(
        SubmitApplicationRequest request,
        SubmitApplicationHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(request.ToCommand(), ct);

        return result.IsApproved
            ? Results.Ok(SubmitApplicationResponse.Approved(result.ApplicationId!.Value))
            : Results.UnprocessableEntity(SubmitApplicationResponse.Denied(result.DenialReason!));
    }
}
```