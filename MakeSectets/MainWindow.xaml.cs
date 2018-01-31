using System;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace MakeSectets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] files = null;
        string[] jsones = null;
        string SIDIpath = null;
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.AssemblyResolve += AppDomain_AssemblyResolve;
        }

        private static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("Newtonsoft.Json"))
            {
                return Assembly.Load(MakeSectets.Properties.Resources.Newtonsoft_Json);
            }
            return null;
        }

        private void maFiles_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult result = folderBrowser.ShowDialog();
            
            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                files = Directory.GetFiles(folderBrowser.SelectedPath);
            }

            textmaFiles.Text = "maFiles: " + folderBrowser.SelectedPath;
        }

        private void SID_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult result = folderBrowser.ShowDialog();

            SIDIpath = folderBrowser.SelectedPath;
            textSIDI.Text = "SIDI: " + SIDIpath;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            jsones = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                if (System.IO.Path.GetFileName(files[i]) == @"manifest.json") continue;
                using (StreamReader sr = new StreamReader(files[i], System.Text.Encoding.Default))
                {
                    jsones[i] = sr.ReadToEnd();
                }
            }
            int counter = 0;
            for (int i = 0; i < jsones.Length; i++)
            {
                if (jsones[i] == null) continue;
                Account ac = null;
                try
                {
                    ac = JsonConvert.DeserializeObject<Account>(jsones[i]);
                    string path = SIDIpath + @"\" + ac.account_name + ".secret";

                    using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                    {
                        sw.Write(ac.shared_secret);
                    }
                    counter++;
                }
                catch(Exception)
                {

                }
            }
            if(counter == 0) System.Windows.MessageBox.Show($"Generated {counter} files.\nSeems that you entered the wrong way or files are crypted.\n\nСгенерировано {counter} файлов.\nПохоже Вы указали неверный путь или файлы зашифрованы.");
            else System.Windows.MessageBox.Show($"Generated {counter} files\n\nСгенерировано {counter} файлов.");
        }
    }

    public class Account
    {
        public string account_name { get; set; }
        public string shared_secret { get; set; }
        public Account(string name, string secret)
        {
            account_name = name;
            shared_secret = secret;
        }
    }
}
