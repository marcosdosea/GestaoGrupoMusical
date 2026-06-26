using Core;
using Core.Service;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurações de Banco de Dados
var connectionString = builder.Configuration.GetConnectionString("GrupoMusicalDatabase")
                       ?? throw new InvalidOperationException("Connection string 'GrupoMusicalDatabase' not found.");

builder.Services.AddDbContext<IdentityContext>(options => options.UseMySQL(connectionString));
builder.Services.AddDbContext<GrupoMusicalContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddIdentity<UsuarioIdentity, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<IdentityContext>()
.AddDefaultTokenProviders();

// 2. Configuração do Firebase Admin (Inicialização Única)
var caminhoFirebase = Path.Combine(builder.Environment.ContentRootPath, "firebase-admin.json");
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
    Console.WriteLine($"ERRO: Arquivo de credenciais não encontrado em: {caminhoFirebase}");
}

// 3. Configuração de JWT, Services e CORS
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["ChaveSecreta"] ?? "CHAVE_PADRAO_COM_MAIS_DE_32_CARACTERES";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Emissor"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audiencia"],
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// Registro dos Serviços
builder.Services.AddScoped<INotificacaoAdminService, NotificacaoAdminService>();
builder.Services.AddTransient<IDispositivoService, DispositivoService>();
builder.Services.AddScoped<IInformativoService, InformativoService>();
builder.Services.AddScoped<IFinanceiroService, FinanceiroService>();
builder.Services.AddScoped<IEnsaioService, EnsaioService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IMaterialEstudoService, MaterialEstudoService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Pipeline de Requisição
app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UsePathBase("/grupogestaomusical");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();