namespace AspNetCoreExample.Identity
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using AspNetCoreExample.Ddd.Connection;
    using AspNetCoreExample.Identity.Data;
    using AspNetCoreExample.Identity.Services;

    public class Startup
    {
        IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public string ConnectionString => this.Configuration.GetConnectionString("DefaultConnection");

        public (string appId, string appSecret) FacebookOptions
            => (this.Configuration["Authentication:Facebook:AppId"],
                this.Configuration["Authentication:Facebook:AppSecret"]);

        public (string clientId, string clientSecret) GoogleOptions
            => (this.Configuration["Authentication:Google:ClientId"],
                this.Configuration["Authentication:Google:ClientSecret"]);

        public (string consumerKey, string consumerSecret) TwitterOptions
            => (this.Configuration["Authentication:Twitter:ConsumerKey"],
                this.Configuration["Authentication:Twitter:ConsumerSecret"]);

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<NHibernate.ISessionFactory>(serviceProvider =>
                AspNetCoreExample.Ddd.Mapper.TheMapper.BuildSessionFactory(this.ConnectionString)
            );

            services.AddSingleton<IDatabaseFactory, DatabaseFactory>();

            services.AddTransient<Microsoft.AspNetCore.Identity.IUserStore<Ddd.IdentityDomain.User>, UserStore>();

            services.AddTransient<Microsoft.AspNetCore.Identity.IRoleStore<Ddd.IdentityDomain.Role>, RoleStore>();

            services.AddIdentity<Ddd.IdentityDomain.User, Ddd.IdentityDomain.Role>().AddDefaultTokenProviders();

            services.AddAuthentication()
                    .AddFacebook(options => (options.AppId, options.AppSecret) = this.FacebookOptions)
                    .AddGoogle(options => (options.ClientId, options.ClientSecret) = this.GoogleOptions)
                    .AddTwitter(options => (options.ConsumerKey, options.ConsumerSecret) = this.TwitterOptions)
                    ;


            services.ConfigureApplicationCookie(config =>
            {
                config.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                        {
                            ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0);
                    }
                };
            });


            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
