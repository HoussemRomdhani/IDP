using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace IDP
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(env.ContentRootPath)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                  .AddEnvironmentVariables();
            Configuration = builder.Build();
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddTransient<Config>();
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            string connectionString = Configuration["ConnectionString"];
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // configure identity server with in-memory stores, keys, clients and scopes
           var builder = services.AddIdentityServer()
                 .AddDeveloperSigningCredential()
                .AddTestUsers(TestUsers.Users)
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

            //    builder.AddDeveloperSigningCredential();
          // builder.AddSigningCredential(LoadCertificateFromStore());

        }

        private X509Certificate2 LoadCertificateFromStore()
        {
            X509Certificate2 cert = 
                new X509Certificate2("bookstore.pfx", 
                "password", X509KeyStorageFlags.MachineKeySet);
            return cert;
        }

        public void Configure(IApplicationBuilder app)
        {

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeDatabase(app);
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider
                    .GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider
                    .GetRequiredService<ConfigurationDbContext>();

                var config = serviceScope.ServiceProvider
                   .GetRequiredService<Config>();

                context.Database.Migrate();
                //if (!context.Clients.Any())
                //{
                //    foreach (var client in config.Clients)
                //    {
                //        context.Clients.Add(client.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                //if (!context.ApiResources.Any())
                //{
                //    foreach (var resource in config.Apis)
                //    {
                //        context.ApiResources.Add(resource.ToEntity());
                //    }
                //    context.SaveChanges();
                //}
            }
        }
    }
}
