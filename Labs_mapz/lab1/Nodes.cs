using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{
	class eNode : ITokenUpdater
	{
		public eNode(eToken _token, eNode _next = null)
		{
			token	=	_token;
			next	=	_next;
		}
		public eNode(){}

		public virtual void		PushBack(eToken _token)
		{
			eNode tail = this;
			if(tail.Token == null)
			{
				tail.token = _token;
				return;
			}
			while(tail.next != null)
			{
				tail = tail.next;
			}
			tail.next = new eNode(_token);
		}

		public virtual void		PushBack(eNode _node)
		{
			eNode tail = this;
			if(tail.Token == null)
			{
				tail.token = _node.token;
				tail.next = _node.next;
				return;
			}
			while(tail.next != null)
			{
				tail = tail.next;
			}
			tail.next = _node;
		}

		public virtual bool		IsValid()	{

			bool isValid = token.IsValid();
			if(isValid && next != null)
			{
				isValid &= next.IsValid();
			}
			return isValid;
		}

		public uint		Size()	{

			uint size = Convert.ToUInt32(token.IsValid());
			if(size > 0 && next != null)
			{
				size += next.Size();
			}
			return size;
		}

		public virtual string	Dump()
		{
			string dump = token.Dump();
			if(next != null)
			{
				dump += next.Dump();
			}
			return dump;
		}
		public eToken			Token { get{ return token; } }
		public virtual eNode	Next()		{ return next; }
		public eNode			Next_()		{ return next; }
		public virtual void Update(eToken _token)
		{
			eNode tail = this;
			while(tail.next != null)
			{
				if(_token.Type == eTokenType.VAR && tail.Token.Name == _token.Name)
				{
					tail.Token.Val = _token.Val;
				}
				tail = tail.next;
			}
		}

		public virtual void Reset()
		{
			//eNode tail = this;
			//while(tail.next != null)
			//{
			//	tail.Reset();
			//	tail = tail.next;
			//}
		}

		protected eNode next	= null;
		protected eToken token	= null;
	}

	class eExpressionNode: eNode
	{
		public eExpressionNode(eToken	_token,
							   eNode	_expression = null)
		: base(_token, _expression)
		{}
		public eExpressionNode(): base(){}

		public override bool		IsValid()	{
			return base.IsValid() && IsExpression();
		}

		public override eNode	Next()		{ return new eNode(Calculate(), null); }

		public eToken	Calculate()	{
			if(next == null)
				return token;
			Stack<eToken> stack	= new Stack<eToken>();
			eToken result		= new eToken();
			result.Type			= eTokenType.NUM;
			result.Val			= Token.Val;
			eNode current		= this;
			eToken [] t			= new eToken[5];
			eToken tmp			= null;
			bool	isValid		= true;
			double	res			= 0;
			while(current != null)
			{
				tmp = current.Token;
				stack.Push(tmp);
				if(stack.Count >= 5 && tmp.Type == eTokenType.CLOSE_BRACKET)
				{
					t[0]	= stack.Pop();
					isValid	= isValid & (t[0].Type == eTokenType.CLOSE_BRACKET);
					t[1]	= stack.Pop();
					isValid	= isValid & (t[1].Type == eTokenType.NUM || t[1].Type == eTokenType.VAR);
					t[2]	= stack.Pop();
					isValid	= isValid & (t[2].Type == eTokenType.OPERAND);
					t[3]	= stack.Pop();
					isValid	= isValid & (t[3].Type == eTokenType.NUM || t[3].Type == eTokenType.VAR);
					t[4]	= stack.Pop();
					isValid	= isValid & (t[4].Type == eTokenType.OPEN_BRACKET);
					if(isValid)
					{
						res			= Calculate_(t[1],t[2],t[3]);
						tmp			= new eToken(eTokenType.NUM, res.ToString());
						stack.Push(tmp);
					}
					else break;//added calculate exception
				}
				current = current.Next_();
			}
			if (isValid)
			{
			  result.Val = res.ToString();
			}
			return isValid ? result : null;
		}

		protected bool	IsExpression()
		{
			//if (Size() < 3
			//	|| token.Type != eTokenType.VAR
			//	|| next.Token.Val != "="
			//)
			//{
			//	throw new InvalidParse("So small params to expr");
			//}
			//bool isValid = true;
			//bool isFullBrackets = Calculate() != null;
			//eNode node = next;
			//while(node != null)
			//{
			//	if(node.Token.Type == eTokenType.CLOSE_BRACKET
			//		|| node.Token.Type == eTokenType.OPEN_BRACKET)
			//	node = node.Next();
			//}
			return true;
		}

		private double Calculate_(eToken left, eToken op, eToken right)
		{
			bool isLeftInt	= true;
			bool isRightInt	= true;
			double _r		= GetVal(left, ref isLeftInt);
			double _l		= GetVal(right, ref isRightInt);
			double res		= 0.0f;
			eToken result	= new eToken();
			switch (op.Val)
			{
				case "+": return _l + _r;
				case "-": return _l - _r;
				case "*": return _l * _r;
				case "/": return _l / _r;
				case "%": return _l % _r;
				case "^": return Math.Pow(_l, _r);
				case "=": return _r;
			}
			return res;
		}

		protected double GetVal(eToken token, ref bool isInt)
		{
			double l = 0.0;
			if(token.Type == eTokenType.TEXT)
			{
				throw new InvalidParse("Text type not supported now" + token.Dump());
			}
			if (token.Type == eTokenType.VAR
				|| token.Type == eTokenType.NUM)
			{
				if(token.Val == "")
				{
					throw new InvalidParse("Undefined variable:" + token.Dump());
				}
				else
				{
					l = Convert.ToDouble(token.Val);
					isInt = false;
				}
			}
			return l;
		}
	}

	class eConditionNode: eNode
	{
		public eConditionNode(eToken _token,
							  eNode _condition)
		: base(_token, _condition)
		{}

		public eConditionNode()
		: base()
		{}

		public override eNode	Next()		{ return new eNode(new eToken(eTokenType.NUM, IsConditionSuccess().ToString(), null)); }
		

		public override bool		IsValid()	{
			return base.IsValid() && IsCondition();
		}

		public bool IsConditionSuccess() { return ResolveCondition(); }

		protected bool IsCondition()		{ return true; }
		protected bool ResolveCondition()	{
			
			if(!isConditionParsed)
			{
				leftExpr = new eExpressionNode();
				rightExpr = new eExpressionNode();
				condition = null;
				eNode node = this.Next_();
				bool isLeft = true;
				while(node.Next_() != null)
				{
					if(node.Token.Type == eTokenType.CONDITION)
					{
						if(!isLeft)
						{
							throw new InvalidParse("not supported more one conditiom and|or|not");
						}
						isLeft = false;
						condition = node.Token;
						node = node.Next_();
						continue;
					}
					if(isLeft)
					{
						leftExpr.PushBack(node.Token);
					}
					else
					{
						rightExpr.PushBack(node.Token);
					}
					node = node.Next_();
					isConditionParsed = true;
				}
			}
			return ResolveCondition_();
		}
		public override void Update(eToken token)
		{
			base.Update(token);
			if(leftExpr != null)
			{
				leftExpr.Update(token);
			}
			if (rightExpr != null)
			{
				rightExpr.Update(token);
			}
		}

		public override void Reset()
		{
			//base.Reset();
			//if(leftExpr != null)
			//{
			//	leftExpr.Reset();
			//}
			//if (rightExpr != null)
			//{
			//	rightExpr.Reset();
			//}
		}
		protected bool ResolveCondition_()
		{
			if(condition!=null && rightExpr==null)
			{
				throw new InvalidParse("has condition but hasn`t right expr");
			}
			if(condition==null)
			{
				eToken result = leftExpr.Calculate();
				if(result == null)
				{
					throw new InvalidParse("Cannot Calculate left expr");
				}
				return Convert.ToDouble(result.Val) > 0;
			}
			eToken lResult = leftExpr.Calculate();
			if(lResult == null)
			{
				throw new InvalidParse("Cannot Calculate left expr");
			}
			eToken rResult = rightExpr.Calculate();
			if(rResult == null)
			{
				throw new InvalidParse("Cannot Calculate right expr");
			}
			double lVal	= Convert.ToDouble(lResult.Val);
			double rVal	= Convert.ToDouble(rResult.Val);
			switch(condition.Val)
			{
				case ">":  return lVal > rVal;
				case "<":  return lVal < rVal;
				case "==": return lVal == rVal;
				case "!=": return lVal != rVal;
				case ">=": return lVal >= rVal;
				case "<=": return lVal <= rVal;
			}
			return false;
		}
		protected bool				isConditionParsed	= false;
		protected eExpressionNode	leftExpr			= null;
		protected eToken			condition			= null;
		protected eExpressionNode	rightExpr			= null;

	}

	class eIfElseNode : eNode
	{
		public eIfElseNode(eToken			_token,
						   eConditionNode	_condition,
						   List<eNode>		_nextIfTrue	= null,
						   List<eNode>		_otherwise	= null)
		: base(_token, null)
		{
			condition		= _condition;
			inCondition		= _nextIfTrue;
			otherwise		= _otherwise;
		}

		public override bool IsValid()
		{
			foreach(eNode node in inCondition)
			{
				if(!node.IsValid())
				{
					return false;
				}
			}
			foreach(eNode node in otherwise)
			{
				if(!node.IsValid())
				{
					return false;
				}
			}
			return (condition	!= null && condition.IsValid());
		}
		public override eNode	Next()
		{
			if(!isChecked)
			{
				isInCondition	= condition.IsConditionSuccess();
				isChecked = true;
			}
			if (isInCondition)
			{
				if(id < inCondition.Count)
					return inCondition[id++];
				isInCondition = false;
				id = 0;
			}
			if(otherwise!= null && id < otherwise.Count)
				return otherwise[id++];
			return null;
		}


		public override void Update(eToken _token)
		{
			base.Update(_token);
			condition.Update(_token);
			foreach(eNode node in inCondition)
			{
				node.Update(_token);
			}
			if(otherwise != null)
			{
				foreach(eNode node in otherwise)
				{
					node.Update(_token);
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			//condition.Reset();
			id = 0;
			isChecked= false;
			foreach(eNode node in inCondition)
			{
				node.Reset();
			}
			if(otherwise != null)
			{
				foreach(eNode node in otherwise)
				{
					node.Reset();
				}
			}
		}

		//public override string	Dump()
		//{
		//	string dump = token.Dump();
		//	dump += "[CONDITION BEGIN]\r\n" + condition.Dump() + "[CONDITION END]\r\n";
		//	if(next != null)
		//	{
		//		dump += "[IF_BEGIN]\r\n";
		//		dump += next.Dump();
		//		dump += "[IF_END]\r\n";
		//	}
		//	if(otherwise != null)
		//	{
		//		dump += "[OTHERWISE_BEGIN]\r\n";
		//		dump += otherwise.Dump();
		//		dump += "[OTHERWISE_END]\r\n";
		//	}
		//	return dump;
		//}

		protected eConditionNode	condition	= null;
		protected List<eNode>		inCondition	= null;
		protected List<eNode>		otherwise	= null;
		protected int				id = 0;
		protected bool				isInCondition	= false;
		protected bool				isChecked		= false;
	}

	class eLoopNode: eIfElseNode
	{
		public eLoopNode(eToken			_token, 
						 eConditionNode	_condition,
						 List<eNode>	_inLoop		= null,
						 List<eNode>	_afterLoop	= null)
		: base(_token, _condition, _inLoop, _afterLoop)
		{}

		//public override bool IsValid()
		//{
		//	return (condition	!= null && condition.IsValid())
		//		&& (next		!= null && base.IsValid())
		//		&& ((afterLoop != null && afterLoop.IsValid()) || afterLoop == null);
		//}
		public bool				IsLoopEnded()	{ return condition.IsConditionSuccess(); }
		public override eNode	Next()
		{
			if(!isChecked)
			{
				isInCondition	= condition.IsConditionSuccess();
				isChecked = true;
			}
			if(isInCondition)
			{
				if(id < inCondition.Count)
					return inCondition[id++];
				isInCondition = IsLoopEnded();
				id = 0;
				return Next();
			}
			if(otherwise!= null && id < otherwise.Count)
				return otherwise[id++];
			return null;

		}
		//public override string	Dump()
		//{
		//	string dump = token.Dump();
		//	dump += "[CONDITION BEGIN]\r\n" + condition.Dump() + "[CONDITION END]\r\n";
		//	if(next != null)
		//	{
		//		dump += "[LOOP_BEGIN]\r\n";
		//		dump += next.Dump();
		//		dump += "[LOOP_END]\r\n";
		//	}
		//	if(afterLoop != null)
		//	{
		//		dump += "[AFTER_LOOP_BEGIN]\r\n";
		//		dump += afterLoop.Dump();
		//		dump += "[AFTER_LOOP_END]\r\n";
		//	}
		//	return dump;
		//}
	}
}
