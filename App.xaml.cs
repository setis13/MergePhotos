using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace MergePhotos
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-IN");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-IN");
        }
    }
}
