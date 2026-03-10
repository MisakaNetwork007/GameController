using AutoMapper;
using GameController.Model;
using SharpDX.DirectInput;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameController
{
    internal class CustomProfile : Profile
    {
        public CustomProfile()
        {
            //DirectInput 的 JoystickState 映射到项目通用的 StateModelView。
            CreateMap<JoystickState, StateModel>()
                .ForMember(dest => dest.LeftX, opt => opt.MapFrom(src => src.X - 32768))
                .ForMember(dest => dest.LeftY, opt => opt.MapFrom(src => src.Y - 32768))
                .ForMember(dest => dest.RightX, opt => opt.MapFrom(src => src.Z - 32768))
                .ForMember(dest => dest.RightY, opt => opt.MapFrom(src => src.RotationZ - 32768))
                .ForMember(dest => dest.LeftTriggerRotation, opt => opt.MapFrom(src => src.RotationX))
                .ForMember(dest => dest.RightTriggerRotation, opt => opt.MapFrom(src => src.RotationY))
                .ForMember(dest => dest.IsLeftTrigger, opt => opt.MapFrom(src => (src.Buttons[6] ? 1 : 0)))
                .ForMember(dest => dest.IsRightTrigger, opt => opt.MapFrom(src => (src.Buttons[7] ? 1 : 0)))
                .ForMember(dest => dest.IsLeftBumper, opt => opt.MapFrom(src => (src.Buttons[4] ? 1 : 0)))
                .ForMember(dest => dest.IsRightBumper, opt => opt.MapFrom(src => (src.Buttons[5] ? 1 : 0)))
                .ForMember(dest => dest.IsLeftStick, opt => opt.MapFrom(src => (src.Buttons[10] ? 1 : 0)))
                .ForMember(dest => dest.IsRightStick, opt => opt.MapFrom(src => (src.Buttons[11] ? 1 : 0)))
                .ForMember(dest => dest.IsLeftButton, opt => opt.MapFrom(src => (src.PointOfViewControllers[0]>=22500 && src.PointOfViewControllers[0] <= 31500 ? 1 : 0)))
                .ForMember(dest => dest.IsRightButton, opt => opt.MapFrom(src => (src.PointOfViewControllers[0] >= 4500 && src.PointOfViewControllers[0] <=13500 ? 1 : 0)))
                .ForMember(dest => dest.IsUpButton, opt => opt.MapFrom(src => ((src.PointOfViewControllers[0] <= 4500 && src.PointOfViewControllers[0] >=0)|| src.PointOfViewControllers[0] >= 31500 ? 1 : 0)))
                .ForMember(dest => dest.IsDownButton, opt => opt.MapFrom(src => (src.PointOfViewControllers[0] >= 13500 && src.PointOfViewControllers[0] <= 22500 ? 1 : 0)))
                .ForMember(dest => dest.IsAButton, opt => opt.MapFrom(src => (src.Buttons[1] ? 1 : 0)))
                .ForMember(dest => dest.IsBButton, opt => opt.MapFrom(src => (src.Buttons[2] ? 1 : 0)))
                .ForMember(dest => dest.IsXButton, opt => opt.MapFrom(src => (src.Buttons[0] ? 1 : 0)))
                .ForMember(dest => dest.IsYButton, opt => opt.MapFrom(src => (src.Buttons[3] ? 1 : 0)))
                .ForMember(dest => dest.IsStartButton, opt => opt.MapFrom(src => (src.Buttons[12] ? 1 : 0)))
                .ForMember(dest => dest.IsOptionsButton, opt => opt.MapFrom(src => (src.Buttons[8] ? 1 : 0)))
                .ForMember(dest => dest.IsHomeButton, opt => opt.MapFrom(src => (src.Buttons[9] ? 1 : 0)))
                .ForMember(dest => dest.IsTouchpadButton, opt => opt.MapFrom(src => (src.Buttons[13] ? 1 : 0)));

            CreateMap<State,StateModel>()
                .ForMember(dest => dest.LeftX, opt => opt.MapFrom(src => src.Gamepad.LeftThumbX))
                .ForMember(dest => dest.LeftY, opt => opt.MapFrom(src => src.Gamepad.LeftThumbY))
                .ForMember(dest => dest.RightX, opt => opt.MapFrom(src => src.Gamepad.RightThumbX))
                .ForMember(dest => dest.RightY, opt => opt.MapFrom(src => src.Gamepad.RightThumbY))
                .ForMember(dest => dest.LeftTriggerRotation, opt => opt.MapFrom(src => src.Gamepad.LeftTrigger * 257))
                .ForMember(dest => dest.RightTriggerRotation, opt => opt.MapFrom(src => src.Gamepad.RightTrigger * 257))
                .ForMember(dest => dest.IsLeftTrigger, opt => opt.MapFrom(src => (src.Gamepad.LeftTrigger>0 ? 1 : 0)))
                .ForMember(dest => dest.IsRightTrigger, opt => opt.MapFrom(src => (src.Gamepad.RightTrigger>0 ? 1 : 0)))
                .ForMember(dest => dest.IsLeftBumper, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) ? 1 : 0)))
                .ForMember(dest => dest.IsRightBumper, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) ? 1 : 0)))
                .ForMember(dest => dest.IsLeftStick, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb) ? 1 : 0)))
                .ForMember(dest => dest.IsRightStick, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb) ? 1 : 0)))
                .ForMember(dest => dest.IsLeftButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) ? 1 : 0)))
                .ForMember(dest => dest.IsRightButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight) ? 1 : 0)))
                .ForMember(dest => dest.IsUpButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp) ? 1 : 0)))
                .ForMember(dest => dest.IsDownButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown) ? 1 : 0)))
                .ForMember(dest => dest.IsAButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A) ? 1 : 0)))
                .ForMember(dest => dest.IsBButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B) ? 1 : 0)))
                .ForMember(dest => dest.IsXButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X) ? 1 : 0)))
                .ForMember(dest => dest.IsYButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y) ? 1 : 0)))
                .ForMember(dest => dest.IsStartButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start) ? 1 : 0)))
                .ForMember(dest => dest.IsOptionsButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back) ? 1 : 0)))
                .ForMember(dest => dest.IsHomeButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start) ? 1 : 0)))
                .ForMember(dest => dest.IsTouchpadButton, opt => opt.MapFrom(src => (src.Gamepad.Buttons.HasFlag(GamepadButtonFlags.None) ? 1 : 0)));
        }
    }
}
