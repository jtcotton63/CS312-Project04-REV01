using System;
using System.Collections.Generic;

namespace GeneticsLab
{
    class PairWiseAlignBanded
    {
        private int MAX_ALIGN_LENGTH = -1;
        private int BAND_LENGTH = 3;
        private List<char> noAlignmentPossibleInReverse = new List<char>
        {'e','l','b', 'i', 's', 's', 'o', 'P', ' ', 't', 'n', 'e', 'm', 'n', 'g', 'i', 'l', 'A', ' ', 'o', 'N'};

        public PairWiseAlignBanded(int maxAlignLength)
        {
            this.MAX_ALIGN_LENGTH = maxAlignLength;
        }

        public PairWiseAlignBanded(int maxAlignLength, int bandLength)
        {
            this.MAX_ALIGN_LENGTH = maxAlignLength;
            this.BAND_LENGTH = bandLength;
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
            // If Item2 is null, the alignment could not be computed
            // using the banded algorithm
            if (scoreAndAlignments.Item2 == null)
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

        /**
         * Computes the optimal alignment of the two gene sequences.
         * Returns a tuple that contains the following:
                item1: the optimal alignment score
                item2: a tuple of two lists of chars:
                        item1: the first alignment sequence as a List<char> 
                               IN REVERSE ORDER.
                        item2: the second alignment sequence as a List<char> 
                               IN REVERSE ORDER.
                * Please note that the two List<char> will be put in the proper
                order before they are displayed to the screen. This is handled
                by the ResultTable.Result class.
                * If this sequence cannot be aligned, item2 will return null.
         */
        public Tuple<int, Tuple<List<char>, List<char>>> computeOptimalAlignment(GeneSequence a, GeneSequence b)
        {
            // Should only compare at most maxAlignLength chars
            int m = Math.Min(a.getLength() + 1, MAX_ALIGN_LENGTH + 1);
            int n = Math.Min(b.getLength() + 1, MAX_ALIGN_LENGTH + 1);

            int[,] computedWeight = new int[m, n];
            int[,] parent = new int[m, n];
            parent[0, 0] = PairWiseHelper.PARENT_INIT;
            // Indicates if using the banded algorithm is possible
            // for aligning these two sequences. If parent[m-1,n-1] is
            // still Int32.Max when the program terminates, then the algorithm
            // never reached the end of the array and the banded algorithm
            // cannot be used for these two sequences.
            parent[m - 1, n - 1] = Int32.MaxValue;

            PairWiseHelper.intializeFirsts(computedWeight, parent, m, n);

            for (int i = 1; i < m; i++)
            {
                // Fill in the scores for the first BAND_LENGTH cells
                // down from the current cell
                int len = Math.Min(BAND_LENGTH, m - i);
                for (int j = i; j < i + len; j++)
                {
                    PairWiseHelper.scoreIndividualCell(a, b, computedWeight, parent, j, i);
                }

                // Fill in the scores for the first BAND_LENGTH cells
                // left from the current cell
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
            // If the value of parent[m-1,n-1] is still 
            // set to Int32.MaxValue, then the parent pointer
            // was never set at [m-1,n-1]. This means that
            // the sizes of the two strings are such that
            // the two strings cannot be aligned using
            // the banded algorithm.
            if (parent[m - 1, n - 1] == Int32.MaxValue)
                return null;

            return PairWiseHelper.getAlignments(a, b, parent, m, n);
        }
    }
}
