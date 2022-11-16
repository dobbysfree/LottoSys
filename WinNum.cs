namespace LottoSys
{
    public class WinNum
    {
        public int Idx { get; set; }
        public DateTime Date { get; set; }

        public int[] Nums { get; set; } = new int[6];
        public string StrNums { get { return string.Join(',', Nums); } }

        public int Bonus { get; set; }

        public int SumNum { get; set; }         // 번호합
        public int OddNumCnt { get; set; }      // 홀수 갯수
        public int EvenNumCnt { get; set; }     // 짝수 갯수
        public int FrontSumNum { get; set; }    // 앞수합
        public int BackSumNum { get; set; }     // 뒷수합
        public string Position { get; set; }
    }
}