using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{
	enum eTokenType
	{
		VAR,
		NUM,
		TEXT,
		OPERAND,
		CONDITION,
		COMMAND,
		OPEN_BRACKET,
		CLOSE_BRACKET,
		END_OP,
		UNKNOWN
	};

	enum Command
	{
		START_COMMANDS,
		ROTATE_RIGHT = START_COMMANDS,
		ROTATE_LEFT,
		MOVE,
		WHEN,
		IF,
		ELSE,
		BEGIN,
		END,
		EXPR,
		NAN,
		EMPTY,
		END_COMMANDS = EXPR,
	};
}
