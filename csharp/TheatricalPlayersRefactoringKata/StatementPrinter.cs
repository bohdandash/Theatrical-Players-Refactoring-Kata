using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    
    public class StatementPrinter
    {
        private const int TragedyBaseAmount = 40000;
        private const int ComedyBaseAmount = 30000;
        private const int TragedyAudienceThreshold = 30;
        private const int ComedyAudienceThreshold = 20;
        private const decimal ComedyCreditFactor = 5;

        static void Main(string[] args)
        {
            var invoice = new Invoice("BigCo", new List<Performance>
        {
            new Performance("hamlet", 55),
            new Performance("as-like", 35),
            new Performance("othello", 40)
        });

            var plays = new Dictionary<string, Play>
        {
            {"hamlet", new Play("Hamlet", "tragedy")},
            {"as-like", new Play("As You Like It", "comedy")},
            {"othello", new Play("Othello", "tragedy")}
        };

            var statementPrinter = new StatementPrinter();
            var statement = statementPrinter.Print(invoice, plays);
            Console.WriteLine(statement);
        }

        //no change
        private readonly CultureInfo _cultureInfo = new CultureInfo("en-US");

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var statement = new StatementBuilder(invoice, plays);
            var totalAmount = statement.GetTotalAmount();
            var volumeCredits = statement.GetVolumeCredits();
            var result = statement.GenerateStatement(totalAmount, volumeCredits, _cultureInfo);
            return result;
        }

        private class StatementBuilder
        {
            private readonly Invoice _invoice;
            private readonly Dictionary<string, Play> _plays;

            public StatementBuilder(Invoice invoice, Dictionary<string, Play> plays)
            {
                _invoice = invoice;
                _plays = plays;
            }

            public int GetTotalAmount()
            {
                int totalAmount = 0;
                foreach (var perf in _invoice.Performances)
                {
                    var play = _plays[perf.PlayID];
                    var thisAmount = CalculateThisAmount(play, perf);
                    totalAmount += thisAmount;
                }
                return totalAmount;
            }

            public int GetVolumeCredits()
            {
                int volumeCredits = 0;
                foreach (var perf in _invoice.Performances)
                {
                    var play = _plays[perf.PlayID];
                    volumeCredits += CalculateVolumeCredits(play, perf);
                }
                return volumeCredits;
            }

            public string GenerateStatement(int totalAmount, int volumeCredits, CultureInfo cultureInfo)
            {
                var result = $"Statement for {_invoice.Customer}\n";
                foreach (var perf in _invoice.Performances)
                {
                    var play = _plays[perf.PlayID];
                    var thisAmount = CalculateThisAmount(play, perf);
                    result += $"  {play.Name}: {Convert.ToDecimal(thisAmount / 100):C} ({perf.Audience} seats)\n";
                }
                result += $"Amount owed is {Convert.ToDecimal(totalAmount / 100):C}\n";
                result += $"You earned {volumeCredits} credits\n";
                return result;
            }

            private int CalculateThisAmount(Play play, Performance perf)
            {
                int thisAmount = 0;
                switch (play.Type)
                {
                    case "tragedy":
                        thisAmount = TragedyBaseAmount;
                        if (perf.Audience > TragedyAudienceThreshold)
                        {
                            thisAmount += 1000 * (perf.Audience - 30);
                        }
                        break;
                    case "comedy":
                        thisAmount = ComedyBaseAmount;
                        if (perf.Audience > ComedyAudienceThreshold)
                        {
                            thisAmount += 10000 + 500 * (perf.Audience - 20);
                        }
                        thisAmount += 300 * perf.Audience;
                        break;
                    default:
                        throw new Exception($"unknown type: {play.Type}");
                }
                return thisAmount;
            }

            private int CalculateVolumeCredits(Play play, Performance perf)
            {
                int volumeCredits = Math.Max(perf.Audience - 30, 0);
                if ("comedy" == play.Type)
                {
                    volumeCredits += (int)Math.Floor((decimal)perf.Audience / ComedyCreditFactor);
                }
                return volumeCredits;
            }
        }
    }
}
