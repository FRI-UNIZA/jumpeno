var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Load appsettings.json from the Shared project:
var assembly = typeof(AppSettings).Assembly;
using var stream = assembly.GetManifestResourceStream("Jumpeno.Shared.appsettings.json")
?? throw new FileNotFoundException("Shared configuration file not found.");

var config = new ConfigurationBuilder()
    .AddJsonStream(stream)
    .Build();
AppSettings.Init(config);

builder.Services.AddLocalization();
builder.Services.AddAntDesign();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var app = builder.Build();
AppEnvironment.Init(
    () => false,
    () => false,
    () => false,
    #if IS_DEVELOPMENT
        () => true,
    #else
        () => false,
    #endif
    T => app.Services.GetService(T)!
);
RequestStorage.Init();
Navigator.Init();
URL.Init(
    () => {
        var manager = AppEnvironment.GetService<NavigationManager>();
        return manager.Uri;
    },
    ThemeProvider.ThemeCSSClass
);
I18N.Init(app.Services.GetRequiredService<IStringLocalizer<Resource>>());
JS.Init(app.Services.GetRequiredService<IJSRuntime>());
HTTP.Init(
    async (iteration, e) => {
        if (!Auth.IsLoggedIn) throw e;
        await Auth.Refresh(iteration);
    },
    async (e, form) => {
        if (e is AppException eApp) ErrorHandler.Display(eApp, form);
        else ErrorHandler.Notify(EXCEPTION.DEFAULT);
        await Task.CompletedTask;
    },
    async callback => await Window.Lock(callback.Invoke, WINDOW_LOCK.HTTP)
);
CookieStorage.Init(
    key => {
        var value = JS.Invoke<string>(JSCookies.Get, key);
        if (value is null) return value;
        else return URL.DecodeValue(value);
    },
    cookie => {
        JS.InvokeVoid(
            JSCookies.Set,
            cookie.Key.String(),
            URL.EncodeValue(cookie.Value),
            cookie.Expires is not null ? ((DateTimeOffset) cookie.Expires).UtcDateTime.ToString("R") : null,
            cookie.Domain == Cookie.NormDomain(cookie.Domain),
            cookie.Path,
            cookie.Secure,
            cookie.SameSite == SAME_SITE.UNSPECIFIED ? null : cookie.SameSite.String()
        );
    },
    (key, domain, path) => {
        JS.InvokeVoid(JSCookies.Delete, key, Cookie.NormDomain(domain), path);
    },
    async unclosable => {
        var modal = RequestStorage.Get<CookieModal>(nameof(CookieModal));
        if (modal is null) return;
        await modal.OpenModal(unclosable);
    }
);
ThemeProvider.Init();

await app.RunAsync();
