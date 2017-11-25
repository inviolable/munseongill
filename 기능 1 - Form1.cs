using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;//해쉬 테이블(=해쉬 맵) 이용하려고
using System.Threading;

namespace 졸작기능2
{
    public partial class Form1 : Form
    {
        public Form1()
        {            
            InitializeComponent();
            textBox_Range.Text = "1-700";
        }

        
        string url = null;
        WebBrowser wb = null;
        
        Hashtable ht = null;
        int start_Page = 0;
        int end_Page = 0;
        int current_Page = 0;

        SortedList sl = null;//이건 키를 기준으로 정렬 됨


        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string range_str = textBox_Range.Text;
                string start_page = range_str.Split('-')[0];
                string end_page = range_str.Split('-')[1];

                start_Page = Convert.ToInt32(start_page);
                end_Page = Convert.ToInt32(end_page);

                if (start_Page < 1)
                {
                    state_textBox.AppendText("시작 페이지는 1보다 작을 수 없습니다.\r\n");
                    return;
                }

                if (start_Page > end_Page)
                {
                    state_textBox.AppendText("끝 페이지는 시작 페이지보다 크거나 같아야 합니다.\r\n");
                    return;
                }

            }
            catch (Exception e1)
            {
                state_textBox.AppendText(e1.Message + "\r\n");
            }


            ht = new Hashtable();
            sl = new SortedList();

            wb = new WebBrowser();
            wb.DocumentCompleted +=new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);


            url = "http://www.devpia.com/MAEUL/Contents/List.aspx?page=" + start_Page + "&BoardID=69&MAEULNO=28";
            current_Page = start_Page;

            wb.Navigate(url);

            /////////////////////
                                 
           
            

        }

        void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            if (wb.Url.AbsoluteUri == e.Url.AbsoluteUri)
            {
                if(current_Page > end_Page)
                {
                    state_textBox.AppendText("작업이 완료되었습니다.\r\n");
                    state_textBox.AppendText(ht.Count + "개의 아이디 추가\r\n\r\n");

                    foreach (DictionaryEntry entry in ht)
                    { 
                        state_textBox.AppendText("키: " + entry.Key + ", 값: " + entry.Value + "\r\n");
                    }

                    return;
                }
                    
                작업();
                current_Page++;

                url = "http://www.devpia.com/MAEUL/Contents/List.aspx?page=" + current_Page + "&BoardID=69&MAEULNO=28";

                wb.Navigate(url);
                


            }

        }

        void 작업()
        {
            page_label.Text = "페이지 " + start_Page + "-" + current_Page;
            
            HtmlDocument hd = wb.Document;

            HtmlElementCollection td_Collection = hd.GetElementsByTagName("td");

            foreach (HtmlElement e1 in td_Collection)
            {
                if (e1.GetAttribute("width") == "180")
                {
                    string id = e1.InnerText;

                    if (ht.Contains(id) == true)
                    {
                        int count = (int)ht[id];
                        count++;
                        ht[id] = count;
                    }
                    else if (ht.Contains(id) == false)
                    {
                        ht.Add(id, 1);
                    }
                }
            }
                                   

        }

        private void button2_Click(object sender, EventArgs e)
        {

            state_textBox.Clear();

            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (DictionaryEntry entry in ht)
            {
                dict.Add((string)entry.Key, (int)entry.Value);
            }

            List < KeyValuePair<string, int >> sortList = new List<KeyValuePair<string, int>>();
            sortList.AddRange(dict);
            sortList.Sort
            (
                delegate(KeyValuePair<string,int> kvp1, KeyValuePair<string, int> kvp2)
                {
                    return Comparer<int>.Default.Compare(kvp1.Value, kvp2.Value);
                }
            );

            foreach (KeyValuePair<string, int> str in sortList)
            {
                state_textBox.AppendText("키: " + str.Key + ", 값: " + str.Value + "\r\n");
            }

            state_textBox.AppendText("\r\n총 " + sortList.Count + "개의 아이디\r\n");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            state_textBox.Clear();

            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (DictionaryEntry entry in ht)
            {
                dict.Add((string)entry.Key, (int)entry.Value);
            }

            List<KeyValuePair<string, int>> sortList = new List<KeyValuePair<string, int>>();
            sortList.AddRange(dict);
            sortList.Sort
            (
                delegate(KeyValuePair<string, int> kvp1, KeyValuePair<string, int> kvp2)
                {
                    return Comparer<int>.Default.Compare(kvp2.Value, kvp1.Value);
                }
            );

            foreach (KeyValuePair<string, int> str in sortList)
            {
                state_textBox.AppendText("키: " + str.Key + ", 값: " + str.Value + "\r\n");
            }

            state_textBox.AppendText("\r\n총 " + sortList.Count + "개의 아이디\r\n");

        }
    

    }
}
