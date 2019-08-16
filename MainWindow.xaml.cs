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
        public static string[] allergyList = new string[19] { "\0", "난류", "우유", "메밀", "땅콩", "대두", "밀", "고등어", "게", "새우", "돼지고기", "복숭아", "토마토", "아황산류", "호두", "닭고기", "쇠고기", "오징어", "조개류" };
        public MainWindow()
        {
            InitializeComponent();
            DateTime current = DateTime.Now;
            current.AddHours(9);
            string day = Convert.ToString(current.Day);
            string month = Convert.ToString(current.Month);
            string year = Convert.ToString(current.Year);
            string URL = "https://stu.jne.go.kr/sts_sci_md00_001.do?schulCode=Q100000188&schulCrseScCode=4&schulKndScCode=04&schYm=" + year + (current.Month < 10 ? "0" + month : month); //능주고
            //string URL = "https://stu.jne.go.kr/sts_sci_md00_001.do?schulCode=Q100000299&schulCrseScCode=4&schulKndScCode=04&schYm=" + year + (current.Month < 10 ? "0" + month : month); //장흥고
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string html = wc.DownloadString(URL);

            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            document.LoadHtml(html);
            HtmlAgilityPack.HtmlNode node = document.DocumentNode.SelectSingleNode("//table[@class = 'tbl_type3 tbl_calendar']");
            HtmlAgilityPack.HtmlNode tbody = node.SelectSingleNode("./tbody");
            HtmlAgilityPack.HtmlNodeCollection divs = document.DocumentNode.SelectNodes("//table[@class = 'tbl_type3 tbl_calendar']/tbody/tr/td/div");

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
                    int lunchStart = str1.IndexOf("[중식]");
                    str1 = str1.Remove(0, lunchStart);

                    MatchCollection matchCollection = Regex.Matches(str1, "[0-9]*[.]*(<br>)*", RegexOptions.Singleline);
                    string allergyStr = string.Join("", from Match match in matchCollection select match.Value);
                    allergyStr = str1.Replace("<br>", "\r\n");
                    str1 = Regex.Replace(str1, "[0-9]*[.]*", "", RegexOptions.Singleline);
                    
                    //int removeStart = str1.IndexOf("양념류");
                    //str1 = str1.Remove(removeStart, 7); //양념류 + <br>
                    str1 = str1.Replace("<br>", "\r\n");
                    lunchAllergy.Content = allergyStr;
                    lunchAllergy.Visibility = Visibility.Visible;

                    int dinnerStart = str1.IndexOf("[석식]");
                    lunchMenu = str1.Substring(5, dinnerStart - 5);
                    dinnerMenu = str1.Substring(dinnerStart + 5);

                    lunch.Content = lunchMenu;
                    dinner.Content = dinnerMenu;
                }
            }
        }

        private void DinnerCopy_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new MainWindow();
            Clipboard.SetText(tmp.dinnerMenu);
        }

        private void Allergy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LunchCopy_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new MainWindow();
            Clipboard.SetText(tmp.lunchMenu);
        }
    }
}
