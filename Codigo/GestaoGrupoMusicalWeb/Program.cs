using Core;
using Core.Service;
using DotNetEnv;
using FirebaseAdmin;
using GestaoGrupoMusicalWeb.Helpers;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service;
using System.Globalization;

namespace GestaoGrupoMusicalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Carrega as variáveis do arquivo .env
            Env.TraversePath().Load();

            var builder = WebApplication.CreateBuilder(args);

            // Define a cultura base como pt-BR
            var cultureInfo = new CultureInfo("pt-BR");

            // Força o separador decimal a ser o ponto (.)
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

            // Aplica a configuração ao thread padrão
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            // Configura a localização da aplicação
            builder.Services.Configure<
                Microsoft.AspNetCore.Builder.RequestLocalizationOptions>(
                options =>
                {
                    options.DefaultRequestCulture =
                        new Microsoft.AspNetCore.Localization.RequestCulture(
                            culture: cultureInfo,
                            uiCulture: cultureInfo);

                    options.SupportedCultures = new[] { cultureInfo };
                    options.SupportedUICultures = new[] { cultureInfo };
                });

            // Configuração do Firebase
            var caminhoFirebase = Path.Combine(
                builder.Environment.ContentRootPath,
                "firebase-admin.json");

            if (File.Exists(caminhoFirebase))
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(caminhoFirebase)
                    });
                }
            }
            else
            {
                Console.WriteLine(
                    $"ERRO: Arquivo de credenciais não encontrado em: {caminhoFirebase}");
            }

            // Serviços MVC
            builder.Services.AddControllersWithViews();

            // Banco de dados
            var connectionString =
                builder.Configuration.GetConnectionString("GrupoMusicalDatabase");

            builder.Services.AddDbContext<GrupoMusicalContext>(
                options => options.UseMySQL(connectionString));

            builder.Services.AddDbContext<IdentityContext>(
                options => options.UseMySQL(connectionString));

            // Identity
            builder.Services.AddIdentity<UsuarioIdentity, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan =
                    TimeSpan.FromMinutes(5);

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            // Validade dos tokens
            builder.Services.Configure<DataProtectionTokenProviderOptions>(
                options =>
                {
                    options.TokenLifespan = TimeSpan.FromHours(2);
                });

            // Cookie de autenticação
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Autenticar";
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Autenticar";
                options.ReturnUrlParameter =
                    CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            // Registro dos serviços da aplicação
            builder.Services.AddScoped<
                INotificacaoAdminService, NotificacaoAdminService>();

            builder.Services.AddTransient<
                IDispositivoService, DispositivoService>();

            builder.Services.AddAutoMapper(
                AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddTransient<
                IGrupoMusicalService, GrupoMusicalService>();

            builder.Services.AddTransient<
                IPessoaService, PessoaService>();

            builder.Services.AddTransient<
                IEventoService, EventoService>();

            builder.Services.AddTransient<
                IInstrumentoMusicalService, InstrumentoMusicalService>();

            builder.Services.AddTransient<
                IManequimService, ManequimService>();

            builder.Services.AddTransient<
                IInformativoService, InformativoService>();

            builder.Services.AddTransient<
                IFigurinoService, FigurinoService>();

            builder.Services.AddTransient<
                IFinanceiroService, FinanceiroService>();

            builder.Services.AddScoped<
                IEnsaioService, EnsaioService>();

            builder.Services.AddScoped<
                IMaterialEstudoService, MaterialEstudoService>();

            builder.Services.AddScoped<
                IMovimentacaoInstrumentoService,
                MovimentacaoInstrumentoService>();

            builder.Services.AddScoped<
                IMovimentacaoFigurinoService,
                MovimentacaoFigurinoService>();

            builder.Services.AddScoped<
                IUserClaimsPrincipalFactory<UsuarioIdentity>,
                ApplicationUserClaims>();

            var app = builder.Build();

            // Pipeline HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (!app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            // Aplica as configurações de localização
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