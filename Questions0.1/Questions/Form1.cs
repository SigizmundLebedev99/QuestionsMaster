using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace Questions
{
    public partial class Form1 : Form
    {        
        static List<Question> questionList { get; set; }
        static Answer CurrentAnswer { get; set; }
        static List<Answer> myAnswers { get; set; }
        static int count { get; set; } = 0;
        StreamWriter writer = new StreamWriter(Form2.fileName, true);
        int totalScore { get; set; } = 0;
        public Form1()       
        {
            myAnswers = new List<Answer>();
            InitializeComponent();
            ReadFile();
            ShowQuestion(count);//Первый вопрос
            timer1.Start();
            timeBar.Maximum = 300;
            timeBar.Value = 0;
        }
        /// <summary>
        /// Заполняем список вопросов из XML документа
        /// </summary>
        void ReadFile()
        {
            questionList = new List<Question>();
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("MyQuestions.xml");                   
            XmlElement element = xdoc.DocumentElement;
            foreach (XmlNode node in element)
            {
                Question question = new Question();
                if (node.Attributes.Count > 0)
                {
                    XmlNode attribute = node.Attributes.GetNamedItem("text");
                    if (attribute != null) question.Text = attribute.Value;
                }
                foreach (XmlNode cn in node.ChildNodes)
                {
                    Answer answer = new Answer();
                    if (cn.Attributes.Count > 0)
                    {
                        XmlNode attribute = cn.Attributes.GetNamedItem("iscorrect");
                        if (attribute != null) answer.iscorrect = attribute.InnerText == "true";
                    }
                    answer.Text = cn.InnerText;
                    answer.question = question;
                    question.AnswerList.Add(answer);
                }
                questionList.Add(question);
            }
            questionList = GetRandomList(questionList).ToList();
        }

        void ShowQuestion(int index)
        {
            answerButton.Enabled = false;
            DisposeContols();
            countLabel.Text = string.Format("Вопрос: {0}/{1}", count + 1, questionList.Count);
            Question quest = questionList[index];
            questLabel.Text = quest.Text;
            Point point = new Point(questLabel.Left, questLabel.Top);
            //quest.AnswerList = new List<Answer>(GetRandomList(quest.AnswerList));
            DisposeContols();
            foreach (Answer answer in GetRandomList(quest.AnswerList))//Выводим варианты ответов
            {
                point.Y += 30;
                RadioButton rb = new RadioButton();rb.Location = point; rb.Size = new Size(answerPanel.Width, 25); rb.Text = answer.Text;
                answerPanel.Controls.Add(rb);
                rb.CheckedChanged += delegate {
                    answerButton.Enabled = true;
                    CurrentAnswer = answer;/*quest.AnswerList.Single(ans => ans.Text == rb.Text);*/                   
                };                
            }
        }

        static List<T> GetRandomList<T>(List<T> list)//Перемешиватель
        {
            List<T> sublist = list.ToList();
            List<T> resultSet = new List<T>();
            Random rnd = new Random();
            foreach(T i in list)
            {
                int index = rnd.Next(sublist.Count);
                resultSet.Add(sublist[index]);
                sublist.RemoveAt(index);
            }           
            return resultSet;
        }

        private void answerButton_Click(object sender, EventArgs e)
        {
            WriteToFile();
            count++;
            myAnswers.Add(CurrentAnswer);                      
            if (count < questionList.Count)
            {
                DisposeContols();
                ShowQuestion(count);
            }
            else
            {
                ShowResult();
                WriteToFile("Тест завершен!");
                writer.Close();
                this.Close();
            }
        }
        void DisposeContols()
        {
            foreach (Control contr in answerPanel.Controls)
            {
                answerPanel.Controls.Remove(contr);
                contr.Dispose();
            }
        }
        void ShowResult()
        {
            foreach(Answer answer in myAnswers)
            {
                if (answer.iscorrect)
                {
                    totalScore++;
                }
            }
            MessageBox.Show("Количество правильных ответов: " + totalScore, "Тест окончен - " + System.DateTime.Now);
        }

        private void сНачалаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart("Вы действительно хотите начать с начала?");
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?", "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ShowResult();
                WriteToFile("Прохождение теста было прервано!");
                Environment.Exit(1);
            }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            timeBar.Value++;            
            if(timeBar.Value >= 300)
            {
                timer1.Stop();
                MessageBox.Show("Время вышло!", "Warning");
                if(!Restart("Хотите начать с начала?"))
                {
                    ShowResult();
                    WriteToFile("Время вышло");
                    Environment.Exit(1);
                }
                else writer.Write("\nРестарт теста");
            }
        }
        
        bool Restart(string message)
        {
            if (MessageBox.Show(message, "Внимание!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                totalScore = 0;
                count = 0;
                myAnswers = new List<Answer>();
                CurrentAnswer = null;
                ShowQuestion(count);
                timeBar.Value = 0;
                timer1.Start();
                return true;
            }
            else return false;
        }
        void WriteToFile()
        {
            writer.WriteLine("Вопрос: " + questionList[count].Text);
            writer.WriteLine("Ответ: " + CurrentAnswer.Text + "; Балл: " + string.Format(CurrentAnswer.iscorrect?"1":"0"));
            writer.WriteLine();
        }
        void WriteToFile(string str)//запись по окончании теста
        {
            writer.WriteLine(str + "; " + "Количество правильных ответов: " + totalScore + ": Время окнчания - " + System.DateTime.Now);
            writer.Close();
        }
    }

   
}
