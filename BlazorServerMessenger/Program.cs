using BlazorServerMessenger.Areas.Identity;
using BlazorServerMessenger.Data;
using BlazorServerMessenger.Data.Hubs;
using BlazorServerMessenger.Data.Models;
using BlazorServerMessenger.Data.Repository;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

/// Планы
/// Переход в профиль пользователя при клике на его логин в сообщениях
/// Подтверждение аккаунта через email
/// Изучить Identity, возможности поменять страницы входа или написать собственню на основе уже имеющийся через partial (как минимум темная тема)
/// Добавить загрузку сообщений при скроле
 
///Первая необходимость
///Сайт как будто мертвый - нет интерактивности - Совсем (hover: движение вправо, увеличение, смена цвета; анимация на фоне)
///Нужны цвета, разнообразящие темную картину чата (один с парой вариаций)
///Вкладка профиля - identity со свои дизайном
///Действия с чатами (удаление), пользователями (блокировка), редактирование сообщений

// Надо бы разобраться в identity, посмотреть доки по нему 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
SetupDatabase(builder);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

SetupServices(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHub<ChatHub>("/chat/{chatId}");

app.Run();

void SetupDatabase(WebApplicationBuilder builder)
{
    var services = builder.Services;

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
    services.AddDatabaseDeveloperPageExceptionFilter();
    services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
}

void SetupServices(WebApplicationBuilder builder)
{
    var services = builder.Services;

    services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();
    services.AddScoped<UserRepository>();
    services.AddScoped<ChatRepository>();
    services.AddScoped<ClaimsPrincipal>(s =>
    {
        var stateprovider = s.GetRequiredService<AuthenticationStateProvider>();
        var state = stateprovider.GetAuthenticationStateAsync().Result;
        return state.User;
    });
    services.AddAntDesign();
    services.AddSignalR();
}

/*
 * todo Как сделать many to many без 3-й таблицы
 */