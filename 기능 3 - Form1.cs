using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace 졸작_기능_4__게시판_크롤링
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            th = new Thread(new ThreadStart(글_분석_담당_스레드_폼2));
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
        
        WebBrowser browser;
        
        string id = null;

        Thread th;

        Form2 Form_2;
        
        string title = null;

        public int start_page = 0;
        public int end_page = 0;
        public int current_page = 0;
        public int 글_갯수 = 0;
        public string[] 글_링크_목록 = new string[100000]; 

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string range = textBox_Range.Text;
                string[] range2 = range.Split('-');
                start_page = Convert.ToInt32(range2[0]);
                end_page = Convert.ToInt32(range2[1]);

                if (end_page > 700)
                {
                    textBox_Error.AppendText("끝 페이지는 700 이하로 설정하셔야 합니다.\r\n\r\n");
                    return;
                }                

                if(start_page > end_page)
                {
                    textBox_Error.AppendText("시작 페이지는 끝 페이지 보다 작거나 같아야 합니다.\r\n\r\n");
                    return;
                }

                if (start_page < 1)
                {
                    textBox_Error.AppendText("시작 페이지는 1보다 작을 수 없습니다.\r\n\r\n");
                    return;
                }



            }
            catch (Exception e1)
            {
                textBox_Error.AppendText(e1.Message + "\r\n");
                textBox_Error.AppendText(e1.TargetSite + "\r\n");
                return;

            }


            try
            {
                current_page = start_page;
                
                browser = new WebBrowser();

                browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(document_completed);

                string strAddress = "http://www.devpia.com/MAEUL/Contents/List.aspx?page=" + current_page + "&BoardID=69&MAEULNO=28";

                browser.Navigate(strAddress);
            }
            catch (Exception e1)
            {
                textBox_Error.AppendText(e1.Message + "\r\n");
                textBox_Error.AppendText(e1.TargetSite + "\r\n");
                return;

            }

        }

        void document_completed(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri == browser.Url.AbsoluteUri)
            {                
                if (current_page > end_page)
                {
                    textBox1.AppendText("글 총 갯수 : " + 글_갯수 + "개 검색\r\n");
                    Form_2.when_link_is_found_2();
                    return;
                }

                textBox1.AppendText("현재 url : " + e.Url.AbsoluteUri + "\r\n\r\n");

                글_수집();
                current_page++;
                string strAddress = "http://www.devpia.com/MAEUL/Contents/List.aspx?page=" + current_page + "&BoardID=69&MAEULNO=28";

                try
                {
                    browser.Navigate(strAddress);

                }
                catch (Exception e1)
                {
                    textBox_Error.AppendText(e1.Message + "\r\n");
                    textBox_Error.AppendText(e1.TargetSite + "\r\n");
                    return;
                }
                                

            }
            
        }

        private void 글_수집()
        {
            string link = null;
            
            //richTextBox1.Text += "글 수집, current_page : " + current_page + "\r\n";
            label_페이지_현황.Text = "페이지 " + start_page + "-" + current_page;
            HtmlDocument HtmlDocument1 = browser.Document;

            HtmlElementCollection tdElements = HtmlDocument1.GetElementsByTagName("td");

            int count1 = 0;
            foreach (HtmlElement e1 in tdElements)
            {
                if (e1.GetAttribute("className") == "font_small")//글 번호
                {
                    if (e1.InnerText != null)
                        break;

                    if (e1.InnerText == "...")
                        return;
                }
                        

                count1++;
                if (count1 == tdElements.Count)
                    return;
                

            }

            count1 = 0;
            foreach (HtmlElement e1 in tdElements)
            {
                if (e1.GetAttribute("width") == "180")//작성자
                {
                    if (e1.InnerText != null)
                        break;
                }
                

                count1++;
                if (count1 == tdElements.Count)
                    return;


            }
            foreach(HtmlElement e1 in tdElements)
            {                
                if (e1.GetAttribute("className") == "board_con")
                {
                    HtmlElementCollection bElements = e1.GetElementsByTagName("b");
                    if (bElements.Count == 2)
                        continue;
                    HtmlElementCollection aElements = e1.GetElementsByTagName("a");
                    
                    link = aElements[0].GetAttribute("href");
                                        
                    글_링크_목록[글_갯수] = link;
                    글_갯수++;
                    
                }


            }

          
            
        }

        void 글_분석_담당_스레드_폼2()
        {
            Form_2 = new Form2(this);
            Application.Run(Form_2);
        }

        
    }
}
