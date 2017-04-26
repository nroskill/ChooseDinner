using System;
using System.Text;
using System.Windows;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace chooseDinner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<DinnerInfo> bid = new ObservableCollection<DinnerInfo>();

        public static string dataSource = "chooseDinner.txt";

        Random rid = new Random();
        public MainWindow()
        {
            InitializeComponent();
            readFile();
            this.datagrid1.ItemsSource = bid;
        }
        private void readFile()
        {
            FileStream sw;
            byte[] bytestr = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?>\r\n<data>\r\n</data>");
            try
            {
                sw = new FileStream(dataSource, FileMode.Open);
            }
            catch(System.IO.FileNotFoundException)
            {
                sw = new FileStream(dataSource, FileMode.OpenOrCreate);
                sw.Write(bytestr,0,bytestr.Length);
            }
            sw.Close();
            var file = new XmlDocument();
            file.Load(dataSource);
            foreach (XmlNode i in file.SelectSingleNode("data").ChildNodes)
            {
                bid.Add(new DinnerInfo{ name = i.Name, valid = true});
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var validCount = 0;
            var file = new XmlDocument();
            foreach (var i in bid)
            {
                if (i.valid)
                    validCount++;
            }
            if (validCount == 0)
                MessageBox.Show("根本没的选！", "你特么在逗我！");
            else
            {
                var i = 0;
                for(i = rid.Next(bid.Count); !bid[i%bid.Count].valid ; i++ );
                MessageBox.Show(bid[i%bid.Count].name, "决定就是你了！");
            }
            file.Load(dataSource);
            file.SelectSingleNode("data").RemoveAll();
            foreach (var i in bid)
            {
                if(!(i.name[0] >= '0' && i.name[0] <= '9') && !i.name.Contains(" "))
                    file.SelectSingleNode("data").AppendChild(file.CreateElement(i.name));
            }
            file.Save(dataSource);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.newToilet.Text == "")
                MessageBox.Show("左侧框输厕所名", "你特么在逗我！");
            else
            {
                bid.Add(new DinnerInfo{ name = this.newToilet.Text, valid = true});
                this.newToilet.Text = "";
            }
        }
    }
    public class DinnerInfo:INotifyPropertyChanged
    {
        private string _name = String.Empty;
        public bool valid { get; set; }
        public event PropertyChangedEventHandler PropertyChanged; 
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (PropertyChanged != null)
                    this.PropertyChanged(this, new PropertyChangedEventArgs("name"));
            }
        }
    }
}
