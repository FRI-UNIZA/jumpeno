var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Load AppSettings.Client.json:
var assembly = typeof(AppSettings).Assembly;
using var stream = assembly.GetManifestResourceStream("Jumpeno.Client.AppSettings.Client.json")
?? throw new FileNotFoundException("App configuration file not found.");
var appSettingsClient = new ConfigurationBuilder().AddJsonStream(stream).Build();
AppSettings.Init(builder.Configuration, appSettingsClient);

builder.Services.AddLocalization();
builder.Services.AddAntDesign();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<CookieStorage, CookieStorageClient>();

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
ThemeProvider.Init();

await app.RunAsync();
