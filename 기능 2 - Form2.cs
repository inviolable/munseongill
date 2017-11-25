using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net; //WebRequest 객체 사용하기 위한 것
using System.IO; // Stream 객체 등 사용하기 위한 것
using System.Net.Json; // 커스텀 dll, Json 파싱 라이브러리 사용하기 위한 것
using System.Xml; //Xml 파서 이용하기 위한 것

namespace http게시판글긁어오기2
{
    public partial class Form2 : Form
    {

        public WebBrowser browser2;
        string title = null;

        string[] reply = new string[100];

        string reply_total_count = null;
                

        int title_index = 0;
        int body_index = 0;
        int[] reply_index = new int[100];

        int reply_total_count_index = 0;

        int 글_count = 0;
        Form1 Form_1 = null;

        int[] index = new int[100000];
        string find_str = null;
             
        public Form2(Form1 frm)
        {       
            InitializeComponent();
            Form_1 = frm;
                        
        }


        private void Form2_Load(object sender, EventArgs e)
        {

        }




        public void when_link_is_found_2()
        {

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
     
                if (글_count >= Form_1.글_갯수)
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

            title_index = richTextBox1.Text.Length;

            HtmlElementCollection titleCollection = hd.GetElementsByTagName("td");
                        

            foreach (HtmlElement e1 in titleCollection)
            {                
                if (e1.GetAttribute("width") == "100%")
                {
                    if (e1.Children.Count == 2)
                    {

                        title = e1.InnerText;

                    }
                   
                }
            }

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

                    HtmlElementCollection reply_content_header_dt = e1.Children[0].Children[0].Children;


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
                       
            
            reply_total_count = "댓글 총 갯수 : " + reply_count + "개";
 

            reply_total_count_index = richTextBox1.Text.Length;

            this.Invoke(new Action(delegate()
            {
                richTextBox1.AppendText(reply_total_count + "\r\n\r\n");

            }));


            this.Invoke(new Action(delegate()
            {
                richTextBox1.AppendText("-----------------------------------------------------------------\r\n\r\n");
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

            word_listBox.Items.Clear();
            textBox_Error.Clear();

            find_str = word_textBox.Text;
            int count = richTextBox1.Find(find_str);


            if (count == -1)
                textBox_Error.Text += "해당 단어가 발견되지 않았습니다.\r\n";
            
            string richTextBox1_str = richTextBox1.Text;
                        
            int word_count = 0;
            int nextStartIndex = 0;

            while (true)
            {
                index[word_count] = richTextBox1_str.IndexOf(find_str, nextStartIndex);
                if (index[word_count] == -1)
                    break;

                textBox_Error.Text += "[" + word_count + "]" + " 인덱스 : " + index[word_count] + "\r\n";
                
                word_listBox.Items.Add('"' + find_str + '"' + " 단어의 " + (word_count+1) + "번째 발견 위치");
                nextStartIndex = index[word_count] + 1;

                word_count++;
                
            }
            
            

        }

        private void clear_button_click(object sender, EventArgs e)
        {            
            //글_마지막_인덱스 = 0;
            //form2 컨트롤
            textBox_Error.Text = null;
            textBox1.Text = null;
            richTextBox1.Text = null;
            //form1 컨트롤
            Form_1.textBox_id.Text = null;
            Form_1.textBox_Range.Text = null;
            Form_1.textBox_Error.Text = null;
            Form_1.label_글_갯수.Text = "글 갯수가 표시됩니다.";
            Form_1.label_페이지_현황.Text = "페이지 검색 현황이 표시됩니다.";
            Form_1.richTextBox1.Text = null;

            //form1 변수
            Form_1.start_page = 0;
            Form_1.current_page = 0;
            Form_1.end_page = 0;
            Form_1.글_갯수 = 0;               
           
        


            

        }

        int count = 0;
        int last_index = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            if (count == 0)
            {
                //richTextBox1.Text += "동해물과 백두산이\r\n\r\n";
                richTextBox1.AppendText("동해물과 백두산이\r\n\r\n");
                richTextBox1.Select(0, 3);
                richTextBox1.SelectionColor = Color.BurlyWood;
                System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericSansSerif, 20, FontStyle.Italic);
                richTextBox1.SelectionFont = font;
                count++;
                last_index = richTextBox1.Text.Length;
            }
            else if (count == 1)
            {
                //richTextBox1.Text += "마르고 닳도록 하느님이 보우하사\r\n\r\n";
                richTextBox1.AppendText("마르고 닳도록 하느님이 보우하사\r\n\r\n");
                richTextBox1.Select(last_index, 5);
                richTextBox1.SelectionColor = Color.BlueViolet;

                System.Drawing.Font font = new System.Drawing.Font(FontFamily.GenericSansSerif, 15);
                richTextBox1.SelectionFont = font;
                count = 0;
                last_index = 0;

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.Copy();

            textBox_Error.Text += "리치 텍스트 박스의 글이 전체 선택 + 복사 되었습니다. 한글이나 워드 문서에 붙여넣기 하시면 됩니다.\r\n\r\n";

        }

        private void word_listBox_DoubleClick(object sender, EventArgs e)
        {
            int index_count = word_listBox.SelectedIndex;

            richTextBox1.DeselectAll();
         
            richTextBox1.Select(index[index_count], find_str.Length);
            
            richTextBox1.ScrollToCaret();
            richTextBox1.Focus();
            

            

        }

        private void button4_Click(object sender, EventArgs e)
        {            
            SaveFileDialog sfd = new SaveFileDialog();
            textBox_Error.Text += "저장 가능 확장자 : hwp, rtf 등\r\n";
            DialogResult diresult = sfd.ShowDialog();
            if (diresult != DialogResult.OK && diresult != DialogResult.Yes)
            {
                textBox_Error.Text += "저장이 취소되었습니다.\r\n";
                return;
            }
            
            string path = sfd.FileName;
            textBox_Error.Text += path + " 경로에 저장되었습니다.\r\n";

            richTextBox1.SaveFile(path);

        }

        
        bool 한글_판별함수(string str)
        {
            char[] charArr = str.ToCharArray();
            int len = charArr.Length;
            int count = 0;

            foreach(char c in charArr)
            {
                if(char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    count++;
                }
            }

            if(count == len)
                return true;
            else
                return false;
                       



               
        }


        string 영어_단어_한글_뜻으로_변환(string word)
        {
            byte[] bt = new byte[100000];
            HttpWebRequest wReq;
            HttpWebResponse wRes;

            if (word == null)
            {
                return null;
                
            }

            word = word.ToLower();
                        
            string url = "https://glosbe.com/gapi/translate?from=eng&dest=kor&format=xml&pretty=true&phrase=" + word;

            string str = null;

            try
            {
                
                Uri uri = new Uri(url); // string 을 Uri 로 형변환
                wReq = (HttpWebRequest)WebRequest.Create(uri); // WebRequest 객체 형성 및 HttpWebRequest 로 형변환
                wReq.Method = "GET"; // 전송 방법 "GET" or "POST"
                wReq.ServicePoint.Expect100Continue = false;

                using (wRes = (HttpWebResponse)wReq.GetResponse())
                {
                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, ASCIIEncoding.UTF8, true);//인코딩 방식이 안맞으면 글자 깨져서 보임,
                    //확인 결과 UTF-8로 하면 글자 잘 나옴
                    string tempStr = null;
                    
                    XmlReader reader = XmlReader.Create(readerPost);
                                        
                    for (int i = 0; i < 50; i++)
                    {
                        bool exist = reader.ReadToFollowing("string");

                        if (exist == false)
                            break;

                        tempStr = reader.ReadString();
                        
                        if (한글_판별함수(tempStr) == false)
                            continue;

                        str += tempStr + "\r\n";
                        
                    } 

                    
                }
            }
            catch (Exception e1)
            {
                textBox_Error.Text += "에러 메시지 : " + e1.Message + ", 에러 원인 메소드 : " + e1.TargetSite + "스택 trace : " + e1.StackTrace + "\r\n";
            }

            return str;
        }


        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {                
                Point pos1 = new Point(e.X, e.Y);
                string 검색_대상_단어 = richTextBox1.SelectedText;
                textBox_Error.AppendText(검색_대상_단어 + "\r\n");
                                
                
                if (richTextBox1.Focused == true)
                {
                    string str = 영어_단어_한글_뜻으로_변환(검색_대상_단어);
                    toolTip_단어_뜻.Show(str,richTextBox1);

                }
                

            }
            

        }


    }
}
