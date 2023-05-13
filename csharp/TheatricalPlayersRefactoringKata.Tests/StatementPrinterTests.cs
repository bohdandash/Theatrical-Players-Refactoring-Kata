using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

namespace TheatricalPlayersRefactoringKata.Tests
{
    public class StatementPrinterTests
    {
        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void TestStatementExample()
        {
            // Arrange
            var plays = new Dictionary<string, Play>();
            plays.Add("hamlet", new Play("Hamlet", "tragedy"));
            plays.Add("as-like", new Play("As You Like It", "comedy"));
            plays.Add("othello", new Play("Othello", "tragedy"));

            var invoice = new Invoice("BigCo", new List<Performance>{
            new Performance("hamlet", 55),
            new Performance("as-like", 35),
            new Performance("othello", 40)});

            var statementPrinter = new StatementPrinter();

            // Act
            var result = statementPrinter.Print(invoice, plays);

            // Assert
            Approvals.Verify(result);
        }

        [Fact]
        public void TestStatementWithNewPlayTypes()
        {
            // Arrange
            var plays = new Dictionary<string, Play>();
            plays.Add("henry-v", new Play("Henry V", "history"));
            plays.Add("as-like", new Play("As You Like It", "pastoral"));

            var invoice = new Invoice("BigCoII", new List<Performance>{
            new Performance("henry-v", 53),
            new Performance("as-like", 55)});

            var statementPrinter = new StatementPrinter();

            // Act and Assert
            var ex = Assert.Throws<Exception>(() => statementPrinter.Print(invoice, plays));
            Assert.Equal("unknown type: history", ex.Message);
        }
    }

}
