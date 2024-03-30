//using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialAPI.DBContexts;
using SocialAPI.Services.Comment;
using SocialAPI.Services.Image;
using SocialAPI.Services.Interface;
using SocialAPI.Services.Like;
using SocialAPI.Services.Post;
using SocialAPI.Services.User;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserInterface, UserService>();
builder.Services.AddScoped<ImageUserInterface, ImageUserService>();
builder.Services.AddScoped<ImagePostInterface, ImagePostService>();
builder.Services.AddScoped<LikeInterface, LikeService>();
builder.Services.AddScoped<PostInterface, PostService>();
builder.Services.AddScoped<CommentInterface, CommentService>();

//builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddDbContext<Context>(dbContextOptions => dbContextOptions.UseSqlite("Filename=./FVSocial.db"));

builder.Services.AddScoped<UserInterface, UserService>();

builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("FVSocialBearerJWT", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Token Bearer"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "FVSocialBearerJWT" }
                    }, new List<string>() }
    });
});
builder.Services.AddAuthentication("Bearer")
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new()
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = builder.Configuration["Authentication:Issuer"],
           ValidAudience = builder.Configuration["Authentication:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
       };
   }
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
