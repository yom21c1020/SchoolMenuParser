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
        public string lunchMenu, dinnerMenu;

        public MainWindow()
        {
            InitializeComponent();
            DateTime current = DateTime.Now;
            current.AddHours(9);
            string day = Convert.ToString(current.Day);
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
                    string str1 = div.InnerHtml;
                    string substr1 = "";
                    if (str1.Length == 0) continue;
                    if (str1.Length == 1) substr1 = str1.Substring(0, 1);
                    else substr1 = str1.Substring(0, 2);

                    substr1 = Regex.Replace(substr1, @"[^a-zA-Z0-9가-힣]", "", RegexOptions.Singleline);
                    int raw_date;
                    int.TryParse(substr1, out raw_date);
                    if (raw_date == Convert.ToInt32(day))
                    {
                        //label1.Content = str1;

                        int lunchStart = str1.IndexOf("[중식]");
                        //label1.Content = Convert.ToString(lunchStart);
                        str1 = str1.Remove(0, lunchStart);
                        str1 = Regex.Replace(str1, "[0-9]*[.]*", "", RegexOptions.Singleline);
                        int removeStart = str1.IndexOf("양념류");
                        str1 = str1.Remove(removeStart, 7);
                        str1 = str1.Replace("<br>", "\r\n");
                        int dinnerStart = str1.IndexOf("[석식]");
                        lunchMenu = str1.Substring(5, dinnerStart - 5);
                        dinnerMenu = str1.Substring(dinnerStart + 5);
                        lunch.Content = lunchMenu;
                        dinner.Content = dinnerMenu;
                    }
                }
            }
        }

        private void DinnerCopy_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new MainWindow();
            Clipboard.SetText(tmp.dinnerMenu);
        }

        private void LunchCopy_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new MainWindow();
            Clipboard.SetText(tmp.lunchMenu);
        }
    }
}
