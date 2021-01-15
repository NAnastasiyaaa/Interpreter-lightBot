using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LABA1_INTER
{
	class eBoard
	{
		public eBoard(TableLayoutPanel _tableLayoutPanel)
		{
			tableLayoutPanel = _tableLayoutPanel;
		}

		public void Init(int [,] _boardMap, Label _robotAngle)
		{
			robotAngle = _robotAngle;
			if(_boardMap == null)
			{
				throw new GameLogicException(" Unsetted board Map");
			}
			boardMap = _boardMap;
			InitPictureBoxes();
			BrushBoard();
			if(robot == null)
			{
				throw new GameLogicException(" Unsetted start point");
			}
			InitRobotView();
			robot.Init(boardMap);
            
        }

		public eRobot Robot { get{return robot; } }

		private void InitPictureBoxes()
		{
			for(int i = 0; i < 10; ++i)
            {
                for(int j = 0; j < 10; ++j)
                {
                    tableLayoutPanel.Controls.Add(new PictureBox(), i, j);
                }
            }
            
        }

		public void Do(eToken _token)
		{
			RemovePosition();
			robot.Do(_token);
			UpdatePosition();
		}

		private void InitRobotView()
		{
			robotView = new PictureBox();
			robotView.BackgroundImage = Properties.Resources.robotImg;
			robotView.BackgroundImageLayout = ImageLayout.Stretch;
			robotView.Width = 50;
			robotView.Height = 50;
            UpdatePosition();
        }
		private void RemovePosition()
		{
			Control bg = tableLayoutPanel.GetControlFromPosition( robot.X, robot.Y);
			bg.Controls.Remove(robotView);
		}
		private void UpdatePosition()
		{
			Control bg = tableLayoutPanel.GetControlFromPosition(robot.X, robot.Y);
			robotView.Location = new Point((bg.Width-robotView.Width)/2, 0);
			bg.Controls.Add(robotView);
			string textAngle = "";
			switch(robot.Angle)
			{
				case eAngle.LEFT:	textAngle = "LEFT"; break;
				case eAngle.RIGHT:	textAngle = "RIGHT"; break;
				case eAngle.TOP:	textAngle = "TOP"; break;
				case eAngle.BOTTOM:	textAngle = "BOTTOM"; break;
			}
			robotAngle.Text = $"Angle: {textAngle}, degree: {(int)robot.Angle}";
		}
		private void BrushBoard()
		{
			for(int i = 0; i < tableLayoutPanel.ColumnCount;++i)
			{
				for(int j = 0; j < tableLayoutPanel.RowCount;++j)
				{
					System.Windows.Forms.Control clr = tableLayoutPanel.GetControlFromPosition(i,j);
					if(clr != null)
					{
						switch (boardMap[i,j])
						{
							case 0: clr.BackColor = Color.White;	break;
							case 1: clr.BackColor = Color.Red;		break;
							case 3: clr.BackColor = Color.DeepPink;		break;
							case 4: clr.BackColor = Color.Orange;
								robot = new eRobot(i,j);
								break;
						}

					}
				}
			}
		}
		private eRobot		robot						= null;
		private PictureBox	robotView					= null;
		private Label		robotAngle					= null;
		private int		[,] boardMap					= null;
		private TableLayoutPanel tableLayoutPanel		= null;
	}
}
