using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 졸작_기능_4__게시판_크롤링
{
    public partial class Form2 : Form
    {
        Form1 Form_1 = null;
        int 글_count = 0;
        WebBrowser browser2 = null;

        int 글_갯수_폼2 = 0;
        int title_index = 0;
        string title = null;
        int body_index = 0;

        int[] reply_index = new int[10000];
        
        string[] reply = new string[10000];

        //string reply_total_count = null;
        //int reply_total_count_index = 0;

        public Form2(Form1 frm)
        {
            InitializeComponent();
            Form_1 = frm;
        }

        public void when_link_is_found_2()
        {            
            글_갯수_폼2 = (Form_1.글_갯수 -1);
            
            
            string url = Form_1.글_링크_목록[글_count];
                        
            try
            {                

                browser2 = new WebBrowser();

                browser2.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser2_DocumentCompleted);
                
                browser2.Navigate(url);



            }
            catch (Exception e1)
            {
                textBox_Error.AppendText(e1.Message + "\r\n");
                textBox_Error.AppendText(e1.TargetSite + "\r\n");


            }

        }

        void browser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {            

            if (e.Url.AbsoluteUri == browser2.Url.AbsoluteUri)
            {
                //richTextBox1.AppendText(e.Url.AbsoluteUri + "에 대한 document_completed 이벤트, 글 분석 전\r\n");  디버깅 용    70,71,72 중 72에만 발생      
                if (글_count > 글_갯수_폼2)
                    return;

                글_분석(e.Url.AbsoluteUri);
                글_count++;
                
                try
                {
                    string url = Form_1.글_링크_목록[글_count];
                    browser2.Navigate(url);
                }
                catch (Exception e1)
                {
                    textBox_Error.AppendText(e1.Message + "\r\n");
                    textBox_Error.AppendText(e1.TargetSite + "\r\n");
                    return;
                }


            }
        }
                

        private void 글_분석(string url)
        {
            richTextBox1.AppendText("현재 url : " + url + "\r\n\r\n");


            HtmlDocument hd = browser2.Document;

            HtmlElement contentsTable = hd.GetElementById("Contents");
            HtmlElementCollection contents = contentsTable.Children;
            string contentsStr = null;
                        

            HtmlElementCollection titleCollection = hd.GetElementsByTagName("td");


            foreach (HtmlElement e1 in titleCollection)
            {
                if (e1.GetAttribute("width") == "100%")//제목
                {
                    if (e1.Children.Count == 2)
                    {
                        //e1.Focus();
                        //textBox_Error.Text += e1.InnerText + "," + e1.Children[0].InnerText + "," + e1.Children[1].InnerText + "," + e1.Children[2].InnerText + "\r\n";
                        //title = e1.Children[2].InnerText;
                        title = e1.InnerText;
                        //textBox_Error.Text += "제목 : " + title + "\r\n\r\n";
                    }

                }
            }
            title_index = richTextBox1.Text.Length;
            title = "제목 : " + title;

            this.Invoke(new Action(delegate()
            {
                richTextBox1.AppendText(title + "\r\n\r\n");

            }));

            body_index = richTextBox1.Text.Length;

            this.Invoke(new Action(delegate()
            {
                richTextBox1.AppendText("본문 : ");
            }));

            foreach (HtmlElement e1 in contents)
            {
                contentsStr += e1.InnerText;
            }

            richTextBox1.AppendText(contentsStr + "\r\n\r\n");

            HtmlElementCollection divTable = hd.GetElementsByTagName("div");
            int reply_count = 0;
            foreach (HtmlElement e1 in divTable)
            {
                if (e1.GetAttribute("className") == "content_layout")//댓글 className
                {
                    reply_count++;
                    e1.Focus();
                    //HtmlElementCollection reply_content_header = e1.Children;
                    HtmlElementCollection reply_content_header_dt = e1.Children[0].Children[0].Children;
                    //HtmlElementCollection reply_content_header_dt = reply_content_header.GetAttribute("dt");
                    
                    reply_index[reply_count - 1] = richTextBox1.Text.Length;

                    reply[reply_count - 1] = "댓글 " + reply_count + "번";
                    this.Invoke(new Action(delegate()
                    {
                        richTextBox1.AppendText(reply[reply_count - 1] + "\r\n");

                    }));
                    HtmlElement reply_writer = reply_content_header_dt[reply_content_header_dt.Count - 1];
                    this.Invoke(new Action(delegate()
                    {
                        richTextBox1.AppendText("작성자 이름 : " + reply_writer.InnerText + "\r\n\r\n");

                    }));

                    HtmlElement reply_contents = e1.Children[1].Children[0].Children[0];

                    string reply_contents_str = reply_contents.InnerText;
                    reply_contents_str.Replace("<br>", "\r\n\r\n");
                    this.Invoke(new Action(delegate()
                    {
                        richTextBox1.AppendText(reply_contents_str + "\r\n\r\n");

                    }));

                }
            }
            
            string reply_total_count = "댓글 총 갯수 : " + reply_count + "개";
            
            int reply_total_count_index = richTextBox1.Text.Length;

            this.Invoke(new Action(delegate()
            {
                richTextBox1.AppendText(reply_total_count + "\r\n\r\n");

            }));
            

            this.Invoke(new Action(delegate()
            {
                richTextBox1.AppendText("-----------------------------------------------------------------------------------------------\r\n\r\n");
            }));


            richTextBox1.Select(title_index, title.Length);

            richTextBox1.SelectionColor = Color.Blue;
            System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericSansSerif, 20);
            richTextBox1.SelectionFont = font;

            richTextBox1.Select(body_index, 2);

            richTextBox1.SelectionColor = Color.Chocolate;
            System.Drawing.Font font1 = new System.Drawing.Font(FontFamily.GenericSansSerif, 15);
            richTextBox1.SelectionFont = font1;
                      

            for (int i = 0; i < reply_count; i++)
            {
                int index = reply_index[i];

                richTextBox1.Select(index, reply[i].Length);

                System.Drawing.Font font2 = new System.Drawing.Font(FontFamily.GenericSansSerif, 15, FontStyle.Bold);
                richTextBox1.SelectionFont = font2;
            }

            richTextBox1.Select(reply_total_count_index, reply_total_count.Length);

            richTextBox1.SelectionColor = Color.CadetBlue;

            System.Drawing.Font font3 = new System.Drawing.Font(FontFamily.GenericSansSerif, 20);
            richTextBox1.SelectionFont = font3;
                                  

        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            textBox_Error.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.Copy();
            textBox_Error.AppendText("워드패드나 한글에서 붙혀넣기(ctrl+v) 하시면 됩니다.\r\n");

        }

        
    }
}
