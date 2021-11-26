using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }

    }

    public class Lexer
    {

        protected int position;
        protected char currentCh; // ��������� ��������� ������
        protected int currentCharValue; // ����� �������� ���������� ���������� �������
        protected System.IO.StringReader inputReader;
        protected string inputString;

        public Lexer(string input)
        {
            inputReader = new System.IO.StringReader(input);
            inputString = input;
        }

        public void Error()
        {
            System.Text.StringBuilder o = new System.Text.StringBuilder();
            o.Append(inputString + '\n');
            o.Append(new System.String(' ', position - 1) + "^\n");
            o.AppendFormat("Error in symbol {0}", currentCh);
            throw new LexerException(o.ToString());
        }

        protected void NextCh()
        {
            this.currentCharValue = this.inputReader.Read();
            this.currentCh = (char)currentCharValue;
            this.position += 1;
        }

        public virtual bool Parse()
        {
            return true;
        }
    }

    public class IntLexer : Lexer
    {

        protected System.Text.StringBuilder intString;
        public int parseResult = 0;

        public IntLexer(string input)
            : base(input)
        {
            intString = new System.Text.StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                intString.Append(currentCh);
                NextCh();
            }

            if (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }


            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = int.Parse(intString.ToString());
            return true;
        }
    }

    public class IdentLexer : Lexer
    {
        private string parseResult;
        protected StringBuilder builder;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public IdentLexer(string input) : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsLetter(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }
            while (char.IsLetterOrDigit(currentCh) || currentCh == '_')
            {
                builder.Append(currentCh);
                NextCh();
            }
            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = builder.ToString();
            return true;
        }

    }

    public class IntNoZeroLexer : IntLexer
    {
        public IntNoZeroLexer(string input)
            : base(input)
        {
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {
                intString.Append(currentCh);
                NextCh();
            }
            if (char.IsDigit(currentCh) && currentCh != '0')
            {
                intString.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }


            if (currentCharValue != -1)
            {
                Error();
            }

            parseResult = int.Parse(intString.ToString());
            return true;
        }
    }

    public class LetterDigitLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsLetter(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }
            while (true)
            {
                if (currentCharValue == -1)
                {
                    break;
                }
                if (char.IsDigit(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }

                if (currentCharValue == -1)
                {
                    break;
                }
                if (char.IsLetter(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }
            }
            parseResult = builder.ToString();
            return true;
        }

    }

    public class LetterListLexer : Lexer
    {
        protected List<char> parseResult;

        public List<char> ParseResult
        {
            get { return parseResult; }
        }

        public LetterListLexer(string input)
            : base(input)
        {
            parseResult = new List<char>();
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsLetter(currentCh))
            {
                parseResult.Add(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }
            while (true)
            {
                if (currentCharValue == -1)
                {
                    break;
                }
                if (currentCh == ',' || currentCh == ';')
                {
                    NextCh();
                }
                else
                {
                    Error();
                }

                if (currentCharValue == -1)
                {
                    Error();
                }
                if (char.IsLetter(currentCh))
                {
                    parseResult.Add(currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }
            }
            return true;
        }
    }

    public class DigitListLexer : Lexer
    {
        private bool check;
        protected List<int> parseResult;

        public List<int> ParseResult
        {
            get { return parseResult; }
        }

        public DigitListLexer(string input)
            : base(input)
        {
            parseResult = new List<int>();
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsDigit(currentCh))
            {
                check = true;
                parseResult.Add(currentCh - (int)'0');
                NextCh();
            }
            else
            {
                Error();
            }
            while (true)
            {
                if (currentCharValue == -1)
                {
                    break;
                }
                while (char.IsWhiteSpace(currentCh))
                {
                    check = false;
                    NextCh();
                }
                if (char.IsDigit(currentCh) && !check)
                {
                    parseResult.Add(currentCh - (int)'0');
                    NextCh();
                    check = true;
                }
                else
                {
                    Error();
                }
            }
            return true;
        }
    }

    public class LetterDigitGroupLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitGroupLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }
        private int letterCount = 0;
        private int digitCount = 0;


        public override bool Parse()
        {
            NextCh();
            if (char.IsLetter(currentCh))
            {
                builder.Append(currentCh);
                letterCount = 1;
                NextCh();
            }
            else
            {
                Error();
            }
            while (currentCharValue != -1)
            {
                if (!char.IsLetterOrDigit(currentCh))
                {
                    Error();
                }
                while (char.IsLetter(currentCh))
                {
                    letterCount += 1;
                    builder.Append(currentCh);
                    NextCh();
                }
                if (letterCount > 2)
                {
                    Error();
                }
                else
                {
                    letterCount = 0;
                }

                while (char.IsDigit(currentCh))
                {
                    digitCount += 1;
                    builder.Append(currentCh);
                    NextCh();
                }
                if (digitCount > 2)
                {
                    Error();
                }
                else
                {
                    digitCount = 0;
                }

            }
            parseResult = builder.ToString();
            return true;
        }

    }

    public class DoubleLexer : Lexer
    {
        private StringBuilder builder;
        private double parseResult;

        public double ParseResult
        {
            get { return parseResult; }

        }

        public DoubleLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsDigit(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }
            while (char.IsDigit(currentCh))
            {
                builder.Append(currentCh);
                NextCh();
            }
            if (currentCh == '.')
            {
                builder.Append(currentCh);
                NextCh();
                if (char.IsDigit(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }
                while (char.IsDigit(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
            }

            if (currentCharValue == -1)
            {
                parseResult = double.Parse(builder.ToString());
                return true;
            }
            else
            {
                Error();
            }
            return false;
        }

    }

    public class StringLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public StringLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '\'')
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }
            while (currentCh != '\'' && currentCharValue != -1)
            {
                builder.Append(currentCh);
                NextCh();
            }
            if (currentCh == '\'')
            {
                builder.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }
            if (currentCharValue == -1)
            {
                parseResult = builder.ToString();
                return true;
            }
            else
            {
                Error();
                return false;
            }
        }
    }

    public class CommentLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public CommentLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '/')
            {
                builder.Append(currentCh);
                NextCh();
                if (currentCh == '*')
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }
            }
            else
            {
                Error();
            }
            while (currentCharValue != -1)
            {
                if (currentCh == '*')
                {
                    builder.Append(currentCh);
                    NextCh();
                    if (currentCh == '/')
                    {
                        builder.Append(currentCh);
                        NextCh();
                        if (currentCharValue == -1)
                        {
                            parseResult = builder.ToString();
                            return true;
                        }
                        else
                        {
                            Error();
                        }
                    }
                }
                else
                {
                    builder.Append(currentCh);
                    NextCh();
                }
            }
            Error();
            return false;
        }
    }

    public class IdentChainLexer : Lexer
    {
        private StringBuilder builder;
        private List<string> parseResult;

        public List<string> ParseResult
        {
            get { return parseResult; }

        }

        public IdentChainLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
            parseResult = new List<string>();
        }

        public override bool Parse()
        {
            NextCh();
            while (currentCharValue != -1)
            {
                if (char.IsLetter(currentCh))
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                else
                {
                    Error();
                }
                while (char.IsLetterOrDigit(currentCh) || currentCh == '_')
                {
                    builder.Append(currentCh);
                    NextCh();
                }
                if (currentCh == '.')
                {
                    parseResult.Add(builder.ToString());
                    builder.Clear();
                    NextCh();
                    if (currentCharValue == -1)
                    {
                        Error();
                    }
                }


            }
            return true;
        }
    }

    public class Program
    {
        public static void Main()
        {
            string input = "154216";
            Lexer L = new IntLexer(input);
            try
            {
                L.Parse();
            }
            catch (LexerException e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
    }
}