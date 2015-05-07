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

        [Test]
        public void Zzz()
        {
            string s = @"<# ForEach((p, f) => #><#= f ? "", "" : """" #><#); #>";
            var tokens = new Parser().Parse(@"@{ForEach((p, f) => f ? @"", "" : @"""");}");

        }

        [Test]
        public void Xxx()
        {
            var tokens = new Parser().Parse(@"namespace @Entity.Group.Name
{
    public class @Entity.Name
    {
        public @(Entity.Name)(@{
ForEachProperty((p, f) => );
})
        {
        }
    }
}");

            //Assert.That(tokens.Count, Is.EqualTo(4));
            Assert.That(tokens[0].Text, Is.EqualTo(@"namespace "));
            Assert.That(tokens[0].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[1].Text, Is.EqualTo(@"Entity.Group.Name"));
            Assert.That(tokens[1].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(tokens[2].Text, Is.EqualTo(@"
{
    public class "));
            Assert.That(tokens[2].TokenType, Is.EqualTo(TokenType.Text));
            Assert.That(tokens[3].Text, Is.EqualTo(@"Entity.Name"));
            Assert.That(tokens[3].TokenType, Is.EqualTo(TokenType.Statement));
            Assert.That(tokens[3].Text, Is.EqualTo(@"Entity.Name"));
            Assert.That(tokens[3].TokenType, Is.EqualTo(TokenType.Statement));
        }
    }

    public enum State
    {
        None,
        Text,
        Statement,
        StatementBlock,
        Block,
        TextInBlock
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
