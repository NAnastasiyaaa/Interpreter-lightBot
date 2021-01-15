using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LABA1_INTER
{
	enum eAngle
	{
		LEFT	= 180,
		RIGHT	= 0,
		TOP		= 90,
		BOTTOM	= 270,
	}

	class eRobot
	{

		public eRobot(int _x,
					  int _y,
					  eAngle _angle = eAngle.RIGHT)
		{
			x		= _x;
			y		= _y;
			angle	= _angle;
		}

		public void Init(int [,] mas)
		{
			boardMap = mas;
		}

		public int X		{get{ return x;} }
		public int Y		{get{ return y;} }
		public eAngle Angle	{get{ return angle;} }

		public bool CanPassOnBoard(int _x, int _y)
		{
			if(boardMap[_x,_y] == 1)
				return false;
			return true;
		}

		public bool IsFinished()
		{
			if(boardMap[x,y] == 3)
				return true;
			return false;
		}

		public void Do(eToken _token)
		{
			if(_token.Type == eTokenType.COMMAND)
			{
				switch(TypesConverter.FromString(_token.Val))
				{
					case Command.ROTATE_LEFT:	Rotate(true);	break;
					case Command.ROTATE_RIGHT:	Rotate(false);	break;
					case Command.MOVE:			Move();			break;
				}
			}
		}

		private void Rotate(bool isLeft)
		{
			switch(angle)
			{
				case eAngle.LEFT:
					angle= isLeft ? eAngle.BOTTOM : eAngle.TOP;
					break;
				case eAngle.RIGHT:
					angle= isLeft ? eAngle.TOP : eAngle.BOTTOM;
					break;
				case eAngle.TOP:
					angle= isLeft ? eAngle.LEFT : eAngle.RIGHT;
					break;
				case eAngle.BOTTOM:
					angle= isLeft ? eAngle.RIGHT : eAngle.LEFT;
					break;
			}

//			angle = (eAngle) (360-Math.Abs((_angle + (int)angle) % 360));
		}

		private void Move()
		{
			int newX = x;
			int newY = y;
			switch(angle)
			{
				case eAngle.LEFT:		--newX; break;
				case eAngle.RIGHT:		++newX; break;
				case eAngle.TOP:		--newY; break;
				case eAngle.BOTTOM:		++newY; break;
			}
			if(!CanPassOnBoard(newX, newY))
			{
				throw new GameLogicException(" Cannot pass on board");
			}
			x = newX;
			y = newY;
			if(IsFinished())
			{
				MessageBox.Show("You will DO IT");
			}
		}
		private int [,]		boardMap	= null;
		private int			x			= 0;
		private int			y			= 0;
		private eAngle		angle		= 0;
	}
}
