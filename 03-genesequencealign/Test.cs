using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            PairWiseAlign pwa = new PairWiseAlign(5000);
            int result = pwa.computeOptimalAlignment(seqA, seqB);
            if (result != expected)
                throw new SystemException("Test01 failed");
        }

        private static void test02()
        {
            int expected = -1;
            GeneSequence seqA = new GeneSequence(null, "POLYNOMIAL");
            GeneSequence seqB = new GeneSequence(null, "EXPONENTIAL");

            PairWiseAlign pwa = new PairWiseAlign(5000);
            int result = pwa.computeOptimalAlignment(seqA, seqB);
            if (result != expected)
                throw new SystemException("Test02 failed");
        }
    }
}
