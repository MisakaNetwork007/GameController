using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameController.Model;
using GameController.Services;
using SharpDX.DirectInput;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace GameController.ModelView
{
    /// <summary>
    /// ViewModel：从 IInputService 接收输入变更并通知 UI（INotifyPropertyChanged）。
    /// 通过构造函数注入 IInputService（依赖注入）。
    /// </summary>
    internal partial class MainWindowModelView : ObservableRecipient, IDisposable
    {
        private readonly IMapper _mapper;
        private readonly InputServiceSelector _serviceSelector;

        public MainWindowModelView(IMapper mapper, InputServiceSelector serviceSelector)
        {
            
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serviceSelector = serviceSelector ?? throw new ArgumentNullException(nameof(serviceSelector));

            foreach (var service in _serviceSelector.Services)
            {
                // 订阅输入变化事件。事件可能在后台线程触发，所以在处理时切回 UI 线程。
                service.InputStateChanged += OnInputStateChanged;
                // 订阅输入变化事件。事件可能在后台线程触发，所以在处理时切回 UI 线程。
                service.DeviceInstanceChanged += OnDeviceInstanceChanged;
            }
            
        }

        // 事件处理：切换到 UI 线程并设置属性（以触发 NotifyPropertyChanged）
        private void OnInputStateChanged(Object state)
        {
            // ViewModel 属于 UI 层，确保在 UI 线程上更新属性以避免跨线程异常
            Application.Current?.Dispatcher.Invoke(() =>
            {
                var data = _mapper.Map<StateModel>(state); // 使用 AutoMapper 映射输入状态到 ViewModel 模型
                UpdateState(data);
            });
        }

        private void OnDeviceInstanceChanged(object device)
        {
            var deviceInstance = device as DeviceInstance;
            DeviceName = deviceInstance?.InstanceName ?? "未知设备";
        }

        private void UpdateState(StateModel state)
        {
            // 直接更新模型属性（会触发 PropertyChanged）
            LeftX = state.LeftX;
            LeftY = state.LeftY;
            RightX = state.RightX;
            RightY = state.RightY;
            LeftTriggerRotation = state.LeftTriggerRotation;
            RightTriggerRotation = state.RightTriggerRotation;
            IsLeftTrigger = state.IsLeftTrigger;
            IsRightTrigger = state.IsRightTrigger;
            IsLeftBumper = state.IsLeftBumper;
            IsRightBumper = state.IsRightBumper;
            IsLeftStick = state.IsLeftStick;
            IsRightStick = state.IsRightStick;
            IsLeftButton = state.IsLeftButton;
            IsRightButton = state.IsRightButton;
            IsUpButton = state.IsUpButton;
            IsDownButton = state.IsDownButton;
            IsAButton = state.IsAButton;
            IsBButton = state.IsBButton;
            IsXButton = state.IsXButton;
            IsYButton = state.IsYButton;
            IsStartButton = state.IsStartButton;
            IsOptionsButton = state.IsOptionsButton;
            IsHomeButton = state.IsHomeButton;
            IsTouchpadButton = state.IsTouchpadButton;
        }

        [ObservableProperty]
        private int _leftX;

        [ObservableProperty]
        private int _leftY;

        [ObservableProperty]
        private int _rightX;

        [ObservableProperty]
        private int _rightY;

        [ObservableProperty]
        private int _leftTriggerRotation;

        [ObservableProperty]
        private int _rightTriggerRotation;

        [ObservableProperty]
        private bool _isLeftTrigger;

        [ObservableProperty]
        private bool _isRightTrigger;

        [ObservableProperty]
        private bool _isLeftBumper;

        [ObservableProperty]
        private bool _isRightBumper;

        [ObservableProperty]
        private bool _isLeftStick;

        [ObservableProperty]
        private bool _isRightStick;

        [ObservableProperty]
        private bool _isLeftButton;

        [ObservableProperty]
        private bool _isRightButton;

        [ObservableProperty]
        private bool _isUpButton;

        [ObservableProperty]
        private bool _isDownButton;

        [ObservableProperty]
        private bool _isAButton;

        [ObservableProperty]
        private bool _isBButton;

        [ObservableProperty]
        private bool _isXButton;

        [ObservableProperty]
        private bool _isYButton;

        [ObservableProperty]
        private bool _isStartButton;

        [ObservableProperty]
        private bool _isOptionsButton;

        [ObservableProperty]
        private bool _isHomeButton;

        [ObservableProperty]
        private bool _isTouchpadButton;

        [ObservableProperty]
        private string _deviceName = "";

        [ObservableProperty]
        private int _deviceProtocol;

        [RelayCommand]
        private void StartConnection()
        {
            var serviceType = DeviceProtocol == 0 ? "directInput" : "xInput";
            var service = _serviceSelector.GetService(serviceType);
            service.Start();
        }

        [RelayCommand]
        private void StopConnection()
        {
            var serviceType = DeviceProtocol == 0 ? "directInput" : "xInput";
            var service = _serviceSelector.GetService(serviceType);
            service.Stop();
            DeviceName = "未连接";
        }


        public void Dispose()
        {
            foreach (var service in _serviceSelector.Services)
            {
                // 订阅输入变化事件。事件可能在后台线程触发，所以在处理时切回 UI 线程。
                service.InputStateChanged -= OnInputStateChanged;
                // 订阅输入变化事件。事件可能在后台线程触发，所以在处理时切回 UI 线程。
                service.DeviceInstanceChanged -= OnDeviceInstanceChanged;
                service.Stop(); // 确保停止服务以释放资源
            }
        }
    }
}