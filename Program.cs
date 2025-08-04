using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TreeStore.Mappers;
using TreeStore.Models.Entities;
using TreeStore.Services.Interfaces;
using TreeStore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TreeStore.Controllers;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using TreeStore.Services.FavoriteServices;
using TreeStore.Services.Mail;

var builder = WebApplication.CreateBuilder(args);

// Access IConfiguration from the builder
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();

// Đăng ký HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IOrderServices, OrderServices>();    
builder.Services.AddScoped<IEmailService, EmailService>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<TreeStoreDBContext>(options =>
                                          //    options.UseSqlServer(
                                          //    configuration.GetConnectionString("DefaultConnection")
                                          //));
                                          options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                                          );
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowSpecificOrigin",
    //    builder =>
    //    {
    //        builder.WithOrigins("http://localhost:4200", "http://localhost:4300") // Cho ph�p frontend Angular
    //               .AllowAnyHeader()
    //               .AllowAnyMethod();
    //    });
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
    options.MultipartHeadersLengthLimit = int.MaxValue;
});


#region Inject Services
builder.Services.AddScoped<ITreeStoreDBContextProcedures, TreeStoreDBContextProcedures>();
builder.Services.AddScoped<ILoginServices, LoginServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerServices, CustomerServices>();
builder.Services.AddScoped<IReviewServices, ReviewServices>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IPromotionServices, PromotionServices>();
builder.Services.AddScoped<IPayPalService, PayPalService>();
builder.Services.AddScoped<IFavoriteServices, FavoriteService>();
#endregion
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder =>
    builder
    .WithOrigins("http://localhost:4200", "http://localhost:4300") // Ch? cho ph�p origin c? th?
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()); // Ch?p nh?n credentials (cookie, authentication headers)
// ✅ Áp dụng chính sách CORS đã khai báo ở trên
app.UseCors("AllowAll");


app.UseHttpsRedirection();
var pathFileClient = Path.Combine(Directory.GetCurrentDirectory(), @"FilesClient");
if (!Directory.Exists(pathFileClient))
{
    Directory.CreateDirectory(pathFileClient);
}
app.UseStaticFiles(new StaticFileOptions()
{
    //FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"FilesClient")),
    FileProvider = new PhysicalFileProvider(pathFileClient),
    RequestPath = new PathString("/FilesClient")
});

app.UseStaticFiles();

// Cho phép truy cập thư mục avatars trong wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars")),
    RequestPath = "/avatars"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();