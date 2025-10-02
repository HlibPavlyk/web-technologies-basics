using Lab5.GraphQL;
using Lab5.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Queries>()
    .AddMutationType<Mutations>();

// register mediator handlers
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// register appDbContext
builder.Services.AddDbEfConnection(builder.Configuration);

var app = builder.Build();

// Configure GraphQL Playground
app.MapGraphQL();

app.Run();