using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rk3.Toolset;

namespace ToolsetTest
{
    [TestClass]
    public class WordSearchTest
    {
        [TestMethod]
        public void Add_WhenGivenTwoStrings_CreatesBKTree()
        {
            var ws = new WordSearch();

            ws.Add("Book");
            ws.Add("Cake");
            ws.Add("Computer");

            var lstWords = ws.Search("Cape", 2);

            CollectionAssert.Contains(lstWords, "cake");
        }
    }
}
