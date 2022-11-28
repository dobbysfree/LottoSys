using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LottoSys
{
    public class CrawWeb
    {
        public static void Get()
        {
            string url = "https://search.naver.com/search.naver?where=nexearch&sm=top_hty&fbm=0&ie=utf8&query=%EB%A1%9C%EB%98%90";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            var nodes = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'win_ball')]");
            if (nodes == null) return;

            WinNum wn   = new WinNum();
            wn.Idx      = Program.WinNums.Count + 1;            
            wn.Date     = DateTime.Now.AddDays(CnvtDate());

            var nums = nodes.InnerText.Trim().Split(' ');
            for (int y = 0; y < 6; y++)
            {
                wn.Nums[y] = int.Parse(nums[y]);
            }
            wn.Bonus = int.Parse(nums.Last());

            wn.SumNum       = wn.Nums.Sum();            
            wn.OddNumCnt    = wn.Nums.Count(x => (x % 2) == 1);
            wn.EvenNumCnt   = wn.Nums.Count(x => (x % 2) == 0);
            wn.FrontSumNum  = wn.Nums[0] + wn.Nums[1] + wn.Nums[2];
            wn.BackSumNum   = wn.Nums[3] + wn.Nums[4] + wn.Nums[5];

            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO tb_win_num VALUES (");
            sb.Append($"{wn.Idx}").Append(", ");
            sb.Append($"'{wn.Date.ToString("yyyy-MM-dd")}'").Append(", ");
            sb.Append($"{wn.Nums[0]}").Append(", ");
            sb.Append($"{wn.Nums[1]}").Append(", ");
            sb.Append($"{wn.Nums[2]}").Append(", ");
            sb.Append($"{wn.Nums[3]}").Append(", ");
            sb.Append($"{wn.Nums[4]}").Append(", ");
            sb.Append($"{wn.Nums[5]}").Append(", ");
            sb.Append($"{wn.Bonus}").Append(", ");
            sb.Append($"{wn.SumNum}").Append(", ");
            sb.Append($"{wn.OddNumCnt}").Append(", ");
            sb.Append($"{wn.EvenNumCnt}").Append(", ");
            sb.Append($"{wn.FrontSumNum}").Append(", ");
            sb.Append($"{wn.BackSumNum}").AppendLine(");");
            DB.Execute(sb.ToString());

            Program.WinNums.Add(wn);
        }

        static int CnvtDate()
        {
            int i = 0;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:      i = -2; break;
                case DayOfWeek.Tuesday:     i = -3; break;
                case DayOfWeek.Wednesday:   i = -4; break;
                case DayOfWeek.Thursday:    i = -5; break;
                case DayOfWeek.Friday:      i = -6; break;
                case DayOfWeek.Saturday:    i = 0; break;
                case DayOfWeek.Sunday:      i = -1; break;
            }
            return i;
        }
    }
}