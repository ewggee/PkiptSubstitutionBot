using PkiptSubstitutionBot.client.Application.Extensions;
using PkiptSubstitutionBot.client.DataAccess.Extensions;
using PkiptSubstitutionBot.client.MVC.Extensions;

namespace PkiptSubstitutionBot.client.MVC;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services
            .AddAuth(builder.Configuration)
            .AddData(builder.Configuration)
            .AddRabbitMq(builder.Configuration);

        var app = builder.Build();

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=UploadMessage}/{id?}");

        app.Run();
    }
}
