
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{
	class eLexer
	{
		public eLexer()
		{
			tokens = new List<eToken>();
		}

		public void FindLexems(string _text) 
		{
			pos = 0;
			text = _text.ToLower();
			string dev = "'";
			while(pos < text.Length)
			{
				if(text[pos] == ' ' || text[pos] == '\t' || text[pos] == '\n'  || text[pos] == '\r')
				{
					++pos;
					continue;
				}
				if(text[pos] == ';')
				{
					tokens.Add(new eToken(eTokenType.END_OP, ";"));
					++pos;
					continue;
				}
				if(AddTokenIfString(dev[0])
					|| AddTokenIfString('"')
					|| AddTokenIfNumber()
					|| AddTokenIfOperandOrCondition()
					|| AddTokenIfCommand()
					|| AddTokenIfVariable()
				)
				{
					continue;
				}
				throw new InvalidLex(text, text[pos].ToString(), "Unknown Error");
			}
			if(!IsValid())
			{
				throw new InvalidLex(text, "", "Invalid Lexems");
			}
		}

		protected bool IsValid()
		{
			eToken prevToken	= null;
			int countBrackets	= 0;
			int countBeginEnd	= 0;
			foreach(eToken token in tokens)
			{
				if(prevToken != null)
				{
					if(token.Type == eTokenType.OPEN_BRACKET)
					{
						countBrackets++;
					}
					if(token.Type == eTokenType.CLOSE_BRACKET)
					{
						countBrackets--;
						if(countBrackets < 0)
						{
							throw new InvalidLex(token.Name, prevToken.Name, "Invalid Lexem )(");
						}
					}
					if(token.Type == eTokenType.COMMAND
						&& TypesConverter.FromString(token.Val) == Command.BEGIN)
					{
						countBeginEnd++;
					}
					if(token.Type == eTokenType.COMMAND
						&& TypesConverter.FromString(token.Val) == Command.END)
					{
						countBeginEnd--;
						if(countBeginEnd < 0)
						{
							throw new InvalidLex(token.Name, prevToken.Name, "Invalid Lexem End after begin");
						}
					}
					switch(prevToken.Type)
					{
						case eTokenType.VAR:
						case eTokenType.NUM:
						case eTokenType.TEXT:
							if (token.Type == eTokenType.CONDITION || token.Type == eTokenType.OPERAND)
							{
								break;
							}
							throw new InvalidLex(token.Name, prevToken.Name, "Invalid Lexem after token");
						case eTokenType.OPEN_BRACKET:
						case eTokenType.CLOSE_BRACKET:
						case eTokenType.OPERAND:
						case eTokenType.CONDITION:
						case eTokenType.COMMAND:
							if (token.Type == eTokenType.VAR 
								|| token.Type == eTokenType.NUM
								|| token.Type == eTokenType.TEXT
								|| token.Type == eTokenType.OPEN_BRACKET
								|| token.Type == eTokenType.CLOSE_BRACKET
							)
							{
								break;
							}
							if(token.Type == eTokenType.COMMAND
								&& IsValidCommands(prevToken,token)
							)
							{
								throw new InvalidLex(token.Name, prevToken.Name, "Invalid command after command");
							}
							throw new InvalidLex(token.Name, prevToken.Name, "Invalid Lexem after token");
					}
					prevToken = token;
				}
			}
			if(countBrackets == 0
				&& countBeginEnd == 0)
			{
				return true;
			}
			else if (countBrackets != 0)
			{
				throw new InvalidLex("", "", "Has some not closed brackets");
			}
			else if (countBeginEnd != 0)
			{
				throw new InvalidLex("", "", "Has some not closed command Begin");
			}
			return false;
		}

		public List<eToken> GetTokens()	{ return tokens; }

		protected bool IsValidCommands(eToken prev, eToken current)
		{
			Command prevCommand		= TypesConverter.FromString(prev.Val);
			Command currentCommand	= TypesConverter.FromString(current.Val);
			switch(prevCommand)
			{
				case Command.WHEN:
				case Command.IF:
				case Command.ELSE:			return false; //todo check if forwarded else

				case Command.ROTATE_RIGHT:
				case Command.ROTATE_LEFT:
				case Command.MOVE:
				case Command.BEGIN:
				case Command.END:			return true; //todo check begin forwarded end
			}
			return false;
		}

		protected bool AddTokenIfVariable()
		{
			if (text[pos] >= 'a' && text[pos] <= 'z')
			{
				eToken t = new eToken();
				t.Type = eTokenType.VAR;
				t.Name = text[pos].ToString();
				t.Val  = "";
				++pos;
				while((text[pos] >= 'a' && text[pos] <= 'z')
					|| (text[pos] >= '0' && text[pos] <= '9')
				)
				{
					t.Name += text[pos].ToString();
					pos++;
				}
				tokens.Add(t);
				return true;
			}
			return false;
		}
		protected bool AddTokenIfCommand()
		{
			string t;
			int j = pos;
			for (int i = (int)Command.START_COMMANDS; i < (int)Command.END_COMMANDS; ++i)
			{
				t = TypesConverter.ToString((Command)i);
				j = text.IndexOf(t, pos);
				if (pos == j)
				{
					eToken token	= new eToken();
					token.Type		= eTokenType.COMMAND;
					token.Val		= t;
					pos				+=t.Length;
					tokens.Add(token);
					return true;
				}
			}
			return false;
		}

		protected bool AddTokenIfString(char devider = '"')
		{
			if (text[pos] == devider)
			{
				eToken t = new eToken(eTokenType.TEXT, "");
				++pos;
				while (pos < text.Length && text[pos] != devider)
				{
					t.Val += text[pos];
					++pos;
				}
				if(pos >= text.Length)
				{
					throw new InvalidLex("", t.Val, $"Don`t closed Text {devider}");
				}
				++pos;
				tokens.Add(t);
				return true;
			}
			return false;
		}

		protected bool AddTokenIfOperandOrCondition()
		{
			eToken t = null;
			if (text[pos] == '!' && text[pos+1] == '=')
			{
				t = new eToken(eTokenType.OPERAND, "!=");
				pos += 2;
			}
			else if (text[pos] == '=')
			{
				if (text[pos + 1] == '=')
				{
					t = new eToken(eTokenType.CONDITION, "==");
					pos+=2;
				}
				else
				{
					t = new eToken(eTokenType.OPERAND, "=");
					pos++;
				}
			}
			else if (text[pos] == '>' || text[pos] == '<')
			{
				t = new eToken(eTokenType.CONDITION, text[pos].ToString());
				pos++;
				if (text[pos] == '=')
				{
					t.Val += text[pos].ToString();
					pos++;
				}
			}
			else if (text[pos] == '+' 
					|| text[pos] == '-'
					|| text[pos] == '*'
					|| text[pos] == '/'
					|| text[pos] == '%'
					|| text[pos] == '^')
			{
				t = new eToken(eTokenType.OPERAND, text[pos].ToString());
				pos++;
			}
			else if (text[pos] == '(')
			{
				t = new eToken(eTokenType.OPEN_BRACKET, "(");
				pos++;
				if (text.IndexOf(')',pos) == -1)
				{
					throw new InvalidLex(text, text[pos-1].ToString(), "Cannot find close bracket");
				}
			}
			else if (text[pos] == ')')
			{
				t = new eToken(eTokenType.CLOSE_BRACKET, ")");
				pos++;
			}

			if(t != null)
			{
				tokens.Add(t);
				return true;
			}
			return false;
		}

		protected bool AddTokenIfNumber()
		{
			if (text[pos] >= '0' && text[pos] <= '9')
			{
				eToken t = new eToken(eTokenType.NUM,"");
				bool hasDot = false;
				while(pos < text.Length)
				{
					if (text[pos] >= 'a' && text[pos] <= 'z')
					{
						throw new InvalidLex(text, text[pos].ToString(), "Invalid variable");
					}
					if (text[pos] == '.' || text[pos] == ',')
					{
						if(hasDot)
						{
							throw new InvalidLex(text, text[pos].ToString(),"Has to or more points");
						}

						hasDot = true;
						t.Val = t.Val + '.';
					}
					else if (text[pos] >= '0' && text[pos] <= '9')
					{
						t.Val = t.Val + text[pos];
					}
					else
					{
						break;
					}
					pos++;
				}
				tokens.Add(t);
				return true;
			}
			return false;
		}

		protected List<eToken> tokens;
		string	text;
		int		pos		= 0;
	}
}
