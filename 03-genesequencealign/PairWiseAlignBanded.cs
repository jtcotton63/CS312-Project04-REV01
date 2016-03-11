using System;
using System.Collections.Generic;

namespace GeneticsLab
{
    class PairWiseAlignBanded
    {
        private char ALIGN_PLACEHOLDER = '-';
        private int MAX_ALIGN_LENGTH = -1;
        private int BAND_LENGTH = 3;
        private List<char> noAlignmentPossibleInReverse = new List<char>
        {'e','l','b', 'i', 's', 's', 'o', 'P', ' ', 't', 'n', 'e', 'm', 'n', 'g', 'i', 'l', 'A', ' ', 'o', 'N'};

        public PairWiseAlignBanded(int maxAlignLength)
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
        public ResultTable.Result Align_And_Extract(GeneSequence sequenceA, GeneSequence sequenceB)
        {
            ResultTable.Result result = new ResultTable.Result();
            Tuple<int, Tuple<List<char>, List<char>>> scoreAndAlignments = computeOptimalAlignment(sequenceA, sequenceB);
            int score;
            List<char> one;
            List<char> two;
            if(scoreAndAlignments.Item2 == null)
            {
                score = Int32.MaxValue;
                one = noAlignmentPossibleInReverse;
                two = noAlignmentPossibleInReverse;
            }
            else
            {
                score = scoreAndAlignments.Item1;
                one = scoreAndAlignments.Item2.Item1;
                two = scoreAndAlignments.Item2.Item2;
            }
                
            result.Update(score, one, two);
            return result;
        }

        public Tuple<int, Tuple<List<char>, List<char>>> computeOptimalAlignment(GeneSequence a, GeneSequence b)
        {
            // Should only compare at most maxAlignLength chars
            int m = Math.Min(a.getLength() + 1, MAX_ALIGN_LENGTH + 1);
            int n = Math.Min(b.getLength() + 1, MAX_ALIGN_LENGTH + 1);

            int[,] computedWeight = new int[m, n];
            int[,] parent = new int[m, n];
            parent[0, 0] = PairWiseHelper.PARENT_INIT;
            parent[m - 1, n - 1] = Int32.MaxValue;

            PairWiseHelper.intializeFirsts(computedWeight, parent, m, n);

            for (int i = 1; i < m; i++)
            {
                // Down from i,i
                int len = Math.Min(BAND_LENGTH, m - i);
                for (int j = i; j < i + len; j++)
                {
                    PairWiseHelper.scoreIndividualCell(a, b, computedWeight, parent, j, i);
                }

                // Right from i,i
                int len2 = Math.Min(BAND_LENGTH, n - i);
                for (int j = i; j < i + len2; j++)
                {
                    PairWiseHelper.scoreIndividualCell(a, b, computedWeight, parent, i, j);
                }
            }

            Tuple<List<char>, List<char>> alignments = getAlignmentsCheckBandedSuccessful(a, b, parent, m, n);
            return new Tuple<int, Tuple<List<char>, List<char>>>(
                computedWeight[m - 1, n - 1],
                alignments
                );
        }

        private Tuple<List<char>, List<char>> getAlignmentsCheckBandedSuccessful(GeneSequence a, GeneSequence b, int[,] parent, int m, int n)
        {
            // These two sequences can't be reconciled thru
            // the banded algorithm
            if (parent[m - 1, n - 1] == Int32.MaxValue)
                return null;

            return PairWiseHelper.getAlignments(a, b, parent, m, n);
        }
    }
}
