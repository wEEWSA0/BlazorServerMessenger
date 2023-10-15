using BlazorServerMessenger.Areas.Identity;
using BlazorServerMessenger.Data;
using BlazorServerMessenger.Data.Hubs;
using BlazorServerMessenger.Data.Models;
using BlazorServerMessenger.Data.Repository;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

/// �����
/// ������� � ������� ������������ ��� ����� �� ��� ����� � ����������
/// ������������� �������� ����� email
/// ������� Identity, ����������� �������� �������� ����� ��� �������� ���������� �� ������ ��� ��������� ����� partial (��� ������� ������ ����)
/// �������� �������� ��������� ��� ������
 
///������ �������������
///���� ��� ����� ������� - ��� ��������������� - ������ (hover: �������� ������, ����������, ����� �����; �������� �� ����)
///����� �����, �������������� ������ ������� ���� (���� � ����� ��������)
///������� ������� - identity �� ���� ��������
///�������� � ������ (��������), �������������� (����������), �������������� ���������

// ���� �� ����������� � identity, ���������� ���� �� ���� 

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
 * todo ��� ������� many to many ��� 3-� �������
 */