using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace HaloSigner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                // Try to load the theme from Liberty 3.0.0.0
                RegistryKey keyTheme = Registry.CurrentUser.OpenSubKey("Software\\Xeraxic\\Liberty\\themeSettings\\");
                int AccentColour = (int)keyTheme.GetValue("accentColour", 1);
                updateAccent(AccentColour);
            }
            catch
            {
                updateAccent(1);
            }
        }

        public static void updateAccent(int AccentColour)
        {
            ResourceDictionary rd;
            switch (AccentColour)
            {
                case 1:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Orange.xaml", UriKind.Relative) };
                    break;
                case 2:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Blue.xaml", UriKind.Relative) };
                    break;
                case 3:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Purple.xaml", UriKind.Relative) };
                    break;
                case 4:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Pink.xaml", UriKind.Relative) };
                    break;
                case 5:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Red.xaml", UriKind.Relative) };
                    break;
                case 6:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Green.xaml", UriKind.Relative) };
                    break;
                case 7:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Lime.xaml", UriKind.Relative) };
                    break;
                case 8:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Silver.xaml", UriKind.Relative) };
                    break;
                default:
                    rd = new ResourceDictionary { Source = new Uri("Themes/Accents/Orange.xaml", UriKind.Relative) };
                    break;
            }

            App.Current.Resources.MergedDictionaries.Add(rd);
        }
    }
}
