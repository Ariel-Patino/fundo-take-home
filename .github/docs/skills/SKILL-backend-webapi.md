# SKILL — Backend WebApi (Minimal APIs)

## Purpose
Thin HTTP layer. No logic. Maps requests to use cases, returns results.

## Rules
- Minimal APIs ONLY. No controllers.
- Program.cs stays under 60 lines — extract to extension methods.
- Endpoints grouped by feature with `MapGroup`.
- No try/catch around every handler — use `IExceptionHandler` middleware.
- OpenAPI via `Microsoft.AspNetCore.OpenApi` (built-in .NET 10).

## Structure
Fundo.WebApi/
├── Endpoints/
│ ├── LoanApplicationEndpoints.cs
│ └── HealthEndpoints.cs
├── Middleware/
│ └── GlobalExceptionHandler.cs
├── Contracts/
│ ├── SubmitApplicationRequest.cs
│ └── SubmitApplicationResponse.cs
├── Program.cs
└── appsettings.json

## Endpoint Shape
```csharp
public static class LoanApplicationEndpoints
{
    public static IEndpointRouteBuilder MapLoanApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/applications").WithTags("Applications");

        group.MapPost("/", Submit)
            .Produces<SubmitApplicationResponse>(StatusCodes.Status201Created)
            .Produces<SubmitApplicationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return app;
    }

    private static async Task<IResult> Submit(
        SubmitApplicationRequest request,
        SubmitApplicationHandler handler,
        CancellationToken ct)
    {
        var result = await handler.Handle(request.ToCommand(), ct);
        return result.IsApproved
            ? Results.Ok(result.ToResponse())
            : Results.UnprocessableEntity(result.ToDeniedResponse());
    }
}
```

Contracts
Request/Response records live in Contracts/, NOT in Application.

Mapping extension methods: request.ToCommand(), result.ToResponse().

Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();
app.UseExceptionHandler();
app.MapLoanApplicationEndpoints();
app.MapHealthEndpoints();
app.Run();
```
CORS
Allow http://localhost:3000 in dev only.

Checklist
- [ ] No business logic in endpoints
- [ ] No DbContext injected in endpoints
- [ ] Contracts are records
- [ ] OpenAPI enabled




