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

namespace SchoolMenuParser
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DateTime current = DateTime.Now;
            current.AddHours(9);
            string hours = Convert.ToString(current.Hour);
            string month = Convert.ToString(current.Month);
            string year = Convert.ToString(current.Year);
            string URL = "https://stu.jne.go.kr/sts_sci_md00_001.do?schulCode=Q100000299&schulCrseScCode=4&schulKndScCode=04&schYm=" + year + (current.Month < 10 ? "0" + month : month);
            label1.Content = year + "-" + (current.Month < 10 ? "0" + month : month);


            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(URL);
            HtmlAgilityPack.HtmlNodeCollection node = document.DocumentNode.SelectNodes("//table[@class = tbl_calendar]");
            foreach(HtmlAgilityPack.HtmlNode iter in node)
            {
                
            }
        }
    }
}
