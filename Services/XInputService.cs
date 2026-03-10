using GameController.ModelView;
using SharpDX.DirectInput;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameController.Services
{
    internal class XInputService : IInputService
    {
        // 一个 Controller 对象 (来自 SharpDX.XInput)
        private Controller _controller;
        private CancellationTokenSource _cts = new();
        private Task? _pollingTask;


        public event Action<object>? InputStateChanged;
        public event Action<object>? DeviceInstanceChanged;

        private void Instance()
        {
            _controller = new Controller(UserIndex.One); // 选择第一个控制器
            if (!_controller.IsConnected)
            {
                MessageBox.Show("控制器未连接");
                return;
            }

            var directInput = new DirectInput();

            foreach (var deviceInstance in directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly))
            {
                DeviceInstanceChanged?.Invoke(deviceInstance); // 触发设备实例变化事件
                break;
            }
        }
        

        async Task PollWorker()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var state = _controller.GetState();
                    InputStateChanged?.Invoke(state);
                }
                catch (Exception ex)
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"XInput 轮询出错：{ex.Message}");
                    });
                    break;
                }
                await Task.Delay(15, _cts.Token).ConfigureAwait(true);
            }
        }

        public void Start()
        {
            if (_pollingTask != null && !_pollingTask.IsCompleted)  return; // 已经在轮询了，幂等
                
            Instance();
            if (!_controller.IsConnected) return;

            if (_cts.IsCancellationRequested)
            {
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }

            _pollingTask = Task.Run(PollWorker, _cts.Token);
        }

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

            _pollingTask?.Dispose();
            _pollingTask = null;
        }
        
        public void Dispose()
        {
            Stop();
            _cts.Dispose();
        }
    }
}
