using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplateTest
{
    public class Parser
    {
        public List<Token> Parse(string template)
        {
            List<Token> tokens = new List<Token>();
            char c = ' ';
            char? next = null;
            Token token = null;
            State state = State.None;
            int braces = 0;

            Action<TokenType, State> newToken = (tokenType, s) =>
            {
                token = new Token {TokenType = tokenType};
                tokens.Add(token);
                state = s;
            };

            Action appendChar = () => token.Text += c;

            for (int i = 0; i < template.Length; ++i)
            {
                c = template[i];
                next = i < template.Length - 1 
                    ? template[i + 1] 
                    : (char?) null;

                switch (c)
                {
                    case '@':
                        switch (state)
                        {
                            case State.None:
                            case State.Text:
                            case State.Statement:
                            case State.StatementBlock:
                                if (next.HasValue && next == '{')
                                {
                                    newToken(TokenType.Block, State.Block);
                                }
                                else if (next.HasValue && next == '(')
                                {
                                    newToken(TokenType.Statement, State.StatementBlock);
                                }
                                else
                                {
                                    newToken(TokenType.Statement, State.Statement);
                                }
                                break;
                            case State.Block:
                                newToken(TokenType.Text, State.TextInBlock);
                                break;
                            case State.TextInBlock:
                                newToken(TokenType.Block, State.Block);
                                break;
                        }
                        break;
                    case '.':
                        appendChar();
                        break;
                    case '(':
                        switch (state)
                        {
                            case State.None:
                                newToken(TokenType.Text, State.Text);
                                appendChar();
                                break;
                            case State.Text:
                            case State.TextInBlock:
                                appendChar();
                                break;
                            case State.Statement:
                            case State.StatementBlock:
                            case State.Block:
                                appendChar();
                                braces++;
                                break;
                        }
                        break;
                    case ')':
                        switch (state)
                        {
                            case State.None:
                                newToken(TokenType.Text, State.Text);
                                appendChar();
                                break;
                            case State.Text:
                            case State.TextInBlock:
                                appendChar();
                                break;
                            case State.Statement:
                                appendChar();
                                braces--;

                                if (braces == 0)
                                {
                                    state = State.None;
                                }
                                break;
                            case State.StatementBlock:
                            case State.Block:
                                appendChar();
                                braces--;

                                if (braces == 0)
                                {
                                    state = State.None;
                                }
                                break;
                        }
                        break;
                    case '{':
                        switch (state)
                        {
                            case State.None:
                                newToken(TokenType.Text, State.Text);
                                appendChar();
                                break;
                            case State.Text:
                            case State.TextInBlock:
                                appendChar();
                                break;
                            case State.Statement:
                            case State.StatementBlock:
                            case State.Block:
                                appendChar();
                                braces++;
                                break;
                        }
                        break;
                    case '}':
                        switch (state)
                        {
                            case State.None:
                                newToken(TokenType.Text, State.Text);
                                appendChar();
                                break;
                            case State.Text:
                            case State.TextInBlock:
                                appendChar();
                                break;
                            case State.Statement:
                                appendChar();
                                braces--;

                                if (braces == 0)
                                {
                                    state = State.None;
                                }
                                break;
                            case State.StatementBlock:
                            case State.Block:
                                appendChar();
                                braces--;

                                if (braces == 0)
                                {
                                    state = State.None;
                                }
                                break;
                        }
                        break;
                    default:
                        if (Char.IsLetter(c))
                        {
                            switch (state)
                            {
                                case State.None:
                                    newToken(TokenType.Text, State.Text);
                                    appendChar();
                                    break;
                                default:
                                    appendChar();
                                    break;
                            }
                        }
                        else
                        {
                            switch (state)
                            {
                                case State.None:
                                    newToken(TokenType.Text, State.Text);
                                    appendChar();
                                    break;
                                case State.Text:
                                case State.Block:
                                case State.TextInBlock:
                                case State.StatementBlock:
                                    appendChar();
                                    break;
                                case State.Statement:
                                    newToken(TokenType.Text, State.Text);
                                    appendChar();
                                    break;
                            }
                        }
                        break;
                }
            }

            tokens = tokens.Where(item => item.Text != null).ToList();

            return tokens;
        }
    }
}