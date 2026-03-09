using Core;
using Core.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("GrupoMusicalDatabase")
                      ?? throw new InvalidOperationException("Connection string 'GrupoMusicalDatabase' not found.");

builder.Services.AddDbContext<IdentityContext>(options => options.UseMySQL(connectionString));
builder.Services.AddDbContext<GrupoMusicalContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("GrupoMusicalDatabase") ?? ""));

builder.Services.AddIdentity<UsuarioIdentity, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<IdentityContext>()
.AddDefaultTokenProviders();

// 3. Configuração do JWT
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Gestão Grupo Musical API",
        Version = "v1"
    });

    // Isso cria o botão "Authorize" (Cadeado) no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Digite: 'Bearer {seu_token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddTransient<IPessoaService, PessoaService>();
builder.Services.AddScoped<IInformativoService, InformativoService>();
builder.Services.AddScoped<IFinanceiroService, FinanceiroService>();
builder.Services.AddScoped<IEnsaioService, EnsaioService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IMaterialEstudoService, MaterialEstudoService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UsePathBase("/grupogestaomusical");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();