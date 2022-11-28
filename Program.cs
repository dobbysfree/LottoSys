using Microsoft.Extensions.Configuration;
using System.Text;

namespace LottoSys
{
    class Program
    {
        #region variable
        const int GameCount = 10;
        public static List<WinNum> WinNums { get; set; } = new List<WinNum>();
        public static IConfiguration IConf { get; set; }
        static Dictionary<int, (int, int)> Gubun { get; set; } = new Dictionary<int, (int, int)>
        {
            { 0, (1, 10) },
            { 1, (10, 20) },
            { 2, (20, 30) },
            { 3, (30, 40) },
            { 4, (40, 46) }
        };
        #endregion

        #region main
        static void Main(string[] args)
        {
            IConf = new ConfigurationBuilder().AddJsonFile("conf.json").Build();

            DB.SelectSingle("SELECT * FROM tb_win_num ORDER BY idx;");

            if (bool.Parse(IConf["ActiveCraw"])) CrawWeb.Get();
            
            FindGame();
        }
        #endregion

        #region Find Game Number
        static void FindGame()
        {
            var recentWin   = WinNums.TakeLast(30);
            var rankPos     = WinNums.GroupBy(x => x.Position).Select(g => new { Key = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).Take(10).ToList();


            foreach (var pos in recentWin)
            {
                var key = rankPos.FirstOrDefault(x => x.Key == pos.Position);
                if (key != null) rankPos.Remove(key);
            }

            var positions   = rankPos.Select(x => x.Key).ToList();

            var recent      = WinNums.TakeLast(5); // 최근 5주 번호 삭제
            var baseNum     = Enumerable.Range(1, 45).ToList();

            foreach (var arr in recent)
            {
                foreach (var num in arr.Nums)
                {
                    if (baseNum.Contains(num)) baseNum.Remove(num);
                }
            }

            int missNum = 5;
            int minSum = 110;
            StringBuilder sb = new StringBuilder();
            string date = DateTime.Now.AddDays(CnvtDate()).ToString("yyyy-MM-dd");

            List<string> GameNum = new List<string>();
            foreach (var pos in positions)
            {
                while (true)
                {
                    List<int> game = new List<int>();

                    string[] ky = pos.Split('-');
                    for (int i = 0; i < ky.Length; i++)
                    {
                        var group = baseNum.Where(x => x >= Gubun[i].Item1 && x < Gubun[i].Item2).ToList();

                        for (int j = 0; j < int.Parse(ky[i]); j++)
                        {
                            int number = group[new Random().Next(group.Count)];
                            game.Add(number);
                            group.Remove(number);                            
                        }
                    }

                    int sum     = game.Sum();
                    string str  = string.Join(',', game);

                    if (WinNums.Where(x => x.StrNums == str).Count() == 0 && (sum > minSum && sum < 170))
                    {
                        Console.WriteLine($"SET > {str} > {sum}");
                        GameNum.Add(str);
                        //sb.AppendLine($"INSERT INTO tb_game VALUES ('{date}', {str});");
                        break;
                    }
                    else
                    {
                        missNum--;
                        if (missNum == 0) 
                        {
                            minSum -= 10;
                            missNum = 5;
                        }
                    }
                }
            }


            try
            {
                baseNum = Enumerable.Range(1, 45).ToList();
                foreach (var gm in GameNum)
                {
                    string[] arr = gm.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        int num = int.Parse(arr[i]);
                        if (baseNum.Contains(num)) baseNum.Remove(num);
                    }
                }

                recent = WinNums.TakeLast(1);
                foreach (var arr in recent)
                {
                    foreach (var num in arr.Nums)
                    {
                        if (baseNum.Contains(num)) baseNum.Remove(num);
                    }
                }

                rankPos = WinNums.GroupBy(x => x.Position).Select(g => new { Key = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count).Take(30).ToList();
                var extraPos = rankPos[new Random().Next(rankPos.Count)];


                while (true)
                {
                    List<int> game = new List<int>();
                    string[] ky = extraPos.Key.Split('-');

                    for (int i = 0; i < ky.Length; i++)
                    {
                        var group = baseNum.Where(x => x >= Gubun[i].Item1 && x < Gubun[i].Item2).ToList();

                        for (int j = 0; j < int.Parse(ky[i]); j++)
                        {
                            int number = group[new Random().Next(group.Count)];


                            game.Add(number);
                            group.Remove(number);
                            if (GameNum.Count % 2 == 0) baseNum.Remove(number);
                        }
                    }

                    int sum     = game.Sum();
                    string str  = string.Join(',', game);

                    if (WinNums.Where(x => x.StrNums == str).Count() == 0 && !GameNum.Contains(str))
                    {
                        Console.WriteLine($"{str} > {sum}");
                        GameNum.Add(str);
                        sb.AppendLine($"INSERT INTO tb_game VALUES ('{date}', {str});");
                    }

                    if (GameNum.Count == 10) break;
                }
                                
                if (GameNum.Count > 0) DB.Execute(sb.ToString());
            }
            catch (Exception)
            {
                FindGame();
            }  
        }
        #endregion

        static int CnvtDate()
        {
            int i = 0;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:      i = 5; break;
                case DayOfWeek.Tuesday:     i = 4; break;
                case DayOfWeek.Wednesday:   i = 3; break;
                case DayOfWeek.Thursday:    i = 2; break;
                case DayOfWeek.Friday:      i = 1; break;
                case DayOfWeek.Saturday:    i = 0; break;
                case DayOfWeek.Sunday:      i = 6; break;
            }
            return i;
        }
    }
}