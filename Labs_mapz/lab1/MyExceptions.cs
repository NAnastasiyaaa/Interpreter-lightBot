using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{
	class InvalidLex : Exception
	{
		public InvalidLex(string _line, string _src, string _desc)
		{
			line_	= _line;
			src_	= _src ;
			desc_	= _desc;
		}

		public string What()
		{
			return "[InvalidLex] [" + desc_ + " ] [" + src_ + "] in [" + line_ + "]";
		}
		public override string ToString()
		{
			return What();
		}
		protected string line_;
		protected string src_;
		protected string desc_;

	}

	class InvalidParseExpr : Exception
	{
		public InvalidParseExpr(eToken _left, eToken _op, eToken _right)
		{
			left_	= _left;
			op_		= _op;
			right_	= _right;
		}
		public override string ToString()
		{
			return What();
		}
		public string What()
		{
			return "[InvalidParseExpr] [" + left_.Dump() + op_.Dump() + right_.Dump() + "]";
		}

		protected eToken left_;
		protected eToken op_;
		protected eToken right_;

	}

	class InvalidParse : Exception
	{
		public InvalidParse(string _msg)
		{
			msg_ = _msg;
		}

		public string What()
		{
			return "[InvalidParse] [" + msg_ + "]";
		}
		public override string ToString()
		{
			return What();
		}
		protected string msg_;

	}

	class GameLogicException : Exception
	{
		public GameLogicException(string _msg)
		{
			msg_ = _msg;
		}

		public string What()
		{
			return "[GameLogicException] [" + msg_ + "]";
		}
		public override string ToString()
		{
			return What();
		}
		protected string msg_;

	}
}
