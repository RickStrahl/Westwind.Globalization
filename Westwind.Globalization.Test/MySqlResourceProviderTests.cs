using System;
using NUnit.Framework;
using Westwind.Utilities.Data;

namespace Westwind.Globalization.Test
{
    [TestFixture]
    public class DbResTests
    {

   

        [Test]
        public void DbResSimpleValues()
        {
            Console.WriteLine(DbRes.T("Today", "CommonPhrases", "de-de"));
            Console.WriteLine(DbRes.T("Yesterday", "CommonPhrases", "de-de"));
            Console.WriteLine(DbRes.T("Save", "CommonPhrases", "de-de"));

            Console.WriteLine(DbRes.T("Today","CommonPhrases","en-US"));
            Console.WriteLine(DbRes.T("Yesterday", "CommonPhrases","en-US"));
            Console.WriteLine(DbRes.T("Save", "CommonPhrases","en-US"));
        }
    }
}
