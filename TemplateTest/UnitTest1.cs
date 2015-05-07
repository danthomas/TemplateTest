using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TemplateTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void AtSignAndLettersRecognisedAsStatement()
        {
            var tokens = new Parser().Parse(@"class @Name");

            Assert.That(tokens.Count, Is.EqualTo(2));
            Assert.That(tokens[0].Text, Is.EqualTo("class "));
            Assert.That(tokens[1].Text, Is.EqualTo("Name"));
        }

        [Test]
        public void StatementEndedByNonLetter()
        {
            var tokens = new Parser().Parse(@"class @Name{}");

            Assert.That(tokens.Count, Is.EqualTo(3));
            Assert.That(tokens[0].Text, Is.EqualTo("class "));
            Assert.That(tokens[1].Text, Is.EqualTo("Name"));
            Assert.That(tokens[2].Text, Is.EqualTo("{}"));
        }

        [Test]
        public void StatementCanHaveDots()
        {
            var tokens = new Parser().Parse(@"class @Entity.Name");

            Assert.That(tokens.Count, Is.EqualTo(2));
            Assert.That(tokens[0].Text, Is.EqualTo("class "));
            Assert.That(tokens[1].Text, Is.EqualTo("Entity.Name"));
        }

        [Test]
        public void StatementCanHaveParens()
        {
            var tokens = new Parser().Parse(@"class @Name.ToSafeString()");

            Assert.That(tokens.Count, Is.EqualTo(2));
            Assert.That(tokens[0].Text, Is.EqualTo("class "));
            Assert.That(tokens[1].Text, Is.EqualTo("Name.ToSafeString()"));
        }

        [Test]
        public void XXX()
        {
            var tokens = new Parser().Parse(@"Delete (@Identifier.Type @Identifier.Name.ToCamelCase())");

            Assert.That(tokens.Count, Is.EqualTo(5));
            Assert.That(tokens[0].Text, Is.EqualTo("Delete ("));
            Assert.That(tokens[1].Text, Is.EqualTo("Identifier.Type"));
            Assert.That(tokens[2].Text, Is.EqualTo(" "));
            Assert.That(tokens[3].Text, Is.EqualTo("Identifier.Name.ToCamelCase()"));
            Assert.That(tokens[4].Text, Is.EqualTo(")"));
        }
    }

    public class Parser
    {
        public List<Token> Parse(string template)
        {
            List<Token> tokens = new List<Token>();

            Token token = new Token { TokenType = TokenType.Text };
            tokens.Add(token);

            for (int i = 0; i < template.Length; ++i)
            {
                char? prev = i > 0 ? template[i - 1] : (char?)null;
                char c = template[i];
                char? next = i < template.Length - 1 ? template[i + 1] : (char?)null;

                int parens = 0;

                if (token.TokenType == TokenType.Statement)
                {
                    bool isCode = false;

                    if (Char.IsLetter(c))
                    {
                        isCode = true;
                    }
                    else if (c == '(')
                    {
                        parens++;
                        isCode = true;
                    }
                    else if (c == ')')
                    {
                        parens--;
                        isCode = true;
                    }

                    if (isCode)
                    {
                        token.Text += c;
                    }
                    else
                    {
                        token = new Token { TokenType = TokenType.Text, Text = c.ToString() };
                        tokens.Add(token);
                    }
                }
                else if (token.TokenType == TokenType.Block)
                {
                }
                else
                {
                    if (c == '@')
                    {
                        token = new Token { TokenType = TokenType.Statement };
                        tokens.Add(token);
                    }
                    else
                    {
                        token.Text += c;
                    }
                }
            }

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
