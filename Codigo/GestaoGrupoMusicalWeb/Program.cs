using Core;
using Microsoft.EntityFrameworkCore;
using Core.Service;
using Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using GestaoGrupoMusicalWeb.Helpers;
using System.Globalization;

namespace GestaoGrupoMusicalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Define a cultura base como pt-BR
            var cultureInfo = new CultureInfo("pt-BR");

            // 2. FORÇA o separador decimal a ser o PONTO ('.')
            //    Isso garante que o Model Binder (que recebe 50.5 do front-end) 
            //    o interprete corretamente como 50.5, resolvendo o problema de salvar 505.
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

            // 3. Aplica essa configuração ao thread padrão
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // 4. Configura o RequestLocalizationOptions para toda a aplicação
            builder.Services.Configure<Microsoft.AspNetCore.Builder.RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture: cultureInfo, uiCulture: cultureInfo);
                options.SupportedCultures = new[] { cultureInfo };
                options.SupportedUICultures = new[] { cultureInfo };
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<GrupoMusicalContext>(
                options => options.UseMySQL(builder.Configuration.GetConnectionString("GrupoMusicalDatabase")));

            builder.Services.AddDbContext<IdentityContext>(
                options => options.UseMySQL(builder.Configuration.GetConnectionString("GrupoMusicalDatabase")));

            builder.Services.AddIdentity<UsuarioIdentity, IdentityRole>(options =>
            {
                // SignIn settings
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

                // Default User settings.
                options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // Default Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            }).AddEntityFrameworkStores<IdentityContext>()
              .AddDefaultTokenProviders();


            //Configure tokens life
            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
                //sets a 2 hour lifetime of the generated token to reset password/email/phone number
                options.TokenLifespan = TimeSpan.FromHours(2)
            );

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Autenticar";
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Autenticar";
                // ReturnUrlParameter requires 
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddTransient<IGrupoMusicalService, GrupoMusicalService>();
            builder.Services.AddTransient<IPessoaService, PessoaService>();
            builder.Services.AddTransient<IEventoService, EventoService>();
            builder.Services.AddTransient<IInstrumentoMusicalService, InstrumentoMusicalService>();
            builder.Services.AddTransient<IManequimService, ManequimService>();
            builder.Services.AddTransient<IInformativoService, InformativoService>();
            builder.Services.AddTransient<IFigurinoService, FigurinoService>();
            builder.Services.AddTransient<IFinanceiroService, FinanceiroService>();
            builder.Services.AddScoped<IEnsaioService, EnsaioService>();
            builder.Services.AddScoped<IMaterialEstudoService, MaterialEstudoService>();
            builder.Services.AddScoped<IMovimentacaoInstrumentoService, MovimentacaoInstrumentoService>();
            builder.Services.AddScoped<IMovimentacaoFigurinoService, MovimentacaoFigurinoService>();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<UsuarioIdentity>, ApplicationUserClaims>();

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

            // NOVO: Aplica as configurações de localização (incluindo o separador decimal)
            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}