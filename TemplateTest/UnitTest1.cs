using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace TemplateTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void AtSignAndLettersRecognisedAsStatement()
        {
            var tokens = new Parser().Parse(@"one@two");

            Assert.That(tokens.Count, Is.EqualTo(2));
            Assert.That(tokens[0].Text, Is.EqualTo("one"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[1].Text, Is.EqualTo("two"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
        }

        [Test]
        public void StatementEndedBySpace()
        {
            var tokens = new Parser().Parse(@"one@two three");

            Assert.That(tokens.Count, Is.EqualTo(3));
            Assert.That(tokens[0].Text, Is.EqualTo("one"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[1].Text, Is.EqualTo("two"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(tokens[2].Text, Is.EqualTo(" three"));
            Assert.That(tokens[2].TokenType, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void StatementCanHaveDots()
        {
            var tokens = new Parser().Parse(@"one@two.three");

            Assert.That(tokens.Count, Is.EqualTo(2));
            Assert.That(tokens[0].Text, Is.EqualTo("one"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[1].Text, Is.EqualTo("two.three"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
        }

        [Test]
        public void StatementCanHaveParens()
        {
            var tokens = new Parser().Parse(@"one@two()");

            Assert.That(tokens.Count, Is.EqualTo(2));
            Assert.That(tokens[0].Text, Is.EqualTo("one"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[1].Text, Is.EqualTo("two()"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
        }

        [Test]
        public void StatementParensAreMatched()
        {
            var tokens = new Parser().Parse(@"one(@two())");

            Assert.That(tokens.Count, Is.EqualTo(3));
            Assert.That(tokens[0].Text, Is.EqualTo("one("));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[1].Text, Is.EqualTo("two()"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(tokens[2].Text, Is.EqualTo(")"));
            Assert.That(tokens[2].TokenType, Is.EqualTo(TokenType.Text));
        }

        [Test]
        public void StatementCanBeEnclosedInParens()
        {
            var tokens = new Parser().Parse(@"@(one two three)");

            Assert.That(tokens.Count, Is.EqualTo(1));
            Assert.That(tokens[0].Text, Is.EqualTo(@"(one two three)"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Statement));
        }

        [Test]
        public void StatementsCanFollowEachOther()
        {
            var tokens = new Parser().Parse(@"@one@two");

            Assert.That(tokens.Count, Is.EqualTo(2));
            Assert.That(tokens[0].Text, Is.EqualTo(@"one"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(tokens[1].Text, Is.EqualTo(@"two"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
        }

        [Test]
        public void BlocksAreEnclosedInCurlyBraces()
        {
            var tokens = new Parser().Parse(@"@{one two three}");

            Assert.That(tokens.Count, Is.EqualTo(1));
            Assert.That(tokens[0].Text, Is.EqualTo(@"{one two three}"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Block));
        }

        [Test]
        public void BlocksCanContainText()
        {
            var tokens = new Parser().Parse(@"@{one @two@ three}");

            Assert.That(tokens.Count, Is.EqualTo(3));
            Assert.That(tokens[0].Text, Is.EqualTo(@"{one "));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Block));
            Assert.That(tokens[1].Text, Is.EqualTo(@"two"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[2].Text, Is.EqualTo(@" three}"));
            Assert.That(tokens[2].TokenType, Is.EqualTo(TokenType.Block));
        }

        [Test]
        public void BlocksTextStatementText()
        {
            var tokens = new Parser().Parse(@"@{block}text@(statement)text");

            Assert.That(tokens.Count, Is.EqualTo(4));
            Assert.That(tokens[0].Text, Is.EqualTo(@"{block}"));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Block));
            Assert.That(tokens[1].Text, Is.EqualTo(@"text"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[2].Text, Is.EqualTo(@"(statement)"));
            Assert.That(tokens[2].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(tokens[3].Text, Is.EqualTo(@"text"));
            Assert.That(tokens[3].TokenType, Is.EqualTo(TokenType.Text));
        }
    }

    public class Parser
    {
        public List<Token> Parse(string template)
        {
            List<Token> tokens = new List<Token>();

            Token token = new Token { TokenType = TokenType.Text };
            tokens.Add(token);

            int braces = 0;
            bool inBlock = false;

            for (int i = 0; i < template.Length; ++i)
            {
                char? prev = i > 0 ? template[i - 1] : (char?)null;
                char c = template[i];
                char? next = i < template.Length - 1 ? template[i + 1] : (char?)null;

                if (token.TokenType == TokenType.Statement || token.TokenType == TokenType.Block)
                {
                    bool isCode = false;

                    if (braces > 0)
                    {
                        isCode = true;
                    }


                    if (Char.IsLetter(c) || c == '.')
                    {
                        isCode = true;
                    }
                    else if (c == '(' || c == '{')
                    {
                        braces++;
                        isCode = true;
                    }
                    else if ((c == ')' || c == '}') && braces > 0)
                    {
                        braces--;
                        isCode = true;
                    }
                    else if (c == '@')
                    {
                        inBlock = token.TokenType == TokenType.Block;
                        isCode = false;
                    }

                    if (isCode)
                    {
                        token.Text += c;
                    }
                    else if (inBlock)
                    {
                        token = new Token
                        {
                            TokenType = TokenType.Text
                        };

                        tokens.Add(token);
                    }
                    else
                    {
                        token = new Token
                        {
                            TokenType = c == '@'
                            ? (next.HasValue && next == '{' ? TokenType.Block : TokenType.Statement)
                            : TokenType.Text,
                            Text = c == '@' ? null : c.ToString()
                        };
                        tokens.Add(token);
                    }
                }
                else
                {
                    if (c == '@')
                    {
                        if (inBlock)
                        {
                            token = new Token { TokenType = TokenType.Block };
                            tokens.Add(token);
                        }
                        else
                        {
                            braces = 0;
                            token = new Token { TokenType = next.HasValue && next == '{' ? TokenType.Block : TokenType.Statement };
                            tokens.Add(token);
                        }
                    }
                    else
                    {
                        token.Text += c;
                    }
                }
            }

            tokens = tokens.Where(item => item.Text != null).ToList();

            return tokens;
        }
    }

    public enum TokenType
    {
        Text,
        Statement,
        Block
    }

    public class Token
    {
        public TokenType TokenType { get; set; }
        public string Text { get; set; }
    }

    public class Generator
    {


        public Generator(Entity entity, string template)
        {
        }
    }

    public class Entity
    {
        public string Name { get; set; }
    }
}
