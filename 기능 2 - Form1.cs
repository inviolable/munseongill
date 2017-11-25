using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using System.Threading;

namespace http게시판글긁어오기2
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
        public string[] 글_링크_목록 = new string[300];        
        
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

                id = textBox_id.Text;
                if (id == null)
                {
                    textBox_Error.AppendText("id를 입력해 주십시오.\r\n");
                    return;
                }

                int IDCheck = id.CompareTo("");

                if (IDCheck == 0)
                {
                    textBox_Error.AppendText(@"ID = """"" + "\r\n");//큰 따옴표를 표시하기 위한 방법, 다음에 기억 안나면 이거 참조하기
                    textBox_Error.AppendText("id를 입력해 주십시오.\r\n");
                    return;
                }

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
                
            if(e.Url.AbsoluteUri == browser.Url.AbsoluteUri)
            {
                richTextBox1.Text += "현재 url : " + e.Url.AbsoluteUri + "\r\n\r\n";
                
                if (current_page > end_page)
                    return;

                글_수집();
                current_page++;
                string strAddress = "http://www.devpia.com/MAEUL/Contents/List.aspx?page=" + current_page + "&BoardID=69&MAEULNO=28";

                try
                {
                    browser.Navigate(strAddress);

                }
                catch(Exception e1)
                {
                    textBox_Error.AppendText(e1.Message + "\r\n");
                    textBox_Error.AppendText(e1.TargetSite + "\r\n");
                    return;
                }

                if (current_page == end_page)
                    Form_2.when_link_is_found_2();
                
            }


        }
        
        private void 글_수집()
        {
            string link = null;
            string IDstr = null;
            richTextBox1.Text += "글 수집, current_page : " + current_page + "\r\n";
            label_페이지_현황.Text = "페이지 " + start_page + "-" + current_page;
            HtmlDocument HtmlDocument1 = browser.Document;

            HtmlElementCollection tdElementCollection = HtmlDocument1.GetElementsByTagName("td");

            foreach (HtmlElement e1 in tdElementCollection)
            {

                if (e1.GetAttribute("width") == "180")//작성자
                {
                    e1.Focus();
                    IDstr = e1.InnerText;

                    if (id == IDstr)
                    {
                        HtmlElement writerParent = e1.Parent;
                        HtmlElementCollection writerSibling = writerParent.Children;
                        foreach (HtmlElement e2 in writerSibling)
                        {
                            if (e2.GetAttribute("className") == "board_con")//제목
                            {
                                e2.Focus();

                                this.Invoke(new Action(delegate()
                                    {
                                        richTextBox1.AppendText(e2.InnerText + "\r\n\r\n");

                                    }));
                                title = e2.InnerText;

                                link = e2.Children[0].GetAttribute("href");//글 링크 주소

                                this.Invoke(new Action(delegate()
                                {
                                    richTextBox1.AppendText(link + "\r\n\r\n");

                                }));

                                글_링크_목록[글_갯수] = link;
                               
                                글_갯수++;

                                this.Invoke(new Action(delegate()
                                {
                                    label_글_갯수.Text = 글_갯수 + "개 글 검색";                                    

                                }));

                                Form_2.textBox1.AppendText(current_page + "p ");
                                                              
             

                            }
                        }
                    }

                }
            }


        }
                
        void 글_분석_담당_스레드_폼2()
        {
            Form_2 = new Form2(this);
            Application.Run(Form_2);

        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            
            browser.Stop();
            Form_2.browser2.Stop();
            browser = null;
            Form_2.browser2 = null;

        }

               

    }
}
