using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using AmbientSounds.Core;
using NAudio.Wave;

namespace AmbientSounds
{
    public partial class MainWindow : Window
    {
        private List<WavePlayer> Clips = new List<WavePlayer>();
        private DirectSoundOut audioOutput;
        private string[] files = new string[] { "Waves", "Rain", "Flames", "Water", "Birds", "Stars" };
        private bool isProfileLoading = false;

        public MainWindow()
        {
            InitializeComponent();
            audioOutput = new DirectSoundOut();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (WavePlayer clip in Clips)
            {
                clip.Dispose();
            }

            Clips.Clear();
            audioOutput.Dispose();
            audioOutput = new DirectSoundOut();

            foreach (string file in files)
            {
                Clips.Add(new WavePlayer($"Resources\\{file}.wav"));
            }

            var mixer = new MixingWaveProvider32(Clips.Select(c => c.Channel));
            audioOutput.Init(mixer);
            audioOutput.Play();

            string[] profiles = ProfileUtils.GetProfiles();
            string selectedProfile = ProfileUtils.GetSelectedProfile();
            foreach (string profile in profiles) {
                CBox_Profiles.Items.Add(profile);
                if (profile == selectedProfile) {
                    CBox_Profiles.SelectedIndex = CBox_Profiles.Items.Count - 1;
                }
            }
        }

        private void SetVolume(int clipIndex, double volume)
        {
            Clips[clipIndex].Channel.Volume = (float)volume / 10.0f;
            SetUnknownProfile();
        }

        private void SetUnknownProfile()
        {
            if (CBox_Profiles.SelectedIndex != -1) {
                if (!isProfileLoading) {
                    CBox_Profiles.SelectedIndex = -1;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            audioOutput.Stop();
            audioOutput.Dispose();
        }

        private void Seek_Stars_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(5, Seek_Stars.Value);
            Tx_Stars.Text = $"{(int)(Seek_Stars.Value * 10)}%";
        }

        private void Seek_Birds_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(4, Seek_Birds.Value);
            Tx_Birds.Text = $"{(int)(Seek_Birds.Value * 10)}%";
        }

        private void Seek_Water_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(3, Seek_Water.Value);
            Tx_Water.Text = $"{(int)(Seek_Water.Value * 10)}%";
        }

        private void Seek_Flames_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(2, Seek_Flames.Value);
            Tx_Flames.Text = $"{(int)(Seek_Flames.Value * 10)}%";
        }

        private void Seek_Rain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(1, Seek_Rain.Value);
            Tx_Rain.Text = $"{(int)(Seek_Rain.Value * 10)}%";
        }

        private void Seek_Waves_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVolume(0, Seek_Waves.Value);
            Tx_Waves.Text = $"{(int)(Seek_Waves.Value * 10)}%";
        }

        private void Btn_SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            Tab_AddProfile.Visibility = Visibility.Visible;
        }

        private void Btn_RemoveProfile_Click(object sender, RoutedEventArgs e)
        {
            if (CBox_Profiles.SelectedIndex == -1 || String.IsNullOrEmpty(CBox_Profiles.SelectedValue.ToString())) return;
            ProfileUtils.RemoveProfile(CBox_Profiles.SelectedValue.ToString());
            CBox_Profiles.Items.Remove(CBox_Profiles.SelectedValue.ToString());
        }

        private void CBox_Profiles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CBox_Profiles.SelectedIndex == -1 || String.IsNullOrEmpty(CBox_Profiles.SelectedValue.ToString())) return;
            double[] values = ProfileUtils.GetProfileValues(CBox_Profiles.SelectedValue.ToString());

            if (values != null)
            {
                isProfileLoading = true;
                Seek_Waves.Value = values[0];
                Seek_Rain.Value = values[1];
                Seek_Flames.Value = values[2];
                Seek_Water.Value = values[3];
                Seek_Birds.Value = values[4];
                Seek_Stars.Value = values[5];
                isProfileLoading = false;
            }

            ProfileUtils.SetSelectedProfile(CBox_Profiles.SelectedValue.ToString());
        }

        private void Btn_ProfileCancel_Click(object sender, RoutedEventArgs e)
        {
            TBox_ProfileName.Text = "";
            Tab_AddProfile.Visibility = Visibility.Collapsed;
        }

        private void Btn_ProfileAdd_Click(object sender, RoutedEventArgs e)
        {
            string profileName = TBox_ProfileName.Text;
            double[] profileValues = new double[] { 
                Seek_Waves.Value, Seek_Rain.Value, Seek_Flames.Value,
                Seek_Water.Value, Seek_Birds.Value, Seek_Stars.Value };

            if (String.IsNullOrEmpty(profileName)) return;
            if (ProfileUtils.AddProfile(profileName, profileValues)) {
                TBox_ProfileName.Text = "";
                Tab_AddProfile.Visibility = Visibility.Collapsed;
                CBox_Profiles.Items.Add(profileName);
                CBox_Profiles.SelectedIndex = CBox_Profiles.Items.Count - 1;
            } 
            else {
                MessageBox.Show("A profile with this name already exists, please try again!");
            }
        }

        private void TBox_ProfileName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) {
                Btn_ProfileAdd.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
