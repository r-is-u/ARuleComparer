using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ARuleComparer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private String _file1 = String.Empty;
        private String _file2 = String.Empty;

        private RuleParser _parser1;
        private RuleParser _parser2;

        private string outFile = @"C:\Users\Sebastian\Desktop\ruleDiff.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Txt_FileDialog = new OpenFileDialog();  //Benutzer Dialog zur Auswahl der XDV-Datei
            Txt_FileDialog.Multiselect = false;
            Txt_FileDialog.FileName = "Rule File";
            Txt_FileDialog.DefaultExt = ".txt";
            Txt_FileDialog.Filter = "Rule Files (*.txt)|*.txt";
            Nullable<bool> result = Txt_FileDialog.ShowDialog(); // Auswahl einer XDV-Datei durch den Benuzter erfolgt?
            if (result == true) // Auswahl einer XML-Datei durch den Benuzter erfolgt?
            {
                _file1 = Txt_FileDialog.FileName;
                File1Box.Text = _file1;
                _parser1 = new RuleParser(_file1);
                if (_file2 != String.Empty)
                    StartBtn.IsEnabled = true;
            }
        }

        private void Load2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Txt_FileDialog = new OpenFileDialog();  //Benutzer Dialog zur Auswahl der XDV-Datei
            Txt_FileDialog.Multiselect = false;
            Txt_FileDialog.FileName = "Rule File";
            Txt_FileDialog.DefaultExt = ".txt";
            Txt_FileDialog.Filter = "Rule Files (*.txt)|*.txt";
            Nullable<bool> result = Txt_FileDialog.ShowDialog(); // Auswahl einer XDV-Datei durch den Benuzter erfolgt?
            if (result == true) // Auswahl einer XML-Datei durch den Benuzter erfolgt?
            {
                _file2 = Txt_FileDialog.FileName;
                File2Box.Text = _file2;
                _parser2 = new RuleParser(_file2);
                if (_file1 != String.Empty)
                    StartBtn.IsEnabled = true;
            }
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            // This line needs to happen on the UI thread...
            TaskScheduler uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            ProgressLbl.Content = "Parsing rule files...";
            Task[] tasks = new Task[2]
            {
                Task.Factory.StartNew(() => _parser1.ParseFile()),
                Task.Factory.StartNew(() => _parser2.ParseFile())
            };

            Task.Factory.ContinueWhenAll(tasks, (x) => ExportDifferences(uiScheduler));
        }

        private void ExportDifferences(TaskScheduler uiScheduler)
        {
            Task.Factory.StartNew(() =>
            {
                ProgressLbl.Content = "Calculating differences...";
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            

            var rules1_not_in_2 = _parser1.Rules.Except(_parser2.Rules, new RuleComparer()).ToList();
            var rules2_not_in_1 = _parser2.Rules.Except(_parser1.Rules, new RuleComparer()).ToList();

            using (StreamWriter outputFile = new StreamWriter(outFile))
            {
                outputFile.WriteLine(String.Format("Rules present in {0} not appearing in {1}:", _parser1.RuleFile, _parser2.RuleFile));
                outputFile.WriteLine("-----------------------------------------");

                foreach (Rule rule in rules1_not_in_2)
                    outputFile.WriteLine(rule.ToString());

                outputFile.WriteLine();
                outputFile.WriteLine(String.Format("Rules present in {0} not appearing in {1}:", _parser2.RuleFile, _parser1.RuleFile));
                outputFile.WriteLine("-----------------------------------------");

                foreach (Rule rule in rules2_not_in_1)
                    outputFile.WriteLine(rule.ToString());
            }

            Task.Factory.StartNew(() =>
            {
                ProgressLbl.Content = String.Format("Rule diffs written to: {0}", outFile);
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
            
        }
    }
}
