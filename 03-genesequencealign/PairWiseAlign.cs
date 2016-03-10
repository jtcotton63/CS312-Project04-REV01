using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    class PairWiseAlign
    {
        private static int MATCH_COST = -3;
        private static int SUBSTITUTION_COST = 1;
        private static int INDEL_COST = 5;
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
            int score;                                                       // place your computed alignment score here
            string[] alignment = new string[2];                              // place your two computed alignments here


            // ********* these are placeholder assignments that you'll replace with your code  *******
            score = computeOptimalAlignment(sequenceA, sequenceB);                                                
            alignment[0] = "";
            alignment[1] = "";
            // ***************************************************************************************
            

            result.Update(score,alignment[0],alignment[1]);                  // bundling your results into the right object type 
            return(result);
        }

        public int computeOptimalAlignment(GeneSequence a, GeneSequence b)
        {
            // Should only compare at most maxAlignLength chars
            int m = Math.Min(a.getLength() + 1, MAX_ALIGN_LENGTH + 1);
            int n = Math.Min(b.getLength() + 1, MAX_ALIGN_LENGTH + 1);

            int[,] computedWeight = new int[m, n];

            // Initialize the first column
            for (int i = 1; i < m; i++)
            {
                computedWeight[i, 0] = i * INDEL_COST;
            }
            // Initialize the first row
            for (int j = 1; j < n; j++)
            {
                computedWeight[0, j] = j * INDEL_COST;
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

                    computedWeight[i, j] = Math.Min(up, Math.Min(diagonal, left));
                }
            }

            return computedWeight[m - 1, n - 1];
        }
    }
}
