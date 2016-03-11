using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    class PairWiseAlign
    {
        // Cost values
        private int MATCH_COST = -3;
        private int SUBSTITUTION_COST = 1;
        private int INDEL_COST = 5;

        // Parent pathways
        private int PARENT_INIT = -1;
        private int PARENT_UP = 0;
        private int PARENT_DIAGONAL = 1;
        private int PARENT_LEFT = 2;

        private char ALIGN_PLACEHOLDER = '-';

        private int MAX_ALIGN_LENGTH = -1;

        public PairWiseAlign(int maxAlignLength)
        {
            this.MAX_ALIGN_LENGTH = maxAlignLength;
        }

        /// <summary>
        /// this is the function you implement.
        /// </summary>
        /// <param name="sequenceA">the first sequence</param>
        /// <param name="sequenceB">the second sequence, may have length not equal to the length of the first seq.</param>
        /// <param name="banded">true if alignment should be band limited.</param>
        /// <returns>the alignment score and the alignment (in a Result object) for sequenceA and sequenceB.  The calling function places the result in the dispay appropriately.
        /// 
        public ResultTable.Result Align_And_Extract(GeneSequence sequenceA, GeneSequence sequenceB, bool banded)
        {
            ResultTable.Result result = new ResultTable.Result();
            Tuple<int, Tuple<List<char>, List<char>>> scoreAndAlignments = computeOptimalAlignment(sequenceA, sequenceB);
            result.Update(scoreAndAlignments.Item1, scoreAndAlignments.Item2.Item1, scoreAndAlignments.Item2.Item2);
            return result;
        }

        public Tuple<int, Tuple<List<char>, List<char>>> computeOptimalAlignment(GeneSequence a, GeneSequence b)
        {
            // Should only compare at most maxAlignLength chars
            int m = Math.Min(a.getLength() + 1, MAX_ALIGN_LENGTH + 1);
            int n = Math.Min(b.getLength() + 1, MAX_ALIGN_LENGTH + 1);

            int[,] computedWeight = new int[m, n];
            int[,] parent = new int[m, n];
            parent[0, 0] = PARENT_INIT;

            // Initialize the first column
            for (int i = 1; i < m; i++)
            {
                computedWeight[i, 0] = i * INDEL_COST;
                parent[i, 0] = PARENT_UP;
            }
            // Initialize the first row
            for (int j = 1; j < n; j++)
            {
                computedWeight[0, j] = j * INDEL_COST;
                parent[0, j] = PARENT_LEFT;
            }

            for (int i = 1; i < m; i++)
            {
                for (int j = 1; j < n; j++)
                {
                    int up = computedWeight[i - 1, j] + INDEL_COST;
                    int left = computedWeight[i, j - 1] + INDEL_COST;

                    // Do the diagonal stuff
                    int diagonal = computedWeight[i - 1, j - 1];
                    char aLetter = a.getCharAt(i - 1);
                    char bLetter = b.getCharAt(j - 1);
                    if (aLetter.Equals(bLetter))
                        diagonal += MATCH_COST;
                    else
                        diagonal += SUBSTITUTION_COST;

                    // Determine which value is bigger
                    if (up <= diagonal && up <= left)
                    {
                        computedWeight[i, j] = up;
                        parent[i, j] = PARENT_UP;
                    }
                    else if (diagonal <= up && diagonal <= left)
                    {
                        computedWeight[i, j] = diagonal;
                        parent[i, j] = PARENT_DIAGONAL;
                    }
                    else
                    {
                        computedWeight[i, j] = left;
                        parent[i, j] = PARENT_LEFT;
                    }
                }
            }

            Tuple<List<char>, List<char>> alignments = getAlignments(a, b, parent, m, n);
            return new Tuple<int, Tuple<List<char>, List<char>>>(
                computedWeight[m - 1, n - 1],
                alignments
                );
        }

        private Tuple<List<char>, List<char>> getAlignments(GeneSequence a, GeneSequence b, int[,] parent, int m, int n)
        {
            List<char> seqA = new List<char>();
            List<char> seqB = new List<char>();

            int i = m - 1;
            int j = n - 1;
            while(parent[i, j] != PARENT_INIT)
            {
                if (parent[i, j] == PARENT_DIAGONAL)
                {
                    seqA.Add(a.getCharAt(i - 1));
                    seqB.Add(b.getCharAt(j - 1));
                    i--;
                    j--;
                }
                else if (parent[i, j] == PARENT_UP)
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

            if (seqA.Count != seqB.Count)
                throw new SystemException("Sequences of strings " + a.Name + ", " + b.Name + " should be equal in length");

            return new Tuple<List<char>, List<char>>(seqA, seqB);
        }
    }
}
