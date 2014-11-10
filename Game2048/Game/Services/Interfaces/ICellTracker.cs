using System;

namespace Game2048.Game.Services.Interfaces
{
    public interface ICellTracker
    {
        Action<int> NewCellAction
        {
            get;
            set;
        }

        Action WinAction
        {
            get;
            set;
        }

        void Initialize();

        void RegisterState(ulong gameStateValue);
    }
}
