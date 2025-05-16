using EventBooking;
using EventBooking.Application.Models;
using EventBooking.Application.Services;
using EventBooking.Application.Services.IService;
using EventBooking.Core.Entities;
using EventBooking.Core.IRepository;
using EventBooking.Helpers;
using EventBooking.Infrastructure.Data;
using EventBooking.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default30", new CacheProfile()
    {
        Duration = 30
    });
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>

{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Eventa",
        Description = "Event Booking API",
        TermsOfService = new Uri("https://www.linkedin.com/in/mohamedsaad14/"),
        Contact = new OpenApiContact
        {
            Name = "Mohamed Saad",
            Email = "mohamedsaad2756@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/mohamedsaad14/"),
        },
        License = new OpenApiLicense
        {
            Name = "My license",
            Url = new Uri("https://www.linkedin.com/in/mohamedsaad14/")
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT key"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
           new List<string>()
        }
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o => // Corrected from AddBearerToken to AddJwtBearer  
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<EventValidateAttribute>();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddResponseCaching();

builder.Services.AddCors();


var app = builder.Build();

    // Configure the HTTP request pipeline.  
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { Roles.Role_Admin, Roles.Role_User};
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var FirstName = "Mohamed";
    var LastName = "Saad";
    var Username = "msaad";
    var Email = "mohamedsaad2756@gmail.com";
    var password = "Ad@123";

    var user = new ApplicationUser
    {
        FirstName = FirstName,
        LastName = LastName,
        UserName = Username,
        Email = Email,
    };
    await userManager.CreateAsync(user, password);
    await userManager.AddToRoleAsync(user, Roles.Role_Admin);
}

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.Run();


