// $ANTLR 2.7.5 (20050128): "metaphor.txt" -> "L.cs"$

	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using Metaphor.Collections;

namespace Metaphor.Compiler
{
	// Generate header specific to lexer CSharp file
	using System;
	using Stream                          = System.IO.Stream;
	using TextReader                      = System.IO.TextReader;
	using Hashtable                       = System.Collections.Hashtable;
	using Comparer                        = System.Collections.Comparer;
	
	using TokenStreamException            = antlr.TokenStreamException;
	using TokenStreamIOException          = antlr.TokenStreamIOException;
	using TokenStreamRecognitionException = antlr.TokenStreamRecognitionException;
	using CharStreamException             = antlr.CharStreamException;
	using CharStreamIOException           = antlr.CharStreamIOException;
	using ANTLRException                  = antlr.ANTLRException;
	using CharScanner                     = antlr.CharScanner;
	using InputBuffer                     = antlr.InputBuffer;
	using ByteBuffer                      = antlr.ByteBuffer;
	using CharBuffer                      = antlr.CharBuffer;
	using Token                           = antlr.Token;
	using IToken                          = antlr.IToken;
	using CommonToken                     = antlr.CommonToken;
	using SemanticException               = antlr.SemanticException;
	using RecognitionException            = antlr.RecognitionException;
	using NoViableAltForCharException     = antlr.NoViableAltForCharException;
	using MismatchedCharException         = antlr.MismatchedCharException;
	using TokenStream                     = antlr.TokenStream;
	using LexerSharedInputState           = antlr.LexerSharedInputState;
	using BitSet                          = antlr.collections.impl.BitSet;
	
	public 	class L : antlr.CharScanner	, TokenStream
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
		
		public L(Stream ins) : this(new ByteBuffer(ins))
		{
		}
		
		public L(TextReader r) : this(new CharBuffer(r))
		{
		}
		
		public L(InputBuffer ib)		 : this(new LexerSharedInputState(ib))
		{
		}
		
		public L(LexerSharedInputState state) : base(state)
		{
			initialize();
		}
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, (float) 0.4, null, Comparer.Default);
			literals.Add("byte", 25);
			literals.Add("extern", 135);
			literals.Add("public", 125);
			literals.Add("uint", 29);
			literals.Add("namespace", 124);
			literals.Add("case", 121);
			literals.Add("short", 26);
			literals.Add("while", 102);
			literals.Add("break", 107);
			literals.Add("new", 45);
			literals.Add("sealed", 130);
			literals.Add("withtype", 118);
			literals.Add("object", 35);
			literals.Add("sbyte", 24);
			literals.Add("readonly", 131);
			literals.Add("where", 137);
			literals.Add("checked", 113);
			literals.Add("stackalloc", 151);
			literals.Add("decimal", 23);
			literals.Add("typeof", 52);
			literals.Add("lock", 115);
			literals.Add("const", 98);
			literals.Add("unchecked", 114);
			literals.Add("float", 33);
			literals.Add("typeif", 117);
			literals.Add("return", 110);
			literals.Add("foreach", 105);
			literals.Add("throw", 111);
			literals.Add("using", 116);
			literals.Add("operator", 139);
			literals.Add("null", 19);
			literals.Add("fixed", 148);
			literals.Add("sizeof", 150);
			literals.Add("unsafe", 152);
			literals.Add("protected", 126);
			literals.Add("ref", 43);
			literals.Add("class", 136);
			literals.Add("base", 51);
			literals.Add("do", 103);
			literals.Add("event", 140);
			literals.Add("out", 44);
			literals.Add("ushort", 27);
			literals.Add("interface", 144);
			literals.Add("is", 66);
			literals.Add("internal", 127);
			literals.Add("explicit", 142);
			literals.Add("ulong", 31);
			literals.Add("if", 99);
			literals.Add("formember", 119);
			literals.Add("override", 134);
			literals.Add("double", 34);
			literals.Add("volatile", 132);
			literals.Add("as", 65);
			literals.Add("delegate", 54);
			literals.Add("implicit", 141);
			literals.Add("catch", 122);
			literals.Add("try", 112);
			literals.Add("params", 149);
			literals.Add("goto", 109);
			literals.Add("enum", 147);
			literals.Add("lift", 97);
			literals.Add("int", 28);
			literals.Add("for", 104);
			literals.Add("char", 32);
			literals.Add("private", 128);
			literals.Add("string", 36);
			literals.Add("default", 53);
			literals.Add("false", 4);
			literals.Add("this", 50);
			literals.Add("static", 120);
			literals.Add("abstract", 129);
			literals.Add("continue", 108);
			literals.Add("bool", 22);
			literals.Add("struct", 138);
			literals.Add("finally", 123);
			literals.Add("else", 100);
			literals.Add("in", 106);
			literals.Add("void", 37);
			literals.Add("switch", 101);
			literals.Add("true", 5);
			literals.Add("long", 30);
			literals.Add("virtual", 133);
		}
		
		override public IToken nextToken()			//throws TokenStreamException
		{
			IToken theRetToken = null;
tryAgain:
			for (;;)
			{
				IToken _token = null;
				int _ttype = Token.INVALID_TYPE;
				resetText();
				try     // for char stream error handling
				{
					try     // for lexical error handling
					{
						switch ( cached_LA1 )
						{
						case '\n':  case '\r':
						{
							mNEW_LINE(true);
							theRetToken = returnToken_;
							break;
						}
						case '\t':  case '\u000b':  case '\u000c':  case ' ':
						case '\u00a0':
						{
							mWHITESPACE(true);
							theRetToken = returnToken_;
							break;
						}
						case '@':  case 'A':  case 'B':  case 'C':
						case 'D':  case 'E':  case 'F':  case 'G':
						case 'H':  case 'I':  case 'J':  case 'K':
						case 'L':  case 'M':  case 'N':  case 'O':
						case 'P':  case 'Q':  case 'R':  case 'S':
						case 'T':  case 'U':  case 'V':  case 'W':
						case 'X':  case 'Y':  case 'Z':  case '_':
						case 'a':  case 'b':  case 'c':  case 'd':
						case 'e':  case 'f':  case 'g':  case 'h':
						case 'i':  case 'j':  case 'k':  case 'l':
						case 'm':  case 'n':  case 'o':  case 'p':
						case 'q':  case 'r':  case 's':  case 't':
						case 'u':  case 'v':  case 'w':  case 'x':
						case 'y':  case 'z':  case '\u00aa':  case '\u00b5':
						case '\u00ba':  case '\u00c0':  case '\u00c1':  case '\u00c2':
						case '\u00c3':  case '\u00c4':  case '\u00c5':  case '\u00c6':
						case '\u00c7':  case '\u00c8':  case '\u00c9':  case '\u00ca':
						case '\u00cb':  case '\u00cc':  case '\u00cd':  case '\u00ce':
						case '\u00cf':  case '\u00d0':  case '\u00d1':  case '\u00d2':
						case '\u00d3':  case '\u00d4':  case '\u00d5':  case '\u00d6':
						case '\u00d8':  case '\u00d9':  case '\u00da':  case '\u00db':
						case '\u00dc':  case '\u00dd':  case '\u00de':  case '\u00df':
						case '\u00e0':  case '\u00e1':  case '\u00e2':  case '\u00e3':
						case '\u00e4':  case '\u00e5':  case '\u00e6':  case '\u00e7':
						case '\u00e8':  case '\u00e9':  case '\u00ea':  case '\u00eb':
						case '\u00ec':  case '\u00ed':  case '\u00ee':  case '\u00ef':
						case '\u00f0':  case '\u00f1':  case '\u00f2':  case '\u00f3':
						case '\u00f4':  case '\u00f5':  case '\u00f6':  case '\u00f8':
						case '\u00f9':  case '\u00fa':  case '\u00fb':  case '\u00fc':
						case '\u00fd':  case '\u00fe':  case '\u00ff':
						{
							mIDENTIFIER(true);
							theRetToken = returnToken_;
							break;
						}
						case '.':  case '0':  case '1':  case '2':
						case '3':  case '4':  case '5':  case '6':
						case '7':  case '8':  case '9':
						{
							mNUMERIC_LITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						case '\'':
						{
							mCHARACTER_LITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						case '"':
						{
							mSTRING_LITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						case '{':
						{
							mLBRACE(true);
							theRetToken = returnToken_;
							break;
						}
						case '}':
						{
							mRBRACE(true);
							theRetToken = returnToken_;
							break;
						}
						case '[':
						{
							mLBRACK(true);
							theRetToken = returnToken_;
							break;
						}
						case ']':
						{
							mRBRACK(true);
							theRetToken = returnToken_;
							break;
						}
						case '(':
						{
							mLPAREN(true);
							theRetToken = returnToken_;
							break;
						}
						case ')':
						{
							mRPAREN(true);
							theRetToken = returnToken_;
							break;
						}
						case ',':
						{
							mCOMMA(true);
							theRetToken = returnToken_;
							break;
						}
						case ':':
						{
							mCOLON(true);
							theRetToken = returnToken_;
							break;
						}
						case '`':
						{
							mNOT(true);
							theRetToken = returnToken_;
							break;
						}
						case '?':
						{
							mCOND(true);
							theRetToken = returnToken_;
							break;
						}
						case '~':
						{
							mESCAPE(true);
							theRetToken = returnToken_;
							break;
						}
						default:
							if ((cached_LA1=='/') && (cached_LA2=='*'||cached_LA2=='/'))
							{
								mCOMMENT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (cached_LA2=='+')) {
								mINC(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (cached_LA2=='-')) {
								mDEC(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='&') && (cached_LA2=='&')) {
								mLAND(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (cached_LA2=='|')) {
								mLOR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (cached_LA2=='<')) {
								mSHL(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='>')) {
								mSHR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='=') && (cached_LA2=='=')) {
								mEQ(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='!') && (cached_LA2=='=')) {
								mNE(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (cached_LA2=='=')) {
								mLE(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (cached_LA2=='=')) {
								mGE(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (cached_LA2=='=')) {
								mPLUS_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (cached_LA2=='=')) {
								mMINUS_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='*') && (cached_LA2=='=')) {
								mTIMES_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='/') && (cached_LA2=='=')) {
								mQUOT_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='%') && (cached_LA2=='=')) {
								mREM_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='&') && (cached_LA2=='=')) {
								mAND_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (cached_LA2=='=')) {
								mOR_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='^') && (cached_LA2=='=')) {
								mXOR_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (cached_LA2=='>')) {
								mARROW(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (cached_LA2=='|')) {
								mLMETA(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (cached_LA2=='>')) {
								mRMETA(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1==';') && (cached_LA2=='=')) {
								mSEMI_ASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1==';') && (true)) {
								mSEMI(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='+') && (true)) {
								mPLUS(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='-') && (true)) {
								mMINUS(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='*') && (true)) {
								mTIMES(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='/') && (true)) {
								mQUOT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='%') && (true)) {
								mREM(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='&') && (true)) {
								mAND(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='|') && (true)) {
								mOR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='^') && (true)) {
								mXOR(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='!') && (true)) {
								mLNOT(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='=') && (true)) {
								mASSIGN(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='<') && (true)) {
								mLANGLE(true);
								theRetToken = returnToken_;
							}
							else if ((cached_LA1=='>') && (true)) {
								mRANGLE(true);
								theRetToken = returnToken_;
							}
						else
						{
							if (cached_LA1==EOF_CHAR) { uponEOF(); returnToken_ = makeToken(Token.EOF_TYPE); }
				else {throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());}
						}
						break; }
						if ( null==returnToken_ ) goto tryAgain; // found SKIP token
						_ttype = returnToken_.Type;
						returnToken_.Type = _ttype;
						return returnToken_;
					}
					catch (RecognitionException e) {
							throw new TokenStreamRecognitionException(e);
					}
				}
				catch (CharStreamException cse) {
					if ( cse is CharStreamIOException ) {
						throw new TokenStreamIOException(((CharStreamIOException)cse).io);
					}
					else {
						throw new TokenStreamException(cse.Message);
					}
				}
			}
		}
		
	public void mNEW_LINE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NEW_LINE;
		
		{
			if ((cached_LA1=='\r') && (cached_LA2=='\n'))
			{
				match('\u000D');
				match('\u000A');
			}
			else if ((cached_LA1=='\r') && (true)) {
				match('\u000D');
			}
			else if ((cached_LA1=='\n')) {
				match('\u000A');
			}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			
		}
		newline(); _ttype = Token.SKIP;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mWHITESPACE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = WHITESPACE;
		
		{ // ( ... )+
			int _cnt272=0;
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case ' ':
				{
					match('\u0020');
					break;
				}
				case '\u00a0':
				{
					match('\u00A0');
					break;
				}
				case '\t':
				{
					match('\u0009');
					break;
				}
				case '\u000b':
				{
					match('\u000B');
					break;
				}
				case '\u000c':
				{
					match('\u000C');
					break;
				}
				default:
				{
					if (_cnt272 >= 1) { goto _loop272_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				break; }
				_cnt272++;
			}
_loop272_breakloop:			;
		}    // ( ... )+
		_ttype = Token.SKIP;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COMMENT;
		
		{
			if ((cached_LA1=='/') && (cached_LA2=='/'))
			{
				mSINGLE_LINE_COMMENT(false);
			}
			else if ((cached_LA1=='/') && (cached_LA2=='*')) {
				mDELIMITED_COMMENT(false);
			}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			
		}
		_ttype = Token.SKIP;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mSINGLE_LINE_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SINGLE_LINE_COMMENT;
		
		match("//");
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_0_.member(cached_LA1)))
				{
					{
						match(tokenSet_0_);
					}
				}
				else
				{
					goto _loop278_breakloop;
				}
				
			}
_loop278_breakloop:			;
		}    // ( ... )*
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDELIMITED_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DELIMITED_COMMENT;
		
		match("/*");
		{    // ( ... )*
			for (;;)
			{
				if (((cached_LA1=='*') && ((cached_LA2 >= '\u0000' && cached_LA2 <= '\u00ff')))&&(LA(2) != '/'))
				{
					match('*');
				}
				else if ((cached_LA1=='\r') && (cached_LA2=='\n')) {
					match('\u000D');
					match('\u000A');
					newline();
				}
				else if ((cached_LA1=='\r') && ((cached_LA2 >= '\u0000' && cached_LA2 <= '\u00ff'))) {
					match('\u000D');
					newline();
				}
				else if ((cached_LA1=='\n')) {
					match('\u000A');
					newline();
				}
				else if ((tokenSet_1_.member(cached_LA1))) {
					{
						match(tokenSet_1_);
					}
				}
				else
				{
					goto _loop282_breakloop;
				}
				
			}
_loop282_breakloop:			;
		}    // ( ... )*
		match("*/");
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mIDENTIFIER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = IDENTIFIER;
		
		if ((cached_LA1=='@') && (tokenSet_2_.member(cached_LA2)))
		{
			int _saveIndex = 0;
			_saveIndex = text.Length;
			match('@');
			text.Length = _saveIndex;
			mIDENTIFIER_OR_KEYWORD(false);
		}
		else if ((cached_LA1=='@') && (cached_LA2=='"')) {
			int _saveIndex = 0;
			_saveIndex = text.Length;
			match('@');
			text.Length = _saveIndex;
			_saveIndex = text.Length;
			match('\"');
			text.Length = _saveIndex;
			{    // ( ... )*
				for (;;)
				{
					if ((cached_LA1=='"') && (cached_LA2=='"'))
					{
						match('\"');
						match('\"');
						text.Remove(text.Length - 1, 1);
					}
					else if ((tokenSet_3_.member(cached_LA1))) {
						{
							match(tokenSet_3_);
						}
					}
					else
					{
						goto _loop286_breakloop;
					}
					
				}
_loop286_breakloop:				;
			}    // ( ... )*
			_saveIndex = text.Length;
			match('\"');
			text.Length = _saveIndex;
			_ttype = STRING_LITERAL;
		}
		else if ((tokenSet_2_.member(cached_LA1))) {
			mIDENTIFIER_OR_KEYWORD(false);
			_ttype = testLiteralsTable(_ttype);
		}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mIDENTIFIER_OR_KEYWORD(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = IDENTIFIER_OR_KEYWORD;
		
		{
			switch ( cached_LA1 )
			{
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':  case 'G':  case 'H':
			case 'I':  case 'J':  case 'K':  case 'L':
			case 'M':  case 'N':  case 'O':  case 'P':
			case 'Q':  case 'R':  case 'S':  case 'T':
			case 'U':  case 'V':  case 'W':  case 'X':
			case 'Y':  case 'Z':  case 'a':  case 'b':
			case 'c':  case 'd':  case 'e':  case 'f':
			case 'g':  case 'h':  case 'i':  case 'j':
			case 'k':  case 'l':  case 'm':  case 'n':
			case 'o':  case 'p':  case 'q':  case 'r':
			case 's':  case 't':  case 'u':  case 'v':
			case 'w':  case 'x':  case 'y':  case 'z':
			case '\u00aa':  case '\u00b5':  case '\u00ba':  case '\u00c0':
			case '\u00c1':  case '\u00c2':  case '\u00c3':  case '\u00c4':
			case '\u00c5':  case '\u00c6':  case '\u00c7':  case '\u00c8':
			case '\u00c9':  case '\u00ca':  case '\u00cb':  case '\u00cc':
			case '\u00cd':  case '\u00ce':  case '\u00cf':  case '\u00d0':
			case '\u00d1':  case '\u00d2':  case '\u00d3':  case '\u00d4':
			case '\u00d5':  case '\u00d6':  case '\u00d8':  case '\u00d9':
			case '\u00da':  case '\u00db':  case '\u00dc':  case '\u00dd':
			case '\u00de':  case '\u00df':  case '\u00e0':  case '\u00e1':
			case '\u00e2':  case '\u00e3':  case '\u00e4':  case '\u00e5':
			case '\u00e6':  case '\u00e7':  case '\u00e8':  case '\u00e9':
			case '\u00ea':  case '\u00eb':  case '\u00ec':  case '\u00ed':
			case '\u00ee':  case '\u00ef':  case '\u00f0':  case '\u00f1':
			case '\u00f2':  case '\u00f3':  case '\u00f4':  case '\u00f5':
			case '\u00f6':  case '\u00f8':  case '\u00f9':  case '\u00fa':
			case '\u00fb':  case '\u00fc':  case '\u00fd':  case '\u00fe':
			case '\u00ff':
			{
				mLETTER_CHARACTER(false);
				break;
			}
			case '_':
			{
				match('_');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		{    // ( ... )*
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case 'A':  case 'B':  case 'C':  case 'D':
				case 'E':  case 'F':  case 'G':  case 'H':
				case 'I':  case 'J':  case 'K':  case 'L':
				case 'M':  case 'N':  case 'O':  case 'P':
				case 'Q':  case 'R':  case 'S':  case 'T':
				case 'U':  case 'V':  case 'W':  case 'X':
				case 'Y':  case 'Z':  case 'a':  case 'b':
				case 'c':  case 'd':  case 'e':  case 'f':
				case 'g':  case 'h':  case 'i':  case 'j':
				case 'k':  case 'l':  case 'm':  case 'n':
				case 'o':  case 'p':  case 'q':  case 'r':
				case 's':  case 't':  case 'u':  case 'v':
				case 'w':  case 'x':  case 'y':  case 'z':
				case '\u00aa':  case '\u00b5':  case '\u00ba':  case '\u00c0':
				case '\u00c1':  case '\u00c2':  case '\u00c3':  case '\u00c4':
				case '\u00c5':  case '\u00c6':  case '\u00c7':  case '\u00c8':
				case '\u00c9':  case '\u00ca':  case '\u00cb':  case '\u00cc':
				case '\u00cd':  case '\u00ce':  case '\u00cf':  case '\u00d0':
				case '\u00d1':  case '\u00d2':  case '\u00d3':  case '\u00d4':
				case '\u00d5':  case '\u00d6':  case '\u00d8':  case '\u00d9':
				case '\u00da':  case '\u00db':  case '\u00dc':  case '\u00dd':
				case '\u00de':  case '\u00df':  case '\u00e0':  case '\u00e1':
				case '\u00e2':  case '\u00e3':  case '\u00e4':  case '\u00e5':
				case '\u00e6':  case '\u00e7':  case '\u00e8':  case '\u00e9':
				case '\u00ea':  case '\u00eb':  case '\u00ec':  case '\u00ed':
				case '\u00ee':  case '\u00ef':  case '\u00f0':  case '\u00f1':
				case '\u00f2':  case '\u00f3':  case '\u00f4':  case '\u00f5':
				case '\u00f6':  case '\u00f8':  case '\u00f9':  case '\u00fa':
				case '\u00fb':  case '\u00fc':  case '\u00fd':  case '\u00fe':
				case '\u00ff':
				{
					mLETTER_CHARACTER(false);
					break;
				}
				case '0':  case '1':  case '2':  case '3':
				case '4':  case '5':  case '6':  case '7':
				case '8':  case '9':
				{
					mDECIMAL_DIGIT_CHARACTER(false);
					break;
				}
				case '_':
				{
					match('_');
					break;
				}
				case '\u00ad':
				{
					match('­');
					break;
				}
				default:
				{
					goto _loop290_breakloop;
				}
				 }
			}
_loop290_breakloop:			;
		}    // ( ... )*
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mLETTER_CHARACTER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LETTER_CHARACTER;
		
		{
			switch ( cached_LA1 )
			{
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':  case 'G':  case 'H':
			case 'I':  case 'J':  case 'K':  case 'L':
			case 'M':  case 'N':  case 'O':  case 'P':
			case 'Q':  case 'R':  case 'S':  case 'T':
			case 'U':  case 'V':  case 'W':  case 'X':
			case 'Y':  case 'Z':
			{
				matchRange('A','Z');
				break;
			}
			case '\u00c0':  case '\u00c1':  case '\u00c2':  case '\u00c3':
			case '\u00c4':  case '\u00c5':  case '\u00c6':  case '\u00c7':
			case '\u00c8':  case '\u00c9':  case '\u00ca':  case '\u00cb':
			case '\u00cc':  case '\u00cd':  case '\u00ce':  case '\u00cf':
			case '\u00d0':  case '\u00d1':  case '\u00d2':  case '\u00d3':
			case '\u00d4':  case '\u00d5':  case '\u00d6':
			{
				matchRange('À','Ö');
				break;
			}
			case '\u00d8':  case '\u00d9':  case '\u00da':  case '\u00db':
			case '\u00dc':  case '\u00dd':  case '\u00de':
			{
				matchRange('Ø','Þ');
				break;
			}
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':  case 'g':  case 'h':
			case 'i':  case 'j':  case 'k':  case 'l':
			case 'm':  case 'n':  case 'o':  case 'p':
			case 'q':  case 'r':  case 's':  case 't':
			case 'u':  case 'v':  case 'w':  case 'x':
			case 'y':  case 'z':
			{
				matchRange('a','z');
				break;
			}
			case '\u00aa':
			{
				match('ª');
				break;
			}
			case '\u00b5':
			{
				match('µ');
				break;
			}
			case '\u00ba':
			{
				match('º');
				break;
			}
			case '\u00df':
			{
				match('ß');
				break;
			}
			case '\u00e0':  case '\u00e1':  case '\u00e2':  case '\u00e3':
			case '\u00e4':  case '\u00e5':  case '\u00e6':  case '\u00e7':
			case '\u00e8':  case '\u00e9':  case '\u00ea':  case '\u00eb':
			case '\u00ec':  case '\u00ed':  case '\u00ee':  case '\u00ef':
			case '\u00f0':  case '\u00f1':  case '\u00f2':  case '\u00f3':
			case '\u00f4':  case '\u00f5':  case '\u00f6':
			{
				matchRange('à','ö');
				break;
			}
			case '\u00f8':  case '\u00f9':  case '\u00fa':  case '\u00fb':
			case '\u00fc':  case '\u00fd':  case '\u00fe':  case '\u00ff':
			{
				matchRange('ø','ÿ');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDECIMAL_DIGIT_CHARACTER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DECIMAL_DIGIT_CHARACTER;
		
		{
			matchRange('0','9');
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNUMERIC_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NUMERIC_LITERAL;
		
		if ((cached_LA1=='0') && (cached_LA2=='X'||cached_LA2=='x'))
		{
			int _saveIndex = 0;
			_saveIndex = text.Length;
			match('0');
			text.Length = _saveIndex;
			{
				switch ( cached_LA1 )
				{
				case 'x':
				{
					_saveIndex = text.Length;
					match('x');
					text.Length = _saveIndex;
					break;
				}
				case 'X':
				{
					_saveIndex = text.Length;
					match('X');
					text.Length = _saveIndex;
					break;
				}
				default:
				{
					throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
				}
				 }
			}
			{ // ( ... )+
				int _cnt310=0;
				for (;;)
				{
					if ((tokenSet_4_.member(cached_LA1)))
					{
						mHEX_DIGIT(false);
					}
					else
					{
						if (_cnt310 >= 1) { goto _loop310_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
					}
					
					_cnt310++;
				}
_loop310_breakloop:				;
			}    // ( ... )+
			{
				if ((tokenSet_5_.member(cached_LA1)) && (tokenSet_5_.member(cached_LA2)))
				{
					mUNSIGNED_LONG_SUFFIX(false);
					_ttype = UINT64_HEXADECIMAL_INTEGER_LITERAL;
				}
				else if ((cached_LA1=='L'||cached_LA1=='l') && (true)) {
					mLONG_SUFFIX(false);
					_ttype = INT64_HEXADECIMAL_INTEGER_LITERAL;
				}
				else if ((cached_LA1=='U'||cached_LA1=='u') && (true)) {
					mUNSIGNED_SUFFIX(false);
					_ttype = UINT32_HEXADECIMAL_INTEGER_LITERAL;
				}
				else {
					_ttype = INT32_HEXADECIMAL_INTEGER_LITERAL;
				}
				
			}
		}
		else if ((cached_LA1=='.') && ((cached_LA2 >= '0' && cached_LA2 <= '9'))) {
			match('.');
			{ // ( ... )+
				int _cnt313=0;
				for (;;)
				{
					if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
					{
						mDECIMAL_DIGIT(false);
					}
					else
					{
						if (_cnt313 >= 1) { goto _loop313_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
					}
					
					_cnt313++;
				}
_loop313_breakloop:				;
			}    // ( ... )+
			{
				if ((cached_LA1=='E'||cached_LA1=='e'))
				{
					mEXPONENT_PART(false);
				}
				else {
				}
				
			}
			{
				switch ( cached_LA1 )
				{
				case 'F':  case 'f':
				{
					mFLOAT_SUFFIX(false);
					_ttype = FLOAT32_REAL_LITERAL;
					break;
				}
				case 'M':  case 'm':
				{
					mDECIMAL_SUFFIX(false);
					_ttype = DECIMAL_REAL_LITERAL;
					break;
				}
				default:
					{
						{
							if ((cached_LA1=='D'||cached_LA1=='d'))
							{
								mDOUBLE_SUFFIX(false);
							}
							else {
							}
							
						}
						_ttype = FLOAT64_REAL_LITERAL;
					}
				break; }
			}
		}
		else if (((cached_LA1 >= '0' && cached_LA1 <= '9')) && (true)) {
			{ // ( ... )+
				int _cnt297=0;
				for (;;)
				{
					if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
					{
						mDECIMAL_DIGIT(false);
					}
					else
					{
						if (_cnt297 >= 1) { goto _loop297_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
					}
					
					_cnt297++;
				}
_loop297_breakloop:				;
			}    // ( ... )+
			{
				switch ( cached_LA1 )
				{
				case '.':
				{
					{
						match('.');
						{ // ( ... )+
							int _cnt301=0;
							for (;;)
							{
								if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
								{
									mDECIMAL_DIGIT(false);
								}
								else
								{
									if (_cnt301 >= 1) { goto _loop301_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
								}
								
								_cnt301++;
							}
_loop301_breakloop:							;
						}    // ( ... )+
						{
							if ((cached_LA1=='E'||cached_LA1=='e'))
							{
								mEXPONENT_PART(false);
							}
							else {
							}
							
						}
						{
							switch ( cached_LA1 )
							{
							case 'F':  case 'f':
							{
								mFLOAT_SUFFIX(false);
								_ttype = FLOAT32_REAL_LITERAL;
								break;
							}
							case 'M':  case 'm':
							{
								mDECIMAL_SUFFIX(false);
								_ttype = DECIMAL_REAL_LITERAL;
								break;
							}
							default:
								{
									{
										if ((cached_LA1=='D'||cached_LA1=='d'))
										{
											mDOUBLE_SUFFIX(false);
										}
										else {
										}
										
									}
									_ttype = FLOAT64_REAL_LITERAL;
								}
							break; }
						}
					}
					break;
				}
				case 'E':  case 'e':
				{
					{
						mEXPONENT_PART(false);
						{
							switch ( cached_LA1 )
							{
							case 'F':  case 'f':
							{
								mFLOAT_SUFFIX(false);
								_ttype = FLOAT32_REAL_LITERAL;
								break;
							}
							case 'M':  case 'm':
							{
								mDECIMAL_SUFFIX(false);
								_ttype = DECIMAL_REAL_LITERAL;
								break;
							}
							default:
								{
									{
										if ((cached_LA1=='D'||cached_LA1=='d'))
										{
											mDOUBLE_SUFFIX(false);
										}
										else {
										}
										
									}
									_ttype = FLOAT64_REAL_LITERAL;
								}
							break; }
						}
					}
					break;
				}
				case 'D':  case 'd':
				{
					mDOUBLE_SUFFIX(false);
					_ttype = FLOAT64_REAL_LITERAL;
					break;
				}
				case 'F':  case 'f':
				{
					mFLOAT_SUFFIX(false);
					_ttype = FLOAT32_REAL_LITERAL;
					break;
				}
				case 'M':  case 'm':
				{
					mDECIMAL_SUFFIX(false);
					_ttype = DECIMAL_REAL_LITERAL;
					break;
				}
				default:
					if ((tokenSet_5_.member(cached_LA1)) && (tokenSet_5_.member(cached_LA2)))
					{
						mUNSIGNED_LONG_SUFFIX(false);
						_ttype = UINT64_DECIMAL_INTEGER_LITERAL;
					}
					else if ((cached_LA1=='L'||cached_LA1=='l') && (true)) {
						mLONG_SUFFIX(false);
						_ttype = INT64_DECIMAL_INTEGER_LITERAL;
					}
					else if ((cached_LA1=='U'||cached_LA1=='u') && (true)) {
						mUNSIGNED_SUFFIX(false);
						_ttype = UINT32_DECIMAL_INTEGER_LITERAL;
					}
					else {
						_ttype = INT32_DECIMAL_INTEGER_LITERAL;
					}
				break; }
			}
		}
		else if ((cached_LA1=='.') && (true)) {
			match('.');
			_ttype = DOT;
		}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDECIMAL_DIGIT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DECIMAL_DIGIT;
		
		{
			matchRange('0','9');
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mLONG_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LONG_SUFFIX;
		
		{
			switch ( cached_LA1 )
			{
			case 'L':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('L');
				text.Length = _saveIndex;
				break;
			}
			case 'l':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('l');
				text.Length = _saveIndex;
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mUNSIGNED_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = UNSIGNED_SUFFIX;
		
		{
			switch ( cached_LA1 )
			{
			case 'U':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('U');
				text.Length = _saveIndex;
				break;
			}
			case 'u':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('u');
				text.Length = _saveIndex;
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mUNSIGNED_LONG_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = UNSIGNED_LONG_SUFFIX;
		
		{
			if ((cached_LA1=='U') && (cached_LA2=='L'))
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('U');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('L');
				text.Length = _saveIndex;
			}
			else if ((cached_LA1=='U') && (cached_LA2=='l')) {
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('U');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('l');
				text.Length = _saveIndex;
			}
			else if ((cached_LA1=='u') && (cached_LA2=='L')) {
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('u');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('L');
				text.Length = _saveIndex;
			}
			else if ((cached_LA1=='u') && (cached_LA2=='l')) {
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('u');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('l');
				text.Length = _saveIndex;
			}
			else if ((cached_LA1=='L') && (cached_LA2=='U')) {
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('L');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('U');
				text.Length = _saveIndex;
			}
			else if ((cached_LA1=='L') && (cached_LA2=='u')) {
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('L');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('u');
				text.Length = _saveIndex;
			}
			else if ((cached_LA1=='l') && (cached_LA2=='U')) {
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('l');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('U');
				text.Length = _saveIndex;
			}
			else if ((cached_LA1=='l') && (cached_LA2=='u')) {
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('l');
				text.Length = _saveIndex;
				_saveIndex = text.Length;
				match('u');
				text.Length = _saveIndex;
			}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mEXPONENT_PART(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = EXPONENT_PART;
		
		{
			switch ( cached_LA1 )
			{
			case 'E':
			{
				match('E');
				break;
			}
			case 'e':
			{
				match('e');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		{
			switch ( cached_LA1 )
			{
			case '+':
			{
				match('+');
				break;
			}
			case '-':
			{
				match('-');
				break;
			}
			case '0':  case '1':  case '2':  case '3':
			case '4':  case '5':  case '6':  case '7':
			case '8':  case '9':
			{
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		{ // ( ... )+
			int _cnt331=0;
			for (;;)
			{
				if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
				{
					mDECIMAL_DIGIT(false);
				}
				else
				{
					if (_cnt331 >= 1) { goto _loop331_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt331++;
			}
_loop331_breakloop:			;
		}    // ( ... )+
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDOUBLE_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DOUBLE_SUFFIX;
		
		{
			switch ( cached_LA1 )
			{
			case 'D':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('D');
				text.Length = _saveIndex;
				break;
			}
			case 'd':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('d');
				text.Length = _saveIndex;
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mFLOAT_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = FLOAT_SUFFIX;
		
		{
			switch ( cached_LA1 )
			{
			case 'F':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('F');
				text.Length = _saveIndex;
				break;
			}
			case 'f':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('f');
				text.Length = _saveIndex;
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDECIMAL_SUFFIX(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DECIMAL_SUFFIX;
		
		{
			switch ( cached_LA1 )
			{
			case 'M':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('M');
				text.Length = _saveIndex;
				break;
			}
			case 'm':
			{
				int _saveIndex = 0;
				_saveIndex = text.Length;
				match('m');
				text.Length = _saveIndex;
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mHEX_DIGIT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = HEX_DIGIT;
		
		{
			switch ( cached_LA1 )
			{
			case '0':  case '1':  case '2':  case '3':
			case '4':  case '5':  case '6':  case '7':
			case '8':  case '9':
			{
				matchRange('0','9');
				break;
			}
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':
			{
				matchRange('A','F');
				break;
			}
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':
			{
				matchRange('a','f');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCHARACTER_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = CHARACTER_LITERAL;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('\'');
		text.Length = _saveIndex;
		mSINGLE_CHARACTER(false);
		_saveIndex = text.Length;
		match('\'');
		text.Length = _saveIndex;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mSINGLE_CHARACTER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SINGLE_CHARACTER;
		
		if ((cached_LA1=='\\') && (cached_LA2=='\''))
		{
			match('\\');
			match('\'');
			text.Length = _begin; text.Append("\'");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='"')) {
			match('\\');
			match('\"');
			text.Length = _begin; text.Append("\"");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='\\')) {
			match('\\');
			match('\\');
			text.Length = _begin; text.Append("\\");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='0')) {
			match('\\');
			match('0');
			text.Length = _begin; text.Append("\0");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='b')) {
			match('\\');
			match('b');
			text.Length = _begin; text.Append("\b");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='f')) {
			match('\\');
			match('f');
			text.Length = _begin; text.Append("\f");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='n')) {
			match('\\');
			match('n');
			text.Length = _begin; text.Append("\n");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='r')) {
			match('\\');
			match('r');
			text.Length = _begin; text.Append("\r");
		}
		else if ((cached_LA1=='\\') && (cached_LA2=='t')) {
			match('\\');
			match('t');
			text.Length = _begin; text.Append("\t");
		}
		else if ((tokenSet_6_.member(cached_LA1))) {
			{
				match(tokenSet_6_);
			}
		}
		else
		{
			throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
		}
		
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSTRING_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = STRING_LITERAL;
		
		int _saveIndex = 0;
		_saveIndex = text.Length;
		match('\"');
		text.Length = _saveIndex;
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_7_.member(cached_LA1)))
				{
					mSINGLE_CHARACTER(false);
				}
				else
				{
					goto _loop343_breakloop;
				}
				
			}
_loop343_breakloop:			;
		}    // ( ... )*
		_saveIndex = text.Length;
		match('\"');
		text.Length = _saveIndex;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLBRACE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LBRACE;
		
		match('{');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRBRACE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RBRACE;
		
		match('}');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LBRACK;
		
		match('[');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRBRACK(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RBRACK;
		
		match(']');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LPAREN;
		
		match('(');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRPAREN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RPAREN;
		
		match(')');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOMMA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COMMA;
		
		match(',');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOLON(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COLON;
		
		match(':');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSEMI(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SEMI;
		
		match(';');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mPLUS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = PLUS;
		
		match('+');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMINUS(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MINUS;
		
		match('-');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mTIMES(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = TIMES;
		
		match('*');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mQUOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = QUOT;
		
		match('/');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mREM(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = REM;
		
		match('%');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mAND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = AND;
		
		match('&');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = OR;
		
		match('|');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mXOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = XOR;
		
		match('^');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLNOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LNOT;
		
		match('!');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNOT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NOT;
		
		match('`');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ASSIGN;
		
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLANGLE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LANGLE;
		
		match('<');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRANGLE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RANGLE;
		
		match('>');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COND;
		
		match('?');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mINC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = INC;
		
		match('+');
		match('+');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mDEC(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DEC;
		
		match('-');
		match('-');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLAND(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LAND;
		
		match('&');
		match('&');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLOR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LOR;
		
		match('|');
		match('|');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSHL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SHL;
		
		match('<');
		match('<');
		{
			if ((cached_LA1=='='))
			{
				match('=');
				_ttype = SHL_ASSIGN;
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSHR(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SHR;
		
		match('>');
		match('>');
		{
			if ((cached_LA1=='='))
			{
				match('=');
				_ttype = SHR_ASSIGN;
			}
			else {
			}
			
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mEQ(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = EQ;
		
		match('=');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NE;
		
		match('!');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LE;
		
		match('<');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mGE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = GE;
		
		match('>');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mPLUS_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = PLUS_ASSIGN;
		
		match('+');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mMINUS_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = MINUS_ASSIGN;
		
		match('-');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mTIMES_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = TIMES_ASSIGN;
		
		match('*');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mQUOT_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = QUOT_ASSIGN;
		
		match('/');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mREM_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = REM_ASSIGN;
		
		match('%');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mAND_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = AND_ASSIGN;
		
		match('&');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mOR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = OR_ASSIGN;
		
		match('|');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mXOR_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = XOR_ASSIGN;
		
		match('^');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mARROW(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ARROW;
		
		match('-');
		match('>');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mLMETA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = LMETA;
		
		match('<');
		match('|');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mRMETA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = RMETA;
		
		match('|');
		match('>');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mESCAPE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ESCAPE;
		
		match('~');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSEMI_ASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SEMI_ASSIGN;
		
		match(';');
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = new long[8];
		data[0]=-9217L;
		for (int i = 1; i<=3; i++) { data[i]=-1L; }
		for (int i = 4; i<=7; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = new long[8];
		data[0]=-4398046520321L;
		for (int i = 1; i<=3; i++) { data[i]=-1L; }
		for (int i = 4; i<=7; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = new long[8];
		data[0]=0L;
		data[1]=576460745995190270L;
		data[2]=297241973452963840L;
		data[3]=-36028797027352577L;
		for (int i = 4; i<=7; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = new long[8];
		data[0]=-17179869185L;
		for (int i = 1; i<=3; i++) { data[i]=-1L; }
		for (int i = 4; i<=7; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 287948901175001088L, 541165879422L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 0L, 9024791442886656L, 0L, 0L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	private static long[] mk_tokenSet_6_()
	{
		long[] data = new long[8];
		data[0]=-566935692289L;
		data[1]=-268435457L;
		for (int i = 2; i<=3; i++) { data[i]=-1L; }
		for (int i = 4; i<=7; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_6_ = new BitSet(mk_tokenSet_6_());
	private static long[] mk_tokenSet_7_()
	{
		long[] data = new long[8];
		data[0]=-566935692289L;
		for (int i = 1; i<=3; i++) { data[i]=-1L; }
		for (int i = 4; i<=7; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_7_ = new BitSet(mk_tokenSet_7_());
	
}
}
