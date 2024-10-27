var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure the app to load appsettings.json from the Shared project
var assembly = typeof(AppSettings).Assembly;
using var stream = assembly.GetManifestResourceStream("Jumpeno.Shared.appsettings.json");
if (stream == null) {
    throw new FileNotFoundException("Shared configuration file not found.");
}
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
    builder.HostEnvironment.IsDevelopment,
    (Type T) => app.Services.GetService(T)!
);
RequestStorage.Init();
Navigator.Init();
URL.Init(() => {
    var manager = AppEnvironment.GetService<NavigationManager>();
    return manager.Uri;
});
I18N.Init(app.Services.GetRequiredService<IStringLocalizer<Resource>>());
JS.Init(app.Services.GetRequiredService<IJSRuntime>());
HTTP.Init(
    async (HTTPException e) => {
        Notification.Error(e.Message);
        await Task.CompletedTask;
    }
);
CookieStorage.Init(
    (string key) => {
        var value = JS.Invoke<string>(JSCookies.Get, key);
        if (value is null) return value;
        else return URL.DecodeValue(value);
    },
    (Cookie cookie) => {
        JS.InvokeVoid(
            JSCookies.Set,
            cookie.Key.StringValue(),
            URL.EncodeValue(cookie.Value),
            cookie.Expires is not null ? ((DateTimeOffset) cookie.Expires).UtcDateTime.ToString("R") : null,
            cookie.Domain,
            cookie.Path,
            cookie.Secure,
            cookie.SameSite == SAME_SITE.UNSPECIFIED ? null : cookie.SameSite.StringValue()
        );
    },
    (string key, string domain, string path) => {
        JS.InvokeVoid(JSCookies.Delete, key, domain, path);
    },
    async (bool unclosable) => {
        var modal = RequestStorage.Get<CookieConsentModal>(REQUEST_STORAGE_KEYS.COOKIE_CONSENT_MODAL);
        if (modal is null) return;
        await modal.OpenModal(unclosable);
    }
);
ThemeProvider.Init();
SelectCulture.Init();

await app.RunAsync();