using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

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
            dp.Text = year + "-" + month + "-" + day;

            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            document.LoadHtml(html);
            HtmlAgilityPack.HtmlNode node = document.DocumentNode.SelectSingleNode("//table[@class = 'tbl_type3 tbl_calendar']");
            HtmlAgilityPack.HtmlNode tbody = node.SelectSingleNode("./tbody");
            HtmlAgilityPack.HtmlNodeCollection divs = document.DocumentNode.SelectNodes("//table[@class = 'tbl_type3 tbl_calendar']/tbody/tr/td/div");

            //Today.Content = year + ". " + month + ". " + day;

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

                    str1 = str1.Replace("<br>", "\r\n");
                    int dinnerStart = str1.IndexOf("[석식]");
                    lunchMenu = str1.Substring(5, dinnerStart - 5);
                    dinnerMenu = str1.Substring(dinnerStart + 5);

                    MatchCollection matchCollection = Regex.Matches(lunchMenu, "[0-9]*[.]*(\r\n)*", RegexOptions.Singleline);
                    string lunchAllergyStr = string.Join("", from Match match in matchCollection select match.Value);
                    matchCollection = Regex.Matches(dinnerMenu, "[0-9]*[.]*(\r\n)*", RegexOptions.Singleline);
                    string dinnerAllergyStr = string.Join("", from Match match in matchCollection select match.Value);

                    int tmp = 0;
                    string lunchAllergyStrfied = "", dinnerAllergyStrfied = "";
                    foreach (var i in lunchAllergyStr)
                    {
                        if (!Char.IsNumber(i))
                        {
                            //if (i == '\r') lunchAllergyStrfied = lunchAllergyStrfied.Remove(lunchAllergyStrfied.LastIndexOf(','));
                            if (i == '\n') lunchAllergyStrfied += "\r\n";
                            else lunchAllergyStrfied += allergyList[tmp] + " ";
                            tmp = 0;
                            continue;
                        }
                        else tmp = tmp * 10 + (int)char.GetNumericValue(i);
                    }

                    foreach (var i in dinnerAllergyStr)
                    {
                        if (!Char.IsNumber(i))
                        {
                            //if (i == '\r') dinnerAllergyStrfied = dinnerAllergyStrfied.Remove(dinnerAllergyStrfied.LastIndexOf(','));
                            if (i == '\n') dinnerAllergyStrfied += "\r\n";
                            else dinnerAllergyStrfied += allergyList[tmp] + " ";
                            tmp = 0;
                            continue;
                        }
                        else tmp = tmp * 10 + (int)char.GetNumericValue(i);
                    }

                    lunchAllergy.Content = lunchAllergyStrfied;
                    dinnerAllergy.Content = dinnerAllergyStrfied;


                    lunchMenu = Regex.Replace(lunchMenu, "[0-9]*[.]*", "", RegexOptions.Singleline);
                    dinnerMenu = Regex.Replace(dinnerMenu, "[0-9]*[.]*", "", RegexOptions.Singleline);
                    lunchMenu = lunchMenu.Remove(0, 1);
                    dinnerMenu = dinnerMenu.Remove(0, 1);

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
            if(lunchAllergy.IsVisible)
            {
                lunchAllergy.Visibility = Visibility.Hidden;
                dinnerAllergy.Visibility = Visibility.Hidden;
            }
            else
            {
                lunchAllergy.Visibility = Visibility.Visible;
                dinnerAllergy.Visibility = Visibility.Visible;
            }
        }

        private void Dp_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DateTime current = dp.SelectedDate;
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

                    str1 = str1.Replace("<br>", "\r\n");
                    int dinnerStart = str1.IndexOf("[석식]");
                    lunchMenu = str1.Substring(5, dinnerStart - 5);
                    dinnerMenu = str1.Substring(dinnerStart + 5);

                    MatchCollection matchCollection = Regex.Matches(lunchMenu, "[0-9]*[.]*(\r\n)*", RegexOptions.Singleline);
                    string lunchAllergyStr = string.Join("", from Match match in matchCollection select match.Value);
                    matchCollection = Regex.Matches(dinnerMenu, "[0-9]*[.]*(\r\n)*", RegexOptions.Singleline);
                    string dinnerAllergyStr = string.Join("", from Match match in matchCollection select match.Value);

                    int tmp = 0;
                    string lunchAllergyStrfied = "", dinnerAllergyStrfied = "";
                    foreach (var i in lunchAllergyStr)
                    {
                        if (!Char.IsNumber(i))
                        {
                            //if (i == '\r') lunchAllergyStrfied = lunchAllergyStrfied.Remove(lunchAllergyStrfied.LastIndexOf(','));
                            if (i == '\n') lunchAllergyStrfied += "\r\n";
                            else lunchAllergyStrfied += allergyList[tmp] + " ";
                            tmp = 0;
                            continue;
                        }
                        else tmp = tmp * 10 + (int)char.GetNumericValue(i);
                    }

                    foreach (var i in dinnerAllergyStr)
                    {
                        if (!Char.IsNumber(i))
                        {
                            //if (i == '\r') dinnerAllergyStrfied = dinnerAllergyStrfied.Remove(dinnerAllergyStrfied.LastIndexOf(','));
                            if (i == '\n') dinnerAllergyStrfied += "\r\n";
                            else dinnerAllergyStrfied += allergyList[tmp] + " ";
                            tmp = 0;
                            continue;
                        }
                        else tmp = tmp * 10 + (int)char.GetNumericValue(i);
                    }

                    lunchAllergy.Content = lunchAllergyStrfied;
                    dinnerAllergy.Content = dinnerAllergyStrfied;


                    lunchMenu = Regex.Replace(lunchMenu, "[0-9]*[.]*", "", RegexOptions.Singleline);
                    dinnerMenu = Regex.Replace(dinnerMenu, "[0-9]*[.]*", "", RegexOptions.Singleline);
                    lunchMenu = lunchMenu.Remove(0, 1);
                    dinnerMenu = dinnerMenu.Remove(0, 1);

                    lunch.Content = lunchMenu;
                    dinner.Content = dinnerMenu;
                }
            }

        private void LunchCopy_Click(object sender, RoutedEventArgs e)
        {
            var tmp = new MainWindow();
            Clipboard.SetText(tmp.lunchMenu);
        }
    }
}
