using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
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
using System.Text.RegularExpressions;

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
            string date = Convert.ToString(current.Date);
            string month = Convert.ToString(current.Month);
            string year = Convert.ToString(current.Year);
            string URL = "https://stu.jne.go.kr/sts_sci_md00_001.do?schulCode=Q100000299&schulCrseScCode=4&schulKndScCode=04&schYm=" + year + (current.Month < 10 ? "0" + month : month);
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string html = wc.DownloadString(URL);

            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            document.LoadHtml(html);
            HtmlAgilityPack.HtmlNode node = document.DocumentNode.SelectSingleNode("//table[@class = 'tbl_type3 tbl_calendar']");
            HtmlAgilityPack.HtmlNode tbody = node.SelectSingleNode("./tbody");
            HtmlAgilityPack.HtmlNodeCollection divs = document.DocumentNode.SelectNodes("//table[@class = 'tbl_type3 tbl_calendar']/tbody/tr/td/div");
            using (var file = new StreamWriter("test.txt"))
            {
                foreach (var div in divs)
                {
                    string t = div.InnerHtml;
                    
                }
            }
        }
    }
}
