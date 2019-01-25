using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Audio_Pleer
{
    public partial class MainWindow : Window
    {
        public bool Playing;
        public bool Flag;
        public int NumOfCompos;
        public List<string> List = new List<string>();
        public List<string> ReList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            ImageBrush PlayImage = new ImageBrush();
            PlayImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Image/play.png"));
            Play.Background = PlayImage;
            Play.OpacityMask = PlayImage;
            ImageBrush PauseImage = new ImageBrush();
            PauseImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Image/pause.png"));
            Pause.Background = PauseImage;
            Pause.OpacityMask = PauseImage;
            ImageBrush StopImage = new ImageBrush();
            StopImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Image/stop.png"));
            Stop.Background = StopImage;
            Stop.OpacityMask = StopImage;
            Curtail.IsExpanded = false;
            PlayList.ItemsSource = ReList;
            Timer();
        }

        private void ButPlayClick(object sender, RoutedEventArgs e)
        {
            if (Music.HasAudio == true)
            {
                if (Playing == true)
                {
                    Music.Position = new TimeSpan(0, 0, 0, 0);
                    if (List.Count == 0)
                    {
                        Music.Position = new TimeSpan(0, 0, 0, 0);
                        Music.Stop();
                        Playing = false;
                        Name.Text = "";
                    }
                    else
                    {
                        Music.Source = new Uri(List[NumOfCompos]);
                        Music.Play();
                        Name.Text = ReList[NumOfCompos];
                        Music.LoadedBehavior = MediaState.Manual;
                        Playing = true;
                    }
                }
            }
            else
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Filter = "Audio Files|*.mp3;*.wav|" + "All Files|*.*";
                fdlg.Multiselect = true;
                if (fdlg.ShowDialog() == true)
                {
                    Music.Source = new Uri(fdlg.FileNames[0]);
                    Music.LoadedBehavior = MediaState.Manual;
                    Music.Play();
                    int i = 0;
                    while (i < fdlg.FileNames.Length)
                    {
                        List.Add(fdlg.FileNames[i]);
                        ReList.Add(fdlg.FileNames[i].Substring(fdlg.FileNames[i].LastIndexOf("\\") + 1));
                        PlayList.Items.Refresh();
                        i++;
                    }
                    Name.Text = ReList[NumOfCompos];
                    Playing = true;
                }
            }
        }

        private void ButStopClick(object sender, RoutedEventArgs e)
        {
            if (Music.HasAudio == true)
            {
                Music.LoadedBehavior = MediaState.Manual;
                Music.Stop();
                Music.Position = new TimeSpan(0, 0, 0, 0);
            }
            else
                MessageBox.Show("Select a recording");
        }

        private void ButPauseClick(object sender, RoutedEventArgs e)
        {
            if (Playing == true)
            {
                Music.LoadedBehavior = MediaState.Manual;
                Music.Pause();
                Playing = false;
            }
            else
            {
                if (Playing == false)
                {
                    Music.LoadedBehavior = MediaState.Manual;
                    Music.Play();
                    Playing = true;
                }
                else
                    MessageBox.Show("Select a recording");
            }
        }

        private void CurtailExpanded(object sender, RoutedEventArgs e)
        {
            Height = 500;
        }

        private void CurtailCollapsed(object sender, RoutedEventArgs e)
        {
            Height = 200;
        }

        private void PlayListMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PlayList.SelectedIndex != -1)
            {
                Music.Source = new Uri(List[PlayList.SelectedIndex]);
                Name.Text = ReList[PlayList.SelectedIndex];
                Playing = true;
                NumOfCompos = PlayList.SelectedIndex;
            }
        }

        private void ButClickAdd(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Filter = "Audio Files|*.mp3;*.wav|" + "All Files|*.*";
            fdlg.Multiselect = true;
            if (fdlg.ShowDialog() == true)
            {
                int i = 0;
                while (i < fdlg.FileNames.Length)
                {
                    List.Add(fdlg.FileNames[i]);
                    ReList.Add(fdlg.FileNames[i].Substring(fdlg.FileNames[i].LastIndexOf("\\") + 1));
                    PlayList.Items.Refresh();
                    i++;
                }
            }
        }

        private void ButClickRemove(object sender, RoutedEventArgs e)
        {
            NumOfCompos = PlayList.SelectedIndex;
            if (PlayList.SelectedIndex != -1)
            {
                if (List.Count == NumOfCompos)
                {
                    Music.Position = new TimeSpan(0, 0, 0, 0);
                    Music.Stop();
                    Playing = false;
                }
                else
                {
                    List.Remove(List[PlayList.SelectedIndex]);
                    ReList.Remove(ReList[PlayList.SelectedIndex]);
                    Playing = true;
                    NumOfCompos--;
                    PlayList.SelectedIndex = NumOfCompos;
                    PlayList.Items.Refresh();
                }
            }
        }

        private void ButClickSave(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fdlg = new SaveFileDialog();
            if (fdlg.ShowDialog() == true)
            {
                string name = fdlg.FileName;
                string type = ".txt";
                if (name.IndexOf(type) != -1)
                    name = name.Remove(name.IndexOf(type), type.Length);
                name += type;
                FileStream list = new FileStream(name, FileMode.Create, FileAccess.Write);
                StreamWriter save_list = new StreamWriter(list);
                int i = 0;
                while (i != List.Count)
                {
                    save_list.WriteLine(List[i]);
                    i++;
                }
                save_list.Close();
                MessageBox.Show("Saved");
            }
        }

        private void ButClickLoad(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Filter = "Text File |*.txt|" + "All Files |*.*";
            if (fdlg.ShowDialog() == true)
            {
                List.Clear();
                ReList.Clear();
                PlayList.Items.Refresh();
                string name = fdlg.FileName;
                string expension = ".txt";
                if (expension != name.Substring(name.Length - 4))
                {
                    MessageBox.Show("The file is not the correct size!");
                    return;
                }
                FileStream fin = new FileStream(name, FileMode.Open, FileAccess.Read);
                StreamReader load = new StreamReader(fin);
                string s = "";
                while ((s = load.ReadLine()) != null)
                {
                    List.Add(s);
                    ReList.Add(s.Substring(s.LastIndexOf("\\") + 1));
                    PlayList.Items.Refresh();
                }
                try
                {
                    Music.Source = new Uri(List[0]);
                    Music.LoadedBehavior = MediaState.Manual;
                    Music.Play();
                    Name.Text = ReList[NumOfCompos];
                }
                catch (FormatException)
                {
                    MessageBox.Show("Error contents!");
                    List.Clear();
                    ReList.Clear();
                    PlayList.Items.Refresh();
                }
            }
        }

        private void ButClickDellAll(object sender, RoutedEventArgs e)
        {
            if (PlayList.SelectedIndex == 0)
            {
                MessageBox.Show("PlayList is clear");
            }
            else
            {
                List.Clear();
                ReList.Clear();
                PlayList.Items.Refresh();
                MessageBox.Show("PlayList is clear");
            }
        }

        void Timer()
        {
            Flag = true;
            DispatcherTimer Time = new DispatcherTimer();
            Time.Interval = TimeSpan.FromMilliseconds(100);
            Time.Tick += new EventHandler(TimeTick);
            Time.Start();
        }

        void TimeTick(object sender, EventArgs e)
        {
            Time.Content = Music.Position.Hours + ":" + Music.Position.Minutes + ":" + Music.Position.Seconds;
            MusicTimeScal.Value = Music.Position.TotalMilliseconds;
            if (MusicTimeScal.Value == MusicTimeScal.Maximum)
            {
                MusicTimeScal.Value = Music.Position.TotalMilliseconds;
                Playing = false;
                if (List.Count < 1)
                {
                    Music.Position = new TimeSpan(0, 0, 0, 0);
                    Music.Stop();
                    Playing = false;
                }
                else
                {
                    NumOfCompos++;
                    if (NumOfCompos == List.Count)
                    {
                        NumOfCompos = 0;
                        Music.Stop();
                        Playing = false;
                    }
                    else
                    {
                        Music.Source = new Uri(List[NumOfCompos]);
                        Music.LoadedBehavior = MediaState.Manual;
                        Music.Play();
                        Playing = true;
                    }
                    Name.Text = ReList[NumOfCompos];
                }
            }
        }

        private void MusicTimeScalMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Flag = true;
        }

        private void MusicTimeScalValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Flag == true)
                Music.Position = new TimeSpan(0, 0, 0, 0, (int)MusicTimeScal.Value);
            Flag = false;
        }

        private void MusicMediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                MusicTimeScal.Maximum = Music.NaturalDuration.TimeSpan.TotalMilliseconds;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("The file is not the correct size!");
                List.Remove(List[NumOfCompos]);
                ReList.Remove(ReList[NumOfCompos]);
                PlayList.Items.Refresh();
                Name.Text = "";
            }
        }
    }
}