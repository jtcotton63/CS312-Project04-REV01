using System;
using System.Collections.Generic;

namespace GeneticsLab
{
    class Test
    {
        public static void runTests()
        {
            test01();
            test02();
        }

        private static void test01()
        {
            int expected = -2;
            GeneSequence seqA = new GeneSequence(null, "ATGCC");
            GeneSequence seqB = new GeneSequence(null, "TACGCA");

            PairWiseAlignNotBanded pwa = new PairWiseAlignNotBanded(5000);
            Tuple<int, Tuple<List<char>, List<char>>> stuff = pwa.computeOptimalAlignment(seqA, seqB);
            int result = stuff.Item1;
            if (result != expected)
                throw new SystemException("Test01 failed");
        }

        private static void test02()
        {
            int expected = -1;
            GeneSequence seqA = new GeneSequence(null, "POLYNOMIAL");
            GeneSequence seqB = new GeneSequence(null, "EXPONENTIAL");

            PairWiseAlignNotBanded pwa = new PairWiseAlignNotBanded(5000);
            Tuple<int, Tuple<List<char>, List<char>>> stuff = pwa.computeOptimalAlignment(seqA, seqB);
            int result = stuff.Item1;
            if (result != expected)
                throw new SystemException("Test02 failed");
        }
    }
}
