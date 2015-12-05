// $ANTLR 2.7.5 (20050128): "metaphor.txt" -> "P.cs"$

	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using Metaphor.Collections;

namespace Metaphor.Compiler
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using IToken                   = antlr.IToken;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
	public 	class P : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int FALSE = 4;
		public const int TRUE = 5;
		public const int INT32_DECIMAL_INTEGER_LITERAL = 6;
		public const int INT64_DECIMAL_INTEGER_LITERAL = 7;
		public const int UINT32_DECIMAL_INTEGER_LITERAL = 8;
		public const int UINT64_DECIMAL_INTEGER_LITERAL = 9;
		public const int INT32_HEXADECIMAL_INTEGER_LITERAL = 10;
		public const int INT64_HEXADECIMAL_INTEGER_LITERAL = 11;
		public const int UINT32_HEXADECIMAL_INTEGER_LITERAL = 12;
		public const int UINT64_HEXADECIMAL_INTEGER_LITERAL = 13;
		public const int FLOAT32_REAL_LITERAL = 14;
		public const int FLOAT64_REAL_LITERAL = 15;
		public const int DECIMAL_REAL_LITERAL = 16;
		public const int CHARACTER_LITERAL = 17;
		public const int STRING_LITERAL = 18;
		public const int NULL = 19;
		public const int IDENTIFIER = 20;
		public const int DOT = 21;
		public const int BOOL = 22;
		public const int DECIMAL = 23;
		public const int SBYTE = 24;
		public const int BYTE = 25;
		public const int SHORT = 26;
		public const int USHORT = 27;
		public const int INT = 28;
		public const int UINT = 29;
		public const int LONG = 30;
		public const int ULONG = 31;
		public const int CHAR = 32;
		public const int FLOAT = 33;
		public const int DOUBLE = 34;
		public const int OBJECT = 35;
		public const int STRING = 36;
		public const int VOID = 37;
		public const int LBRACK = 38;
		public const int COMMA = 39;
		public const int RBRACK = 40;
		public const int LANGLE = 41;
		public const int RANGLE = 42;
		public const int REF = 43;
		public const int OUT = 44;
		public const int NEW = 45;
		public const int LPAREN = 46;
		public const int RPAREN = 47;
		public const int INC = 48;
		public const int DEC = 49;
		public const int THIS = 50;
		public const int BASE = 51;
		public const int TYPEOF = 52;
		public const int DEFAULT = 53;
		public const int DELEGATE = 54;
		public const int LMETA = 55;
		public const int RMETA = 56;
		public const int COLON = 57;
		public const int SEMI = 58;
		public const int QUESTION = 59;
		public const int PLUS = 60;
		public const int MINUS = 61;
		public const int LNOT = 62;
		public const int NOT = 63;
		public const int ESCAPE = 64;
		public const int AS = 65;
		public const int IS = 66;
		public const int LBRACE = 67;
		public const int RBRACE = 68;
		public const int TIMES = 69;
		public const int QUOT = 70;
		public const int REM = 71;
		public const int AND = 72;
		public const int OR = 73;
		public const int XOR = 74;
		public const int ASSIGN = 75;
		public const int COND = 76;
		public const int LAND = 77;
		public const int LOR = 78;
		public const int SHL = 79;
		public const int EQ = 80;
		public const int NE = 81;
		public const int LE = 82;
		public const int GE = 83;
		public const int PLUS_ASSIGN = 84;
		public const int MINUS_ASSIGN = 85;
		public const int TIMES_ASSIGN = 86;
		public const int QUOT_ASSIGN = 87;
		public const int REM_ASSIGN = 88;
		public const int AND_ASSIGN = 89;
		public const int OR_ASSIGN = 90;
		public const int XOR_ASSIGN = 91;
		public const int SHL_ASSIGN = 92;
		public const int SHR_ASSIGN = 93;
		public const int ARROW = 94;
		public const int SEMI_ASSIGN = 95;
		public const int SHR = 96;
		public const int LIFT = 97;
		public const int CONST = 98;
		public const int IF = 99;
		public const int ELSE = 100;
		public const int SWITCH = 101;
		public const int WHILE = 102;
		public const int DO = 103;
		public const int FOR = 104;
		public const int FOREACH = 105;
		public const int IN = 106;
		public const int BREAK = 107;
		public const int CONTINUE = 108;
		public const int GOTO = 109;
		public const int RETURN = 110;
		public const int THROW = 111;
		public const int TRY = 112;
		public const int CHECKED = 113;
		public const int UNCHECKED = 114;
		public const int LOCK = 115;
		public const int USING = 116;
		public const int TYPEIF = 117;
		public const int WITHTYPE = 118;
		public const int FORMEMBER = 119;
		public const int STATIC = 120;
		public const int CASE = 121;
		public const int CATCH = 122;
		public const int FINALLY = 123;
		public const int NAMESPACE = 124;
		public const int PUBLIC = 125;
		public const int PROTECTED = 126;
		public const int INTERNAL = 127;
		public const int PRIVATE = 128;
		public const int ABSTRACT = 129;
		public const int SEALED = 130;
		public const int READONLY = 131;
		public const int VOLATILE = 132;
		public const int VIRTUAL = 133;
		public const int OVERRIDE = 134;
		public const int EXTERN = 135;
		public const int CLASS = 136;
		public const int WHERE = 137;
		public const int STRUCT = 138;
		public const int OPERATOR = 139;
		public const int EVENT = 140;
		public const int IMPLICIT = 141;
		public const int EXPLICIT = 142;
		public const int TILDE = 143;
		public const int INTERFACE = 144;
		public const int GET = 145;
		public const int SET = 146;
		public const int ENUM = 147;
		public const int FIXED = 148;
		public const int PARAMS = 149;
		public const int SIZEOF = 150;
		public const int STACKALLOC = 151;
		public const int UNSAFE = 152;
		public const int NEW_LINE = 153;
		public const int WHITESPACE = 154;
		public const int COMMENT = 155;
		public const int SINGLE_LINE_COMMENT = 156;
		public const int DELIMITED_COMMENT = 157;
		public const int IDENTIFIER_OR_KEYWORD = 158;
		public const int LETTER_CHARACTER = 159;
		public const int DECIMAL_DIGIT_CHARACTER = 160;
		public const int NUMERIC_LITERAL = 161;
		public const int DECIMAL_DIGIT = 162;
		public const int HEX_DIGIT = 163;
		public const int LONG_SUFFIX = 164;
		public const int UNSIGNED_SUFFIX = 165;
		public const int UNSIGNED_LONG_SUFFIX = 166;
		public const int EXPONENT_PART = 167;
		public const int FLOAT_SUFFIX = 168;
		public const int DOUBLE_SUFFIX = 169;
		public const int DECIMAL_SUFFIX = 170;
		public const int SINGLE_CHARACTER = 171;
		
		
	void NotDone(IToken token, string msg)
	{
		throw new SemanticException(msg, token.getFilename(), token.getLine(), token.getColumn());
	}
	
	T ListReverse<T>(T list) where T : class, System.Collections.IList
	{
		if(list != null)
		{
			int n2 = list.Count / 2;
			int n1 = list.Count - 1;
			for(int i = 0; i < n2; i++)
			{
				object tmp = list[i];
				list[i] = list[n1 - i];
				list[n1 - i] = tmp;
			}
			return list;
		}
		else return null;
	}
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected P(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public P(TokenBuffer tokenBuf) : this(tokenBuf,1)
		{
		}
		
		protected P(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public P(TokenStream lexer) : this(lexer,1)
		{
		}
		
		public P(ParserSharedInputState state) : base(state,1)
		{
			initialize();
		}
		
	public Literal  literal() //throws RecognitionException, TokenStreamException
{
		Literal result;
		
		IToken  f = null;
		IToken  t = null;
		IToken  int32 = null;
		IToken  int64 = null;
		IToken  uInt32 = null;
		IToken  uInt64 = null;
		IToken  int32h = null;
		IToken  int64h = null;
		IToken  uInt32h = null;
		IToken  uInt64h = null;
		IToken  float32 = null;
		IToken  float64 = null;
		IToken  dec = null;
		IToken  chr = null;
		IToken  str = null;
		IToken  n = null;
		result = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case FALSE:
			{
				f = LT(1);
				match(FALSE);
				if (0==inputState.guessing)
				{
					result = new LitBool(f, false);
				}
				break;
			}
			case TRUE:
			{
				t = LT(1);
				match(TRUE);
				if (0==inputState.guessing)
				{
					result = new LitBool(t, true);
				}
				break;
			}
			case INT32_DECIMAL_INTEGER_LITERAL:
			{
				int32 = LT(1);
				match(INT32_DECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitInt(int32, int.Parse(int32.getText()));
				}
				break;
			}
			case INT64_DECIMAL_INTEGER_LITERAL:
			{
				int64 = LT(1);
				match(INT64_DECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitLong(int64, long.Parse(int64.getText()));
				}
				break;
			}
			case UINT32_DECIMAL_INTEGER_LITERAL:
			{
				uInt32 = LT(1);
				match(UINT32_DECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					NotDone(uInt32, "unsigned integer literals");
				}
				break;
			}
			case UINT64_DECIMAL_INTEGER_LITERAL:
			{
				uInt64 = LT(1);
				match(UINT64_DECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					NotDone(uInt64, "unsigned integer literals");
				}
				break;
			}
			case INT32_HEXADECIMAL_INTEGER_LITERAL:
			{
				int32h = LT(1);
				match(INT32_HEXADECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitInt(int32h, int.Parse(int32h.getText(), NumberStyles.AllowHexSpecifier));
				}
				break;
			}
			case INT64_HEXADECIMAL_INTEGER_LITERAL:
			{
				int64h = LT(1);
				match(INT64_HEXADECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitLong(int64h, long.Parse(int64h.getText(), NumberStyles.AllowHexSpecifier));
				}
				break;
			}
			case UINT32_HEXADECIMAL_INTEGER_LITERAL:
			{
				uInt32h = LT(1);
				match(UINT32_HEXADECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					NotDone(uInt32h, "unsigned integer literals");
				}
				break;
			}
			case UINT64_HEXADECIMAL_INTEGER_LITERAL:
			{
				uInt64h = LT(1);
				match(UINT64_HEXADECIMAL_INTEGER_LITERAL);
				if (0==inputState.guessing)
				{
					NotDone(uInt64h, "unsigned integer literals");
				}
				break;
			}
			case FLOAT32_REAL_LITERAL:
			{
				float32 = LT(1);
				match(FLOAT32_REAL_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitFloat(float32, float.Parse(float32.getText()));
				}
				break;
			}
			case FLOAT64_REAL_LITERAL:
			{
				float64 = LT(1);
				match(FLOAT64_REAL_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitDouble(float64, double.Parse(float64.getText()));
				}
				break;
			}
			case DECIMAL_REAL_LITERAL:
			{
				dec = LT(1);
				match(DECIMAL_REAL_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitDecimal(dec, decimal.Parse(dec.getText()));
				}
				break;
			}
			case CHARACTER_LITERAL:
			{
				chr = LT(1);
				match(CHARACTER_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitChar(chr, chr.getText()[0]);
				}
				break;
			}
			case STRING_LITERAL:
			{
				str = LT(1);
				match(STRING_LITERAL);
				if (0==inputState.guessing)
				{
					result = new LitString(str, str.getText());
				}
				break;
			}
			case NULL:
			{
				n = LT(1);
				match(NULL);
				if (0==inputState.guessing)
				{
					result = new LitNull(n);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_0_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Ident  identifier() //throws RecognitionException, TokenStreamException
{
		Ident result;
		
		IToken  i = null;
		result = null;
		
		try {      // for error handling
			i = LT(1);
			match(IDENTIFIER);
			if (0==inputState.guessing)
			{
				result = new Ident(i, i.getText());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_1_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Ident  namespace_name() //throws RecognitionException, TokenStreamException
{
		Ident result;
		
		IToken  i = null;
		IToken  j = null;
		result = null; StringBuilder sb = null;
		
		try {      // for error handling
			i = LT(1);
			match(IDENTIFIER);
			if (0==inputState.guessing)
			{
				sb = new StringBuilder(i.getText());
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						j = LT(1);
						match(IDENTIFIER);
						if (0==inputState.guessing)
						{
							sb.Append("."); sb.Append(j.getText());
						}
					}
					else
					{
						goto _loop5_breakloop;
					}
					
				}
_loop5_breakloop:				;
			}    // ( ... )*
			if (0==inputState.guessing)
			{
				result = new Ident(i, sb.ToString());
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_2_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public ClassType  type_name() //throws RecognitionException, TokenStreamException
{
		ClassType result;
		
		result = null; Ident name = null; List<Typ> typeArgs = null;
		
		try {      // for error handling
			name=identifier();
			{
				switch ( LA(1) )
				{
				case LANGLE:
				{
					typeArgs=type_argument_list();
					break;
				}
				case IDENTIFIER:
				case DOT:
				case LBRACK:
				case COMMA:
				case RANGLE:
				case LPAREN:
				case RPAREN:
				case THIS:
				case SEMI:
				case LBRACE:
				case WHERE:
				case OPERATOR:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				result = new ClassType(null, name, typeArgs); typeArgs = null;
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						name=identifier();
						{
							switch ( LA(1) )
							{
							case LANGLE:
							{
								typeArgs=type_argument_list();
								break;
							}
							case IDENTIFIER:
							case DOT:
							case LBRACK:
							case COMMA:
							case RANGLE:
							case LPAREN:
							case RPAREN:
							case THIS:
							case SEMI:
							case LBRACE:
							case WHERE:
							case OPERATOR:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						if (0==inputState.guessing)
						{
							result = new ClassType(result, name, typeArgs);  typeArgs = null;
						}
					}
					else
					{
						goto _loop10_breakloop;
					}
					
				}
_loop10_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Typ>  type_argument_list() //throws RecognitionException, TokenStreamException
{
		List<Typ> result;
		
		result = null; Typ typ = null;
		
		try {      // for error handling
			match(LANGLE);
			typ=type();
			if (0==inputState.guessing)
			{
				
						result = new List<Typ>(); 
						result.Add(typ); 
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						typ=type();
						if (0==inputState.guessing)
						{
							result.Add(typ);
						}
					}
					else
					{
						goto _loop22_breakloop;
					}
					
				}
_loop22_breakloop:				;
			}    // ( ... )*
			match(RANGLE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_4_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Typ  type() //throws RecognitionException, TokenStreamException
{
		Typ result;
		
		result = null; List<int> ranks = null;
		
		try {      // for error handling
			result=non_array_type();
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					ranks=rank_specifiers();
					if (0==inputState.guessing)
					{
						result = ArrayType.Create(result, ranks);
					}
					break;
				}
				case IDENTIFIER:
				case COMMA:
				case RANGLE:
				case LPAREN:
				case RPAREN:
				case THIS:
				case SEMI:
				case LBRACE:
				case WHERE:
				case OPERATOR:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_5_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Typ  non_array_type() //throws RecognitionException, TokenStreamException
{
		Typ result;
		
		result = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case IDENTIFIER:
			{
				result=type_name();
				break;
			}
			case BOOL:
			case DECIMAL:
			case SBYTE:
			case BYTE:
			case SHORT:
			case USHORT:
			case INT:
			case UINT:
			case LONG:
			case ULONG:
			case CHAR:
			case FLOAT:
			case DOUBLE:
			case OBJECT:
			case STRING:
			case VOID:
			{
				result=predefined_type();
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_3_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<int>  rank_specifiers() //throws RecognitionException, TokenStreamException
{
		List<int> result;
		
		result = null; int dims = 0;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<int>();
			}
			{ // ( ... )+
				int _cnt19=0;
				for (;;)
				{
					if ((LA(1)==LBRACK))
					{
						if (0==inputState.guessing)
						{
							dims = 1;
						}
						match(LBRACK);
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==COMMA))
								{
									match(COMMA);
									if (0==inputState.guessing)
									{
										dims++;
									}
								}
								else
								{
									goto _loop18_breakloop;
								}
								
							}
_loop18_breakloop:							;
						}    // ( ... )*
						match(RBRACK);
						if (0==inputState.guessing)
						{
							result.Add(dims);
						}
					}
					else
					{
						if (_cnt19 >= 1) { goto _loop19_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt19++;
				}
_loop19_breakloop:				;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_6_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public PrimType  predefined_type() //throws RecognitionException, TokenStreamException
{
		PrimType result;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  t4 = null;
		IToken  t5 = null;
		IToken  t6 = null;
		IToken  t7 = null;
		IToken  t8 = null;
		IToken  t9 = null;
		IToken  t10 = null;
		IToken  t11 = null;
		IToken  t12 = null;
		IToken  t13 = null;
		IToken  t14 = null;
		IToken  t15 = null;
		IToken  t16 = null;
		result = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case BOOL:
			{
				t1 = LT(1);
				match(BOOL);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateBool(t1);
				}
				break;
			}
			case DECIMAL:
			{
				t2 = LT(1);
				match(DECIMAL);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateDecimal(t2);
				}
				break;
			}
			case SBYTE:
			{
				t3 = LT(1);
				match(SBYTE);
				if (0==inputState.guessing)
				{
					NotDone(t3, "signed byte type");
				}
				break;
			}
			case BYTE:
			{
				t4 = LT(1);
				match(BYTE);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateByte(t4);
				}
				break;
			}
			case SHORT:
			{
				t5 = LT(1);
				match(SHORT);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateShort(t5);
				}
				break;
			}
			case USHORT:
			{
				t6 = LT(1);
				match(USHORT);
				if (0==inputState.guessing)
				{
					NotDone(t6, "unsigned short type");
				}
				break;
			}
			case INT:
			{
				t7 = LT(1);
				match(INT);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateInt(t7);
				}
				break;
			}
			case UINT:
			{
				t8 = LT(1);
				match(UINT);
				if (0==inputState.guessing)
				{
					NotDone(t8, "unsigned int type");
				}
				break;
			}
			case LONG:
			{
				t9 = LT(1);
				match(LONG);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateLong(t9);
				}
				break;
			}
			case ULONG:
			{
				t10 = LT(1);
				match(ULONG);
				if (0==inputState.guessing)
				{
					NotDone(t10, "unsigned long type");
				}
				break;
			}
			case CHAR:
			{
				t11 = LT(1);
				match(CHAR);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateChar(t11);
				}
				break;
			}
			case FLOAT:
			{
				t12 = LT(1);
				match(FLOAT);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateFloat(t12);
				}
				break;
			}
			case DOUBLE:
			{
				t13 = LT(1);
				match(DOUBLE);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateDouble(t13);
				}
				break;
			}
			case OBJECT:
			{
				t14 = LT(1);
				match(OBJECT);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateObject(t14);
				}
				break;
			}
			case STRING:
			{
				t15 = LT(1);
				match(STRING);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateString(t15);
				}
				break;
			}
			case VOID:
			{
				t16 = LT(1);
				match(VOID);
				if (0==inputState.guessing)
				{
					result = PrimType.CreateVoid(t16);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_7_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Arg>  argument_list() //throws RecognitionException, TokenStreamException
{
		List<Arg> result;
		
		result = null; Arg arg = null;
		
		try {      // for error handling
			arg=argument();
			if (0==inputState.guessing)
			{
				
						result = new List<Arg>(); 
						result.Add(arg); 
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						arg=argument();
						if (0==inputState.guessing)
						{
							result.Add(arg);
						}
					}
					else
					{
						goto _loop25_breakloop;
					}
					
				}
_loop25_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_8_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Arg  argument() //throws RecognitionException, TokenStreamException
{
		Arg result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case FALSE:
			case TRUE:
			case INT32_DECIMAL_INTEGER_LITERAL:
			case INT64_DECIMAL_INTEGER_LITERAL:
			case UINT32_DECIMAL_INTEGER_LITERAL:
			case UINT64_DECIMAL_INTEGER_LITERAL:
			case INT32_HEXADECIMAL_INTEGER_LITERAL:
			case INT64_HEXADECIMAL_INTEGER_LITERAL:
			case UINT32_HEXADECIMAL_INTEGER_LITERAL:
			case UINT64_HEXADECIMAL_INTEGER_LITERAL:
			case FLOAT32_REAL_LITERAL:
			case FLOAT64_REAL_LITERAL:
			case DECIMAL_REAL_LITERAL:
			case CHARACTER_LITERAL:
			case STRING_LITERAL:
			case NULL:
			case IDENTIFIER:
			case BOOL:
			case DECIMAL:
			case SBYTE:
			case BYTE:
			case SHORT:
			case USHORT:
			case INT:
			case UINT:
			case LONG:
			case ULONG:
			case CHAR:
			case FLOAT:
			case DOUBLE:
			case OBJECT:
			case STRING:
			case VOID:
			case NEW:
			case LPAREN:
			case INC:
			case DEC:
			case THIS:
			case BASE:
			case TYPEOF:
			case DEFAULT:
			case DELEGATE:
			case LMETA:
			case PLUS:
			case MINUS:
			case LNOT:
			case NOT:
			case ESCAPE:
			case LIFT:
			{
				expr=expression();
				if (0==inputState.guessing)
				{
					result = new Arg(Dir.In, expr);
				}
				break;
			}
			case REF:
			{
				match(REF);
				expr=primary_expression();
				if (0==inputState.guessing)
				{
					result = new Arg(Dir.InOut, expr);
				}
				break;
			}
			case OUT:
			{
				match(OUT);
				expr=primary_expression();
				if (0==inputState.guessing)
				{
					result = new Arg(Dir.Out, expr);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_9_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		IToken  t = null;
		result = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case LIFT:
			{
				t = LT(1);
				match(LIFT);
				result=assignment_expression();
				if (0==inputState.guessing)
				{
					result = new Lift(t, result);
				}
				break;
			}
			case FALSE:
			case TRUE:
			case INT32_DECIMAL_INTEGER_LITERAL:
			case INT64_DECIMAL_INTEGER_LITERAL:
			case UINT32_DECIMAL_INTEGER_LITERAL:
			case UINT64_DECIMAL_INTEGER_LITERAL:
			case INT32_HEXADECIMAL_INTEGER_LITERAL:
			case INT64_HEXADECIMAL_INTEGER_LITERAL:
			case UINT32_HEXADECIMAL_INTEGER_LITERAL:
			case UINT64_HEXADECIMAL_INTEGER_LITERAL:
			case FLOAT32_REAL_LITERAL:
			case FLOAT64_REAL_LITERAL:
			case DECIMAL_REAL_LITERAL:
			case CHARACTER_LITERAL:
			case STRING_LITERAL:
			case NULL:
			case IDENTIFIER:
			case BOOL:
			case DECIMAL:
			case SBYTE:
			case BYTE:
			case SHORT:
			case USHORT:
			case INT:
			case UINT:
			case LONG:
			case ULONG:
			case CHAR:
			case FLOAT:
			case DOUBLE:
			case OBJECT:
			case STRING:
			case VOID:
			case NEW:
			case LPAREN:
			case INC:
			case DEC:
			case THIS:
			case BASE:
			case TYPEOF:
			case DEFAULT:
			case DELEGATE:
			case LMETA:
			case PLUS:
			case MINUS:
			case LNOT:
			case NOT:
			case ESCAPE:
			{
				result=assignment_expression();
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_10_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  primary_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		IToken  t1 = null;
		result = null; Typ typ = null; List<Expr> exprs = null; List<int> ranks = null; int dims = 0; Ident name = null; List<Typ> typeArgs = null; List<Arg> args = null;
		
		try {      // for error handling
			bool synPredMatched32 = false;
			if (((LA(1)==NEW)))
			{
				int _m32 = mark();
				synPredMatched32 = true;
				inputState.guessing++;
				try {
					{
						match(NEW);
						non_array_type();
						match(LBRACK);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched32 = false;
				}
				rewind(_m32);
				inputState.guessing--;
			}
			if ( synPredMatched32 )
			{
				t1 = LT(1);
				match(NEW);
				typ=non_array_type();
				match(LBRACK);
				{
					switch ( LA(1) )
					{
					case FALSE:
					case TRUE:
					case INT32_DECIMAL_INTEGER_LITERAL:
					case INT64_DECIMAL_INTEGER_LITERAL:
					case UINT32_DECIMAL_INTEGER_LITERAL:
					case UINT64_DECIMAL_INTEGER_LITERAL:
					case INT32_HEXADECIMAL_INTEGER_LITERAL:
					case INT64_HEXADECIMAL_INTEGER_LITERAL:
					case UINT32_HEXADECIMAL_INTEGER_LITERAL:
					case UINT64_HEXADECIMAL_INTEGER_LITERAL:
					case FLOAT32_REAL_LITERAL:
					case FLOAT64_REAL_LITERAL:
					case DECIMAL_REAL_LITERAL:
					case CHARACTER_LITERAL:
					case STRING_LITERAL:
					case NULL:
					case IDENTIFIER:
					case BOOL:
					case DECIMAL:
					case SBYTE:
					case BYTE:
					case SHORT:
					case USHORT:
					case INT:
					case UINT:
					case LONG:
					case ULONG:
					case CHAR:
					case FLOAT:
					case DOUBLE:
					case OBJECT:
					case STRING:
					case VOID:
					case NEW:
					case LPAREN:
					case INC:
					case DEC:
					case THIS:
					case BASE:
					case TYPEOF:
					case DEFAULT:
					case DELEGATE:
					case LMETA:
					case PLUS:
					case MINUS:
					case LNOT:
					case NOT:
					case ESCAPE:
					case LIFT:
					{
						if (0==inputState.guessing)
						{
							ranks = new List<int>();
						}
						exprs=expression_list();
						if (0==inputState.guessing)
						{
							ranks.Add(exprs.Count);
						}
						match(RBRACK);
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==LBRACK))
								{
									if (0==inputState.guessing)
									{
										dims = 1;
									}
									match(LBRACK);
									{    // ( ... )*
										for (;;)
										{
											if ((LA(1)==COMMA))
											{
												match(COMMA);
												if (0==inputState.guessing)
												{
													dims++;
												}
											}
											else
											{
												goto _loop36_breakloop;
											}
											
										}
_loop36_breakloop:										;
									}    // ( ... )*
									match(RBRACK);
									if (0==inputState.guessing)
									{
										ranks.Add(dims);
									}
								}
								else
								{
									goto _loop37_breakloop;
								}
								
							}
_loop37_breakloop:							;
						}    // ( ... )*
						if (0==inputState.guessing)
						{
							result = new ArrayCreation(t1, ArrayType.Create(typ, ranks), exprs);
						}
						break;
					}
					case COMMA:
					case RBRACK:
					{
						if (0==inputState.guessing)
						{
							dims = 1;
						}
						{    // ( ... )*
							for (;;)
							{
								if ((LA(1)==COMMA))
								{
									match(COMMA);
									if (0==inputState.guessing)
									{
										dims++;
									}
								}
								else
								{
									goto _loop39_breakloop;
								}
								
							}
_loop39_breakloop:							;
						}    // ( ... )*
						match(RBRACK);
						exprs=array_initializer();
						if (0==inputState.guessing)
						{
							result = new ArrayCreation(t1, new ArrayType(typ, dims), new ArrayInitialisation(exprs));
						}
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
			}
			else if ((tokenSet_11_.member(LA(1)))) {
				result=primary_expression_start();
				{    // ( ... )*
					for (;;)
					{
						switch ( LA(1) )
						{
						case DOT:
						{
							match(DOT);
							name=identifier();
							typeArgs=type_argument_list_opt();
							if (0==inputState.guessing)
							{
								result = new MemberAccess(result, name, typeArgs);
							}
							break;
						}
						case LPAREN:
						{
							if (0==inputState.guessing)
							{
								args = null;
							}
							match(LPAREN);
							{
								switch ( LA(1) )
								{
								case FALSE:
								case TRUE:
								case INT32_DECIMAL_INTEGER_LITERAL:
								case INT64_DECIMAL_INTEGER_LITERAL:
								case UINT32_DECIMAL_INTEGER_LITERAL:
								case UINT64_DECIMAL_INTEGER_LITERAL:
								case INT32_HEXADECIMAL_INTEGER_LITERAL:
								case INT64_HEXADECIMAL_INTEGER_LITERAL:
								case UINT32_HEXADECIMAL_INTEGER_LITERAL:
								case UINT64_HEXADECIMAL_INTEGER_LITERAL:
								case FLOAT32_REAL_LITERAL:
								case FLOAT64_REAL_LITERAL:
								case DECIMAL_REAL_LITERAL:
								case CHARACTER_LITERAL:
								case STRING_LITERAL:
								case NULL:
								case IDENTIFIER:
								case BOOL:
								case DECIMAL:
								case SBYTE:
								case BYTE:
								case SHORT:
								case USHORT:
								case INT:
								case UINT:
								case LONG:
								case ULONG:
								case CHAR:
								case FLOAT:
								case DOUBLE:
								case OBJECT:
								case STRING:
								case VOID:
								case REF:
								case OUT:
								case NEW:
								case LPAREN:
								case INC:
								case DEC:
								case THIS:
								case BASE:
								case TYPEOF:
								case DEFAULT:
								case DELEGATE:
								case LMETA:
								case PLUS:
								case MINUS:
								case LNOT:
								case NOT:
								case ESCAPE:
								case LIFT:
								{
									args=argument_list();
									break;
								}
								case RPAREN:
								{
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
							match(RPAREN);
							if (0==inputState.guessing)
							{
								result = new Invocation(result, args);
							}
							break;
						}
						case LBRACK:
						{
							match(LBRACK);
							exprs=expression_list();
							match(RBRACK);
							if (0==inputState.guessing)
							{
								result = new ElementAccess(result, exprs);
							}
							break;
						}
						case INC:
						{
							match(INC);
							if (0==inputState.guessing)
							{
								result = new PostIncrement(result);
							}
							break;
						}
						case DEC:
						{
							match(DEC);
							if (0==inputState.guessing)
							{
								result = new PostDecrement(result);
							}
							break;
						}
						default:
						{
							goto _loop42_breakloop;
						}
						 }
					}
_loop42_breakloop:					;
				}    // ( ... )*
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Expr>  expression_list() //throws RecognitionException, TokenStreamException
{
		List<Expr> result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			expr=expression();
			if (0==inputState.guessing)
			{
				
						result = new List<Expr>(); 
						result.Add(expr); 
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						expr=expression();
						if (0==inputState.guessing)
						{
							result.Add(expr);
						}
					}
					else
					{
						goto _loop29_breakloop;
					}
					
				}
_loop29_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_13_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Expr>  array_initializer() //throws RecognitionException, TokenStreamException
{
		List<Expr> result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Expr>();
			}
			match(LBRACE);
			{
				switch ( LA(1) )
				{
				case FALSE:
				case TRUE:
				case INT32_DECIMAL_INTEGER_LITERAL:
				case INT64_DECIMAL_INTEGER_LITERAL:
				case UINT32_DECIMAL_INTEGER_LITERAL:
				case UINT64_DECIMAL_INTEGER_LITERAL:
				case INT32_HEXADECIMAL_INTEGER_LITERAL:
				case INT64_HEXADECIMAL_INTEGER_LITERAL:
				case UINT32_HEXADECIMAL_INTEGER_LITERAL:
				case UINT64_HEXADECIMAL_INTEGER_LITERAL:
				case FLOAT32_REAL_LITERAL:
				case FLOAT64_REAL_LITERAL:
				case DECIMAL_REAL_LITERAL:
				case CHARACTER_LITERAL:
				case STRING_LITERAL:
				case NULL:
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case NEW:
				case LPAREN:
				case INC:
				case DEC:
				case THIS:
				case BASE:
				case TYPEOF:
				case DEFAULT:
				case DELEGATE:
				case LMETA:
				case PLUS:
				case MINUS:
				case LNOT:
				case NOT:
				case ESCAPE:
				case LBRACE:
				case LIFT:
				{
					expr=variable_initializer();
					if (0==inputState.guessing)
					{
						result.Add(expr);
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								expr=variable_initializer();
								if (0==inputState.guessing)
								{
									result.Add(expr);
								}
							}
							else
							{
								goto _loop236_breakloop;
							}
							
						}
_loop236_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  primary_expression_start() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  t4 = null;
		IToken  t5 = null;
		IToken  t6 = null;
		IToken  t7 = null;
		IToken  t8 = null;
		result = null; List<Typ> typeArgs = null; Typ typ = null; Ident name = null; List<Arg> args = null; List<Expr> exprs = null; List<Param> pars = null; List<Stmt> stmts = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case FALSE:
			case TRUE:
			case INT32_DECIMAL_INTEGER_LITERAL:
			case INT64_DECIMAL_INTEGER_LITERAL:
			case UINT32_DECIMAL_INTEGER_LITERAL:
			case UINT64_DECIMAL_INTEGER_LITERAL:
			case INT32_HEXADECIMAL_INTEGER_LITERAL:
			case INT64_HEXADECIMAL_INTEGER_LITERAL:
			case UINT32_HEXADECIMAL_INTEGER_LITERAL:
			case UINT64_HEXADECIMAL_INTEGER_LITERAL:
			case FLOAT32_REAL_LITERAL:
			case FLOAT64_REAL_LITERAL:
			case DECIMAL_REAL_LITERAL:
			case CHARACTER_LITERAL:
			case STRING_LITERAL:
			case NULL:
			{
				result=literal();
				break;
			}
			case IDENTIFIER:
			{
				name=identifier();
				typeArgs=type_argument_list_opt();
				if (0==inputState.guessing)
				{
					result = new Name(name, typeArgs);
				}
				break;
			}
			case LPAREN:
			{
				t1 = LT(1);
				match(LPAREN);
				result=expression();
				match(RPAREN);
				if (0==inputState.guessing)
				{
					result = new Parentheses(t1, result);
				}
				break;
			}
			case BOOL:
			case DECIMAL:
			case SBYTE:
			case BYTE:
			case SHORT:
			case USHORT:
			case INT:
			case UINT:
			case LONG:
			case ULONG:
			case CHAR:
			case FLOAT:
			case DOUBLE:
			case OBJECT:
			case STRING:
			case VOID:
			{
				typ=predefined_type();
				match(DOT);
				name=identifier();
				typeArgs=type_argument_list_opt();
				if (0==inputState.guessing)
				{
					result = new MemberAccess(typ, name, typeArgs);
				}
				break;
			}
			case THIS:
			{
				t2 = LT(1);
				match(THIS);
				if (0==inputState.guessing)
				{
					result = new This(t2);
				}
				break;
			}
			case BASE:
			{
				t3 = LT(1);
				match(BASE);
				match(DOT);
				{
					switch ( LA(1) )
					{
					case IDENTIFIER:
					{
						name=identifier();
						typeArgs=type_argument_list_opt();
						if (0==inputState.guessing)
						{
							result = new BaseMember(t3, name, typeArgs);
						}
						break;
					}
					case LBRACK:
					{
						match(LBRACK);
						exprs=expression_list();
						match(RBRACK);
						if (0==inputState.guessing)
						{
							result = new BaseIndexer(t3, exprs);
						}
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				break;
			}
			case NEW:
			{
				t4 = LT(1);
				match(NEW);
				typ=non_array_type();
				match(LPAREN);
				{
					switch ( LA(1) )
					{
					case FALSE:
					case TRUE:
					case INT32_DECIMAL_INTEGER_LITERAL:
					case INT64_DECIMAL_INTEGER_LITERAL:
					case UINT32_DECIMAL_INTEGER_LITERAL:
					case UINT64_DECIMAL_INTEGER_LITERAL:
					case INT32_HEXADECIMAL_INTEGER_LITERAL:
					case INT64_HEXADECIMAL_INTEGER_LITERAL:
					case UINT32_HEXADECIMAL_INTEGER_LITERAL:
					case UINT64_HEXADECIMAL_INTEGER_LITERAL:
					case FLOAT32_REAL_LITERAL:
					case FLOAT64_REAL_LITERAL:
					case DECIMAL_REAL_LITERAL:
					case CHARACTER_LITERAL:
					case STRING_LITERAL:
					case NULL:
					case IDENTIFIER:
					case BOOL:
					case DECIMAL:
					case SBYTE:
					case BYTE:
					case SHORT:
					case USHORT:
					case INT:
					case UINT:
					case LONG:
					case ULONG:
					case CHAR:
					case FLOAT:
					case DOUBLE:
					case OBJECT:
					case STRING:
					case VOID:
					case REF:
					case OUT:
					case NEW:
					case LPAREN:
					case INC:
					case DEC:
					case THIS:
					case BASE:
					case TYPEOF:
					case DEFAULT:
					case DELEGATE:
					case LMETA:
					case PLUS:
					case MINUS:
					case LNOT:
					case NOT:
					case ESCAPE:
					case LIFT:
					{
						args=argument_list();
						break;
					}
					case RPAREN:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(RPAREN);
				if (0==inputState.guessing)
				{
					result = new ObjectCreation(t4, typ, args);
				}
				break;
			}
			case TYPEOF:
			{
				t5 = LT(1);
				match(TYPEOF);
				match(LPAREN);
				typ=type();
				match(RPAREN);
				if (0==inputState.guessing)
				{
					result = new TypeOf(t5, typ);
				}
				break;
			}
			case DEFAULT:
			{
				t6 = LT(1);
				match(DEFAULT);
				match(LPAREN);
				typ=type();
				match(RPAREN);
				if (0==inputState.guessing)
				{
					result = new DefaultOf(t6, typ);
				}
				break;
			}
			case DELEGATE:
			{
				t7 = LT(1);
				match(DELEGATE);
				{
					switch ( LA(1) )
					{
					case IDENTIFIER:
					{
						name=identifier();
						break;
					}
					case LPAREN:
					case LBRACE:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				{
					switch ( LA(1) )
					{
					case LPAREN:
					{
						match(LPAREN);
						{
							switch ( LA(1) )
							{
							case IDENTIFIER:
							case BOOL:
							case DECIMAL:
							case SBYTE:
							case BYTE:
							case SHORT:
							case USHORT:
							case INT:
							case UINT:
							case LONG:
							case ULONG:
							case CHAR:
							case FLOAT:
							case DOUBLE:
							case OBJECT:
							case STRING:
							case VOID:
							case REF:
							case OUT:
							{
								pars=formal_parameter_list();
								break;
							}
							case RPAREN:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						match(RPAREN);
						break;
					}
					case LBRACE:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				stmts=block();
				if (0==inputState.guessing)
				{
					result = new Function(t7, name, pars, stmts);
				}
				break;
			}
			case LMETA:
			{
				t8 = LT(1);
				match(LMETA);
				stmts=quoted_statement(ref result);
				match(RMETA);
				if (0==inputState.guessing)
				{
					result = new Bracket(t8, ListReverse(stmts), result);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_14_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	protected List<Typ>  type_argument_list_opt() //throws RecognitionException, TokenStreamException
{
		List<Typ> result;
		
		result = null;
		
		try {      // for error handling
			bool synPredMatched52 = false;
			if (((LA(1)==LANGLE)))
			{
				int _m52 = mark();
				synPredMatched52 = true;
				inputState.guessing++;
				try {
					{
						type_argument_list();
						{
							switch ( LA(1) )
							{
							case LPAREN:
							{
								match(LPAREN);
								break;
							}
							case RPAREN:
							{
								match(RPAREN);
								break;
							}
							case RBRACK:
							{
								match(RBRACK);
								break;
							}
							case RANGLE:
							{
								match(RANGLE);
								break;
							}
							case COLON:
							{
								match(COLON);
								break;
							}
							case SEMI:
							{
								match(SEMI);
								break;
							}
							case COMMA:
							{
								match(COMMA);
								break;
							}
							case DOT:
							{
								match(DOT);
								break;
							}
							case QUESTION:
							{
								match(QUESTION);
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
				}
				catch (RecognitionException)
				{
					synPredMatched52 = false;
				}
				rewind(_m52);
				inputState.guessing--;
			}
			if ( synPredMatched52 )
			{
				result=type_argument_list();
			}
			else if ((tokenSet_14_.member(LA(1)))) {
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_14_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Param>  formal_parameter_list() //throws RecognitionException, TokenStreamException
{
		List<Param> result;
		
		result = null; Param par = null;
		
		try {      // for error handling
			par=fixed_parameter();
			if (0==inputState.guessing)
			{
				result = new List<Param>(); result.Add(par);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						par=fixed_parameter();
						if (0==inputState.guessing)
						{
							result.Add(par);
						}
					}
					else
					{
						goto _loop225_breakloop;
					}
					
				}
_loop225_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_15_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Stmt>  block() //throws RecognitionException, TokenStreamException
{
		List<Stmt> result;
		
		IToken  t1 = null;
		result = null; Ident name = null; Typ typ = null; List<Collections.Tuple<Ident, Expr>> vars = null; Stmt stmt = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Stmt>();
			}
			match(LBRACE);
			{    // ( ... )*
				for (;;)
				{
					bool synPredMatched106 = false;
					if (((LA(1)==IDENTIFIER)))
					{
						int _m106 = mark();
						synPredMatched106 = true;
						inputState.guessing++;
						try {
							{
								match(IDENTIFIER);
								match(COLON);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched106 = false;
						}
						rewind(_m106);
						inputState.guessing--;
					}
					if ( synPredMatched106 )
					{
						name=identifier();
						match(COLON);
						if (0==inputState.guessing)
						{
							result.Add(new Label(name));
						}
					}
					else {
						bool synPredMatched108 = false;
						if (((tokenSet_16_.member(LA(1)))))
						{
							int _m108 = mark();
							synPredMatched108 = true;
							inputState.guessing++;
							try {
								{
									type();
									match(IDENTIFIER);
								}
							}
							catch (RecognitionException)
							{
								synPredMatched108 = false;
							}
							rewind(_m108);
							inputState.guessing--;
						}
						if ( synPredMatched108 )
						{
							typ=type();
							vars=variable_declarators();
							match(SEMI);
							if (0==inputState.guessing)
							{
								LocalDecl.Add(result, typ, vars);
							}
						}
						else if ((LA(1)==CONST)) {
							t1 = LT(1);
							match(CONST);
							typ=type();
							vars=variable_declarators();
							match(SEMI);
							if (0==inputState.guessing)
							{
								ConstDecl.Add(result, t1, typ, vars);
							}
						}
						else if ((tokenSet_17_.member(LA(1)))) {
							stmt=embedded_statement();
							if (0==inputState.guessing)
							{
								result.Add(stmt);
							}
						}
						else
						{
							goto _loop109_breakloop;
						}
						}
					}
_loop109_breakloop:					;
				}    // ( ... )*
				match(RBRACE);
			}
			catch (RecognitionException ex)
			{
				if (0 == inputState.guessing)
				{
					reportError(ex);
					recover(ex,tokenSet_18_);
				}
				else
				{
					throw ex;
				}
			}
			return result;
		}
		
	public List<Stmt>  quoted_statement(
		ref Expr expr
	) //throws RecognitionException, TokenStreamException
{
		List<Stmt> result;
		
		IToken  t1 = null;
		result = new List<Stmt>(); List<Stmt> stmts = null; Stmt stmt = null; Ident name = null; Typ typ = null; List<Collections.Tuple<Ident, Expr>> vars = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case RMETA:
			{
				break;
			}
			case CONST:
			{
				t1 = LT(1);
				match(CONST);
				typ=type();
				vars=variable_declarators();
				match(SEMI);
				result=quoted_statement(ref expr);
				if (0==inputState.guessing)
				{
					ConstDecl.Add(result, t1, typ, vars);
				}
				break;
			}
			case LBRACE:
			{
				stmts=block();
				result=quoted_statement(ref expr);
				if (0==inputState.guessing)
				{
					result.Add(new Block(stmts));
				}
				break;
			}
			case SEMI:
			case IF:
			case SWITCH:
			case WHILE:
			case DO:
			case FOR:
			case FOREACH:
			case BREAK:
			case CONTINUE:
			case GOTO:
			case RETURN:
			case THROW:
			case TRY:
			case CHECKED:
			case UNCHECKED:
			case LOCK:
			case USING:
			case TYPEIF:
			case WITHTYPE:
			case FORMEMBER:
			{
				stmt=actual_statement();
				result=quoted_statement(ref expr);
				if (0==inputState.guessing)
				{
					result.Add(stmt);
				}
				break;
			}
			default:
				bool synPredMatched115 = false;
				if (((LA(1)==IDENTIFIER)))
				{
					int _m115 = mark();
					synPredMatched115 = true;
					inputState.guessing++;
					try {
						{
							match(IDENTIFIER);
							match(COLON);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched115 = false;
					}
					rewind(_m115);
					inputState.guessing--;
				}
				if ( synPredMatched115 )
				{
					name=identifier();
					match(COLON);
					result=quoted_statement(ref expr);
					if (0==inputState.guessing)
					{
						result.Add(new Label(name));
					}
				}
				else {
					bool synPredMatched117 = false;
					if (((tokenSet_16_.member(LA(1)))))
					{
						int _m117 = mark();
						synPredMatched117 = true;
						inputState.guessing++;
						try {
							{
								type();
								match(IDENTIFIER);
							}
						}
						catch (RecognitionException)
						{
							synPredMatched117 = false;
						}
						rewind(_m117);
						inputState.guessing--;
					}
					if ( synPredMatched117 )
					{
						typ=type();
						vars=variable_declarators();
						match(SEMI);
						result=quoted_statement(ref expr);
						if (0==inputState.guessing)
						{
							LocalDecl.Add(result, typ, vars);
						}
					}
					else if ((tokenSet_19_.member(LA(1)))) {
						expr=expression();
						{
							switch ( LA(1) )
							{
							case SEMI:
							{
								match(SEMI);
								if (0==inputState.guessing)
								{
									stmt = new Expression(expr); expr = null;
								}
								result=quoted_statement(ref expr);
								if (0==inputState.guessing)
								{
									result.Add(stmt);
								}
								break;
							}
							case RMETA:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
					}
				else
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				}break; }
			}
			catch (RecognitionException ex)
			{
				if (0 == inputState.guessing)
				{
					reportError(ex);
					recover(ex,tokenSet_20_);
				}
				else
				{
					throw ex;
				}
			}
			return result;
		}
		
	public Expr  unary_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  t4 = null;
		IToken  t5 = null;
		IToken  t6 = null;
		IToken  t7 = null;
		IToken  t8 = null;
		result = null; List<Stmt> stmts = null; Ident name = null; Typ typ = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case PLUS:
			{
				t1 = LT(1);
				match(PLUS);
				result=unary_expression();
				if (0==inputState.guessing)
				{
					result = new Plus(t1, result);
				}
				break;
			}
			case MINUS:
			{
				t2 = LT(1);
				match(MINUS);
				result=unary_expression();
				if (0==inputState.guessing)
				{
					result = new Negate(t2, result);
				}
				break;
			}
			case LNOT:
			{
				t3 = LT(1);
				match(LNOT);
				result=unary_expression();
				if (0==inputState.guessing)
				{
					result = new Not(t3, result);
				}
				break;
			}
			case NOT:
			{
				t4 = LT(1);
				match(NOT);
				result=unary_expression();
				if (0==inputState.guessing)
				{
					result = new Complement(t4, result);
				}
				break;
			}
			case INC:
			{
				t5 = LT(1);
				match(INC);
				result=unary_expression();
				if (0==inputState.guessing)
				{
					result = new PreIncrement(t5, result);
				}
				break;
			}
			case DEC:
			{
				t6 = LT(1);
				match(DEC);
				result=unary_expression();
				if (0==inputState.guessing)
				{
					result = new PreDecrement(t6, result);
				}
				break;
			}
			case ESCAPE:
			{
				t7 = LT(1);
				match(ESCAPE);
				{
					result=unary_expression();
					if (0==inputState.guessing)
					{
						result = new EscapeExpr(t7, result);
					}
				}
				break;
			}
			default:
				bool synPredMatched56 = false;
				if (((tokenSet_11_.member(LA(1)))))
				{
					int _m56 = mark();
					synPredMatched56 = true;
					inputState.guessing++;
					try {
						{
							match(LPAREN);
							type();
							match(RPAREN);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched56 = false;
					}
					rewind(_m56);
					inputState.guessing--;
				}
				if ( synPredMatched56 )
				{
					{
						bool synPredMatched60 = false;
						if (((tokenSet_11_.member(LA(1)))))
						{
							int _m60 = mark();
							synPredMatched60 = true;
							inputState.guessing++;
							try {
								{
									match(LPAREN);
									expression();
									match(RPAREN);
									{
										switch ( LA(1) )
										{
										case AS:
										{
											match(AS);
											break;
										}
										case IS:
										{
											match(IS);
											break;
										}
										case LBRACE:
										{
											match(LBRACE);
											break;
										}
										case RBRACE:
										{
											match(RBRACE);
											break;
										}
										case LBRACK:
										{
											match(LBRACK);
											break;
										}
										case RBRACK:
										{
											match(RBRACK);
											break;
										}
										case RPAREN:
										{
											match(RPAREN);
											break;
										}
										case DOT:
										{
											match(DOT);
											break;
										}
										case COMMA:
										{
											match(COMMA);
											break;
										}
										case COLON:
										{
											match(COLON);
											break;
										}
										case SEMI:
										{
											match(SEMI);
											break;
										}
										case PLUS:
										{
											match(PLUS);
											break;
										}
										case MINUS:
										{
											match(MINUS);
											break;
										}
										case TIMES:
										{
											match(TIMES);
											break;
										}
										case QUOT:
										{
											match(QUOT);
											break;
										}
										case REM:
										{
											match(REM);
											break;
										}
										case AND:
										{
											match(AND);
											break;
										}
										case OR:
										{
											match(OR);
											break;
										}
										case XOR:
										{
											match(XOR);
											break;
										}
										case ASSIGN:
										{
											match(ASSIGN);
											break;
										}
										case LANGLE:
										{
											match(LANGLE);
											break;
										}
										case RANGLE:
										{
											match(RANGLE);
											break;
										}
										case COND:
										{
											match(COND);
											break;
										}
										case INC:
										{
											match(INC);
											break;
										}
										case DEC:
										{
											match(DEC);
											break;
										}
										case LAND:
										{
											match(LAND);
											break;
										}
										case LOR:
										{
											match(LOR);
											break;
										}
										case SHL:
										{
											match(SHL);
											break;
										}
										case EQ:
										{
											match(EQ);
											break;
										}
										case NE:
										{
											match(NE);
											break;
										}
										case LE:
										{
											match(LE);
											break;
										}
										case GE:
										{
											match(GE);
											break;
										}
										case PLUS_ASSIGN:
										{
											match(PLUS_ASSIGN);
											break;
										}
										case MINUS_ASSIGN:
										{
											match(MINUS_ASSIGN);
											break;
										}
										case TIMES_ASSIGN:
										{
											match(TIMES_ASSIGN);
											break;
										}
										case QUOT_ASSIGN:
										{
											match(QUOT_ASSIGN);
											break;
										}
										case REM_ASSIGN:
										{
											match(REM_ASSIGN);
											break;
										}
										case AND_ASSIGN:
										{
											match(AND_ASSIGN);
											break;
										}
										case OR_ASSIGN:
										{
											match(OR_ASSIGN);
											break;
										}
										case XOR_ASSIGN:
										{
											match(XOR_ASSIGN);
											break;
										}
										case SHL_ASSIGN:
										{
											match(SHL_ASSIGN);
											break;
										}
										case SHR_ASSIGN:
										{
											match(SHR_ASSIGN);
											break;
										}
										case ARROW:
										{
											match(ARROW);
											break;
										}
										case RMETA:
										{
											match(RMETA);
											break;
										}
										case SEMI_ASSIGN:
										{
											match(SEMI_ASSIGN);
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
								}
							}
							catch (RecognitionException)
							{
								synPredMatched60 = false;
							}
							rewind(_m60);
							inputState.guessing--;
						}
						if ( synPredMatched60 )
						{
							result=primary_expression();
						}
						else if ((LA(1)==LPAREN)) {
							t8 = LT(1);
							match(LPAREN);
							typ=type();
							match(RPAREN);
							result=unary_expression();
							if (0==inputState.guessing)
							{
								result = new Cast(t8, typ, result);
							}
						}
						else
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						
					}
				}
				else if ((tokenSet_11_.member(LA(1)))) {
					result=primary_expression();
				}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			break; }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_12_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  multiplicative_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=unary_expression();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case TIMES:
					{
						match(TIMES);
						expr=unary_expression();
						if (0==inputState.guessing)
						{
							result = new Multiply(result, expr);
						}
						break;
					}
					case QUOT:
					{
						match(QUOT);
						expr=unary_expression();
						if (0==inputState.guessing)
						{
							result = new Divide(result, expr);
						}
						break;
					}
					case REM:
					{
						match(REM);
						expr=unary_expression();
						if (0==inputState.guessing)
						{
							result = new Remainder(result, expr);
						}
						break;
					}
					default:
					{
						goto _loop63_breakloop;
					}
					 }
				}
_loop63_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_21_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  additive_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=multiplicative_expression();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case PLUS:
					{
						match(PLUS);
						expr=multiplicative_expression();
						if (0==inputState.guessing)
						{
							result = new Add(result, expr);
						}
						break;
					}
					case MINUS:
					{
						match(MINUS);
						expr=multiplicative_expression();
						if (0==inputState.guessing)
						{
							result = new Subtract(result, expr);
						}
						break;
					}
					default:
					{
						goto _loop66_breakloop;
					}
					 }
				}
_loop66_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_22_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  shift_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=additive_expression();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case SHL:
					{
						match(SHL);
						expr=additive_expression();
						if (0==inputState.guessing)
						{
							result = new ShiftLeft(result, expr);
						}
						break;
					}
					case SHR:
					{
						match(SHR);
						expr=additive_expression();
						if (0==inputState.guessing)
						{
							result = new ShiftRight(result, expr);
						}
						break;
					}
					default:
					{
						goto _loop69_breakloop;
					}
					 }
				}
_loop69_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_23_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  relational_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null; Typ typ = null;
		
		try {      // for error handling
			result=shift_expression();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case LANGLE:
					{
						match(LANGLE);
						expr=shift_expression();
						if (0==inputState.guessing)
						{
							result = new LessThan(result, expr);
						}
						break;
					}
					case RANGLE:
					{
						match(RANGLE);
						expr=shift_expression();
						if (0==inputState.guessing)
						{
							result = new GreaterThan(result, expr);
						}
						break;
					}
					case LE:
					{
						match(LE);
						expr=shift_expression();
						if (0==inputState.guessing)
						{
							result = new LessEqual(result, expr);
						}
						break;
					}
					case GE:
					{
						match(GE);
						expr=shift_expression();
						if (0==inputState.guessing)
						{
							result = new GreaterEqual(result, expr);
						}
						break;
					}
					case IS:
					{
						match(IS);
						typ=type_in_expression();
						if (0==inputState.guessing)
						{
							result = new Is(result, typ);
						}
						break;
					}
					case AS:
					{
						match(AS);
						typ=type_in_expression();
						if (0==inputState.guessing)
						{
							result = new As(result, typ);
						}
						break;
					}
					default:
					{
						goto _loop72_breakloop;
					}
					 }
				}
_loop72_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_24_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	protected Typ  type_in_expression() //throws RecognitionException, TokenStreamException
{
		Typ result;
		
		result = null; List<int> ranks = null;
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				{
					result=type_name_in_expression();
					break;
				}
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				{
					result=predefined_type();
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case LBRACK:
				{
					ranks=rank_specifiers();
					if (0==inputState.guessing)
					{
						result = ArrayType.Create(result, ranks);
					}
					break;
				}
				case COMMA:
				case RBRACK:
				case LANGLE:
				case RANGLE:
				case RPAREN:
				case RMETA:
				case COLON:
				case SEMI:
				case QUESTION:
				case AS:
				case IS:
				case RBRACE:
				case AND:
				case OR:
				case XOR:
				case ASSIGN:
				case LAND:
				case LOR:
				case EQ:
				case NE:
				case LE:
				case GE:
				case PLUS_ASSIGN:
				case MINUS_ASSIGN:
				case TIMES_ASSIGN:
				case QUOT_ASSIGN:
				case REM_ASSIGN:
				case AND_ASSIGN:
				case OR_ASSIGN:
				case XOR_ASSIGN:
				case SHL_ASSIGN:
				case SHR_ASSIGN:
				case SEMI_ASSIGN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_23_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	protected ClassType  type_name_in_expression() //throws RecognitionException, TokenStreamException
{
		ClassType result;
		
		result = null; Ident name = null; List<Typ> typeArgs = null;
		
		try {      // for error handling
			name=identifier();
			typeArgs=type_argument_list_opt();
			if (0==inputState.guessing)
			{
				result = new ClassType(null, name, typeArgs);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==DOT))
					{
						match(DOT);
						name=identifier();
						typeArgs=type_argument_list_opt();
						if (0==inputState.guessing)
						{
							result = new ClassType(result, name, typeArgs);
						}
					}
					else
					{
						goto _loop75_breakloop;
					}
					
				}
_loop75_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_25_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  equality_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=relational_expression();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case EQ:
					{
						match(EQ);
						expr=relational_expression();
						if (0==inputState.guessing)
						{
							result = new Equals(result, expr);
						}
						break;
					}
					case NE:
					{
						match(NE);
						expr=relational_expression();
						if (0==inputState.guessing)
						{
							result = new NotEqual(result, expr);
						}
						break;
					}
					default:
					{
						goto _loop81_breakloop;
					}
					 }
				}
_loop81_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_26_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  and_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=equality_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==AND))
					{
						match(AND);
						expr=equality_expression();
						if (0==inputState.guessing)
						{
							result = new And(result, expr);
						}
					}
					else
					{
						goto _loop84_breakloop;
					}
					
				}
_loop84_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_27_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  exclusive_or_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=and_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==XOR))
					{
						match(XOR);
						expr=and_expression();
						if (0==inputState.guessing)
						{
							result = new Xor(result, expr);
						}
					}
					else
					{
						goto _loop87_breakloop;
					}
					
				}
_loop87_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_28_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  inclusive_or_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=exclusive_or_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==OR))
					{
						match(OR);
						expr=exclusive_or_expression();
						if (0==inputState.guessing)
						{
							result = new Or(result, expr);
						}
					}
					else
					{
						goto _loop90_breakloop;
					}
					
				}
_loop90_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_29_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  conditional_and_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=inclusive_or_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==LAND))
					{
						match(LAND);
						expr=inclusive_or_expression();
						if (0==inputState.guessing)
						{
							result = new LogicalAnd(result, expr);
						}
					}
					else
					{
						goto _loop93_breakloop;
					}
					
				}
_loop93_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_30_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  conditional_or_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=conditional_and_expression();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==LOR))
					{
						match(LOR);
						expr=conditional_and_expression();
						if (0==inputState.guessing)
						{
							result = new LogicalOr(result, expr);
						}
					}
					else
					{
						goto _loop96_breakloop;
					}
					
				}
_loop96_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_31_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  conditional_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr1 = null; Expr expr2 = null;
		
		try {      // for error handling
			result=conditional_or_expression();
			{
				switch ( LA(1) )
				{
				case QUESTION:
				{
					match(QUESTION);
					expr1=conditional_expression();
					match(COLON);
					expr2=conditional_expression();
					if (0==inputState.guessing)
					{
						result = new Conditional(result, expr1, expr2);
					}
					break;
				}
				case COMMA:
				case RBRACK:
				case RPAREN:
				case RMETA:
				case COLON:
				case SEMI:
				case RBRACE:
				case ASSIGN:
				case PLUS_ASSIGN:
				case MINUS_ASSIGN:
				case TIMES_ASSIGN:
				case QUOT_ASSIGN:
				case REM_ASSIGN:
				case AND_ASSIGN:
				case OR_ASSIGN:
				case XOR_ASSIGN:
				case SHL_ASSIGN:
				case SHR_ASSIGN:
				case SEMI_ASSIGN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_32_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  assignment_expression() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; Expr expr = null;
		
		try {      // for error handling
			result=conditional_expression();
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case ASSIGN:
					{
						match(ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new Assign(result, expr);
						}
						break;
					}
					case PLUS_ASSIGN:
					{
						match(PLUS_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new AddAssign(result, expr);
						}
						break;
					}
					case MINUS_ASSIGN:
					{
						match(MINUS_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new SubtractAssign(result, expr);
						}
						break;
					}
					case TIMES_ASSIGN:
					{
						match(TIMES_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new MultiplyAssign(result, expr);
						}
						break;
					}
					case QUOT_ASSIGN:
					{
						match(QUOT_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new DivideAssign(result, expr);
						}
						break;
					}
					case REM_ASSIGN:
					{
						match(REM_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new RemainderAssign(result, expr);
						}
						break;
					}
					case AND_ASSIGN:
					{
						match(AND_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new AndAssign(result, expr);
						}
						break;
					}
					case XOR_ASSIGN:
					{
						match(XOR_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new XorAssign(result, expr);
						}
						break;
					}
					case OR_ASSIGN:
					{
						match(OR_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new OrAssign(result, expr);
						}
						break;
					}
					case SHL_ASSIGN:
					{
						match(SHL_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new ShiftLeftAssign(result, expr);
						}
						break;
					}
					case SHR_ASSIGN:
					{
						match(SHR_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new ShiftRightAssign(result, expr);
						}
						break;
					}
					case SEMI_ASSIGN:
					{
						match(SEMI_ASSIGN);
						expr=conditional_expression();
						if (0==inputState.guessing)
						{
							result = new SemiAssign(result, expr);
						}
						break;
					}
					default:
					{
						goto _loop101_breakloop;
					}
					 }
				}
_loop101_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_10_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Collections.Tuple<Ident, Expr>>  variable_declarators() //throws RecognitionException, TokenStreamException
{
		List<Collections.Tuple<Ident, Expr>> result;
		
		result = null; Collections.Tuple<Ident, Expr> var = new Collections.Tuple<Ident, Expr>();
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Collections.Tuple<Ident, Expr>>();
			}
			var=variable_declarator();
			if (0==inputState.guessing)
			{
				result.Add(var);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						var=variable_declarator();
						if (0==inputState.guessing)
						{
							result.Add(var);
						}
					}
					else
					{
						goto _loop142_breakloop;
					}
					
				}
_loop142_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_33_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Stmt  embedded_statement() //throws RecognitionException, TokenStreamException
{
		Stmt result;
		
		IToken  t = null;
		result = null; List<Stmt> stmts = null; Expr expr = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case LBRACE:
			{
				stmts=block();
				if (0==inputState.guessing)
				{
					result = new Block(stmts);
				}
				break;
			}
			case SEMI:
			case IF:
			case SWITCH:
			case WHILE:
			case DO:
			case FOR:
			case FOREACH:
			case BREAK:
			case CONTINUE:
			case GOTO:
			case RETURN:
			case THROW:
			case TRY:
			case CHECKED:
			case UNCHECKED:
			case LOCK:
			case USING:
			case TYPEIF:
			case WITHTYPE:
			case FORMEMBER:
			{
				result=actual_statement();
				break;
			}
			default:
				bool synPredMatched112 = false;
				if (((LA(1)==ESCAPE)))
				{
					int _m112 = mark();
					synPredMatched112 = true;
					inputState.guessing++;
					try {
						{
							match(ESCAPE);
							match(LBRACE);
						}
					}
					catch (RecognitionException)
					{
						synPredMatched112 = false;
					}
					rewind(_m112);
					inputState.guessing--;
				}
				if ( synPredMatched112 )
				{
					t = LT(1);
					match(ESCAPE);
					stmts=block();
					if (0==inputState.guessing)
					{
						result = new EscapeStmt(t, stmts);
					}
				}
				else if ((tokenSet_19_.member(LA(1)))) {
					expr=expression();
					match(SEMI);
					if (0==inputState.guessing)
					{
						result = new Expression(expr);
					}
				}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			break; }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_34_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Stmt  actual_statement() //throws RecognitionException, TokenStreamException
{
		Stmt result;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  t4 = null;
		IToken  t5 = null;
		IToken  t6 = null;
		IToken  t7 = null;
		IToken  t8 = null;
		IToken  t9 = null;
		IToken  t10 = null;
		IToken  t11 = null;
		IToken  t12 = null;
		IToken  t13 = null;
		IToken  t14 = null;
		IToken  t15 = null;
		IToken  t16 = null;
		IToken  t17 = null;
		IToken  t18 = null;
		IToken  t19 = null;
		result = null; Expr expr = null; Stmt stmt = null; Stmt stmt2 = null; List<Case> cases = null; Either<LocalDecl, List<Expr>> forInit = new Either<LocalDecl, List<Expr>>(); List<Expr> exprs = null; Typ typ = null; Ident name = null; List<Stmt> stmts = null; List<Catch> catches = null; Finally fin = null; List<Collections.Tuple<Ident,int>> typeParamNames = null; List<TypeParam> typeParamCons = null; bool isStatic = false; Ident retType = null; List<Collections.Tuple<string,bool>> pars = null; Ident paramName = null; bool paramRef = false;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case SEMI:
			{
				match(SEMI);
				if (0==inputState.guessing)
				{
					result  = new Empty();
				}
				break;
			}
			case IF:
			{
				t1 = LT(1);
				match(IF);
				match(LPAREN);
				expr=expression();
				match(RPAREN);
				stmt=embedded_statement();
				{
					if ((LA(1)==ELSE))
					{
						match(ELSE);
						stmt2=embedded_statement();
					}
					else if ((tokenSet_34_.member(LA(1)))) {
					}
					else
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					
				}
				if (0==inputState.guessing)
				{
					result = new If(t1, expr, stmt, stmt2);
				}
				break;
			}
			case SWITCH:
			{
				t2 = LT(1);
				match(SWITCH);
				match(LPAREN);
				expr=expression();
				match(RPAREN);
				match(LBRACE);
				cases=switch_sections();
				match(RBRACE);
				if (0==inputState.guessing)
				{
					result = new Switch(t2, expr, cases);
				}
				break;
			}
			case WHILE:
			{
				t3 = LT(1);
				match(WHILE);
				match(LPAREN);
				expr=expression();
				match(RPAREN);
				stmt=embedded_statement();
				if (0==inputState.guessing)
				{
					result = new While(t3, expr, stmt);
				}
				break;
			}
			case DO:
			{
				t4 = LT(1);
				match(DO);
				stmt=embedded_statement();
				match(WHILE);
				match(LPAREN);
				expr=expression();
				match(RPAREN);
				match(SEMI);
				if (0==inputState.guessing)
				{
					result = new Do(t4, stmt, expr);
				}
				break;
			}
			case FOR:
			{
				t5 = LT(1);
				match(FOR);
				match(LPAREN);
				{
					switch ( LA(1) )
					{
					case FALSE:
					case TRUE:
					case INT32_DECIMAL_INTEGER_LITERAL:
					case INT64_DECIMAL_INTEGER_LITERAL:
					case UINT32_DECIMAL_INTEGER_LITERAL:
					case UINT64_DECIMAL_INTEGER_LITERAL:
					case INT32_HEXADECIMAL_INTEGER_LITERAL:
					case INT64_HEXADECIMAL_INTEGER_LITERAL:
					case UINT32_HEXADECIMAL_INTEGER_LITERAL:
					case UINT64_HEXADECIMAL_INTEGER_LITERAL:
					case FLOAT32_REAL_LITERAL:
					case FLOAT64_REAL_LITERAL:
					case DECIMAL_REAL_LITERAL:
					case CHARACTER_LITERAL:
					case STRING_LITERAL:
					case NULL:
					case IDENTIFIER:
					case BOOL:
					case DECIMAL:
					case SBYTE:
					case BYTE:
					case SHORT:
					case USHORT:
					case INT:
					case UINT:
					case LONG:
					case ULONG:
					case CHAR:
					case FLOAT:
					case DOUBLE:
					case OBJECT:
					case STRING:
					case VOID:
					case NEW:
					case LPAREN:
					case INC:
					case DEC:
					case THIS:
					case BASE:
					case TYPEOF:
					case DEFAULT:
					case DELEGATE:
					case LMETA:
					case PLUS:
					case MINUS:
					case LNOT:
					case NOT:
					case ESCAPE:
					case LIFT:
					{
						forInit=for_initializer();
						break;
					}
					case SEMI:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(SEMI);
				{
					switch ( LA(1) )
					{
					case FALSE:
					case TRUE:
					case INT32_DECIMAL_INTEGER_LITERAL:
					case INT64_DECIMAL_INTEGER_LITERAL:
					case UINT32_DECIMAL_INTEGER_LITERAL:
					case UINT64_DECIMAL_INTEGER_LITERAL:
					case INT32_HEXADECIMAL_INTEGER_LITERAL:
					case INT64_HEXADECIMAL_INTEGER_LITERAL:
					case UINT32_HEXADECIMAL_INTEGER_LITERAL:
					case UINT64_HEXADECIMAL_INTEGER_LITERAL:
					case FLOAT32_REAL_LITERAL:
					case FLOAT64_REAL_LITERAL:
					case DECIMAL_REAL_LITERAL:
					case CHARACTER_LITERAL:
					case STRING_LITERAL:
					case NULL:
					case IDENTIFIER:
					case BOOL:
					case DECIMAL:
					case SBYTE:
					case BYTE:
					case SHORT:
					case USHORT:
					case INT:
					case UINT:
					case LONG:
					case ULONG:
					case CHAR:
					case FLOAT:
					case DOUBLE:
					case OBJECT:
					case STRING:
					case VOID:
					case NEW:
					case LPAREN:
					case INC:
					case DEC:
					case THIS:
					case BASE:
					case TYPEOF:
					case DEFAULT:
					case DELEGATE:
					case LMETA:
					case PLUS:
					case MINUS:
					case LNOT:
					case NOT:
					case ESCAPE:
					case LIFT:
					{
						expr=expression();
						break;
					}
					case SEMI:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(SEMI);
				{
					switch ( LA(1) )
					{
					case FALSE:
					case TRUE:
					case INT32_DECIMAL_INTEGER_LITERAL:
					case INT64_DECIMAL_INTEGER_LITERAL:
					case UINT32_DECIMAL_INTEGER_LITERAL:
					case UINT64_DECIMAL_INTEGER_LITERAL:
					case INT32_HEXADECIMAL_INTEGER_LITERAL:
					case INT64_HEXADECIMAL_INTEGER_LITERAL:
					case UINT32_HEXADECIMAL_INTEGER_LITERAL:
					case UINT64_HEXADECIMAL_INTEGER_LITERAL:
					case FLOAT32_REAL_LITERAL:
					case FLOAT64_REAL_LITERAL:
					case DECIMAL_REAL_LITERAL:
					case CHARACTER_LITERAL:
					case STRING_LITERAL:
					case NULL:
					case IDENTIFIER:
					case BOOL:
					case DECIMAL:
					case SBYTE:
					case BYTE:
					case SHORT:
					case USHORT:
					case INT:
					case UINT:
					case LONG:
					case ULONG:
					case CHAR:
					case FLOAT:
					case DOUBLE:
					case OBJECT:
					case STRING:
					case VOID:
					case NEW:
					case LPAREN:
					case INC:
					case DEC:
					case THIS:
					case BASE:
					case TYPEOF:
					case DEFAULT:
					case DELEGATE:
					case LMETA:
					case PLUS:
					case MINUS:
					case LNOT:
					case NOT:
					case ESCAPE:
					case LIFT:
					{
						exprs=expression_list();
						break;
					}
					case RPAREN:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(RPAREN);
				stmt=embedded_statement();
				if (0==inputState.guessing)
				{
					
							if(forInit.IsLeft)
								result = new For(t5, forInit.Left, expr, exprs, stmt);
							else
								result = new For(t5, forInit.Right, expr, exprs, stmt);
						
				}
				break;
			}
			case FOREACH:
			{
				t6 = LT(1);
				match(FOREACH);
				match(LPAREN);
				typ=type();
				name=identifier();
				match(IN);
				expr=expression();
				match(RPAREN);
				stmt=embedded_statement();
				if (0==inputState.guessing)
				{
					result = new Foreach(t6, typ, name, expr, stmt);
				}
				break;
			}
			case BREAK:
			{
				t7 = LT(1);
				match(BREAK);
				match(SEMI);
				if (0==inputState.guessing)
				{
					result = new Break(t7);
				}
				break;
			}
			case CONTINUE:
			{
				t8 = LT(1);
				match(CONTINUE);
				match(SEMI);
				if (0==inputState.guessing)
				{
					result = new Continue(t8);
				}
				break;
			}
			case GOTO:
			{
				t9 = LT(1);
				match(GOTO);
				name=identifier();
				match(SEMI);
				if (0==inputState.guessing)
				{
					result = new Goto(t9, name);
				}
				break;
			}
			case RETURN:
			{
				t10 = LT(1);
				match(RETURN);
				{
					switch ( LA(1) )
					{
					case FALSE:
					case TRUE:
					case INT32_DECIMAL_INTEGER_LITERAL:
					case INT64_DECIMAL_INTEGER_LITERAL:
					case UINT32_DECIMAL_INTEGER_LITERAL:
					case UINT64_DECIMAL_INTEGER_LITERAL:
					case INT32_HEXADECIMAL_INTEGER_LITERAL:
					case INT64_HEXADECIMAL_INTEGER_LITERAL:
					case UINT32_HEXADECIMAL_INTEGER_LITERAL:
					case UINT64_HEXADECIMAL_INTEGER_LITERAL:
					case FLOAT32_REAL_LITERAL:
					case FLOAT64_REAL_LITERAL:
					case DECIMAL_REAL_LITERAL:
					case CHARACTER_LITERAL:
					case STRING_LITERAL:
					case NULL:
					case IDENTIFIER:
					case BOOL:
					case DECIMAL:
					case SBYTE:
					case BYTE:
					case SHORT:
					case USHORT:
					case INT:
					case UINT:
					case LONG:
					case ULONG:
					case CHAR:
					case FLOAT:
					case DOUBLE:
					case OBJECT:
					case STRING:
					case VOID:
					case NEW:
					case LPAREN:
					case INC:
					case DEC:
					case THIS:
					case BASE:
					case TYPEOF:
					case DEFAULT:
					case DELEGATE:
					case LMETA:
					case PLUS:
					case MINUS:
					case LNOT:
					case NOT:
					case ESCAPE:
					case LIFT:
					{
						expr=expression();
						break;
					}
					case SEMI:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(SEMI);
				if (0==inputState.guessing)
				{
					result = new Return(t10, expr);
				}
				break;
			}
			case THROW:
			{
				t11 = LT(1);
				match(THROW);
				{
					switch ( LA(1) )
					{
					case FALSE:
					case TRUE:
					case INT32_DECIMAL_INTEGER_LITERAL:
					case INT64_DECIMAL_INTEGER_LITERAL:
					case UINT32_DECIMAL_INTEGER_LITERAL:
					case UINT64_DECIMAL_INTEGER_LITERAL:
					case INT32_HEXADECIMAL_INTEGER_LITERAL:
					case INT64_HEXADECIMAL_INTEGER_LITERAL:
					case UINT32_HEXADECIMAL_INTEGER_LITERAL:
					case UINT64_HEXADECIMAL_INTEGER_LITERAL:
					case FLOAT32_REAL_LITERAL:
					case FLOAT64_REAL_LITERAL:
					case DECIMAL_REAL_LITERAL:
					case CHARACTER_LITERAL:
					case STRING_LITERAL:
					case NULL:
					case IDENTIFIER:
					case BOOL:
					case DECIMAL:
					case SBYTE:
					case BYTE:
					case SHORT:
					case USHORT:
					case INT:
					case UINT:
					case LONG:
					case ULONG:
					case CHAR:
					case FLOAT:
					case DOUBLE:
					case OBJECT:
					case STRING:
					case VOID:
					case NEW:
					case LPAREN:
					case INC:
					case DEC:
					case THIS:
					case BASE:
					case TYPEOF:
					case DEFAULT:
					case DELEGATE:
					case LMETA:
					case PLUS:
					case MINUS:
					case LNOT:
					case NOT:
					case ESCAPE:
					case LIFT:
					{
						expr=expression();
						break;
					}
					case SEMI:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(SEMI);
				if (0==inputState.guessing)
				{
					result = new Throw(t11, expr);
				}
				break;
			}
			case TRY:
			{
				t12 = LT(1);
				match(TRY);
				stmts=block();
				{
					switch ( LA(1) )
					{
					case CATCH:
					{
						catches=catch_clauses();
						{
							switch ( LA(1) )
							{
							case FINALLY:
							{
								fin=finally_clause();
								break;
							}
							case FALSE:
							case TRUE:
							case INT32_DECIMAL_INTEGER_LITERAL:
							case INT64_DECIMAL_INTEGER_LITERAL:
							case UINT32_DECIMAL_INTEGER_LITERAL:
							case UINT64_DECIMAL_INTEGER_LITERAL:
							case INT32_HEXADECIMAL_INTEGER_LITERAL:
							case INT64_HEXADECIMAL_INTEGER_LITERAL:
							case UINT32_HEXADECIMAL_INTEGER_LITERAL:
							case UINT64_HEXADECIMAL_INTEGER_LITERAL:
							case FLOAT32_REAL_LITERAL:
							case FLOAT64_REAL_LITERAL:
							case DECIMAL_REAL_LITERAL:
							case CHARACTER_LITERAL:
							case STRING_LITERAL:
							case NULL:
							case IDENTIFIER:
							case BOOL:
							case DECIMAL:
							case SBYTE:
							case BYTE:
							case SHORT:
							case USHORT:
							case INT:
							case UINT:
							case LONG:
							case ULONG:
							case CHAR:
							case FLOAT:
							case DOUBLE:
							case OBJECT:
							case STRING:
							case VOID:
							case NEW:
							case LPAREN:
							case INC:
							case DEC:
							case THIS:
							case BASE:
							case TYPEOF:
							case DEFAULT:
							case DELEGATE:
							case LMETA:
							case RMETA:
							case SEMI:
							case PLUS:
							case MINUS:
							case LNOT:
							case NOT:
							case ESCAPE:
							case LBRACE:
							case RBRACE:
							case LIFT:
							case CONST:
							case IF:
							case ELSE:
							case SWITCH:
							case WHILE:
							case DO:
							case FOR:
							case FOREACH:
							case BREAK:
							case CONTINUE:
							case GOTO:
							case RETURN:
							case THROW:
							case TRY:
							case CHECKED:
							case UNCHECKED:
							case LOCK:
							case USING:
							case TYPEIF:
							case WITHTYPE:
							case FORMEMBER:
							{
								break;
							}
							default:
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							 }
						}
						break;
					}
					case FINALLY:
					{
						fin=finally_clause();
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				if (0==inputState.guessing)
				{
					result = new Try(t12, stmts, catches, fin);
				}
				break;
			}
			case CHECKED:
			{
				t13 = LT(1);
				match(CHECKED);
				stmts=block();
				if (0==inputState.guessing)
				{
					NotDone(t13, "checked blocks");
				}
				break;
			}
			case UNCHECKED:
			{
				t14 = LT(1);
				match(UNCHECKED);
				stmts=block();
				if (0==inputState.guessing)
				{
					NotDone(t14, "unchecked blocks");
				}
				break;
			}
			case LOCK:
			{
				t15 = LT(1);
				match(LOCK);
				match(LPAREN);
				expr=expression();
				match(RPAREN);
				stmt=embedded_statement();
				if (0==inputState.guessing)
				{
					NotDone(t15, "lock blocks");
				}
				break;
			}
			case USING:
			{
				t16 = LT(1);
				match(USING);
				match(LPAREN);
				typ=type();
				name=identifier();
				match(ASSIGN);
				expr=expression();
				match(RPAREN);
				stmt=embedded_statement();
				if (0==inputState.guessing)
				{
					NotDone(t16, "using blocks");
				}
				break;
			}
			case TYPEIF:
			{
				t17 = LT(1);
				match(TYPEIF);
				{
					switch ( LA(1) )
					{
					case LANGLE:
					{
						typeParamNames=type_parameter_list();
						break;
					}
					case LPAREN:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(LPAREN);
				name=identifier();
				match(IS);
				typ=type();
				typeParamCons=type_parameter_constraints_clauses();
				match(RPAREN);
				stmt=embedded_statement();
				{
					if ((LA(1)==ELSE))
					{
						match(ELSE);
						stmt2=embedded_statement();
					}
					else if ((tokenSet_34_.member(LA(1)))) {
					}
					else
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					
				}
				if (0==inputState.guessing)
				{
					result = new TypeIf(t17, typeParamNames, name, typ, typeParamCons, stmt, stmt2);
				}
				break;
			}
			case WITHTYPE:
			{
				t18 = LT(1);
				match(WITHTYPE);
				match(LPAREN);
				name=identifier();
				match(AS);
				expr=expression();
				match(RPAREN);
				stmt=embedded_statement();
				if (0==inputState.guessing)
				{
					result = new WithType(t18, name, expr, stmt);
				}
				break;
			}
			case FORMEMBER:
			{
				t19 = LT(1);
				match(FORMEMBER);
				match(LPAREN);
				{
					switch ( LA(1) )
					{
					case STATIC:
					{
						match(STATIC);
						if (0==inputState.guessing)
						{
							isStatic = true;
						}
						break;
					}
					case IDENTIFIER:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				retType=identifier();
				name=identifier();
				{
					switch ( LA(1) )
					{
					case LPAREN:
					{
						match(LPAREN);
						pars=formal_argument_pattern_list();
						match(RPAREN);
						break;
					}
					case IN:
					{
						break;
					}
					default:
					{
						throw new NoViableAltException(LT(1), getFilename());
					}
					 }
				}
				match(IN);
				typ=type();
				match(RPAREN);
				stmt=embedded_statement();
				if (0==inputState.guessing)
				{
					result = new ForMember(t19, isStatic, retType.Name, name, pars, typ, stmt);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_34_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Case>  switch_sections() //throws RecognitionException, TokenStreamException
{
		List<Case> result;
		
		IToken  t1 = null;
		IToken  t2 = null;
		result = null; Literal lit = null; List<Literal> lits = null; List<Stmt> stmts = null; IToken t = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Case>();
			}
			{    // ( ... )*
				for (;;)
				{
					switch ( LA(1) )
					{
					case CASE:
					{
						if (0==inputState.guessing)
						{
							lits = new List<Literal>();
						}
						{ // ( ... )+
							int _cnt146=0;
							for (;;)
							{
								if ((LA(1)==CASE))
								{
									t1 = LT(1);
									match(CASE);
									lit=literal();
									if (0==inputState.guessing)
									{
										lits.Add(lit); if(t == null) t = t1;
									}
								}
								else
								{
									if (_cnt146 >= 1) { goto _loop146_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
								}
								
								_cnt146++;
							}
_loop146_breakloop:							;
						}    // ( ... )+
						stmts=block();
						if (0==inputState.guessing)
						{
							result.Add(new Case(t, lits, stmts));
						}
						break;
					}
					case DEFAULT:
					{
						t2 = LT(1);
						match(DEFAULT);
						stmts=block();
						if (0==inputState.guessing)
						{
							result.Add(new Case(t2, null, stmts));
						}
						break;
					}
					default:
					{
						goto _loop147_breakloop;
					}
					 }
				}
_loop147_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_35_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Either<LocalDecl, List<Expr>>  for_initializer() //throws RecognitionException, TokenStreamException
{
		Either<LocalDecl, List<Expr>> result;
		
		result = new Either<LocalDecl, List<Expr>>(); Typ typ = null; Ident name = null; Expr var = null;
		
		try {      // for error handling
			bool synPredMatched150 = false;
			if (((tokenSet_16_.member(LA(1)))))
			{
				int _m150 = mark();
				synPredMatched150 = true;
				inputState.guessing++;
				try {
					{
						type();
						match(IDENTIFIER);
					}
				}
				catch (RecognitionException)
				{
					synPredMatched150 = false;
				}
				rewind(_m150);
				inputState.guessing--;
			}
			if ( synPredMatched150 )
			{
				typ=type();
				name=identifier();
				match(ASSIGN);
				var=variable_initializer();
				if (0==inputState.guessing)
				{
					result = new LocalDecl(typ, name, var);
				}
			}
			else if ((tokenSet_19_.member(LA(1)))) {
				result=expression_list();
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_33_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Catch>  catch_clauses() //throws RecognitionException, TokenStreamException
{
		List<Catch> result;
		
		result = null; Catch c = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Catch>();
			}
			{ // ( ... )+
				int _cnt153=0;
				for (;;)
				{
					if ((LA(1)==CATCH))
					{
						c=catch_clause();
						if (0==inputState.guessing)
						{
							result.Add(c);
						}
					}
					else
					{
						if (_cnt153 >= 1) { goto _loop153_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt153++;
				}
_loop153_breakloop:				;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_36_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Finally  finally_clause() //throws RecognitionException, TokenStreamException
{
		Finally result;
		
		IToken  t = null;
		result = null; List<Stmt> stmts = null;
		
		try {      // for error handling
			t = LT(1);
			match(FINALLY);
			stmts=block();
			if (0==inputState.guessing)
			{
				result = new Finally(t, stmts);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_34_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Collections.Tuple<Ident,int>>  type_parameter_list() //throws RecognitionException, TokenStreamException
{
		List<Collections.Tuple<Ident,int>> result;
		
		result = null; Collections.Tuple<Ident,int> typeParam;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Collections.Tuple<Ident,int>>();
			}
			match(LANGLE);
			typeParam=type_parameter();
			if (0==inputState.guessing)
			{
				result.Add(typeParam);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						typeParam=type_parameter();
						if (0==inputState.guessing)
						{
							result.Add(typeParam);
						}
					}
					else
					{
						goto _loop181_breakloop;
					}
					
				}
_loop181_breakloop:				;
			}    // ( ... )*
			match(RANGLE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_37_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<TypeParam>  type_parameter_constraints_clauses() //throws RecognitionException, TokenStreamException
{
		List<TypeParam> result;
		
		result = null; TypeParam typeParam = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<TypeParam>();
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==WHERE))
					{
						typeParam=type_parameter_constraints_clause();
						if (0==inputState.guessing)
						{
							result.Add(typeParam);
						}
					}
					else
					{
						goto _loop190_breakloop;
					}
					
				}
_loop190_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_38_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	protected List<Collections.Tuple<string, bool>>  formal_argument_pattern_list() //throws RecognitionException, TokenStreamException
{
		List<Collections.Tuple<string, bool>> result;
		
		result = new List<Collections.Tuple<string, bool>>(); Collections.Tuple<string, bool> pat = new Collections.Tuple<string, bool>();
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				case REF:
				{
					pat=formal_argument_pattern();
					if (0==inputState.guessing)
					{
						result.Add(pat);
					}
					{    // ( ... )*
						for (;;)
						{
							if ((LA(1)==COMMA))
							{
								match(COMMA);
								pat=formal_argument_pattern();
								if (0==inputState.guessing)
								{
									result.Add(pat);
								}
							}
							else
							{
								goto _loop135_breakloop;
							}
							
						}
_loop135_breakloop:						;
					}    // ( ... )*
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_8_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	protected Collections.Tuple<string, bool>  formal_argument_pattern() //throws RecognitionException, TokenStreamException
{
        Collections.Tuple<string, bool> result;
		
		result = new Collections.Tuple<string, bool>(); Ident name = null;
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case REF:
				{
					match(REF);
					if (0==inputState.guessing)
					{
						result.snd = true;
					}
					break;
				}
				case IDENTIFIER:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			name=identifier();
			if (0==inputState.guessing)
			{
				result.fst = name.Name;
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_9_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Collections.Tuple<Ident, Expr>  variable_declarator() //throws RecognitionException, TokenStreamException
{
            Collections.Tuple<Ident, Expr> result;
		
		result = new Collections.Tuple<Ident, Expr>(); Ident name = null; Expr init = null;
		
		try {      // for error handling
			name=identifier();
			{
				switch ( LA(1) )
				{
				case ASSIGN:
				{
					match(ASSIGN);
					init=variable_initializer();
					break;
				}
				case COMMA:
				case SEMI:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				result = new Collections.Tuple<Ident, Expr>(name, init);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_39_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Expr  variable_initializer() //throws RecognitionException, TokenStreamException
{
		Expr result;
		
		result = null; List<Expr> exprs = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case FALSE:
			case TRUE:
			case INT32_DECIMAL_INTEGER_LITERAL:
			case INT64_DECIMAL_INTEGER_LITERAL:
			case UINT32_DECIMAL_INTEGER_LITERAL:
			case UINT64_DECIMAL_INTEGER_LITERAL:
			case INT32_HEXADECIMAL_INTEGER_LITERAL:
			case INT64_HEXADECIMAL_INTEGER_LITERAL:
			case UINT32_HEXADECIMAL_INTEGER_LITERAL:
			case UINT64_HEXADECIMAL_INTEGER_LITERAL:
			case FLOAT32_REAL_LITERAL:
			case FLOAT64_REAL_LITERAL:
			case DECIMAL_REAL_LITERAL:
			case CHARACTER_LITERAL:
			case STRING_LITERAL:
			case NULL:
			case IDENTIFIER:
			case BOOL:
			case DECIMAL:
			case SBYTE:
			case BYTE:
			case SHORT:
			case USHORT:
			case INT:
			case UINT:
			case LONG:
			case ULONG:
			case CHAR:
			case FLOAT:
			case DOUBLE:
			case OBJECT:
			case STRING:
			case VOID:
			case NEW:
			case LPAREN:
			case INC:
			case DEC:
			case THIS:
			case BASE:
			case TYPEOF:
			case DEFAULT:
			case DELEGATE:
			case LMETA:
			case PLUS:
			case MINUS:
			case LNOT:
			case NOT:
			case ESCAPE:
			case LIFT:
			{
				result=expression();
				break;
			}
			case LBRACE:
			{
				exprs=array_initializer();
				if (0==inputState.guessing)
				{
					result = new ArrayInitialisation(exprs);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_40_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Catch  catch_clause() //throws RecognitionException, TokenStreamException
{
		Catch result;
		
		IToken  t = null;
		result = null; Typ typ = null; Ident name = null; List<Stmt> stmts = null;
		
		try {      // for error handling
			t = LT(1);
			match(CATCH);
			{
				switch ( LA(1) )
				{
				case LPAREN:
				{
					match(LPAREN);
					typ=type();
					{
						switch ( LA(1) )
						{
						case IDENTIFIER:
						{
							name=identifier();
							break;
						}
						case RPAREN:
						{
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					match(RPAREN);
					stmts=block();
					if (0==inputState.guessing)
					{
						result = new Catch(t, typ, name, stmts);
					}
					break;
				}
				case LBRACE:
				{
					stmts=block();
					if (0==inputState.guessing)
					{
						result = new Catch(t, null, null, stmts);
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_41_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Module  compilation_unit() //throws RecognitionException, TokenStreamException
{
		Module result;
		
		result = null; List<Ident> usings = null;
            Collections.Tuple<List<Namespace>, List<TypeDecl>> body = new Collections.Tuple<List<Namespace>, List<TypeDecl>>();
		
		try {      // for error handling
			usings=using_directives();
			body=namespace_body();
			match(Token.EOF_TYPE);
			if (0==inputState.guessing)
			{
				result = new Module(usings, new Namespace(null, null, body.fst, body.snd));
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_42_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Ident>  using_directives() //throws RecognitionException, TokenStreamException
{
		List<Ident> result;
		
		result = null; Ident u = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Ident>();
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==USING))
					{
						u=using_directive();
						if (0==inputState.guessing)
						{
							result.Add(u);
						}
					}
					else
					{
						goto _loop166_breakloop;
					}
					
				}
_loop166_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_43_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Collections.Tuple<List<Namespace>, List<TypeDecl>>  namespace_body() //throws RecognitionException, TokenStreamException
{
            Collections.Tuple<List<Namespace>, List<TypeDecl>> result;
		
		result = new Collections.Tuple<List<Namespace>, List<TypeDecl>>(); Either<Namespace, TypeDecl> member = new Either<Namespace, TypeDecl>();
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				
						result.fst = new List<Namespace>();
						result.snd = new List<TypeDecl>();
					
			}
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_44_.member(LA(1))))
					{
						member=namespace_member_declaration();
						if (0==inputState.guessing)
						{
							
										if(member.IsLeft) result.fst.Add(member.Left);
										else result.snd.Add(member.Right);		
									
						}
					}
					else
					{
						goto _loop163_breakloop;
					}
					
				}
_loop163_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_45_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Namespace  namespace_declaration() //throws RecognitionException, TokenStreamException
{
		Namespace result;
		
		IToken  t = null;
		result = null; Ident name = null; Collections.Tuple<List<Namespace>, List<TypeDecl>> body = new Collections.Tuple<List<Namespace>, List<TypeDecl>>();
		
		try {      // for error handling
			t = LT(1);
			match(NAMESPACE);
			name=namespace_name();
			match(LBRACE);
			body=namespace_body();
			match(RBRACE);
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case EOF:
				case NEW:
				case DELEGATE:
				case RBRACE:
				case STATIC:
				case NAMESPACE:
				case PUBLIC:
				case PROTECTED:
				case INTERNAL:
				case PRIVATE:
				case ABSTRACT:
				case SEALED:
				case READONLY:
				case VOLATILE:
				case VIRTUAL:
				case OVERRIDE:
				case EXTERN:
				case CLASS:
				case STRUCT:
				case INTERFACE:
				case ENUM:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				result = new Namespace(t, name, body.fst, body.snd);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Either<Namespace, TypeDecl>  namespace_member_declaration() //throws RecognitionException, TokenStreamException
{
		Either<Namespace, TypeDecl> result;
		
		result = new Either<Namespace, TypeDecl>();
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case NAMESPACE:
			{
				result=namespace_declaration();
				break;
			}
			case NEW:
			case DELEGATE:
			case STATIC:
			case PUBLIC:
			case PROTECTED:
			case INTERNAL:
			case PRIVATE:
			case ABSTRACT:
			case SEALED:
			case READONLY:
			case VOLATILE:
			case VIRTUAL:
			case OVERRIDE:
			case EXTERN:
			case CLASS:
			case STRUCT:
			case INTERFACE:
			case ENUM:
			{
				result=type_declaration();
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Ident  using_directive() //throws RecognitionException, TokenStreamException
{
		Ident result;
		
		result = null;
		
		try {      // for error handling
			match(USING);
			result=namespace_name();
			match(SEMI);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_47_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeDecl  type_declaration() //throws RecognitionException, TokenStreamException
{
		TypeDecl result;
		
		result = null; Modifier mods = Modifier.None;
		
		try {      // for error handling
			mods=modifiers();
			result=nested_type_declaration(mods);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_46_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Modifier  modifiers() //throws RecognitionException, TokenStreamException
{
		Modifier result;
		
		result = Modifier.None; Modifier mod = Modifier.None;
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_48_.member(LA(1))))
					{
						mod=modifier();
						if (0==inputState.guessing)
						{
										
									if ((result & mod) != 0) throw new SemanticException(string.Format("Duplicate modifier '{0}'.", mod)/*, token.getFilename(), token.getLine(), token.getColumn()*/);
									else result |= mod;
								
						}
					}
					else
					{
						goto _loop173_breakloop;
					}
					
				}
_loop173_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_49_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeDecl  nested_type_declaration(
		Modifier mods
	) //throws RecognitionException, TokenStreamException
{
		TypeDecl result;
		
		result = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case CLASS:
			{
				result=class_declaration(mods);
				break;
			}
			case STRUCT:
			{
				result=struct_declaration(mods);
				break;
			}
			case INTERFACE:
			{
				result=interface_declaration(mods);
				break;
			}
			case ENUM:
			{
				result=enum_declaration(mods);
				break;
			}
			case DELEGATE:
			{
				result=delegate_declaration(mods);
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_50_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeDecl  class_declaration(
		Modifier mods
	) //throws RecognitionException, TokenStreamException
{
		TypeDecl result;
		
		IToken  t = null;
		result = null; Ident name = null; List<Collections.Tuple<Ident,int>> typeParamNames = null; List<Typ> baseTypes = null; List<TypeParam> typeParams = null; List<Member> body = null;
		
		try {      // for error handling
			t = LT(1);
			match(CLASS);
			name=identifier();
			{
				switch ( LA(1) )
				{
				case LANGLE:
				{
					typeParamNames=type_parameter_list();
					break;
				}
				case COLON:
				case LBRACE:
				case WHERE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					baseTypes=class_base();
					break;
				}
				case LBRACE:
				case WHERE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			typeParams=type_parameter_constraints_clauses();
			body=class_body();
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case EOF:
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case NEW:
				case DELEGATE:
				case RBRACE:
				case CONST:
				case STATIC:
				case NAMESPACE:
				case PUBLIC:
				case PROTECTED:
				case INTERNAL:
				case PRIVATE:
				case ABSTRACT:
				case SEALED:
				case READONLY:
				case VOLATILE:
				case VIRTUAL:
				case OVERRIDE:
				case EXTERN:
				case CLASS:
				case STRUCT:
				case EVENT:
				case IMPLICIT:
				case EXPLICIT:
				case TILDE:
				case INTERFACE:
				case ENUM:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				result = new Class(t, mods, name, typeParamNames, baseTypes, typeParams, body);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_50_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeDecl  struct_declaration(
		Modifier mods
	) //throws RecognitionException, TokenStreamException
{
		TypeDecl result;
		
		IToken  t = null;
		result = null; Ident name = null; List<Collections.Tuple<Ident, int>> typeParamNames = null; List<Typ> baseTypes = null; List<TypeParam> typeParams = null; List<Member> body = null;
		
		try {      // for error handling
			t = LT(1);
			match(STRUCT);
			name=identifier();
			{
				switch ( LA(1) )
				{
				case LANGLE:
				{
					typeParamNames=type_parameter_list();
					break;
				}
				case COLON:
				case LBRACE:
				case WHERE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					baseTypes=class_base();
					break;
				}
				case LBRACE:
				case WHERE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			typeParams=type_parameter_constraints_clauses();
			body=class_body();
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case EOF:
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case NEW:
				case DELEGATE:
				case RBRACE:
				case CONST:
				case STATIC:
				case NAMESPACE:
				case PUBLIC:
				case PROTECTED:
				case INTERNAL:
				case PRIVATE:
				case ABSTRACT:
				case SEALED:
				case READONLY:
				case VOLATILE:
				case VIRTUAL:
				case OVERRIDE:
				case EXTERN:
				case CLASS:
				case STRUCT:
				case EVENT:
				case IMPLICIT:
				case EXPLICIT:
				case TILDE:
				case INTERFACE:
				case ENUM:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			if (0==inputState.guessing)
			{
				result = new Struct(t, mods, name, typeParamNames, baseTypes, typeParams, body);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_50_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeDecl  interface_declaration(
		Modifier modifiers
	) //throws RecognitionException, TokenStreamException
{
		TypeDecl result;
		
		result = null;
		
		try {      // for error handling
			match(INTERFACE);
			match(IDENTIFIER);
			{
				switch ( LA(1) )
				{
				case LANGLE:
				{
					type_parameter_list();
					break;
				}
				case LBRACE:
				case WHERE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==WHERE))
					{
						type_parameter_constraints_clause();
					}
					else
					{
						goto _loop240_breakloop;
					}
					
				}
_loop240_breakloop:				;
			}    // ( ... )*
			interface_body();
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case EOF:
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case NEW:
				case DELEGATE:
				case RBRACE:
				case CONST:
				case STATIC:
				case NAMESPACE:
				case PUBLIC:
				case PROTECTED:
				case INTERNAL:
				case PRIVATE:
				case ABSTRACT:
				case SEALED:
				case READONLY:
				case VOLATILE:
				case VIRTUAL:
				case OVERRIDE:
				case EXTERN:
				case CLASS:
				case STRUCT:
				case EVENT:
				case IMPLICIT:
				case EXPLICIT:
				case TILDE:
				case INTERFACE:
				case ENUM:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_50_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeDecl  enum_declaration(
		Modifier modifiers
	) //throws RecognitionException, TokenStreamException
{
		TypeDecl result;
		
		result = null;
		
		try {      // for error handling
			match(ENUM);
			match(IDENTIFIER);
			{
				switch ( LA(1) )
				{
				case COLON:
				{
					enum_base();
					break;
				}
				case LBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			enum_body();
			{
				switch ( LA(1) )
				{
				case SEMI:
				{
					match(SEMI);
					break;
				}
				case EOF:
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case NEW:
				case DELEGATE:
				case RBRACE:
				case CONST:
				case STATIC:
				case NAMESPACE:
				case PUBLIC:
				case PROTECTED:
				case INTERNAL:
				case PRIVATE:
				case ABSTRACT:
				case SEALED:
				case READONLY:
				case VOLATILE:
				case VIRTUAL:
				case OVERRIDE:
				case EXTERN:
				case CLASS:
				case STRUCT:
				case EVENT:
				case IMPLICIT:
				case EXPLICIT:
				case TILDE:
				case INTERFACE:
				case ENUM:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_50_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeDecl  delegate_declaration(
		Modifier modifiers
	) //throws RecognitionException, TokenStreamException
{
		TypeDecl result;
		
		IToken  t = null;
		result = null; Typ retType = null; Ident name = null; List<Collections.Tuple<Ident, int>> typeParamNames = null; List<Param> pars = null; List<TypeParam> typeParams = null;
		
		try {      // for error handling
			t = LT(1);
			match(DELEGATE);
			retType=type();
			name=identifier();
			{
				switch ( LA(1) )
				{
				case LANGLE:
				{
					typeParamNames=type_parameter_list();
					break;
				}
				case LPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case REF:
				case OUT:
				{
					pars=formal_parameter_list();
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RPAREN);
			typeParams=type_parameter_constraints_clauses();
			match(SEMI);
			if (0==inputState.guessing)
			{
				result = new Delegate(t, modifiers, retType, name, typeParamNames, pars, typeParams);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_50_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Modifier  modifier() //throws RecognitionException, TokenStreamException
{
		Modifier result;
		
		result = Modifier.None;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case NEW:
			{
				match(NEW);
				if (0==inputState.guessing)
				{
					result = Modifier.New;
				}
				break;
			}
			case PUBLIC:
			{
				match(PUBLIC);
				if (0==inputState.guessing)
				{
					result = Modifier.Public;
				}
				break;
			}
			case PROTECTED:
			{
				match(PROTECTED);
				if (0==inputState.guessing)
				{
					result = Modifier.Protected;
				}
				break;
			}
			case INTERNAL:
			{
				match(INTERNAL);
				if (0==inputState.guessing)
				{
					result = Modifier.Internal;
				}
				break;
			}
			case PRIVATE:
			{
				match(PRIVATE);
				if (0==inputState.guessing)
				{
					result = Modifier.Private;
				}
				break;
			}
			case ABSTRACT:
			{
				match(ABSTRACT);
				if (0==inputState.guessing)
				{
					result = Modifier.Abstract;
				}
				break;
			}
			case SEALED:
			{
				match(SEALED);
				if (0==inputState.guessing)
				{
					result = Modifier.Sealed;
				}
				break;
			}
			case STATIC:
			{
				match(STATIC);
				if (0==inputState.guessing)
				{
					result = Modifier.Static;
				}
				break;
			}
			case READONLY:
			{
				match(READONLY);
				if (0==inputState.guessing)
				{
					result = Modifier.Readonly;
				}
				break;
			}
			case VOLATILE:
			{
				match(VOLATILE);
				if (0==inputState.guessing)
				{
					result = Modifier.Volatile;
				}
				break;
			}
			case VIRTUAL:
			{
				match(VIRTUAL);
				if (0==inputState.guessing)
				{
					result = Modifier.Virtual;
				}
				break;
			}
			case OVERRIDE:
			{
				match(OVERRIDE);
				if (0==inputState.guessing)
				{
					result = Modifier.Override;
				}
				break;
			}
			case EXTERN:
			{
				match(EXTERN);
				if (0==inputState.guessing)
				{
					result = Modifier.Extern;
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_51_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Typ>  class_base() //throws RecognitionException, TokenStreamException
{
		List<Typ> result;
		
		result = null; Typ typ = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Typ>();
			}
			match(COLON);
			typ=type();
			if (0==inputState.guessing)
			{
				result.Add(typ);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						typ=type();
						if (0==inputState.guessing)
						{
							result.Add(typ);
						}
					}
					else
					{
						goto _loop187_breakloop;
					}
					
				}
_loop187_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_52_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Member>  class_body() //throws RecognitionException, TokenStreamException
{
		List<Member> result;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		IToken  t4 = null;
		IToken  t5 = null;
		result = null; Modifier mods = Modifier.None; Typ typ = null; List<Collections.Tuple<Ident, Expr>> vars = null; Ident name = null; List<Param> pars = null; CtorInit ctorInit = null; List<Stmt> body = null; Expr expr = null; Collections.Tuple<Ident, Expr> var = new Collections.Tuple<Ident, Expr>(); MethodHeader head = null; TypeDecl decl = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<Member>();
			}
			match(LBRACE);
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_51_.member(LA(1))))
					{
						mods=modifiers();
						{
							switch ( LA(1) )
							{
							case CONST:
							{
								t1 = LT(1);
								match(CONST);
								typ=type();
								vars=variable_declarators();
								match(SEMI);
								if (0==inputState.guessing)
								{
									NotDone(t1, "constant members");
								}
								break;
							}
							case EVENT:
							{
								t2 = LT(1);
								match(EVENT);
								typ=type();
								vars=variable_declarators();
								match(SEMI);
								if (0==inputState.guessing)
								{
									NotDone(t2, "event members");
								}
								break;
							}
							case IMPLICIT:
							case EXPLICIT:
							{
								{
									switch ( LA(1) )
									{
									case IMPLICIT:
									{
										t3 = LT(1);
										match(IMPLICIT);
										break;
									}
									case EXPLICIT:
									{
										t4 = LT(1);
										match(EXPLICIT);
										break;
									}
									default:
									{
										throw new NoViableAltException(LT(1), getFilename());
									}
									 }
								}
								typ=type();
								match(LPAREN);
								{
									switch ( LA(1) )
									{
									case IDENTIFIER:
									case BOOL:
									case DECIMAL:
									case SBYTE:
									case BYTE:
									case SHORT:
									case USHORT:
									case INT:
									case UINT:
									case LONG:
									case ULONG:
									case CHAR:
									case FLOAT:
									case DOUBLE:
									case OBJECT:
									case STRING:
									case VOID:
									case REF:
									case OUT:
									{
										pars=formal_parameter_list();
										break;
									}
									case RPAREN:
									{
										break;
									}
									default:
									{
										throw new NoViableAltException(LT(1), getFilename());
									}
									 }
								}
								match(RPAREN);
								body=method_body();
								if (0==inputState.guessing)
								{
									NotDone(t3 != null ? t3: t4, "conversion members");
								}
								break;
							}
							case TILDE:
							{
								t5 = LT(1);
								match(TILDE);
								name=identifier();
								match(LPAREN);
								match(RPAREN);
								body=method_body();
								if (0==inputState.guessing)
								{
									NotDone(t5, "destructors");
								}
								break;
							}
							case DELEGATE:
							case CLASS:
							case STRUCT:
							case INTERFACE:
							case ENUM:
							{
								decl=nested_type_declaration(mods);
								if (0==inputState.guessing)
								{
									result.Add(new NestedType(decl));
								}
								break;
							}
							default:
								bool synPredMatched200 = false;
								if (((LA(1)==IDENTIFIER)))
								{
									int _m200 = mark();
									synPredMatched200 = true;
									inputState.guessing++;
									try {
										{
											match(IDENTIFIER);
											match(LPAREN);
										}
									}
									catch (RecognitionException)
									{
										synPredMatched200 = false;
									}
									rewind(_m200);
									inputState.guessing--;
								}
								if ( synPredMatched200 )
								{
									name=identifier();
									match(LPAREN);
									{
										switch ( LA(1) )
										{
										case IDENTIFIER:
										case BOOL:
										case DECIMAL:
										case SBYTE:
										case BYTE:
										case SHORT:
										case USHORT:
										case INT:
										case UINT:
										case LONG:
										case ULONG:
										case CHAR:
										case FLOAT:
										case DOUBLE:
										case OBJECT:
										case STRING:
										case VOID:
										case REF:
										case OUT:
										{
											pars=formal_parameter_list();
											break;
										}
										case RPAREN:
										{
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
									match(RPAREN);
									{
										switch ( LA(1) )
										{
										case COLON:
										{
											ctorInit=constructor_initializer();
											break;
										}
										case SEMI:
										case LBRACE:
										{
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
									body=method_body();
									if (0==inputState.guessing)
									{
										result.Add(new Constructor(mods, name, pars, ctorInit, body));
									}
								}
								else if ((tokenSet_16_.member(LA(1)))) {
									typ=type();
									{
										switch ( LA(1) )
										{
										case IDENTIFIER:
										{
											name=identifier();
											{
												switch ( LA(1) )
												{
												case COMMA:
												case SEMI:
												case ASSIGN:
												{
													{
														switch ( LA(1) )
														{
														case ASSIGN:
														{
															match(ASSIGN);
															expr=variable_initializer();
															if (0==inputState.guessing)
															{
																result.Add(new Field(mods, typ, name, expr));
															}
															break;
														}
														case COMMA:
														case SEMI:
														{
															if (0==inputState.guessing)
															{
																result.Add(new Field(mods, typ, name, null));
															}
															break;
														}
														default:
														{
															throw new NoViableAltException(LT(1), getFilename());
														}
														 }
													}
													{    // ( ... )*
														for (;;)
														{
															if ((LA(1)==COMMA))
															{
																match(COMMA);
																var=variable_declarator();
																if (0==inputState.guessing)
																{
																	result.Add(new Field(mods, typ, var.fst, var.snd));
																}
															}
															else
															{
																goto _loop207_breakloop;
															}
															
														}
_loop207_breakloop:														;
													}    // ( ... )*
													match(SEMI);
													break;
												}
												case LANGLE:
												case LPAREN:
												{
													head=method_header(typ, name);
													body=method_body();
													if (0==inputState.guessing)
													{
														result.Add(new Method(mods, head, body));
													}
													break;
												}
												case LBRACE:
												{
													match(LBRACE);
													accessor_declarations();
													match(RBRACE);
													if (0==inputState.guessing)
													{
														NotDone(typ.Token, "property members");
													}
													break;
												}
												default:
												{
													throw new NoViableAltException(LT(1), getFilename());
												}
												 }
											}
											break;
										}
										case THIS:
										{
											match(THIS);
											match(LBRACK);
											pars=formal_parameter_list();
											match(RBRACK);
											match(LBRACE);
											accessor_declarations();
											match(RBRACE);
											if (0==inputState.guessing)
											{
												NotDone(typ.Token, "indexer members");
											}
											break;
										}
										case OPERATOR:
										{
											match(OPERATOR);
											{
												switch ( LA(1) )
												{
												case PLUS:
												{
													match(PLUS);
													break;
												}
												case MINUS:
												{
													match(MINUS);
													break;
												}
												case LNOT:
												{
													match(LNOT);
													break;
												}
												case NOT:
												{
													match(NOT);
													break;
												}
												case INC:
												{
													match(INC);
													break;
												}
												case DEC:
												{
													match(DEC);
													break;
												}
												case TRUE:
												{
													match(TRUE);
													break;
												}
												case FALSE:
												{
													match(FALSE);
													break;
												}
												case TIMES:
												{
													match(TIMES);
													break;
												}
												case QUOT:
												{
													match(QUOT);
													break;
												}
												case REM:
												{
													match(REM);
													break;
												}
												case AND:
												{
													match(AND);
													break;
												}
												case OR:
												{
													match(OR);
													break;
												}
												case XOR:
												{
													match(XOR);
													break;
												}
												case SHL:
												{
													match(SHL);
													break;
												}
												case SHR:
												{
													match(SHR);
													break;
												}
												case EQ:
												{
													match(EQ);
													break;
												}
												case NE:
												{
													match(NE);
													break;
												}
												case LANGLE:
												{
													match(LANGLE);
													break;
												}
												case RANGLE:
												{
													match(RANGLE);
													break;
												}
												case LE:
												{
													match(LE);
													break;
												}
												case GE:
												{
													match(GE);
													break;
												}
												default:
												{
													throw new NoViableAltException(LT(1), getFilename());
												}
												 }
											}
											match(LPAREN);
											{
												switch ( LA(1) )
												{
												case IDENTIFIER:
												case BOOL:
												case DECIMAL:
												case SBYTE:
												case BYTE:
												case SHORT:
												case USHORT:
												case INT:
												case UINT:
												case LONG:
												case ULONG:
												case CHAR:
												case FLOAT:
												case DOUBLE:
												case OBJECT:
												case STRING:
												case VOID:
												case REF:
												case OUT:
												{
													pars=formal_parameter_list();
													break;
												}
												case RPAREN:
												{
													break;
												}
												default:
												{
													throw new NoViableAltException(LT(1), getFilename());
												}
												 }
											}
											match(RPAREN);
											body=method_body();
											if (0==inputState.guessing)
											{
												NotDone(typ.Token, "operator members");
											}
											break;
										}
										default:
										{
											throw new NoViableAltException(LT(1), getFilename());
										}
										 }
									}
								}
							else
							{
								throw new NoViableAltException(LT(1), getFilename());
							}
							break; }
						}
					}
					else
					{
						goto _loop212_breakloop;
					}
					
				}
_loop212_breakloop:				;
			}    // ( ... )*
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_53_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public Collections.Tuple<Ident,int>  type_parameter() //throws RecognitionException, TokenStreamException
{
            Collections.Tuple<Ident,int> result;
		
		result = new Collections.Tuple<Ident,int>(); Ident typeParam = null; int levelKind = 0;
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==ESCAPE))
					{
						match(ESCAPE);
						if (0==inputState.guessing)
						{
							levelKind++;
						}
					}
					else
					{
						goto _loop184_breakloop;
					}
					
				}
_loop184_breakloop:				;
			}    // ( ... )*
			typeParam=identifier();
			if (0==inputState.guessing)
			{
				result = new Collections.Tuple<Ident,int>(typeParam, levelKind);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_54_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeParam  type_parameter_constraints_clause() //throws RecognitionException, TokenStreamException
{
		TypeParam result;
		
		IToken  t = null;
		result = null; Ident name = null; List<TypeParamCon> typeParamCons = null;
		
		try {      // for error handling
			t = LT(1);
			match(WHERE);
			name=identifier();
			match(COLON);
			typeParamCons=type_parameter_constraints();
			if (0==inputState.guessing)
			{
				result = new TypeParam(t, name, typeParamCons);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_55_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<TypeParamCon>  type_parameter_constraints() //throws RecognitionException, TokenStreamException
{
		List<TypeParamCon> result;
		
		result = null; TypeParamCon typeParamCon = null;
		
		try {      // for error handling
			if (0==inputState.guessing)
			{
				result = new List<TypeParamCon>();
			}
			typeParamCon=type_parameter_constraint();
			if (0==inputState.guessing)
			{
				result.Add(typeParamCon);
			}
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						typeParamCon=type_parameter_constraint();
						if (0==inputState.guessing)
						{
							result.Add(typeParamCon);
						}
					}
					else
					{
						goto _loop194_breakloop;
					}
					
				}
_loop194_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_55_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public TypeParamCon  type_parameter_constraint() //throws RecognitionException, TokenStreamException
{
		TypeParamCon result;
		
		IToken  t1 = null;
		IToken  t2 = null;
		IToken  t3 = null;
		result = null; Typ typ = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case IDENTIFIER:
			case BOOL:
			case DECIMAL:
			case SBYTE:
			case BYTE:
			case SHORT:
			case USHORT:
			case INT:
			case UINT:
			case LONG:
			case ULONG:
			case CHAR:
			case FLOAT:
			case DOUBLE:
			case OBJECT:
			case STRING:
			case VOID:
			{
				typ=type();
				if (0==inputState.guessing)
				{
					result = new TypeParamTypeCon(typ);
				}
				break;
			}
			case CLASS:
			{
				t1 = LT(1);
				match(CLASS);
				if (0==inputState.guessing)
				{
					result = new TypeParamClassCon(t1);
				}
				break;
			}
			case STRUCT:
			{
				t2 = LT(1);
				match(STRUCT);
				if (0==inputState.guessing)
				{
					result = new TypeParamStructCon(t2);
				}
				break;
			}
			case NEW:
			{
				t3 = LT(1);
				match(NEW);
				match(LPAREN);
				match(RPAREN);
				if (0==inputState.guessing)
				{
					result = new TypeParamCtorCon(t3);
				}
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_56_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public CtorInit  constructor_initializer() //throws RecognitionException, TokenStreamException
{
		CtorInit result;
		
		IToken  t = null;
		result = null; bool b = false; List<Arg> args = null;
		
		try {      // for error handling
			t = LT(1);
			match(COLON);
			{
				switch ( LA(1) )
				{
				case THIS:
				{
					match(THIS);
					if (0==inputState.guessing)
					{
						b = false;
					}
					break;
				}
				case BASE:
				{
					match(BASE);
					if (0==inputState.guessing)
					{
						b = true;
					}
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case FALSE:
				case TRUE:
				case INT32_DECIMAL_INTEGER_LITERAL:
				case INT64_DECIMAL_INTEGER_LITERAL:
				case UINT32_DECIMAL_INTEGER_LITERAL:
				case UINT64_DECIMAL_INTEGER_LITERAL:
				case INT32_HEXADECIMAL_INTEGER_LITERAL:
				case INT64_HEXADECIMAL_INTEGER_LITERAL:
				case UINT32_HEXADECIMAL_INTEGER_LITERAL:
				case UINT64_HEXADECIMAL_INTEGER_LITERAL:
				case FLOAT32_REAL_LITERAL:
				case FLOAT64_REAL_LITERAL:
				case DECIMAL_REAL_LITERAL:
				case CHARACTER_LITERAL:
				case STRING_LITERAL:
				case NULL:
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case REF:
				case OUT:
				case NEW:
				case LPAREN:
				case INC:
				case DEC:
				case THIS:
				case BASE:
				case TYPEOF:
				case DEFAULT:
				case DELEGATE:
				case LMETA:
				case PLUS:
				case MINUS:
				case LNOT:
				case NOT:
				case ESCAPE:
				case LIFT:
				{
					args=argument_list();
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RPAREN);
			if (0==inputState.guessing)
			{
				result = new CtorInit(t, b, args);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_2_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public List<Stmt>  method_body() //throws RecognitionException, TokenStreamException
{
		List<Stmt> result;
		
		result = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case LBRACE:
			{
				result=block();
				break;
			}
			case SEMI:
			{
				match(SEMI);
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_57_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public MethodHeader  method_header(
		Typ retType, Ident name
	) //throws RecognitionException, TokenStreamException
{
		MethodHeader result;
		
		result = null; List<Collections.Tuple<Ident,int>> typeParamNames = null; List<Param> pars = null; List<TypeParam> typeParams = null;
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case LANGLE:
				{
					typeParamNames=type_parameter_list();
					break;
				}
				case LPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(LPAREN);
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case REF:
				case OUT:
				{
					pars=formal_parameter_list();
					break;
				}
				case RPAREN:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RPAREN);
			typeParams=type_parameter_constraints_clauses();
			if (0==inputState.guessing)
			{
				result = new MethodHeader(retType, name, typeParamNames, pars, typeParams);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_2_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public void accessor_declarations() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			{ // ( ... )+
				int _cnt222=0;
				for (;;)
				{
					if ((LA(1)==IDENTIFIER))
					{
						match(IDENTIFIER);
						method_body();
					}
					else
					{
						if (_cnt222 >= 1) { goto _loop222_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
					}
					
					_cnt222++;
				}
_loop222_breakloop:				;
			}    // ( ... )+
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_35_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public Param  fixed_parameter() //throws RecognitionException, TokenStreamException
{
		Param result;
		
		result = null; Dir dir = Dir.In; Typ typ = null; Ident name = null;
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case REF:
				{
					match(REF);
					if (0==inputState.guessing)
					{
						dir = Dir.InOut;
					}
					break;
				}
				case OUT:
				{
					match(OUT);
					if (0==inputState.guessing)
					{
						dir = Dir.Out;
					}
					break;
				}
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			typ=type();
			name=identifier();
			if (0==inputState.guessing)
			{
				result = new Param(dir, typ, name);
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_58_);
			}
			else
			{
				throw ex;
			}
		}
		return result;
	}
	
	public void interface_body() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LBRACE);
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_59_.member(LA(1))))
					{
						interface_member_declaration();
					}
					else
					{
						goto _loop244_breakloop;
					}
					
				}
_loop244_breakloop:				;
			}    // ( ... )*
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_53_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void interface_member_declaration() //throws RecognitionException, TokenStreamException
{
		
		Typ typ = null; Ident name = null;
		
		try {      // for error handling
			{
				switch ( LA(1) )
				{
				case NEW:
				{
					match(NEW);
					break;
				}
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				case EVENT:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				case BOOL:
				case DECIMAL:
				case SBYTE:
				case BYTE:
				case SHORT:
				case USHORT:
				case INT:
				case UINT:
				case LONG:
				case ULONG:
				case CHAR:
				case FLOAT:
				case DOUBLE:
				case OBJECT:
				case STRING:
				case VOID:
				{
					typ=type();
					{
						switch ( LA(1) )
						{
						case IDENTIFIER:
						{
							name=identifier();
							{
								switch ( LA(1) )
								{
								case LANGLE:
								case LPAREN:
								{
									method_header(typ, name);
									match(SEMI);
									break;
								}
								case LBRACE:
								{
									match(LBRACE);
									{ // ( ... )+
										int _cnt251=0;
										for (;;)
										{
											switch ( LA(1) )
											{
											case GET:
											{
												match(GET);
												break;
											}
											case SET:
											{
												match(SET);
												break;
											}
											default:
											{
												if (_cnt251 >= 1) { goto _loop251_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
											}
											break; }
											_cnt251++;
										}
_loop251_breakloop:										;
									}    // ( ... )+
									match(RBRACE);
									break;
								}
								default:
								{
									throw new NoViableAltException(LT(1), getFilename());
								}
								 }
							}
							break;
						}
						case THIS:
						{
							match(THIS);
							match(LBRACK);
							formal_parameter_list();
							match(RBRACK);
							match(LBRACE);
							{ // ( ... )+
								int _cnt253=0;
								for (;;)
								{
									if ((LA(1)==IDENTIFIER))
									{
										match(IDENTIFIER);
										match(SEMI);
									}
									else
									{
										if (_cnt253 >= 1) { goto _loop253_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
									}
									
									_cnt253++;
								}
_loop253_breakloop:								;
							}    // ( ... )+
							match(RBRACE);
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					break;
				}
				case EVENT:
				{
					match(EVENT);
					type();
					match(IDENTIFIER);
					match(SEMI);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_60_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void enum_base() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(COLON);
			{
				switch ( LA(1) )
				{
				case SBYTE:
				{
					match(SBYTE);
					break;
				}
				case BYTE:
				{
					match(BYTE);
					break;
				}
				case SHORT:
				{
					match(SHORT);
					break;
				}
				case USHORT:
				{
					match(USHORT);
					break;
				}
				case INT:
				{
					match(INT);
					break;
				}
				case UINT:
				{
					match(UINT);
					break;
				}
				case LONG:
				{
					match(LONG);
					break;
				}
				case ULONG:
				{
					match(ULONG);
					break;
				}
				case CHAR:
				{
					match(CHAR);
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_61_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void enum_body() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(LBRACE);
			{
				switch ( LA(1) )
				{
				case IDENTIFIER:
				{
					enum_member_declarations();
					break;
				}
				case RBRACE:
				{
					break;
				}
				default:
				{
					throw new NoViableAltException(LT(1), getFilename());
				}
				 }
			}
			match(RBRACE);
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_53_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void enum_member_declarations() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			enum_member_declaration();
			{    // ( ... )*
				for (;;)
				{
					if ((LA(1)==COMMA))
					{
						match(COMMA);
						enum_member_declaration();
					}
					else
					{
						goto _loop263_breakloop;
					}
					
				}
_loop263_breakloop:				;
			}    // ( ... )*
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_35_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	public void enum_member_declaration() //throws RecognitionException, TokenStreamException
{
		
		
		try {      // for error handling
			match(IDENTIFIER);
			match(ASSIGN);
			literal();
		}
		catch (RecognitionException ex)
		{
			if (0 == inputState.guessing)
			{
				reportError(ex);
				recover(ex,tokenSet_62_);
			}
			else
			{
				throw ex;
			}
		}
	}
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""false""",
		@"""true""",
		@"""INT32_DECIMAL_INTEGER_LITERAL""",
		@"""INT64_DECIMAL_INTEGER_LITERAL""",
		@"""UINT32_DECIMAL_INTEGER_LITERAL""",
		@"""UINT64_DECIMAL_INTEGER_LITERAL""",
		@"""INT32_HEXADECIMAL_INTEGER_LITERAL""",
		@"""INT64_HEXADECIMAL_INTEGER_LITERAL""",
		@"""UINT32_HEXADECIMAL_INTEGER_LITERAL""",
		@"""UINT64_HEXADECIMAL_INTEGER_LITERAL""",
		@"""FLOAT32_REAL_LITERAL""",
		@"""FLOAT64_REAL_LITERAL""",
		@"""DECIMAL_REAL_LITERAL""",
		@"""CHARACTER_LITERAL""",
		@"""STRING_LITERAL""",
		@"""null""",
		@"""IDENTIFIER""",
		@"""DOT""",
		@"""bool""",
		@"""decimal""",
		@"""sbyte""",
		@"""byte""",
		@"""short""",
		@"""ushort""",
		@"""int""",
		@"""uint""",
		@"""long""",
		@"""ulong""",
		@"""char""",
		@"""float""",
		@"""double""",
		@"""object""",
		@"""string""",
		@"""void""",
		@"""LBRACK""",
		@"""COMMA""",
		@"""RBRACK""",
		@"""LANGLE""",
		@"""RANGLE""",
		@"""ref""",
		@"""out""",
		@"""new""",
		@"""LPAREN""",
		@"""RPAREN""",
		@"""INC""",
		@"""DEC""",
		@"""this""",
		@"""base""",
		@"""typeof""",
		@"""default""",
		@"""delegate""",
		@"""LMETA""",
		@"""RMETA""",
		@"""COLON""",
		@"""SEMI""",
		@"""QUESTION""",
		@"""PLUS""",
		@"""MINUS""",
		@"""LNOT""",
		@"""NOT""",
		@"""ESCAPE""",
		@"""as""",
		@"""is""",
		@"""LBRACE""",
		@"""RBRACE""",
		@"""TIMES""",
		@"""QUOT""",
		@"""REM""",
		@"""AND""",
		@"""OR""",
		@"""XOR""",
		@"""ASSIGN""",
		@"""COND""",
		@"""LAND""",
		@"""LOR""",
		@"""SHL""",
		@"""EQ""",
		@"""NE""",
		@"""LE""",
		@"""GE""",
		@"""PLUS_ASSIGN""",
		@"""MINUS_ASSIGN""",
		@"""TIMES_ASSIGN""",
		@"""QUOT_ASSIGN""",
		@"""REM_ASSIGN""",
		@"""AND_ASSIGN""",
		@"""OR_ASSIGN""",
		@"""XOR_ASSIGN""",
		@"""SHL_ASSIGN""",
		@"""SHR_ASSIGN""",
		@"""ARROW""",
		@"""SEMI_ASSIGN""",
		@"""SHR""",
		@"""lift""",
		@"""const""",
		@"""if""",
		@"""else""",
		@"""switch""",
		@"""while""",
		@"""do""",
		@"""for""",
		@"""foreach""",
		@"""in""",
		@"""break""",
		@"""continue""",
		@"""goto""",
		@"""return""",
		@"""throw""",
		@"""try""",
		@"""checked""",
		@"""unchecked""",
		@"""lock""",
		@"""using""",
		@"""typeif""",
		@"""withtype""",
		@"""formember""",
		@"""static""",
		@"""case""",
		@"""catch""",
		@"""finally""",
		@"""namespace""",
		@"""public""",
		@"""protected""",
		@"""internal""",
		@"""private""",
		@"""abstract""",
		@"""sealed""",
		@"""readonly""",
		@"""volatile""",
		@"""virtual""",
		@"""override""",
		@"""extern""",
		@"""class""",
		@"""where""",
		@"""struct""",
		@"""operator""",
		@"""event""",
		@"""implicit""",
		@"""explicit""",
		@"""TILDE""",
		@"""interface""",
		@"""GET""",
		@"""SET""",
		@"""enum""",
		@"""fixed""",
		@"""params""",
		@"""sizeof""",
		@"""stackalloc""",
		@"""unsafe""",
		@"""NEW_LINE""",
		@"""WHITESPACE""",
		@"""COMMENT""",
		@"""SINGLE_LINE_COMMENT""",
		@"""DELIMITED_COMMENT""",
		@"""IDENTIFIER_OR_KEYWORD""",
		@"""LETTER_CHARACTER""",
		@"""DECIMAL_DIGIT_CHARACTER""",
		@"""NUMERIC_LITERAL""",
		@"""DECIMAL_DIGIT""",
		@"""HEX_DIGIT""",
		@"""LONG_SUFFIX""",
		@"""UNSIGNED_SUFFIX""",
		@"""UNSIGNED_LONG_SUFFIX""",
		@"""EXPONENT_PART""",
		@"""FLOAT_SUFFIX""",
		@"""DOUBLE_SUFFIX""",
		@"""DECIMAL_SUFFIX""",
		@"""SINGLE_CHARACTER"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 4540692476769337344L, 144115195592044542L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 4541818376677228544L, 4405562699774L, 2560L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 288230376151711744L, 8L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 289572604972367872L, 8L, 2560L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 4541818376677228544L, 7516188670L, 2560L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 289572330094460928L, 8L, 2560L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = { 1082209163046551552L, 3221188382L, 2560L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = { 1082209437926555648L, 3221188382L, 2560L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	private static long[] mk_tokenSet_8_()
	{
		long[] data = { 140737488355328L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_8_ = new BitSet(mk_tokenSet_8_());
	private static long[] mk_tokenSet_9_()
	{
		long[] data = { 141287244169216L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_9_ = new BitSet(mk_tokenSet_9_());
	private static long[] mk_tokenSet_10_()
	{
		long[] data = { 360430356945436672L, 16L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_10_ = new BitSet(mk_tokenSet_10_());
	private static long[] mk_tokenSet_11_()
	{
		long[] data = { 71037522123161584L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_11_ = new BitSet(mk_tokenSet_11_());
	private static long[] mk_tokenSet_12_()
	{
		long[] data = { 4539777408215023616L, 7516188662L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_12_ = new BitSet(mk_tokenSet_12_());
	private static long[] mk_tokenSet_13_()
	{
		long[] data = { 288372213151694848L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_13_ = new BitSet(mk_tokenSet_13_());
	private static long[] mk_tokenSet_14_()
	{
		long[] data = { 4540692476769337344L, 7516188662L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_14_ = new BitSet(mk_tokenSet_14_());
	private static long[] mk_tokenSet_15_()
	{
		long[] data = { 141836999983104L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_15_ = new BitSet(mk_tokenSet_15_());
	private static long[] mk_tokenSet_16_()
	{
		long[] data = { 274874761216L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_16_ = new BitSet(mk_tokenSet_16_());
	private static long[] mk_tokenSet_17_()
	{
		long[] data = { -792809181401841680L, 72053101502136329L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_17_ = new BitSet(mk_tokenSet_17_());
	private static long[] mk_tokenSet_18_()
	{
		long[] data = { -26388279066640L, -1152925903727104001L, 652799L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_18_ = new BitSet(mk_tokenSet_18_());
	private static long[] mk_tokenSet_19_()
	{
		long[] data = { -1081039557553553424L, 8589934593L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_19_ = new BitSet(mk_tokenSet_19_());
	private static long[] mk_tokenSet_20_()
	{
		long[] data = { 72057594037927936L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_20_ = new BitSet(mk_tokenSet_20_());
	private static long[] mk_tokenSet_21_()
	{
		long[] data = { 4539777408215023616L, 7516188438L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_21_ = new BitSet(mk_tokenSet_21_());
	private static long[] mk_tokenSet_22_()
	{
		long[] data = { 1081012894394482688L, 7516188438L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_22_ = new BitSet(mk_tokenSet_22_());
	private static long[] mk_tokenSet_23_()
	{
		long[] data = { 1081012894394482688L, 3221188374L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_23_ = new BitSet(mk_tokenSet_23_());
	private static long[] mk_tokenSet_24_()
	{
		long[] data = { 1081006297324716032L, 3220401936L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_24_ = new BitSet(mk_tokenSet_24_());
	private static long[] mk_tokenSet_25_()
	{
		long[] data = { 1081013169272389632L, 3221188374L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_25_ = new BitSet(mk_tokenSet_25_());
	private static long[] mk_tokenSet_26_()
	{
		long[] data = { 1081006297324716032L, 3220205328L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_26_ = new BitSet(mk_tokenSet_26_());
	private static long[] mk_tokenSet_27_()
	{
		long[] data = { 1081006297324716032L, 3220205072L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_27_ = new BitSet(mk_tokenSet_27_());
	private static long[] mk_tokenSet_28_()
	{
		long[] data = { 1081006297324716032L, 3220204048L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_28_ = new BitSet(mk_tokenSet_28_());
	private static long[] mk_tokenSet_29_()
	{
		long[] data = { 1081006297324716032L, 3220203536L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_29_ = new BitSet(mk_tokenSet_29_());
	private static long[] mk_tokenSet_30_()
	{
		long[] data = { 1081006297324716032L, 3220195344L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_30_ = new BitSet(mk_tokenSet_30_());
	private static long[] mk_tokenSet_31_()
	{
		long[] data = { 1081006297324716032L, 3220178960L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_31_ = new BitSet(mk_tokenSet_31_());
	private static long[] mk_tokenSet_32_()
	{
		long[] data = { 504545545021292544L, 3220178960L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_32_ = new BitSet(mk_tokenSet_32_());
	private static long[] mk_tokenSet_33_()
	{
		long[] data = { 288230376151711744L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_33_ = new BitSet(mk_tokenSet_33_());
	private static long[] mk_tokenSet_34_()
	{
		long[] data = { -720751587363913744L, 72053187401482265L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_34_ = new BitSet(mk_tokenSet_34_());
	private static long[] mk_tokenSet_35_()
	{
		long[] data = { 0L, 16L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_35_ = new BitSet(mk_tokenSet_35_());
	private static long[] mk_tokenSet_36_()
	{
		long[] data = { -720751587363913744L, 648513939704905753L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_36_ = new BitSet(mk_tokenSet_36_());
	private static long[] mk_tokenSet_37_()
	{
		long[] data = { 144185556820033536L, 8L, 512L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_37_ = new BitSet(mk_tokenSet_37_());
	private static long[] mk_tokenSet_38_()
	{
		long[] data = { 288371113640067072L, 8L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_38_ = new BitSet(mk_tokenSet_38_());
	private static long[] mk_tokenSet_39_()
	{
		long[] data = { 288230925907525632L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_39_ = new BitSet(mk_tokenSet_39_());
	private static long[] mk_tokenSet_40_()
	{
		long[] data = { 288230925907525632L, 16L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_40_ = new BitSet(mk_tokenSet_40_());
	private static long[] mk_tokenSet_41_()
	{
		long[] data = { -720751587363913744L, 936744315856617497L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_41_ = new BitSet(mk_tokenSet_41_());
	private static long[] mk_tokenSet_42_()
	{
		long[] data = { 2L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_42_ = new BitSet(mk_tokenSet_42_());
	private static long[] mk_tokenSet_43_()
	{
		long[] data = { 18049582881570818L, -1080863910568919040L, 591359L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_43_ = new BitSet(mk_tokenSet_43_());
	private static long[] mk_tokenSet_44_()
	{
		long[] data = { 18049582881570816L, -1080863910568919040L, 591359L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_44_ = new BitSet(mk_tokenSet_44_());
	private static long[] mk_tokenSet_45_()
	{
		long[] data = { 2L, 16L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_45_ = new BitSet(mk_tokenSet_45_());
	private static long[] mk_tokenSet_46_()
	{
		long[] data = { 18049582881570818L, -1080863910568919024L, 591359L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_46_ = new BitSet(mk_tokenSet_46_());
	private static long[] mk_tokenSet_47_()
	{
		long[] data = { 18049582881570818L, -1076360310941548544L, 591359L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_47_ = new BitSet(mk_tokenSet_47_());
	private static long[] mk_tokenSet_48_()
	{
		long[] data = { 35184372088832L, -2233785415175766016L, 255L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_48_ = new BitSet(mk_tokenSet_48_());
	private static long[] mk_tokenSet_49_()
	{
		long[] data = { 18014673384243200L, 17179869184L, 652544L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_49_ = new BitSet(mk_tokenSet_49_());
	private static long[] mk_tokenSet_50_()
	{
		long[] data = { 18049857756332034L, -1080863893389049840L, 652799L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_50_ = new BitSet(mk_tokenSet_50_());
	private static long[] mk_tokenSet_51_()
	{
		long[] data = { 18049857756332032L, -2233785397995896832L, 652799L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_51_ = new BitSet(mk_tokenSet_51_());
	private static long[] mk_tokenSet_52_()
	{
		long[] data = { 0L, 8L, 512L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_52_ = new BitSet(mk_tokenSet_52_());
	private static long[] mk_tokenSet_53_()
	{
		long[] data = { 306280233908043778L, -1080863893389049840L, 652799L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_53_ = new BitSet(mk_tokenSet_53_());
	private static long[] mk_tokenSet_54_()
	{
		long[] data = { 4947802324992L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_54_ = new BitSet(mk_tokenSet_54_());
	private static long[] mk_tokenSet_55_()
	{
		long[] data = { 288371113640067072L, 8L, 512L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_55_ = new BitSet(mk_tokenSet_55_());
	private static long[] mk_tokenSet_56_()
	{
		long[] data = { 288371663395880960L, 8L, 512L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_56_ = new BitSet(mk_tokenSet_56_());
	private static long[] mk_tokenSet_57_()
	{
		long[] data = { 18049857756332032L, -2233785397995896816L, 652799L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_57_ = new BitSet(mk_tokenSet_57_());
	private static long[] mk_tokenSet_58_()
	{
		long[] data = { 142386755796992L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_58_ = new BitSet(mk_tokenSet_58_());
	private static long[] mk_tokenSet_59_()
	{
		long[] data = { 35459246850048L, 0L, 4096L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_59_ = new BitSet(mk_tokenSet_59_());
	private static long[] mk_tokenSet_60_()
	{
		long[] data = { 35459246850048L, 16L, 4096L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_60_ = new BitSet(mk_tokenSet_60_());
	private static long[] mk_tokenSet_61_()
	{
		long[] data = { 0L, 8L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_61_ = new BitSet(mk_tokenSet_61_());
	private static long[] mk_tokenSet_62_()
	{
		long[] data = { 549755813888L, 16L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_62_ = new BitSet(mk_tokenSet_62_());
	
}
}
