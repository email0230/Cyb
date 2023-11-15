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
                new Captcha{ FileName = "owoce.jpg", WhatToFind = "orange", AnswerX = 162, AnswerY = 158 },
                new Captcha{ FileName = "owoce2.jpg", WhatToFind = "bananas", AnswerX = 34, AnswerY = 161 },
                new Captcha{ FileName = "owoce3.jpg", WhatToFind = "apple", AnswerX = 40, AnswerY = 53 },
                new Captcha{ FileName = "owoce4.jpg", WhatToFind = "strawberry", AnswerX = 106, AnswerY = 49 },
                new Captcha{ FileName = "owoce5.jpg", WhatToFind = "watermelon", AnswerX = 142, AnswerY = 149 },
                new Captcha{ FileName = "owoce6.jpg", WhatToFind = "carrot", AnswerX = 37, AnswerY = 103 },
            };

            var x = Random.Shared.Next(0, list.Count);

            return list[x];
        }
    }
}
