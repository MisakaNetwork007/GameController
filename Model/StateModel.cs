using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameController.Model
{
    public class StateModel
    {
        
        public int LeftX { get; set; }
        
        public int LeftY { get; set; }

        public int RightX { get; set; }

        public int RightY { get; set; }

        public int LeftTriggerRotation { get; set; }

        public int RightTriggerRotation { get; set; }

        public bool IsLeftTrigger { get; set; }

        public bool IsRightTrigger { get; set; }

        public bool IsLeftBumper { get; set; }

        public bool IsRightBumper { get; set; }

        public bool IsLeftStick { get; set; }

        public bool IsRightStick { get; set; }

        public bool IsLeftButton { get; set; }

        public bool IsRightButton { get; set; }

        public bool IsUpButton { get; set; }

        public bool IsDownButton { get; set; }

        public bool IsAButton { get; set; }

        public bool IsBButton { get; set; }

        public bool IsXButton { get; set; }

        public bool IsYButton { get; set; }

        public bool IsStartButton { get; set; }

        public bool IsOptionsButton { get; set; }

        public bool IsHomeButton { get; set; }

        public bool IsTouchpadButton { get; set; }

    }
}
