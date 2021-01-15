using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{
	class SyntaxTree : ITokenUpdater
	{
		public SyntaxTree(List<eToken> _tokens)
		{
			tree = new List<eNode>();
			callStack = new Stack<eNode>();
			src = _tokens;
			Parse(ref tree);
		}

		public bool IsEnd()
		{
			return callStack.Count == 0 && treeId >= tree.Count;
		}

		public eToken Next()
		{
			if(callStack.Count != 0 && callStack.Peek() != null)
			{
				//return Next(ref currentNode);
				eNode node = callStack.Peek();
				if(node.Token.Type == eTokenType.TEXT)
				{
					callStack.Pop();
					return node.Token;
				}
				if(node.Token.Type == eTokenType.VAR
					&& node.Next().Token.Type == eTokenType.OPERAND
					&& node.Next().Token.Val == "="
				)
				{
					eToken var	= node.Token;
					if(node.Next().Next().Token.Type == eTokenType.NUM)
					{
						var.Val		= node.Next().Next().Token.Val;
					}
					if(node.Next().Next().Token.Type == eTokenType.OPEN_BRACKET)
					{
						var.Val		= node.Next().Next().Next().Token.Val;
					}
					Update(var);
					callStack.Pop();
					return Next();
				}
				else if(node.Token.Type == eTokenType.COMMAND)
				{
					switch(TypesConverter.FromString(node.Token.Val))
					{
						case Command.WHEN:
						case Command.IF:
							
							eNode subNode = node.Next();
							if(subNode == null)
							{	
								callStack.Peek().Reset();
								callStack.Pop();
								return Next();
							}
							callStack.Push(subNode);
							return Next();
						default:
							callStack.Pop();
							return node.Token;
					}
				}
				else
				{
					throw new InvalidParse("What can i do!");
				}
			}
			else if(treeId < tree.Count)
			{
				callStack.Push(tree[treeId]);
				++treeId;
				return Next();
			}
			return null;
		}


		public void Update(eToken token)
		{
			foreach(eNode node in tree)
			{
				node.Update(token);
			}
		}

		protected void Parse(ref List<eNode> _dst)
		{
			if(srcId < src.Count)
			{
				switch(src[srcId].Type)
				{
					case eTokenType.VAR:		ParseExpr(ref _dst);				break;
					case eTokenType.COMMAND:	ParseCommand(ref _dst);				break;
					case eTokenType.TEXT:		_dst.Add(new eNode(src[srcId++]));	Parse(ref _dst); break;
					case eTokenType.END_OP:		srcId++;	 Parse(ref _dst);		break;
				}
			}
		}
		protected eConditionNode ParseCondition()
		{
			eNode expr = new eConditionNode();
			return ParseExprOrCond(ref expr) > 0 ? (eConditionNode)expr : null;
		}
		protected void ParseCommand(ref List<eNode> _dst)
		{
			if(srcId < src.Count)
			{
				eToken token = src[srcId];
				Command command = TypesConverter.FromString(token.Val);
				switch(command)
				{
					case Command.ROTATE_RIGHT:
					case Command.ROTATE_LEFT:
					case Command.MOVE:
						_dst.Add(new eNode(token)); break;
					case Command.BEGIN:
						++srcId;
						Parse(ref _dst);
						break;
					case Command.END:
						return;
					case Command.WHEN:
					case Command.IF:
						eToken t = token;
						++srcId;
						eConditionNode condition = ParseCondition();
						if(condition == null)
						{
							throw new InvalidParse("Invalid condition");
						}
						List<eNode> inComm = new List<eNode>();
						//++_id;
						if(src[srcId].Type == eTokenType.COMMAND
							&& TypesConverter.FromString(src[srcId].Val) == Command.BEGIN
						)
						{
							++srcId;
							Parse(ref inComm);
						//	++srcId;
						}
						else
						{
							throw new InvalidParse("BEGIN...END must have after if|when");
						}
						//List<eNode> outComm = new List<eNode>();
						//Parse(ref outComm);
						eNode result = null;
						if (command == Command.WHEN)
						{
							result = new eLoopNode(token, condition, inComm, null);//outComm);
						}
						else
						{
							result = new eIfElseNode(token, condition, inComm,null);// outComm);
						}
						_dst.Add(result);
						break;
				}
				srcId++;
				Parse(ref _dst);
			}
		}

		protected void ParseExpr(ref List<eNode> _dst)
		{
			if(src[srcId + 1].Type == eTokenType.OPERAND
				&& src[srcId + 1].Val == "=")
			{
				eNode var = new eNode(src[srcId++]);
				var.PushBack(src[srcId++]);
				eNode expr = new eExpressionNode();
				if(ParseExprOrCond(ref expr)==0)
				{
					throw new InvalidParse("empty expr");
				}
				var.PushBack(expr);
				_dst.Add(var);
				Parse(ref _dst);
			}
			else
			{
				throw new InvalidParse("has not operator =");
			}
		}

		protected int ParseExprOrCond(ref eNode _result)
		{
			int items = 0;
			int openBrackets = 0;
			while(srcId < src.Count)
			{
				if(src[srcId].Type == eTokenType.OPERAND
					|| src[srcId].Type == eTokenType.OPEN_BRACKET
					|| src[srcId].Type == eTokenType.CLOSE_BRACKET
					|| src[srcId].Type == eTokenType.NUM
					|| src[srcId].Type == eTokenType.VAR
					|| src[srcId].Type == eTokenType.CONDITION
				)
				{
					items++;
					if(src[srcId].Type == eTokenType.OPEN_BRACKET)
						openBrackets++;
					if(src[srcId].Type == eTokenType.CLOSE_BRACKET)
						openBrackets--;
					if(openBrackets < 0)
					{
						throw new InvalidParse("Invalid CLOSE_BRACKET");
					}
					_result.PushBack(src[srcId++]);
				}
				else
				{
					break;
				}
			}
			return items;
		}

		public void Reset()
		{
		}

		//for use
		private List<eNode>		tree		= null;
		private Stack<eNode>	callStack	= null;
		private int				treeId		= 0;
		//for parse
		int						srcId		= 0;
		List<eToken>			src			= null;
	}
}
