using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace BudgetApp
{
    /// <summary>
    /// Логика взаимодействия для AddNewTypeRecord.xaml
    /// </summary>
    ///
    public class RecordType
    {
        public string[] newTypeRecord = {};
    }

    public partial class AddNewTypeRecord : Window
    {
        public string PathToTypes = "D:\\С#\\Budget\\Types.json";

        public AddNewTypeRecord()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            ComboBox comboBox = (ComboBox)mainWindow.FindName("Types");

            string json = File.ReadAllText(PathToTypes);
            RecordType recordTyp = JsonConvert.DeserializeObject<RecordType>(json);
                
            Array.Resize(ref recordTyp.newTypeRecord, recordTyp.newTypeRecord.Length + 1);
            recordTyp.newTypeRecord[recordTyp.newTypeRecord.Length - 1] = NewRecord.Text;

            json = JsonConvert.SerializeObject(recordTyp, Formatting.Indented);
            File.WriteAllText(PathToTypes, json);

            comboBox.Items.Clear();

            foreach (string item in recordTyp.newTypeRecord)
            {
                comboBox.Items.Add(item);
            }
        
            this.Close();
        }
    }
}
