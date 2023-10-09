
using blogpessoal.Configuration;
using blogpessoal.Data;
using blogpessoal.Model;
using blogpessoal.Security;
using blogpessoal.Security.Implements;
using blogpessoal.Service;
using blogpessoal.Service.Implements;
using blogpessoal.Validator;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace blogpessoal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add Controller Class
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });

            // Conex�o com o Banco de dados
            if (builder.Configuration["Environment:Start"] == "PROD")
            {
                builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("secrets.json");

                var connectionString = builder.Configuration
                .GetConnectionString("ProdConnection");

                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString)
                );

            }
            else
            {
                 var connectionString = builder.Configuration
                       .GetConnectionString ("DefaultConnection");

                  builder.Services.AddDbContext<AppDbContext>(options =>
                  options.UseSqlServer(connectionString)
                );

            }

            // Registrar a valida��o das entidades
            builder.Services.AddTransient<IValidator<Postagem>, PostagemValidator>();
            builder.Services.AddTransient<IValidator<Tema>, TemaValidator>();
            builder.Services.AddTransient<IValidator<User>, UserValidator>();

            // Registrar as Classes de Servi�o (Service)
            builder.Services.AddScoped<IPostagemService, PostagemService>();
            builder.Services.AddScoped<ITemaService, TemaService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            // Adicionar a Valida��o do Token
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var Key = Encoding.UTF8.GetBytes(Settings.Secret);
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //Registrar o Swagger
            builder.Services.AddSwaggerGen(options =>
            {
            options.SwaggerDoc("v1", new OpenApiInfo
            {   //Personalizar a P�gna inicial do Swagger
                Version = "v1",
                Title = "Projeto Blog Pessoal",
                Description = "Projeto Blog Pessoal - ASP.NET Core 7.0",
                Contact = new OpenApiContact()
                {
                    Name = "Karina Akina Miyazaki",
                    Email = "karinamzk2@gmail.com",
                    Url = new Uri("https://github.com/karinamzk")
                },
                License = new OpenApiLicense
                {
                    Name = "Gitbub",
                    Url = new Uri("https://github.com/karinamzk")
                }
            });
                // Configura��o de seguran�a no Swegger
                       options.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
                       {
                              In = ParameterLocation.Header,
                              Description = "Digite um token JWT v�lido",
                              Name = "Autorization",
                              Type = SecuritySchemeType.Http,
                              BearerFormat = "JWT",
                              Scheme = "Bearer"

                       });


                //Adicionar a configura��o visual da Seguran�a no Swagger
                options.OperationFilter<AuthResponsesOperationFilter>();

            });

            // Adicionar o Fluent Validation no Swagger

            builder.Services.AddFluentValidationRulesToSwagger();

            // Configura��o do CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "MyPolicy",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            // Criar o Banco de dados e as tabelas 

            using (var scope = app.Services.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
           // if (app.Environment.IsDevelopment())
            // {
                app.UseSwagger();

            // Swagger Como Pagina Inicial na Nuvem
            if (app.Environment.IsProduction())
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/sagger.json", "Blog Pessoal - v1");
                    options.RoutePrefix = string.Empty;
                });

            }

            // Inicializa o CORS

            app.UseCors("MyPolicy");

            app.UseAuthentication();

            // Habilitar a Autentica��o e a Autoriza��o

            app.UseAuthorization();

            // Habilitar Controller para ser acessado para quem tem o token

            app.MapControllers();

            app.Run();
        }
    }
}