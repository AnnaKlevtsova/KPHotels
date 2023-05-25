using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPhotels
{
    internal class DataFile
    {
        public static  int customerid { get; set; }
        public static  string customername { get; set; }
        public static string customersurname { get; set; }
        public static  string customerpatronymic { get; set; }
        public static  int roomid { get; set; }
        public static string hotelsname { get; set; }
        public static  int price { get; set; }
        public static string start { get; set; }

        public static  string finish { get; set; }
        public static  int duration { get; set; }
        public DataFile()
        {
            customerid = Numbers1.customerid;
            customername = Numbers1.customername;
            customersurname = Numbers1.customersurname;
            customerpatronymic = Numbers1.customerpatronymic;
            hotelsname = Numbers1.hotelsname;
            roomid = Numbers1.roomid;
            price = Numbers1.price;
            start = Numbers1.start;
            finish = Numbers1.finish;
            duration = Numbers1.duration;

        }
    }
}
