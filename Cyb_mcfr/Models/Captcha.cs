namespace Cyb_mcfr.Models
{
    public class Captcha
    {
        public string FileName { get; set; }
        public string WhatToFind { get; set; }
        public int AnswerX { get; set; }
        public int AnswerY { get; set; }

        public static Captcha GetRandomCaptcha()
        {
            var list = new List<Captcha>
            {
                new Captcha{ FileName = "owoce.jpg", WhatToFind = "orange", AnswerX = 162, AnswerY = 158 }
            };

            return list.First();
        }
    }
}
