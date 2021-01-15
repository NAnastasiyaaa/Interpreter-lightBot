using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LABA1_INTER
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
			board = new eBoard(tableLayoutPanel1);
			board.Init(mas, label2);
		

        }

       



        private void button1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Name = saveFileDialog1.FileName;
                File.WriteAllText(Name, richTextBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Name = openFileDialog1.FileName;
                richTextBox1.Clear();
                richTextBox1.Text = File.ReadAllText(Name);
            }
        }

		private void button3_Click(object sender, EventArgs e)
		{
				button3.Enabled = false;
				timer1.Enabled = true;
				try
				{
					eLexer lexer = new eLexer();
					lexer.FindLexems(richTextBox1.Text);
					parser= new SyntaxTree(lexer.GetTokens());
				}
				catch(Exception _exp)
				{
					richTextBox2.SelectionColor = Color.Red;
					richTextBox2.AppendText(_exp.ToString());
					richTextBox2.SelectionColor = richTextBox2.ForeColor;
					timer1.Enabled = false;
					button3.Enabled = true;
				}
				richTextBox2.Clear();
				richTextBox2.AppendText("Logs:\r\n");
		}
		private int [,] mas = { 
						{ 0,4,0,0,0,0,0,0,0,0},
						{ 0,0,0,1,0,0,0,0,0,0},
						{ 0,0,0,1,0,0,0,0,0,0},
						{ 0,0,0,1,0,0,0,0,0,0},
						{ 0,0,0,1,0,0,0,0,0,0},
						{ 0,0,0,1,0,0,0,0,0,0},
						{ 0,0,0,1,0,0,1,1,1,0},
						{ 0,0,0,0,0,0,0,0,0,0},
						{ 0,0,0,0,0,0,0,0,0,0},
						{ 0,0,0,0,0,0,0,0,0,3}
					};

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			try
			{
				if(!parser.IsEnd())
				{
					eToken t = parser.Next();
					if(t!=null)
					{
						switch (t.Type)
						{
							case eTokenType.TEXT:
								label1.Text = $"[Robot says]: {t.Val}";
                               
								break;
							default:
								board.Do(t);
								if(board.Robot.IsFinished())
								{
									ResetGame();
								}
								break;
						}
						richTextBox2.AppendText(t.Dump());
					}
				}
				else
				{
					timer1.Enabled = false;
					button3.Enabled = true;
				}
			}
			catch(Exception _exp)
			{
				richTextBox2.SelectionColor = Color.Red;
				richTextBox2.AppendText(_exp.ToString());
				richTextBox2.SelectionColor = richTextBox2.ForeColor;
				timer1.Enabled = false;
				button3.Enabled = true;
			}
		}
		SyntaxTree	parser	= null;
		eBoard		board	= null;

		private void button4_Click(object sender, EventArgs e)
		{
			ResetGame();
		}

		private void ResetGame()
		{
			tableLayoutPanel1.Controls.Clear();
			board = null;
			board = new eBoard(tableLayoutPanel1);
			board.Init(mas ,label2);
			timer1.Enabled =false;
			button3.Enabled = true;
		}

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
