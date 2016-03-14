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

        public static void intializeFirsts(int[,] computedWeight, int[,] parent, int m, int n)
        {
            // Initialize the first column
            for (int i = 1; i < m; i++)
            {
                computedWeight[i, 0] = i * PairWiseHelper.INDEL_COST;
                parent[i, 0] = PairWiseHelper.PARENT_UP;
            }
            // Initialize the first row
            for (int j = 1; j < n; j++)
            {
                computedWeight[0, j] = j * PairWiseHelper.INDEL_COST;
                parent[0, j] = PairWiseHelper.PARENT_LEFT;
            }
        }

        // Computes the score of an individual cell 
        public static void scoreIndividualCell(GeneSequence a, GeneSequence b,
            int[,] computedScore, int[,] parent, int i, int j)
        {
            // Compute the score if the cell above
            // were chosen as the parent
            int up = computedScore[i - 1, j] + INDEL_COST;
            // Compute the score if the cell to the left
            // were chosen as the parent
            int left = computedScore[i, j - 1] + INDEL_COST;

            // Compute the score if the cell diagonally
            // up and to the left were chosen as the parent.
            // If the two letters are the same, this means that
            // it would be a match; otherwise it is a substitution.
            int diagonal = computedScore[i - 1, j - 1];
            char aLetter = a.getCharAt(i - 1);
            char bLetter = b.getCharAt(j - 1);
            if (aLetter.Equals(bLetter))
                diagonal += MATCH_COST;
            else
                diagonal += SUBSTITUTION_COST;

            // Determine which score is the smallest
            // The use of <= instead of < helps to 
            // diffuse cases when two of the values
            // are equal to each other.
            if (up <= diagonal && up <= left)
            {
                computedScore[i, j] = up;
                parent[i, j] = PARENT_UP;
            }
            else if (diagonal <= up && diagonal <= left)
            {
                computedScore[i, j] = diagonal;
                parent[i, j] = PARENT_DIAGONAL;
            }
            else
            {
                computedScore[i, j] = left;
                parent[i, j] = PARENT_LEFT;
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
    }
}
