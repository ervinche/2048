using Game2048.Infrastructure.Interfaces;
using Microsoft.Win32;
using mshtml;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Game2048.Infrastructure
{
    public class BrowserManager : IBrowserManager
    {
        #region Fields

        private WebBrowser browser;

        #endregion

        #region Properties]

        /// <summary>
        /// Gets or sets the navigated.
        /// </summary>
        /// <value>
        /// The navigated.
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

        #region Events

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

        #region Methods

        /// <summary>
        /// Deactivates the errors.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">browser</exception>
        public void DeactivateErrors()
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { true });
                }
            }

        }

        /// <summary>
        /// Upgrades this instance.
        /// </summary>
        public void Upgrade()
        {
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
                return;
            UInt32 mode = 10000;
            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, mode);
        }


        /// <summary>
        /// Sets the browser feature control key.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="appName">Name of the application.</param>
        /// <param name="value">The value.</param>
        private void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        /// <summary>
        /// Navigates to.
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
        /// <returns></returns>
        public object ExecuteScript(string script)
        {
            object[] args = { script };

            return browser.InvokeScript("eval", args);
        }

        /// <summary>
        /// Sends the key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void SendKey(string key)
        {
            ExecuteScript("sendkey(" + key + ")");
        }

        /// <summary>
        /// Clicks the control class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void ClickControlClass(string className)
        {
            var links = ((mshtml.HTMLDocument)browser.Document).getElementsByTagName("a");
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
            mshtml.IHTMLElement se = ((mshtml.HTMLDocument)browser.Document).createElement("script") as mshtml.IHTMLElement; //create the script
            mshtml.IHTMLScriptElement element = (mshtml.IHTMLScriptElement)se; //cast it as a script element

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Game2048.Resources.script.txt";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    element.text = reader.ReadToEnd();
                }
            }
            
            IHTMLElement body = ((mshtml.HTMLDocument)browser.Document).body as mshtml.IHTMLElement; //get the body element
            IHTMLElement2 childhead = (IHTMLElement2)body.children[0]; //get it's first child
            childhead.insertAdjacentElement("afterEnd", (IHTMLElement)element); //add the script element after the first child
        }

        #endregion

     }
}
