using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllOrigins", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.Configure<CookiePolicyOptions>(options => {
    options.Secure = CookieSecurePolicy.Always; // Enforce secure cookies
});
builder.Services.AddAntiforgery(options => {
    options.Cookie.Name = COOKIE_MANDATORY.ASP_NET_CORE_ANTIFORGERY.StringValue();
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// appsettings.json
ServerSettings.Init(builder.Configuration);
// Configure the app to load appsettings.json from the Shared project
var sharedSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Jumpeno.Shared", "appsettings.json");
if (!File.Exists(sharedSettingsPath)) sharedSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.shared.json");
var sharedConfig = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(sharedSettingsPath, optional: true, reloadOnChange: true)
    .Build();
AppSettings.Init(sharedConfig);

// Port configuration:
if (builder.Environment.IsProduction()) {
    builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(ServerSettings.Port));
}

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews(options => {
    options.Conventions.Add(new ApiRoutePrefixConvention(AppSettings.Api.Base.Prefix));
}).AddNewtonsoftJson(options => {
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
builder.Services.AddRazorPages();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Localization
builder.Services.AddLocalization();
builder.Services.Configure(CultureController.SetupAction());

// Ant-Design
builder.Services.AddAntDesign();
builder.Services.AddServerSideBlazor().AddCircuitOptions(o => {
    if (builder.Environment.IsDevelopment()) {
        o.DetailedErrors = true;
    }
});

// HttpClient
builder.Services.AddSingleton(sp => {
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext!.Request;
    var baseAddress = $"{request.Scheme}://{request.Host.Value}";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});

// SignalR & Hubs:
builder.Services.AddSignalR();

var app = builder.Build();
// Apply the CORS middleware
app.UseCors("AllowAllOrigins");
app.UseStaticFiles();
// Configure Forwarded Headers Middleware
var forwardedHeadersOptions = new ForwardedHeadersOptions {
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost
};
forwardedHeadersOptions.KnownNetworks.Clear(); // Clears the default networks
forwardedHeadersOptions.KnownProxies.Clear();  // Clears the default proxies
app.UseForwardedHeaders(forwardedHeadersOptions);

AppEnvironment.Init(
    () => true,
    () => {
        var accessor = AppEnvironment.GetService<IHttpContextAccessor>();
        HttpContext ctx = accessor.HttpContext!;
        return ctx.Request.Path.StartsWithSegments(AppSettings.Api.Base.Prefix);
    },
    builder.Environment.IsDevelopment,
    (Type T) => app.Services.GetService(T)!
);
RequestStorage.Init(
    (string key) => {
        var ctx = ServerContext.Get();
        return ctx.Items[key]!;
    },
    (string key, object o) => {
        var ctx = ServerContext.Get();
        ctx.Items[key] = o;
    },
    (string key) => {
        var ctx = ServerContext.Get();
        return ctx.Items.Remove(key);
    }
);
URL.Init(
    () => {
        var ctx = ServerContext.Get();
        var replaceURL = RequestStorage.Get<string>(REQUEST_STORAGE_KEYS.REPLACE_URL);
        return replaceURL is not null ? replaceURL : ctx.Request.GetEncodedUrl(); 
    }
);
app.UseRequestLocalization();
I18N.Init(app.Services.GetRequiredService<IStringLocalizer<Resource>>());
HTTP.Init(
    async (HTTPException e) => {
        if (AppEnvironment.IsController) return;
        Notification.Error(e.Message);
        await Task.CompletedTask;
    },
    (HttpRequestMessage request) => {
        var ctx = ServerContext.Get();
        var cookies = ctx.Request.Cookies;

        if (cookies is not null) {
            foreach (var cookie in cookies) {
                request.Headers.Add("Cookie", $"{cookie.Key}={cookie.Value}");
            }
        }
    }
);
Navigator.Init(
    (string url, bool forceLoad, bool replace) => {
        var ctx = ServerContext.Get();
        var currentURL = URL.Url();
        if (url != currentURL) {
            ctx.Response.Redirect(url);
        }
    },
    () => {}
);
CookieStorage.Init(
    (string key) => {
        var ctx = ServerContext.Get();
        ctx.Request.Cookies.TryGetValue(key, out string? cookie);
        return cookie;
    },
    (Cookie cookie) => {
        var ctx = ServerContext.Get();
        ctx.Response.Cookies.Append(
            cookie.Key.StringValue(),
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
    (string key, string domain, string path) => {
        var ctx = ServerContext.Get();
        ctx.Response.Cookies.Delete(
            key,
            new CookieOptions {
                Domain = Cookie.NormDomain(domain),
                Path = path,
            }
        );
    },
    (bool unclosable) => throw new InvalidOperationException("Can not run on the server!")
);
ThemeProvider.Init();

// Configure the HTTP request pipeline:
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();

// Hubs:
GameHub.Init(app);

app.UseBlazorFrameworkFiles();

// Custom Middlewares:
app.UseMiddleware<StaticFileMiddleware>();
app.UseMiddleware<ErrorMiddleware>();
app.UseMiddleware<HeadersMiddleware>();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
