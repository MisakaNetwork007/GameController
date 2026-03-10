using GameController.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameController.Services
{
    /// <summary>
    /// 简化的输入状态 DTO —— 与硬件/第三方库解耦（只包含需要绑定的基本值）。
    /// 使用 record 方便不可变数据表示。
    /// </summary>
    //public sealed record InputState(int X, int Y, int Z);

    /// <summary>
    /// 抽象输入服务接口。ViewModel 通过此接口订阅输入变化，且不依赖具体实现（Direct/XInput 等）。
    /// 提供 Start/Stop 生命周期方法以及事件发布模型。
    /// </summary>
    public interface IInputService : IDisposable
    {
        /// <summary>
        /// 当有新的输入时会触发（可能在后台线程触发，订阅方负责线程切换到 UI 线程）。
        /// </summary>
        event Action<object>? InputStateChanged;
        event Action<object>? DeviceInstanceChanged;

        /// <summary>
        /// 启动底层输入轮询/监测（实现可选择在构造时自动启动或在此方法中启动）。
        /// </summary>
        void Start();

        /// <summary>
        /// 停止轮询并释放资源。
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 服务策略选择器：根据协议类型（DirectInput/XInput）选择对应的输入服务实现。
    /// </summary>
    public class InputServiceSelector
    {
        private readonly IEnumerable<IInputService> _services;

        public InputServiceSelector(IEnumerable<IInputService> services)
        {
            _services = services;
        }

        public IInputService GetService(string type)
        {
            return type switch
            {
                "directInput" => _services.OfType<DirectInputService>().First(),
                "xInput" => _services.OfType<XInputService>().First(),
                _ => throw new NotSupportedException()
            };
        }

        public IEnumerable<IInputService> Services => _services;
    }
}
