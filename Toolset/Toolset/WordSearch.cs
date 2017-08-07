using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rk3.Toolset
{
    public class WordSearch
    {
        private WSNode _Root;

        public void Add(string word)
        {
            word = word.ToLower();
            if (_Root == null)
            {
                _Root = new WSNode(word);
                return;
            }

            var currentNode = _Root;

            var distance = LevenshteinDistance(currentNode.Word, word);

            while (currentNode.ContainsKeys(distance))
            {
                if (distance == 0)
                    return;

                currentNode = currentNode[distance];
                distance = LevenshteinDistance(currentNode.Word, word);
            }

            currentNode.AddChild(distance, word);
        }

        public List<string> Search(string word, int d)
        {
            var rtn = new List<string>();

            word = word.ToLower();

            RecursiveSearch(_Root, rtn, word, d);

            return rtn;
        }

        private void RecursiveSearch(WSNode node, List<string> rtn, string word, int d)
        {
            var currentDistance = LevenshteinDistance(node.Word, word);
            var minDistance = currentDistance - d;
            var maxDistance = currentDistance + d;

            if (currentDistance <= d)
                rtn.Add(node.Word);

            foreach (var key in node.Keys.Cast<int>().Where(x => minDistance <= x && x <= maxDistance))
            {
                RecursiveSearch(node[key], rtn, word, d);
            }
        }

        public static int LevenshteinDistance(string first, string second)
        {
            //if the word length is 0, then the distance will always equal the other word length
            if (first.Length == 0) return second.Length;
            if (second.Length == 0) return first.Length;

            var lenFirst = first.Length;
            var lenSecond = second.Length;

            var iDistance = new int[lenFirst + 1, lenSecond + 1];

            for (var i = 0; i <= lenFirst; i++)
            {
                iDistance[i, 0] = i;
            }

            for (var i = 0; i <= lenSecond; i++)
            {
                iDistance[0, i] = i;
            }

            for (var i = 1; i <= lenFirst; i++)
            {
                for (var j = 1; j <= lenSecond; j++)
                {
                    //substition cost
                    var match = (first[i - 1] == second[j - 1]) ? 0 : 1;

                    //Minimum distance after deletion, insertion, or subsitution
                    iDistance[i, j] = Math.Min(Math.Min(iDistance[i - 1, j] + 1, iDistance[i, j - 1] + 1), iDistance[i - 1, j - 1] + match);
                }
            }

            return iDistance[lenFirst, lenSecond];
        }
    }

    internal class WSNode
    {
        public string Word { get; set; }
        public HybridDictionary Children { get; set; }

        public WSNode()
        {

        }

        public WSNode(string word)
        {
            this.Word = word.ToLower();
        }

        public WSNode this[int key]
        {
            get { return (WSNode)Children[key]; }
        }

        public ICollection Keys
        {
            get
            {
                if (Children == null)
                    return new ArrayList();
                return Children.Keys;
            }
        }

        public bool ContainsKeys(int Key)
        {
            return Children != null && Children.Contains(Key);
        }

        public void AddChild(int Key, string word)
        {
            if (this.Children == null)
                Children = new HybridDictionary();

            this.Children[Key] = new WSNode(word);
        }
    }
}
