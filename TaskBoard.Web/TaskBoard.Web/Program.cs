using TaskBoard.Web.Mapping;
using TaskBoard.Web.Services;


var builder = WebApplication.CreateBuilder(args);
// Reads ApiBaseUrl from appsettings (Dev overrides base)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException("Missing ApiBaseUrl. Set it in appsettings or Azure App Settings.");
}

// Add services to the container.
builder.Services.AddControllersWithViews();
// 3) REGISTER ITaskApiClient -> TaskApiClient (typed HttpClient)
builder.Services.AddHttpClient<ITaskApiClient, TaskApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});


// (Optional) AutoMapper for ViewModel/DTO mapping
builder.Services.AddAutoMapper(typeof(WebMappingProfile).Assembly);
 // Add this using directive at the top of the file
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
