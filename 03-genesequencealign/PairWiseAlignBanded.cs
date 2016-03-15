using System;
using System.Collections.Generic;

namespace GeneticsLab
{
    class PairWiseAlignBanded
    {
        private int MAX_ALIGN_LENGTH = -1;
        private int BAND_LENGTH = 3;
        private int BANDED_EMPTY_CELL_PLACEHOLDER = Int32.MaxValue;
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

            int[,] computedScore = new int[m, n];
            // Initialize all the values in computedScore
            // array to be Int32.Max. This prevents the banded
            // algorithm from choosing values outside the band,
            // which are typically set to 0 by default.
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                    computedScore[i, j] = BANDED_EMPTY_CELL_PLACEHOLDER;
            }
            int[,] parent = new int[m, n];
            // Indicates if using the banded algorithm is possible
            // for aligning these two sequences. If parent[m-1,n-1] is
            // still Int32.Max when the program terminates, then the algorithm
            // never reached the end of the array and the banded algorithm
            // cannot be used for these two sequences.
            parent[m - 1, n - 1] = Int32.MaxValue;

            PairWiseHelper.intializeFirsts(computedScore, parent, m, n);

            for (int i = 1; i < m; i++)
            {
                // Compute the score of the current cell
                scoreIndividualCell(a, b, computedScore, parent, i, i);

                // Fill in the scores for the first BAND_LENGTH cells
                // down from the current cell
                int len = Math.Min(BAND_LENGTH, m - i);
                for (int j = i; j < i + len; j++)
                {
                    scoreIndividualCell(a, b, computedScore, parent, j, i);
                }

                // Fill in the scores for the first BAND_LENGTH cells
                // left from the current cell
                int len2 = Math.Min(BAND_LENGTH, n - i);
                for (int j = i; j < i + len2; j++)
                {
                    scoreIndividualCell(a, b, computedScore, parent, i, j);
                }
            }

            Tuple<List<char>, List<char>> alignments = getAlignmentsCheckBandedSuccessful(a, b, parent, m, n);
            return new Tuple<int, Tuple<List<char>, List<char>>>(
                computedScore[m - 1, n - 1],
                alignments
                );
        }

        // Computes the score of an individual cell 
        public void scoreIndividualCell(GeneSequence a, GeneSequence b,
            int[,] computedScore, int[,] parent, int i, int j)
        {
            // Compute the score if the cell above
            // were chosen as the parent.
            // If computedScore[i, j] == BANDED_EMPTY_CELL_PLACEHOLDER,
            // this means that the cell value is outside the bounds
            // of the banded algorithm. It is necessary to keep the
            // placeholder value in the up cell.
            // If it is not equal to BANDED_EMPTY_CELL_PLACEHOLDER,
            // we want the up cell to be taken into account when
            // choosing the smallest parent score for the current cell. 
            int up = computedScore[i - 1, j];
            if(up != BANDED_EMPTY_CELL_PLACEHOLDER)
                up += PairWiseHelper.INDEL_COST;
            // Compute the score if the cell to the left
            // were chosen as the parent
            // 
            int left = computedScore[i, j - 1];
            if(left != BANDED_EMPTY_CELL_PLACEHOLDER)
                left += PairWiseHelper.INDEL_COST;

            // Compute the score if the cell diagonally
            // up and to the left were chosen as the parent.
            // If the two letters are the same, this means that
            // it would be a match; otherwise it is a substitution.
            int diagonal = computedScore[i - 1, j - 1];
            if(diagonal != BANDED_EMPTY_CELL_PLACEHOLDER)
            {
                char aLetter = a.getCharAt(i - 1);
                char bLetter = b.getCharAt(j - 1);
                if (aLetter.Equals(bLetter))
                    diagonal += PairWiseHelper.MATCH_COST;
                else
                    diagonal += PairWiseHelper.SUBSTITUTION_COST;
            }

            // Determine which score is the smallest
            // The use of <= instead of < helps to 
            // diffuse cases when two of the values
            // are equal to each other.
            if (up <= diagonal && up <= left)
            {
                computedScore[i, j] = up;
                parent[i, j] = PairWiseHelper.PARENT_UP;
            }
            else if (diagonal <= up && diagonal <= left)
            {
                computedScore[i, j] = diagonal;
                parent[i, j] = PairWiseHelper.PARENT_DIAGONAL;
            }
            else
            {
                computedScore[i, j] = left;
                parent[i, j] = PairWiseHelper.PARENT_LEFT;
            }
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
