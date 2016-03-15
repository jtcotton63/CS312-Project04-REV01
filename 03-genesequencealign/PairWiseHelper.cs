using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneticsLab
{
    class PairWiseHelper
    {
        // Cost values
        public static int MATCH_COST = -3;
        public static int SUBSTITUTION_COST = 1;
        public static int INDEL_COST = 5;

        // Parent pathways
        public static int PARENT_INIT = -1;
        public static int PARENT_UP = 0;
        public static int PARENT_DIAGONAL = 1;
        public static int PARENT_LEFT = 2;

        // Other
        private static char ALIGN_PLACEHOLDER = '-';

        public static void intializeFirsts(int[,] computedScore, int[,] parent, int m, int n)
        {
            // Initialize the first cell
            computedScore[0, 0] = 0;
            // Initialize parent
            parent[0, 0] = PARENT_INIT;

            // Initialize the first column
            for (int i = 1; i < m; i++)
            {
                computedScore[i, 0] = i * PairWiseHelper.INDEL_COST;
                parent[i, 0] = PairWiseHelper.PARENT_UP;
            }
            // Initialize the first row
            for (int j = 1; j < n; j++)
            {
                computedScore[0, j] = j * PairWiseHelper.INDEL_COST;
                parent[0, j] = PairWiseHelper.PARENT_LEFT;
            }
        }

        public static Tuple<List<char>, List<char>> getAlignments(GeneSequence a, GeneSequence b, int[,] parent, int m, int n)
        {
            List<char> seqA = new List<char>();
            List<char> seqB = new List<char>();

            int i = m - 1;
            int j = n - 1;
            while (parent[i, j] != PairWiseHelper.PARENT_INIT)
            {
                if (parent[i, j] == PairWiseHelper.PARENT_DIAGONAL)
                {
                    seqA.Add(a.getCharAt(i - 1));
                    seqB.Add(b.getCharAt(j - 1));
                    i--;
                    j--;
                }
                else if (parent[i, j] == PairWiseHelper.PARENT_UP)
                {
                    seqA.Add(a.getCharAt(i - 1));
                    seqB.Add(ALIGN_PLACEHOLDER);
                    i--;
                }
                else
                {
                    seqA.Add(ALIGN_PLACEHOLDER);
                    seqB.Add(b.getCharAt(j - 1));
                    j--;
                }
            }

            return new Tuple<List<char>, List<char>>(seqA, seqB);
        }

        public static void printArray(int[,] array, int m, int n)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                    Console.Write(array[i, j] + ",");
                Console.Write("\n");
            }
        }
    }
}
