using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace SRTFaker
{
    public partial class MainWindow : Window
    {
        public string FormatDigits(int value, int digits = 2)
        {
            var str = value.ToString();
            var len = str.Count();

            var buffer = "";
            if (len < digits)
            {
                for (int i = 0; i < digits - len; ++i)
                    buffer += '0';
            }
            else if (len > digits)
            {
                return str.Substring(0, digits);
            }

            return buffer + str;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                var duration = new TimeSpan(int.Parse(this.tbHours.Text), int.Parse(this.tbMinutes.Text), int.Parse(this.tbSeconds.Text));
                var lineCount = File.ReadLines(filename).Count();
                var distance = duration.Ticks / lineCount;

                using (var stream = new System.IO.StreamReader(filename))
                {
                    int index = 1;
                    var curtime = new TimeSpan(0, 0, 0);
                    string line;
                    var srtlines = new List<string>();
                    while ((line = stream.ReadLine()) != null)
                    {
                        var nexttime = curtime.Add(TimeSpan.FromTicks(distance));
                        srtlines.Add(index.ToString());
                        srtlines.Add(string.Format("{0}:{1}:{2},{3} --> {4}:{5}:{6},{7}",
                                FormatDigits(curtime.Hours),
                                FormatDigits(curtime.Minutes),
                                FormatDigits(curtime.Seconds),
                                FormatDigits(curtime.Milliseconds, 3),
                                FormatDigits(nexttime.Hours),
                                FormatDigits(nexttime.Minutes),
                                FormatDigits(nexttime.Seconds),
                                FormatDigits(nexttime.Milliseconds, 3)
                            )
                        );
                        srtlines.Add(line);
                        srtlines.Add("\n");
                        ++index;
                        curtime = nexttime;
                    }
                    File.WriteAllLines(System.IO.Path.GetFileNameWithoutExtension(filename) + ".srt", srtlines.ToArray());
                }
            }
        }
    }
}
