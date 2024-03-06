using ETL_Front.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IHttpClientWrapper, HttpClientWrapper>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();

app.Run();
