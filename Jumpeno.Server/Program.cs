using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Load AppSettings.Client.json:
const string sharedSettingsPath = "AppSettings.Client.json";
var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Jumpeno.Client", sharedSettingsPath);
if (!File.Exists(appSettingsPath)) appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), sharedSettingsPath);
var appSettingsClient = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true)
    .Build();
AppSettings.Init(builder.Configuration, appSettingsClient);

// Load AppSettings.Server.json:
var assembly = typeof(ServerSettings).Assembly;
using var stream = assembly.GetManifestResourceStream("Jumpeno.Server.AppSettings.Server.json")
    ?? throw new FileNotFoundException("Server configuration file not found.");
var appSettingsServer = new ConfigurationBuilder().AddJsonStream(stream).Build();
ServerSettings.Init(builder.Configuration, appSettingsServer);

// Origin policy:
const string originPolicy = "OriginPolicy";
builder.Services.AddCors(options => {
    options.AddPolicy(originPolicy, policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.Configure<CookiePolicyOptions>(options => {
    options.Secure = CookieSecurePolicy.Always; // Enforce secure cookies
});
builder.Services.AddAntiforgery(options => {
    options.Cookie.Name = Cookies.Mandatory.AspNetCoreAntiforgery.String();
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Port configuration:
#if IS_PRODUCTION
    builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(ServerSettings.Port));
#endif

// Add services to the container:
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews(options => {
    options.Conventions.Add(new ApiRoutePrefixConvention(API.Base.Prefix));
}).AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
builder.Services.AddRazorPages();
#if IS_DEVELOPMENT
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options => {
        options.SwaggerDoc(AppSettings.Version, new OpenApiInfo { Title = AppSettings.Name, Version = AppSettings.Version });

        // Enable JWT Authentication in Swagger:
        options.AddSecurityDefinition(AuthTypes.Bearer, new OpenApiSecurityScheme
        {
            Name = Header.Authorization,
            Type = SecuritySchemeType.Http,
            Scheme = AuthTypes.Bearer,
            BearerFormat = AuthTypes.Jwt,
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {token}'"
        });

        // Security Requirement:
        options.AddSecurityRequirement(new OpenApiSecurityRequirement {{
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = AuthTypes.Bearer
                    }
                },
                Array.Empty<string>()
            }
        });

        // Required:
        options.SupportNonNullableReferenceTypes();
        options.NonNullableReferenceTypesAsRequired();

        // Add filters:
        options.OperationFilter<RoleFilter>();
        options.OperationFilter<ContentTypeFilter>();

        // Add XML comments from the Server project:
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
        // Add XML comments from the Client project:
        var referencedAssembly = typeof(App).Assembly;
        xmlFile = $"{referencedAssembly.GetName().Name}.xml";
        var referencedXmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(referencedXmlPath)) options.IncludeXmlComments(referencedXmlPath);
    });
#endif

// Storage:
builder.Services.AddSingleton<CookieStorage, CookieStorageServer>();
builder.Services.AddScoped<RequestStorage>();

// Localization:
builder.Services.AddLocalization();
builder.Services.Configure(CultureController.SetupAction());

// Ant-Design:
builder.Services.AddAntDesign();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => {
    #if IS_DEVELOPMENT
        o.DetailedErrors = true;
    #endif
});

// HttpClient:
builder.Services.AddSingleton(sp => {
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext!.Request;
    var baseAddress = $"{request.Scheme}://{request.Host.Value}";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});

// Security Services:
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<AttemptService>();
builder.Services.AddScoped<CaptchaValidatorService>();

// SignalR & Hubs:
builder.Services.AddSignalR();

// Database:
builder.Services.AddDbContextFactory<DB>(DB.Setup);

var app = builder.Build();

// Database migrations:
using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<DB>();
    while (!dbContext.Database.CanConnect()) {
        Console.WriteLine("Waiting for database connection...");
        await Task.Delay(5000);
    }
    dbContext.Database.Migrate();
}

// Apply the CORS middleware:
app.UseCors(originPolicy);
app.UseStaticFiles();
// Configure Forwarded Headers Middleware:
var forwardedHeadersOptions = new ForwardedHeadersOptions {
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto |
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost,
    KnownNetworks = {}, // Clears the default networks
    KnownProxies = {} // Clears the default proxies
};

app.UseForwardedHeaders(forwardedHeadersOptions);

// App services:
AppEnvironment.Init(
    () => true,
    () => {
        var accessor = AppEnvironment.GetService<IHttpContextAccessor>();
        HttpContext ctx = accessor.HttpContext!;
        if (ctx == null) return false;
        return ctx.Request.Path.StartsWithSegments(API.Base.Prefix);
    },
    () => {
        var accessor = AppEnvironment.GetService<IHttpContextAccessor>();
        HttpContext ctx = accessor.HttpContext!;
        if (ctx == null) return false;
        return ctx.Request.Path.StartsWithSegments(HUB.Base.Prefix);
    },
    #if IS_DEVELOPMENT
        () => true,
    #else
        () => false,
    #endif
    T => app.Services.GetService(T)!
);


URL.Init(
    () => {
        var ctx = ServerContext.Instance;
        return ctx.Request.GetEncodedUrl(); 
    }
);
app.UseRequestLocalization();
I18N.Init(app.Services.GetRequiredService<IStringLocalizer<Resource>>());
HTTP.Init(
    (iteration, e) => throw e,
    async (e, form) => {
        if (AppEnvironment.IsController) return;
        if (e is AppException eApp) ErrorHandler.Notify(eApp);
        else ErrorHandler.Notify(Exceptions.Default);
        await Task.CompletedTask;
    },
    async callback => await callback.Invoke(),
    request => {
        var ctx = ServerContext.Instance;
        var cookies = ctx.Request.Cookies;
        if (cookies is null) return;
        foreach (var cookie in cookies) {
            request.Headers.Add("Cookie", $"{cookie.Key}={cookie.Value}");
        }
    }
);


// Swagger:
#if IS_DEVELOPMENT
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint($"/swagger/{AppSettings.Version}/swagger.json", AppSettings.Version);
    });
    app.UseWebAssemblyDebugging();
#endif
// HTTPS:
app.UseHttpsRedirection();

// Hubs:
GameHub.Init(app);

// Framework files:
app.UseBlazorFrameworkFiles();

// Custom Middlewares:
app.UseMiddleware<ErrorMiddleware>();
app.UseMiddleware<VersionMiddleware>();
app.UseMiddleware<APIMiddleware>();
app.UseMiddleware<AuthMiddleware>();
app.UseMiddleware<HeadersMiddleware>();
app.UseMiddleware<DisposeMiddleware>();

// Mapping:
app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Index");

// Start app:
CronService.Start();
app.Run();
