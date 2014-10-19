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
        private WebBrowser browser;

        public BrowserManager(WebBrowser browser)
        {
            this.browser = browser;
            browser.Navigated += browser_Navigated;
        }

        async void browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Navigated != null)
            {
                await Task.Delay(1000);
                Navigated();
            }
        }

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

        public void Upgrade()
        {
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
                return;
            UInt32 mode = 10000;
            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, mode);
        }


        private void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        public void NavigateTo(string url)
        {
            browser.Navigate(url);
           
        }

        public object ExecuteScript(string script)
        {
            object[] args = { script };

            return browser.InvokeScript("eval", args);
        }

        public void SendKey(string key)
        {
            ExecuteScript("sendkey(" + key + ")");
        }

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

        public Action Navigated
        {
            get;
            set;
        }

        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }


    }
}
