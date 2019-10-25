%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END CYCLE INUM RNUM ID ASSIGN SEMICOLON MINUS PLUS MULT DIVIDE OPENROUND CLOSEROUND IF ELSE THEN

%%

progr   : block
		;

stlist	: statement 
		| stlist SEMICOLON statement 
		;

statement: assign
		| block  
		| cycle  
        | if
		;

ident 	: ID 
		;
	
assign 	: ident ASSIGN expr 
		;

expr	: e1
		;

e1      : e2 
        | e1 PLUS e2
        | e1 MINUS e2
        ;
        
e2      : e3 
        | e2 MULT e3
        | e2 DIVIDE e3
        ;
        
e3      : ident 
        | INUM
        | RNUM
        | OPENROUND expr CLOSEROUND
        ;
        
if		: IF expr THEN statement
		| IF expr THEN statement ELSE statement
		;


block	: BEGIN stlist END 
		;

cycle	: CYCLE expr statement 
		;
	
%%
