using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GeneticsLab
{

    class ResultTable
    {
        DataGridView m_dataView;

        int m_numberOfSequences = MainForm.NUMBER_OF_SEQUENCES;

        Result[,] m_results;

        public ResultTable(DataGridView dataView, string[] names)                                 // constructor.  Uses array of names for column/row header labels
        {
            m_dataView = dataView;
            m_results = new Result[m_numberOfSequences, m_numberOfSequences];

            for (int i = 0; i < m_numberOfSequences; ++i)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = "seq" + (i+1);
                column.MaxInputLength = 8;
                column.Width = 50;
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                column.CellTemplate = cell;
                m_dataView.Columns.Add(column);
            }
            for (int j = 0; j < m_numberOfSequences; ++j)
            {
                DataGridViewRow row = new DataGridViewRow();                                    // how can I remove the dumb current cell pointer that makes the header cells so big/obfuscates the header text?
                row.HeaderCell.Value = "seq"+(j+1);
                row.Height = 20;
                for (int i = 0; i < m_numberOfSequences; ++i)
                {
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    row.Cells.Add(cell);
                }
                dataView.RowHeadersWidthSizeMode=DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                m_dataView.Rows.Add(row);
            }
            dataView.CurrentCell = null;
        }

        public class Result
        // a structure to hold the results from one alignment: a score and two sequences                               
        {
            int score;
            string mutationSeq1;
            string mutationSeq2;
            List<char> mutationSequence1Reverse;
            List<char> mutationSequence2Reverse;

            public Result()                                                    // constructor
            {
                this.score = 0;
                this.mutationSeq1 = null;
                this.mutationSeq2 = null;
                this.mutationSequence1Reverse = null;
                this.mutationSequence2Reverse = null;
            }

            public int GetScore()                                              // accessor methods
            {
                return score;
            }

            public string GetFirst()
            {
                if (mutationSeq1 == null && mutationSequence1Reverse != null)
                    mutationSeq1 = getMutationStringFromReversed(mutationSequence1Reverse);

                return mutationSeq1;
            }

            public string GetSecond()
            {
                if (mutationSeq2 == null && mutationSequence2Reverse != null)
                    mutationSeq2 = getMutationStringFromReversed(mutationSequence2Reverse);

                return mutationSeq2;
            }

            public void SetScore(int s)                                       // mutator methods
            {
                score = s;
            }

            public void SetFirst(string seq)
            {
                mutationSeq1 = seq;
            }

            public void SetSecond(string seq)
            {
                mutationSeq2 = seq;
            }

            public void Update(int s, List<char> mutSeq1, List<char> mutSeq2)                    // turbo mutator---set all three things at once
            {
                score = s;
                mutationSequence1Reverse = mutSeq1;
                mutationSequence2Reverse = mutSeq2;
            }

            public void Clear()                  
            {
                score = 0;
                mutationSeq1 = null;
                mutationSeq2 = null;
                mutationSequence1Reverse = null;
                mutationSequence2Reverse = null;
            }

            private string getMutationStringFromReversed(List<char> chars)
            {
                StringBuilder sb = new StringBuilder(chars.Count);
                for (int i = chars.Count - 1; i >= 0; i--)
                    sb.Append(chars[i]);

                return sb.ToString();
            }
        }

        public void Clear()                                                                  // clears both results table and dataview 
        {
            for(int i = 0; i < m_numberOfSequences; i++)
            {
                for(int j = i; j < m_numberOfSequences; j++)
                {
                    m_dataView.Rows[i].Cells[j].Value="";
                    m_results[i, j].Clear();
                }
            }
        }

        public void DisplayResult(int x, int y)                                                 // new method to display results in dataview
        {
            int score = m_results[x,y].GetScore();
            if (score == int.MaxValue)
                m_dataView.Rows[x].Cells[y].Value = "***";
            else
                m_dataView.Rows[x].Cells[y].Value = score;
            m_dataView.Update();
        }

        public void AddResult(int x, int y, Result result)                                     // new method to add a result to the results table
        {
            m_results[x, y] = result;
        }

        public Result GetCell(int x, int y)                                                    // accessor method for a result in the results table
        {                                                                                      
            return (m_results[x, y]);
        }
   }
}
