using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LABA1_INTER
{
	class eToken
	{
		public eToken() { }
		public eToken(eTokenType	_tokenType,
					  string		_val,
					  string		_name = "")
		{
			type	= _tokenType;
			val		= _val;
			name	= _name;
		}
		public eTokenType	Type	{ get{ return type;} set{type = value; } }
		public string		Val		{ get{ return val;} set{val = value; } }
		public string		Name	{ get{ return name;} set{name = value; } }

		public string		Dump() //[type][value][name]
		{
			return "["+TypesConverter.ToString(type)+"]\t["+val+"]\t["+name+"]\r\n";
		}

		public bool			IsValid()
		{
			return type != eTokenType.UNKNOWN && val.Length >= 0;
		}

		protected eTokenType	type	= eTokenType.UNKNOWN;
		protected string		val;
		protected string		name;
	}
}
