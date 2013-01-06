/*
 * Copyright (C) 2011 Aaron Dierking, Alex Reed
 * This file is part of halosigner.
 *
 * halosigner is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * halosigner is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with halosigner.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace HaloSigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string originalInfoText;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            originalInfoText = txtInfo.Text;
        }

        void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    try
                    {
                        ResignFile(file);
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(System.IO.Path.GetFileName(file) + ": " + ex.Message);
                        return;
                    }
                }

                if (files.Length != 1)
                    ShowMessage("All files resigned successfully!");
            }
        }

        private void headerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Left = Left + e.HorizontalChange;
            Top = Top + e.VerticalChange;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ResignFile(string path)
        {
            // Ugh this is hax, the file ends up getting opened twice -_-
            Stream fileStream = File.OpenRead(path);
            Util.ContentType type = Util.ContentTypes.CheckFileType(fileStream);
            fileStream.Close();

            switch (type)
            {
                case Util.ContentType.ReachGPD:
                    ResignGPD(path);
                    break;

                case Util.ContentType.ReachCampaignSave:
                    ResignReachCampaignSave(path);
                    break;

                case Util.ContentType.Halo3CampaignSave:
                    ResignHalo3CampaignSave(path);
                    break;

                case Util.ContentType.Halo3ODSTCampaignSave:
                    ResignHalo3ODSTCampaignSave(path);
                    break;

                case Util.ContentType.HCEXCampaignSave:
                    ResignHCEXCampaignSave(path);
                    break;

                case Util.ContentType.Halo4CampaignSave:
                    ResignHalo4CampaignSave(path);
                    break;

                default:
                    throw new ArgumentException("Unknown file type");
            }   
        }

        private void ResignGPD(string path)
        {
            Reach.ReachGPD gpd = null;
            try
            {
                gpd = new Reach.ReachGPD(path);
#if DEBUG
                Debug.WriteLine("Old hash: " + HexToString(gpd.Hash));
#endif
                gpd.Update();
#if DEBUG
                Debug.WriteLine("New hash: " + HexToString(gpd.Hash));
#endif
                ShowMessage("GPD resigned successfully!");
            }
            finally
            {
                if (gpd != null)
                    gpd.Close();
            }
        }

        private void ResignReachCampaignSave(string path)
        {
            Reach.CampaignSave save = new Reach.CampaignSave(path);
            save.Resign();
            save.WriteTo(path);

            ShowMessage("Gamesave resigned successfully!");
        }

        private void ResignHalo3CampaignSave(string path)
        {
            Halo3.CampaignSave save = new Halo3.CampaignSave(path);
            save.Resign();
            save.WriteTo(path);

            ShowMessage("Gamesave resigned successfully!");
        }

        private void ResignHalo3ODSTCampaignSave(string path)
        {
            Halo3.ODST.CampaignSave save = new Halo3.ODST.CampaignSave(path);
            save.Resign();
            save.WriteTo(path);

            ShowMessage("Gamesave resigned successfully!");
        }

        private void ResignHCEXCampaignSave(string path)
        {
            HCEX.CampaignSave save = new HCEX.CampaignSave(path);
            save.Resign();
            save.WriteTo(path);

            ShowMessage("Gamesave resigned successfully!");
        }

        private void ResignHalo4CampaignSave(string path)
        {
            Halo4.CampaignSave save = new Halo4.CampaignSave(path);
            save.Resign();
            save.WriteTo(path);

            ShowMessage("Gamesave resigned successfully!");
        }

        private void ShowMessage(string message)
        {
            txtInfo.Text = message;

            timer.Stop();
            timer.Interval = new TimeSpan(0, 0, 0, 3);
            timer.Tick += new EventHandler(HideMessage);
            timer.Start();
        }

        void HideMessage(object sender, EventArgs e)
        {
            txtInfo.Text = originalInfoText;
        }

#if DEBUG
        static string HexToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.AppendFormat("{0:X2}", b);
            return builder.ToString();
        }
#endif
    }
}
