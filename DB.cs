using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace LottoSys
{
    public class DB
    {
        #region single
        public static void SelectSingle(string query)
        {
            DataTable dt = new DataTable();

            using (MySqlConnection con = new MySqlConnection(Program.IConf["db"] + ";sslmode=None;ConnectionTimeout=100"))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(query.ToString(), con))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {
                        sda.SelectCommand = cmd;
                        cmd.CommandTimeout = 180;

                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(ds);
                            dt = ds.Tables[0];

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var row = dt.Rows[i].ItemArray;

                                var wn      = new WinNum();
                                wn.Idx      = (int)row[0];
                                wn.Date     = (DateTime)row[1];

                                (int, int, int, int, int) pos = new();

                                for (int y = 0; y < 6; y++)
                                {
                                    int number = (int)row[2 + y];
                                    wn.Nums[y] = number;

                                    if (number >= 1 && number < 10) pos.Item1++;
                                    else if (number >= 10 && number < 20) pos.Item2++;
                                    else if (number >= 20 && number < 30) pos.Item3++;
                                    else if (number >= 30 && number < 40) pos.Item4++;
                                    else if (number >= 40 && number <= 45) pos.Item5++;
                                }
                                
                                wn.Bonus    = (int)row[8];
                                wn.Nums     = wn.Nums.OrderBy(x => x).ToArray();

                                wn.SumNum       = (int)row[9];
                                wn.OddNumCnt    = (int)row[10];
                                wn.EvenNumCnt   = (int)row[11];
                                wn.FrontSumNum  = (int)row[12];
                                wn.BackSumNum   = (int)row[13];

                                wn.Position = $"{pos.Item1}-{pos.Item2}-{pos.Item3}-{pos.Item4}-{pos.Item5}";
                                //Console.WriteLine($"{wn.Idx} > {wn.Position}");

                                Program.WinNums.Add(wn);
                            }
                        }
                    }
                }
            }
        }
        #endregion


        #region execute
        public static void Execute(string query)
        {
            if (string.IsNullOrEmpty(query)) return;

            try
            {
                using (MySqlConnection con = new MySqlConnection(Program.IConf["db"] + ";sslmode=None;ConnectionTimeout=100"))
                {
                    con.Open();
                    new MySqlCommand(query, con).ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        #endregion
    }
}