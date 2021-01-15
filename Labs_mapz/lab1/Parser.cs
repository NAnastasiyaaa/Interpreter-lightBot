using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{

	class Parser
	{
		public Parser()
		{
			commands = new List<Command>();
			currentCommandId = 0;
		}

		public void	ParseLine(string line)
		{
			FindAllLex(line);
		}
		public Command	Next()
		{
			int currentId = currentCommandId;
			currentCommandId++;
			if(IsEnd())
			{
				return Command.NAN;
			}
			return commands[currentId];
		}

		public bool	IsEnd()
		{
			return currentCommandId > commands.Count;
		}

		public void	Clear()
		{
			commands.Clear();
			currentCommandId = 0;
		}
		//move;move;right   лексеми!!!!!
		protected void FindAllLex(string line)
		{
			line.ToLower();
			string [] lexems = line.Split(';');
			foreach(string lex in lexems)
			{
				Command command = ContainsCommands(lex);
				if(command == Command.NAN)
				{
					Console.WriteLine("Has Errors at {0} in line {1}", lex, line);
					//do throw;
					break;
				}
				if(command != Command.EMPTY)
				{
					commands.Add(command);
				}
			}
		}

		Command ContainsCommands(string lex)
		{
			if(lex.Length == 0)
				return Command.EMPTY;
			for(int i = (int)Command.START_COMMANDS; i < (int)Command.END_COMMANDS; ++i)
			{
				if(lex.Contains(ToString((Command)i)))
				{
					return (Command)i;
				}
				//do has other lexems in this lex
			}
			return Command.NAN;
		}

		public string ToString(Command command)
		{
			switch(command)
			{
				case Command.ROTATE_RIGHT:	return "right";
				case Command.ROTATE_LEFT:	return "left";
				case Command.MOVE:			return "move";
				case Command.WHEN:			return "when";
				case Command.IF:			return "if";
				case Command.ELSE:			return "else";
				case Command.BEGIN:			return "begin";
				case Command.END:			return "end";
				case Command.EXPR:			return "expr"; //do parse expr
			}
			return "NAN";
		}

		protected List<Command> commands;
		protected int currentCommandId = 0;
	}
}
