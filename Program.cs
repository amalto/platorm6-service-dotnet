using System.Text;
using Io.Platform6.Imdg.Cm;
using P6Connector;
using P6Connector.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "hh:mm:ss ";
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyMethod();
            builder.AllowAnyHeader();
            builder.WithHeaders("Authorization");
        });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();

WebApplication? app = builder.Build();

app.MapGet("/", () => "ok!");

app.MapGet("/crypto", async () =>
{
    var dataBuilder = new platform6_service.PortalPayloadBuilder(app.Services.GetService<IHttpClientFactory>(), app.Logger, "");
    return await dataBuilder.buildDataAsync();
});

app.MapGet(Constants.BaseUrl + "/portal", () =>
{
    var bundlePath = Environment.GetEnvironmentVariable(variable: "UI_BUNDLE_PATH") ?? Path.Combine(app.Environment.WebRootPath, "ServiceConfiguration.bundle.js");

    if (bundlePath == null)
    {
        return Task.FromResult(Results.BadRequest("UI_BUNDLE_PATH not set!"));
    }
    else
    {
        var javaScriptBundle = File.ReadAllText(bundlePath, Encoding.UTF8);
        var payloadBuilder = new platform6_service.PortalPayloadBuilder(app.Services.GetService<IHttpClientFactory>(), app.Logger, javaScriptBundle);
        return Task.FromResult(payloadBuilder.build());
    }
});

app.Logger.LogInformation("Starting...");

var service = DeployService(app.Logger, onCommonMessage);

CommonMessage onCommonMessage(CommonMessage request)
{
    app.Logger.LogInformation("Got common message request: " + request);

    var response = new CommonMessage();

    response.Id = request.Id;
    response.Destination = request.ReplyTo;

    var action = BusConnection.ReadHeaderValue(request, Constants.MyServiceId + ".action");

    switch (action)
    {
        case "price":
            var dataBuilder = new platform6_service.PortalPayloadBuilder(app.Services.GetService<IHttpClientFactory>(), app.Logger, "");
            var id = BusConnection.ReadHeaderValue(request, "crypto.id");
            var result = dataBuilder.getPriceAsync(id).Result;
            response.Headers.Add(BusConnection.CreateHeader(P6Connector.Constants.ResponsePrefix + "status", "true"));
            response.Headers.Add(BusConnection.CreateHeader(P6Connector.Constants.ResponsePrefix + "value", result));
            break;

        default:
            response.Headers.Add(BusConnection.CreateHeader(P6Connector.Constants.ResponsePrefix + "status", "false"));
            response.Headers.Add(BusConnection.CreateHeader(P6Connector.Constants.ResponsePrefix + "exception.message", "CMD action not supported: " + action));
            break;
    }

    return response;
}

static Service DeployService(ILogger logger, Func<CommonMessage, CommonMessage> onMessage)
{
    var parameters = new DeployParameters
    {
        Id = Constants.MyServiceId,
        Path = Constants.Path,
        BasePath = Environment.GetEnvironmentVariable("EXTERNAL_URL") ?? "http://docker.for.mac.host.internal:9192",
        Versions = new Versions
        {
            Client = Constants.ClientVersion,
            Server = Constants.ServerVersion
        },
        Ui = new UserInterfaceProperties
        {
            visible = true,
            iconName = "fab fa-windows",
            weight = 30,
            label = new Dictionary<string, string> {
                        {"en-US", ".NET Service"},
                        {"fr-FR", ".NET Service"}
                    }
        }
    };

    return new Service(parameters, logger, onMessage);
}

app.UseCors();
app.Run();

