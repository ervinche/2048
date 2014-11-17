using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using Game2048.Infrastructure.Interfaces;
using Microsoft.Win32;
using mshtml;

namespace Game2048.Infrastructure
{
    /// <summary>
    /// Browser manager implementation
    /// </summary>
    public class BrowserManager : IBrowserManager
    {
        #region Constants

        private const string GUID_BROWSER_APP = "0002DF05-0000-0000-C000-000000000046";
        private const string GUID_BROWSER = "D30C1661-CDAF-11d0-8A3E-00C04FC9E26E";
        private const string BROWSER_EMULATION = "FEATURE_BROWSER_EMULATION";
        private const string DEVENV = "devenv.exe";
        private const string XDESPROC = "XDesProc.exe";
        private const string RESOURCE_SCRIPT = "Game2048.Resources.script.txt";
        private const string SCRIPT_INSERT_POSITION = "afterEnd";
        private const string BROWSER_REG_KEY = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\";
        private const string ERROR_METHOD = "Silent";
        private const string JS_EVAL = "eval";
        private const string SCRIPT = "script";
        private const string SENDKEY_SCRIPT = "sendkey({0})";
        private const string ELEMENT_TAG = "a";

        #endregion

        #region Fields

        private WebBrowser browser;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the navigated action.
        /// </summary>
        /// <value>
        /// The navigated action.
        /// </value>
        public Action Navigated
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserManager"/> class.
        /// </summary>
        /// <param name="browser">The browser.</param>
        public BrowserManager(WebBrowser browser)
        {
            this.browser = browser;
            browser.Navigated += browser_Navigated;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deactivates the errors.
        /// </summary>        
        public void DeactivateErrors()
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid(GUID_BROWSER_APP);
                Guid IID_IWebBrowser2 = new Guid(GUID_BROWSER);

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember(ERROR_METHOD, BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { true });
                }
            }

        }

        /// <summary>
        /// Upgrades this instance.
        /// </summary>
        public void Upgrade()
        {
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            if (String.Compare(fileName, DEVENV, true) == 0 || String.Compare(fileName, XDESPROC, true) == 0)
                return;
            UInt32 mode = 10000;
            SetBrowserFeatureControlKey(BROWSER_EMULATION, fileName, mode);
        }        

        /// <summary>
        /// Navigates to an url.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void NavigateTo(string url)
        {
            browser.Navigate(url);
           
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>Returns the execution result.</returns>
        public object ExecuteScript(string script)
        {
            object[] args = { script };

            return browser.InvokeScript(JS_EVAL, args);
        }

        /// <summary>
        /// Sends the key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SendKey(string key)
        {
            ExecuteScript(string.Format(SENDKEY_SCRIPT, key));
        }

        /// <summary>
        /// Clicks the control class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void ClickControlClass(string className)
        {
            var links = ((mshtml.HTMLDocument)browser.Document).getElementsByTagName(ELEMENT_TAG);
            foreach (mshtml.IHTMLElement link in links)
            {
                if (link.className == className)
                {
                    link.click();
                    break;
                }
            }
        }

        /// <summary>
        /// Injects the script.
        /// </summary>
        public void InjectScript()
        {
            mshtml.IHTMLElement se = ((mshtml.HTMLDocument)browser.Document).createElement(SCRIPT) as mshtml.IHTMLElement; //create the script
            mshtml.IHTMLScriptElement element = (mshtml.IHTMLScriptElement)se; //cast it as a script element
            
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_SCRIPT))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    element.text = reader.ReadToEnd();
                }
            }

            IHTMLElement body = (IHTMLElement)((mshtml.HTMLDocument)browser.Document).body; //get the body element
            IHTMLElement2 childhead = (IHTMLElement2)body.children[0]; //get it's first child
            childhead.insertAdjacentElement(SCRIPT_INSERT_POSITION, (IHTMLElement)element); //add the script element after the first child
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Navigated event of the browser control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Navigation.NavigationEventArgs"/> instance containing the event data.</param>
        async void browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Navigated != null)
            {
                await Task.Delay(1000);
                Navigated();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Sets the browser feature control key.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="value">The value.</param>
        private void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(String.Concat(BROWSER_REG_KEY, feature), RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        #endregion
    }
}
