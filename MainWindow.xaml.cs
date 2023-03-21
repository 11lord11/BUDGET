using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Windows.Controls;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Utilities;
using System.Collections.ObjectModel;

namespace BudgetApp
{
    public class RecordAllNotes
    {
        public string NoteName { get; set; }
        public string NoteTypeName { get; set; }
        public int NoteMoney { get; set; }
        public bool NoteIsIncome { get; set; }

        public RecordAllNotes(string notename, string notetypename, int notemoney, bool noteisincome)
        {
            NoteName = notename;
            NoteTypeName = notetypename;
            NoteMoney = notemoney;
            NoteIsIncome = noteisincome;
        }
    }

    public partial class MainWindow : Window
    {
        public string PathToAllNotes= "D:\\С#\\Budget\\AllNotes.json";
        public string PathToTypes = "D:\\С#\\Budget\\Types.json";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AddNewTypeRecord_func(object sender, RoutedEventArgs e)
        {
            AddNewTypeRecord addtyperecord = new AddNewTypeRecord();
            addtyperecord.Show();
        }

        private void AddRecord(object sender, RoutedEventArgs e)
        {
            bool status;

            if (int.Parse(sum.Text) > 0) 
            {
                status = true;
            }
            else
            {
                status = false;
            }

            int num = int.Parse(sum.Text);
            if (num < 0)
            {
                num = -num;
            }

            var data = new { Name = RecordName.Text, TypeName = Types.SelectedValue.ToString(), Money = num, IsIncome = status};
            datagrid.Items.Add(data);

            RecordName.Text = "";
            Types.SelectedValue = "";
            sum.Text = "";

            total.Content = $"Итог: {Recalculate()}";
        }
        private int Recalculate()
        {
            int totalsum = 0;
            foreach (var item in datagrid.Items)
            {
                var money = (int)((dynamic)item).Money;
                var isIncome = (bool)((dynamic)item).IsIncome;
                totalsum += isIncome ? money : -money;
            }
            return totalsum;
        }

        private void SaveDataToJson()
        {
            Dictionary<string, List<RecordAllNotes>> recordsByDate;
            string json = File.ReadAllText(PathToAllNotes);
            if (json.Length == 0)
            {
                recordsByDate = new Dictionary<string, List<RecordAllNotes>>();
            }
            else
            {
                recordsByDate = JsonConvert.DeserializeObject<Dictionary<string, List<RecordAllNotes>>>(json);
            }

            string date = calendar.SelectedDate?.ToString("dd.MM.yyyy");

            List<RecordAllNotes> recordsForDate = new List<RecordAllNotes>();
            foreach (dynamic item in datagrid.Items)
            {
                RecordAllNotes record = new RecordAllNotes(
                    (string)item.Name,
                    (string)item.TypeName,
                    (int)item.Money,
                    (bool)item.IsIncome
                );
                recordsForDate.Add(record);
            }

            if (!recordsByDate.ContainsKey(date))
            {
                recordsByDate.Add(date, recordsForDate);
            }
            else
            {
                recordsByDate[date] = recordsForDate;
            }

            json = JsonConvert.SerializeObject(recordsByDate, Formatting.Indented);
            File.WriteAllText(PathToAllNotes, json);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveDataToJson();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string json = File.ReadAllText(PathToTypes);
            RecordType recordTyp = JsonConvert.DeserializeObject<RecordType>(json);

            foreach (string item in recordTyp.newTypeRecord)
            {
                Types.Items.Add(item);
            }
        }

        private void calendar_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selectedDate = calendar.SelectedDate?.ToString("dd.MM.yyyy");

            string json = File.ReadAllText(PathToAllNotes);
            Dictionary<string, List<RecordAllNotes>> recordsByDate = JsonConvert.DeserializeObject<Dictionary<string, List<RecordAllNotes>>>(json);

            if (recordsByDate.ContainsKey(selectedDate))
            {
                datagrid.Items.Clear();
                List<RecordAllNotes> recordsForDate = recordsByDate[selectedDate];
                foreach (RecordAllNotes record in recordsForDate)
                {
                    dynamic data = new { Name = record.NoteName, TypeName = record.NoteTypeName, Money = record.NoteMoney, IsIncome = record.NoteIsIncome };
                    datagrid.Items.Add(data);
                }
                total.Content = $"Итог: {Recalculate()}";
            }
            else
            {
                datagrid.Items.Clear();
            }
        }

        private void datagrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dynamic selectedItem = datagrid.SelectedItem;
            if (selectedItem != null)
            {
                RecordName.Text = selectedItem.Name;
                Types.SelectedValue = selectedItem.TypeName;
                sum.Text = selectedItem.Money.ToString();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem != null)
            {
                dynamic selectedItem = datagrid.SelectedItem;
                int selectedIndex = datagrid.SelectedIndex;

                bool status;

                if (int.Parse(sum.Text) > 0)
                {
                    status = true;
                }
                else
                {
                    status = false;
                }

                int num = int.Parse(sum.Text);
                if (num < 0)
                {
                    num = -num;
                }

                var updatedItem = new { Name = RecordName.Text, TypeName = Types.SelectedValue.ToString(), Money = num, IsIncome = status };

                datagrid.Items.RemoveAt(selectedIndex);
                datagrid.Items.Insert(selectedIndex, updatedItem);

                RecordName.Text = "";
                Types.SelectedValue = "";
                sum.Text = "";

                total.Content = $"Итог: {Recalculate()}";
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (datagrid.SelectedItem != null)
            {
                int selectedIndex = datagrid.SelectedIndex;
                datagrid.Items.RemoveAt(selectedIndex);

                RecordName.Text = "";
                Types.SelectedValue = "";
                sum.Text = "";
                total.Content = $"Итог: {Recalculate()}";
            }
        }
    }
}
