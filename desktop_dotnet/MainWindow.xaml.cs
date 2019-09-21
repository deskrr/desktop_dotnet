using desktop_dotnet.win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace desktop_dotnet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public int a = 2;
        public BehaviourMonitor monitor = new BehaviourMonitor();

        public List<UserActivity> activities = new List<UserActivity>();

        public MainWindow()
        {
            InitializeComponent();
            this.updateActiveProcess();

            listView.ItemsSource = this.activities;
        }

        async void updateActiveProcess()
        {
            while (true)
            {
                string activeProcess = "";
                await Task.Delay(1000);
                try
                {
                    activeProcess = this.monitor.GetActiveProcessInfo();
                    using (Icon ico = this.monitor.getIcon(activeProcess))
                    {
                        image.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    using (Bitmap b = this.monitor.getScreenshot())
                    {
                        BitmapImage img = this.BitmapToImageSource(b);
                        ss.Source = img;
                    }
                    this.getActivity(activeProcess).addTime(1);
                    this.lastInput.Content = "Last Input: " + DateTime.Now.Subtract(this.monitor.GetLastInputTime()).Seconds;
                } catch (Exception e)
                {
                    activeProcess = "error";
                    debugText.Text += e.Message;
                    debugText.Text += e.StackTrace;
                    Console.WriteLine(e.StackTrace);
                }
                labl.Content = System.IO.Path.GetFileName(activeProcess);
            }
        }

        UserActivity getActivity(string path)
        {
            string filename = System.IO.Path.GetFileName(path);
            UserActivity activity = this.activities.FirstOrDefault((a) => a.fileName == filename);
            if (activity == null)
            {
                activity = new UserActivity();
                activity.fileName = filename;
                using (Icon ico = this.monitor.getIcon(path))
                {
                    activity.icon = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                this.activities.Add(activity);
                listView.ItemsSource = null;
                listView.ItemsSource = this.activities;
            }
            return activity;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
{
    using (MemoryStream memory = new MemoryStream())
    {
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        memory.Position = 0;
        BitmapImage bitmapimage = new BitmapImage();
        bitmapimage.BeginInit();
        bitmapimage.StreamSource = memory;
        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapimage.EndInit();

        return bitmapimage;
    }
}
    }

    public class UserActivity : INotifyPropertyChanged
    {
        public string fileName { get; set; }
        public ImageSource icon { get; set; }
        public TimeSpan duration { get; set ; }
        public string durationFormatted { get { return this.duration.ToString(); } }

        public UserActivity()
        {
            this.duration = new TimeSpan();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void addTime(int seconds)
        {
            this.duration = this.duration.Add(new TimeSpan((long)(seconds * Math.Pow(10, 7))));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("durationFormatted"));
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
