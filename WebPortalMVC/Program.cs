

using WebPortalMVC.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .ConfigureBuilder();

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); 

var app = builder.Build().ConfigureApplication();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name : "area",
    pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.Run();
