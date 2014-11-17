using System;

namespace Game2048.Infrastructure.Interfaces
{
    /// <summary>
    /// Browser Manager.
    /// </summary>
    public interface IBrowserManager
    {
        /// <summary>
        /// Gets or sets the navigated action.
        /// </summary>
        /// <value>
        /// The navigated action.
        /// </value>
        Action Navigated
        {
            get;
            set;
        }

        /// <summary>
        /// Deactivates the errors.
        /// </summary>   
        void DeactivateErrors();

        /// <summary>
        /// Upgrades this instance.
        /// </summary>
        void Upgrade();

        /// <summary>
        /// Navigates to an url.
        /// </summary>
        /// <param name="url">The URL.</param>
        void NavigateTo(string url);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>Returns the execution result.</returns>
        object ExecuteScript(string script);

        /// <summary>
        /// Sends the key.
        /// </summary>
        /// <param name="key">The key.</param>
        void SendKey(string key);

        /// <summary>
        /// Clicks the control class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        void ClickControlClass(string className);

        /// <summary>
        /// Injects the script.
        /// </summary>
        void InjectScript();

    }
}
