using GameController.Model;
using GameController.ModelView;
using GameController.Services;
using Microsoft.Extensions.DependencyInjection;
using SharpDX.DirectInput;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GameController
{
    /// <summary>
    /// 应用程序入口：在 OnStartup 中构建 DI 容器并解析 MainWindow 与 ViewModel。
    /// 通过 Microsoft.Extensions.DependencyInjection 提供简单、可测试的 IoC。
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 配置服务容器
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddTransient<StateModel>(); // 注册 StateModel（按需创建新实例）

            services.AddSingleton<InputServiceSelector>();
            // 注册 IInputService 的实现（DirectInputBackend）
            // 使用单例（硬件后端通常只需一个实例），也可按需改为 Scoped/Transient
            services.AddSingleton<IInputService, DirectInputService>();
            services.AddSingleton<IInputService, XInputService>();

            services.AddAutoMapper(cfg=> { },typeof(CustomProfile)); 

            // 注册 ViewModel（生命周期与 App 相同的单例也可以，或者使用 transient）
            services.AddSingleton<MainWindowModelView>();

            // 注册 MainWindow（XAML 对应的窗口类）
            services.AddSingleton<MainWindow>();

            // 构建容器
            ServiceProvider = services.BuildServiceProvider();

            // 解析并启动主窗口：将 ViewModel 注入到 DataContext
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowModelView>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 清理容器（若需要释放托管资源）
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }

}
