namespace GeneticsLab
{
    class GeneSequence
    {
        private string name;
        private string sequence;

        public GeneSequence(string name, string sequence)
        {
            this.name = name;
            this.sequence = sequence;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string Sequence
        {
            get
            {
                return sequence;
            }
        }

        public int getLength()
        {
            return sequence.Length;
        }

        public char getCharAt(int index)
        {
            return sequence[index];
        }
    }
}
