using System;

namespace Game2048.Game.Services.Interfaces
{
    public interface ICellTracker
    {
        void Initialize();

        void RegisterState(ulong gameStateValue);

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
    }
}
