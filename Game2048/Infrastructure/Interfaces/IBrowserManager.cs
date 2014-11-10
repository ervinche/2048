using System;

namespace Game2048.Infrastructure.Interfaces
{
    public interface IBrowserManager
    {
        Action Navigated
        {
            get;
            set;
        }

        void DeactivateErrors();

        void Upgrade();

        void NavigateTo(string url);

        object ExecuteScript(string script);

        void SendKey(string key);

        void ClickControlClass(string className);

        void InjectScript();

    }
}
