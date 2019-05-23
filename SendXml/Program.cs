using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendXml
{
    class Program
    {
        static void Main(string[] args)
        {
            SendSmsXml r =new SendSmsXml("","");
            r.CreateMessage("89671423232","polz","Текст смсмсмс!");
            r.Send();
        }
    }
}
