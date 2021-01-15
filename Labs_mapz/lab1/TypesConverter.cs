using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{
	static class TypesConverter
	{
		public static string ToString(eTokenType type)
		{
			switch (type)
			{
				case eTokenType.VAR:			return "VAR";
				case eTokenType.NUM:			return "NUM";
				case eTokenType.TEXT:			return "TEXT";
				case eTokenType.OPERAND:		return "OPERAND";
				case eTokenType.CONDITION:		return "CONDITION";
				case eTokenType.COMMAND:		return "COMMAND";
				case eTokenType.OPEN_BRACKET:	return "OPEN_BRACKET";
				case eTokenType.CLOSE_BRACKET:	return "CLOSE_BRACKET";
				case eTokenType.END_OP:			return "END_OP";
				case eTokenType.UNKNOWN:		return "UNKNOWN";
			}
			return "";
		}

		public static string ToString(Command command)
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
			}
			return "NAN";
		}

		public static Command FromString(string command)
		{
			switch(command)
			{
				case "right":	return Command.ROTATE_RIGHT;
				case "left":	return Command.ROTATE_LEFT;
				case "move":	return Command.MOVE;
				case "when":	return Command.WHEN;
				case "if":		return Command.IF;
				case "else":	return Command.ELSE;
				case "begin":	return Command.BEGIN;
				case "end":		return Command.END;
			}
			return Command.NAN;
		}
	}
}
