namespace Security2.Dto.Models
{
    public class TimePasswordInfo
    {
        public string FirstPass { get; set; }
        public string SecondPass { get; set; }
        public string ThreePass { get; set; }
        public string FourPass { get; set; }
        public string FivePass { get; set; }
        public string SixPass { get; set; }
        public string SevenPass { get; set; }

        public TimePasswordInfo()
        {
            
        }
        
        public TimePasswordInfo(string firstPass, string secondPass, string threePass, string fourPass, string fivePass, string sixPass, string sevenPass)
        {
            FirstPass = firstPass;
            SecondPass = secondPass;
            ThreePass = threePass;
            FourPass = fourPass;
            FivePass = fivePass;
            SixPass = sixPass;
            SevenPass = sevenPass;
        }
    }
}