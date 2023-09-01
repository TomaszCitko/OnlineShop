using Codecool.CodecoolShop.Migrations;
using Codecool.CodecoolShop.Services;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Codecool.CodecoolShop
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();

            Log.Information("Inside Startup ctor");
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<CodecoolshopContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CodecoolShopConnectionString")));
            services.AddSession();
            services.Configure<EmailContext>(Configuration.GetSection("Email"));
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Product/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseStaticFiles();


            app.UseSerilogRequestLogging();          

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Product}/{action=Index}/{id?}");
            });
            

            //SetupInMemoryDatabases();
        }

        //private void SetupInMemoryDatabases()
        //{
        //    IProductDao productDataStore = ProductDaoMemory.GetInstance();
        //    IProductCategoryDao productCategoryDataStore = ProductCategoryDaoMemory.GetInstance();
        //    ISupplierDao supplierDataStore = SupplierDaoMemory.GetInstance();

        //    Supplier amazon = new Supplier{Name = "Amazon", Description = "Digital content and services"};
        //    supplierDataStore.Add(amazon);
        //    Supplier lenovo = new Supplier{Name = "Lenovo", Description = "Computers"};
        //    supplierDataStore.Add(lenovo);
        //    ProductCategory tablet = new ProductCategory {Name = "Tablet", Department = "Hardware", Description = "A tablet computer, commonly shortened to tablet, is a thin, flat mobile computer with a touchscreen display." };
        //    productCategoryDataStore.Add(tablet);
        //    productDataStore.Add(new Product { Name = "Amazon Fire", DefaultPrice = 49.9m, Currency = "USD", Description = "Fantastic price. Large content ecosystem. Good parental controls. Helpful technical support.", ProductCategory = tablet, Supplier = amazon });
        //    productDataStore.Add(new Product { Name = "Lenovo IdeaPad Miix 700", DefaultPrice = 479.0m, Currency = "USD", Description = "Keyboard cover is included. Fanless Core m5 processor. Full-size USB ports. Adjustable kickstand.", ProductCategory = tablet, Supplier = lenovo });
        //    productDataStore.Add(new Product { Name = "Amazon Fire HD 8", DefaultPrice = 89.0m, Currency = "USD", Description = "Amazon's latest Fire HD 8 tablet is a great value for media consumption.", ProductCategory = tablet, Supplier = amazon });
        //}
    }
}
