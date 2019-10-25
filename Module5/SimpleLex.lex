%using SimpleParser;
%using QUT.Gppg;
%using System.Linq;

%namespace SimpleScanner

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INTNUM  {Digit}+
REALNUM {INTNUM}\.{INTNUM}
ID {Alpha}{AlphaDigit}* 

%%

{INTNUM} { 
  return (int)Tokens.INUM; 
}

{REALNUM} { 
  return (int)Tokens.RNUM;
}

{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  return res;
}

":=" { return (int)Tokens.ASSIGN; }
";"  { return (int)Tokens.SEMICOLON; }
"-"  { return (int)Tokens.MINUS; }
"+"  { return (int)Tokens.PLUS; }
"*"  { return (int)Tokens.MULT; }
"/"  { return (int)Tokens.DIVIDE; }
"("  { return (int)Tokens.OPENROUND; }
")"  { return (int)Tokens.CLOSEROUND; }

[^ \r\n] {
	LexError();
	return (int)Tokens.EOF; // конец разбора
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); // позиция символа (терминального или нетерминального), возвращаемая @1 @2 и т.д.
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): Got '{2}', expected '{3}'", yyline, yycol, args[0], string.Join(" или ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
	string errorMsg = string.Format("({0},{1}): Unknown symbol '{2}'", yyline, yycol, yytext);
    throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("begin",(int)Tokens.BEGIN);
    keywords.Add("end",(int)Tokens.END);
    keywords.Add("cycle",(int)Tokens.CYCLE);
    keywords.Add("if",(int)Tokens.IF);
    keywords.Add("else",(int)Tokens.ELSE);
    keywords.Add("then",(int)Tokens.THEN);
  }
  public static int GetIDToken(string s)
  {
    if (keywords.ContainsKey(s.ToLower())) // язык нечувствителен к регистру
      return keywords[s];
    else
      return (int)Tokens.ID;
  }
}
