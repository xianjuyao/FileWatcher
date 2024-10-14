using System;
using System.Configuration;
using System.Windows;
using FileWatcher.Services;
using FileWatcher.ViewModels;
using LogLib;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace FileWatcher
{
    public sealed partial class App : Application
    {
        private static readonly string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        public App()
        {
            Services = ConfigureServices();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton(sp => new LogService());
            services.AddSingleton<ISqlSugarClient>(sp =>
            {
                //Scoped用SqlSugarClient 
                var sqlSugar = new SqlSugarScope(new ConnectionConfig
                    {
                        DbType = DbType.SqlServer,
                        ConnectionString = connStr,
                        IsAutoCloseConnection = true
                    },
                    db =>
                    {
                        db.Aop.OnLogExecuting = (sql, pars) =>
                        {
                            Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
                        };
                    });
                return sqlSugar;
            });
            services.AddSingleton<IDialogService, DialogService>();
            services.AddScoped<IDbService, DbService>();
            services.AddTransient<MainViewModel>();
            services.AddTransient(sp => new MainWindow { DataContext = sp.GetService<MainViewModel>() });
            return services.BuildServiceProvider();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = Services.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}