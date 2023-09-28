using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using uStoreAPI;
using uStoreAPI.ModelsAzureDB;
using uStoreAPI.Services;
using Microsoft.Extensions.Azure;
using Hangfire;
using Hangfire.SqlServer;
using uStoreAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UstoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<PlazasService>();
builder.Services.AddScoped<TiendasService>();
builder.Services.AddScoped<HorariosService>();
builder.Services.AddScoped<CategoriasService>();
builder.Services.AddScoped<ProductosService>();
builder.Services.AddScoped<PeriodosPredeterminadosService>();
builder.Services.AddScoped<CalificacionesService>();
builder.Services.AddScoped<SolicitudesApartadoService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PublicacionesService>();
builder.Services.AddScoped<TendenciasService>();

builder.Services.AddSingleton<NotificacionesApartadoService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.WebSockets.IsWebSocketRequest)
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureBlobStorageConnection:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureBlobStorageConnection:queue"], preferMsi: true);
});

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangFireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    })
);
builder.Services.AddHangfireServer();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsWebPage",
    builder => builder.WithOrigins("https://ustoree.azurewebsites.net")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("CorsWebPage");

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ApartadosHub>("/apartadosHub");
    endpoints.MapControllers();
});

app.Run();
