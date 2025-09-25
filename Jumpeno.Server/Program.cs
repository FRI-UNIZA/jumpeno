using System.Reflection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Load AppSettings.Client.json:
const string SharedSettingsPath = "AppSettings.Client.json";
var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Jumpeno.Client", SharedSettingsPath);
if (!File.Exists(appSettingsPath)) appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), SharedSettingsPath);
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
const string ORIGIN_POLICY = "OriginPolicy";
builder.Services.AddCors(options => {
    options.AddPolicy(ORIGIN_POLICY, policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.Configure<CookiePolicyOptions>(options => {
    options.Secure = CookieSecurePolicy.Always; // Enforce secure cookies
});
builder.Services.AddAntiforgery(options => {
    options.Cookie.Name = COOKIE.MANDATORY.ASP_NET_CORE_ANTIFORGERY.String();
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Port configuration:
if (builder.Environment.IsProduction()) {
    builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(ServerSettings.Port));
}

// Add services to the container:
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews(options => {
    options.Conventions.Add(new ApiRoutePrefixConvention(API.BASE.PREFIX));
}).AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment()) {
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options => {
        options.SwaggerDoc(AppSettings.Version, new OpenApiInfo { Title = AppSettings.Name, Version = AppSettings.Version });

        // Enable JWT Authentication in Swagger:
        options.AddSecurityDefinition(AUTH.BEARER, new OpenApiSecurityScheme {
            Name = HEADER.AUTHORIZATION,
            Type = SecuritySchemeType.Http,
            Scheme = AUTH.BEARER,
            BearerFormat = AUTH.JWT,
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {token}'"
        });

        // Security Requirement:
        options.AddSecurityRequirement(new OpenApiSecurityRequirement {{
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = AUTH.BEARER
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

        // Add comments:
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
}

// Localization:
builder.Services.AddLocalization();
builder.Services.Configure(CultureController.SetupAction());

// Ant-Design:
builder.Services.AddAntDesign();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => {
    if (builder.Environment.IsDevelopment()) {
        o.DetailedErrors = true;
    }
});

// HttpClient:
builder.Services.AddSingleton(sp => {
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext!.Request;
    var baseAddress = $"{request.Scheme}://{request.Host.Value}";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});

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
app.UseCors(ORIGIN_POLICY);
app.UseStaticFiles();
// Configure Forwarded Headers Middleware:
var forwardedHeadersOptions = new ForwardedHeadersOptions {
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
};
forwardedHeadersOptions.KnownNetworks.Clear(); // Clears the default networks
forwardedHeadersOptions.KnownProxies.Clear();  // Clears the default proxies
app.UseForwardedHeaders(forwardedHeadersOptions);

// App services:
AppEnvironment.Init(
    () => true,
    () => {
        var accessor = AppEnvironment.GetService<IHttpContextAccessor>();
        HttpContext ctx = accessor.HttpContext!;
        if (ctx == null) return false;
        return ctx.Request.Path.StartsWithSegments(API.BASE.PREFIX);
    },
    () => {
        var accessor = AppEnvironment.GetService<IHttpContextAccessor>();
        HttpContext ctx = accessor.HttpContext!;
        if (ctx == null) return false;
        return ctx.Request.Path.StartsWithSegments(HUB.BASE.PREFIX);
    },
    #if IS_DEVELOPMENT
        () => true,
    #else
        () => false,
    #endif
    T => app.Services.GetService(T)!
);
RequestStorage.Init(
    key => {
        var ctx = ServerContext.Instance;
        return ctx.Items[key]!;
    },
    (key, o) => {
        var ctx = ServerContext.Instance;
        ctx.Items[key] = o;
    },
    key => {
        var ctx = ServerContext.Instance;
        return ctx.Items.Remove(key);
    }
);
URL.Init(
    () => {
        var ctx = ServerContext.Instance;
        var replaceURL = RequestStorage.Get<string>(nameof(URL));
        return replaceURL is not null ? replaceURL : ctx.Request.GetEncodedUrl(); 
    },
    ThemeProvider.ThemeCSSClass
);
app.UseRequestLocalization();
I18N.Init(app.Services.GetRequiredService<IStringLocalizer<Resource>>());
HTTP.Init(
    (iteration, e) => throw e,
    async (e, form) => {
        if (AppEnvironment.IsController) return;
        if (e is AppException eApp) ErrorHandler.Notify(eApp);
        else ErrorHandler.Notify(EXCEPTION.DEFAULT);
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
Navigator.Init(
    (url, forceLoad, replace) => {
        var ctx = ServerContext.Instance;
        var currentURL = URL.Url();
        if (url != currentURL) {
            ctx.Response.Redirect(url);
        }
    },
    () => {}
);
CookieStorage.Init(
    key => {
        var ctx = ServerContext.Instance;
        ctx.Request.Cookies.TryGetValue(key, out string? cookie);
        return cookie;
    },
    cookie => {
        var ctx = ServerContext.Instance;
        ctx.Response.Cookies.Append(
            cookie.Key.String(),
            cookie.Value,
            new CookieOptions {
                Expires = cookie.Expires,
                Domain = Cookie.NormDomain(cookie.Domain),
                Path = cookie.Path,
                HttpOnly = cookie.HttpOnly,
                Secure = cookie.Secure,
                SameSite = (SameSiteMode)(int)cookie.SameSite
            }
        );
    },
    (key, domain, path) => {
        var ctx = ServerContext.Instance;
        ctx.Response.Cookies.Delete(
            key,
            new CookieOptions {
                Domain = Cookie.NormDomain(domain),
                Path = path,
            }
        );
    },
    unclosable => throw new InvalidOperationException("Can not run on the server!")
);
ThemeProvider.Init();

// Swagger:
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint($"/swagger/{AppSettings.Version}/swagger.json", AppSettings.Version);
    });
    app.UseWebAssemblyDebugging();
}
// HTTPS:
app.UseHttpsRedirection();

// Hubs:
GameHub.Init(app);

// Framework files:
app.UseBlazorFrameworkFiles();

// Custom Middlewares:
app.UseMiddleware<StaticFileMiddleware>();
app.UseMiddleware<ErrorMiddleware>();
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
