using System;
using System.Collections.Generic;

namespace GeneticsLab
{
    class PairWiseAlignNotBanded
    {
        private int MAX_ALIGN_LENGTH = -1;

        public PairWiseAlignNotBanded(int maxAlignLength)
        {
            this.MAX_ALIGN_LENGTH = maxAlignLength;
        }

        /// <summary>
        /// this is the function you implement.
        /// </summary>
        /// <param name="sequenceA">the first sequence</param>
        /// <param name="sequenceB">the second sequence, may have length not equal to the length of the first seq.</param>
        /// <returns>the alignment score and the alignment (in a Result object) for sequenceA and sequenceB.  The calling function places the result in the dispay appropriately.
        /// 
        public ResultTable.Result Align_And_Extract(GeneSequence sequenceA, GeneSequence sequenceB)
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
            parent[0, 0] = PairWiseHelper.PARENT_INIT;

            PairWiseHelper.intializeFirsts(computedWeight, parent, m, n);

            for (int i = 1; i < m; i++)
            {
                for (int j = 1; j < n; j++)
                {
                    PairWiseHelper.scoreIndividualCell(a, b, computedWeight, parent, i, j);
                }
            }

            Tuple<List<char>, List<char>> alignments = PairWiseHelper.getAlignments(a, b, parent, m, n);
            return new Tuple<int, Tuple<List<char>, List<char>>>(
                computedWeight[m - 1, n - 1],
                alignments
                );
        }

    }
}