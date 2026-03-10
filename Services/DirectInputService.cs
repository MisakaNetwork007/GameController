using GameController.Model;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameController.Services
{
    /// <summary>
    /// DirectInput 后端实现 IInputService。
    /// - 将 SharpDX 的 JoystickState 映射为项目通用的 InputState；
    /// - 在后台轮询并通过 InputStateChanged 事件通知订阅者（例如 ViewModel）。
    /// 重要：此类可能会在非 UI 线程触发事件，订阅者需要在 UI 线程上处理 UI 更新。
    /// </summary>
    internal class DirectInputService : IInputService
    {
        private DirectInput? _directInput;
        private Joystick? _joystick;
        private CancellationTokenSource _cts = new();
        private Task? _pollingTask;

        // 事件：当检测到新输入时触发（订阅方负责线程切换）
        public event Action<object>? InputStateChanged;
        public event Action<object>? DeviceInstanceChanged;

        /// <summary>
        /// 构造函数尝试查找并 Acquire 第一个连接的操纵杆设备。
        /// 若未找到操纵杆，会尝试在 UI 线程上提示用户。
        /// 不在构造中立即启动轮询，Start 方法用于显式启动。
        /// </summary>
        private void Instance()
        {
            _directInput = new DirectInput();

            // 查找第一个连接的操纵杆（Gamepad）
            var joystickGuid = Guid.Empty;
            foreach (var deviceInstance in _directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AttachedOnly))
            {
                joystickGuid = deviceInstance.InstanceGuid;
                DeviceInstanceChanged?.Invoke(deviceInstance); // 触发设备实例变化事件
                break;
            }

            if (joystickGuid == Guid.Empty)
            {
                MessageBox.Show("未找到连接的操纵杆（DirectInput）");
                
                _joystick = null;
                return;
            }
            
            _joystick = new Joystick(_directInput, joystickGuid);
            _joystick.Acquire();
        }

        /// <summary>
        /// 启动后台轮询任务（幂等）。
        /// </summary>
        public void Start()
        {

            if (_pollingTask != null && !_pollingTask.IsCompleted) return;

            Instance();
            if (_joystick == null) return;

            if (_cts.IsCancellationRequested)
            {
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }

            _pollingTask = Task.Run(PollLoopAsync, _cts.Token);
        }

        /// <summary>
        /// 停止轮询并释放资源（安全可重复调用）。
        /// </summary>
        public void Stop()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
            try
            {
                _pollingTask?.Wait();
            }
            catch { /* 忽略超时/取消异常 */ }
            _pollingTask?.Dispose(); // 释放之前的轮询任务资源（如果存在）
            _pollingTask = null; // 重置任务引用，允许后续重新启动
        }

        /// <summary>
        /// 轮询循环：读取操纵杆状态、映射为 InputState 并在有变化时触发事件。
        /// 使用 CancellationToken 优雅退出。
        /// </summary>
        private async Task PollLoopAsync()
        {
            var token = _cts.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _joystick?.Poll();
                    var state = _joystick?.GetCurrentState();
                    if (state != null)
                    {
                        InputStateChanged?.Invoke(state);
                    }
                }
                catch (SharpDX.SharpDXException ex)
                {
                    // 处理可能的设备丢失或访问异常 —— 在 UI 上提示并终止轮询
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"DirectInput 轮询出错：{ex.Message}");
                    });
                    break;
                }
                catch (Exception)
                {
                    // 忽略短期错误，继续尝试（或可在日志记录）
                }

                // 控制轮询频率
                await Task.Delay(15, token).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// 释放资源（停止轮询并释放 DirectInput / Joystick）。
        /// </summary>
        public void Dispose()
        {
            Stop();
            _joystick?.Unacquire();
            _joystick?.Dispose();
            _directInput?.Dispose();
            _cts.Dispose();
        }
    }
}
