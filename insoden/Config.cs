using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace insoden
{
    public class Config : AppSettings<Config>
    {
        public ThongTin HSC = new ThongTin() { };
        public ThongTin TraNoc = new ThongTin() { };
        public ThongTin NinhKieu = new ThongTin() { };
        public ThongTin ThotNot = new ThongTin() { };

    }

    public class ThongTin {
        
        public string TenChiNhanh { get; set; }
        public string DiaChi { get; set; }
        public string tencn_vi { get; set; }
        public string tencn_en { get; set; }
        public string diachi { get; set; }
        public string tinh { get; set; }
        public string fax { get; set; }
        public string dt { get; set; }
        public string tennguoiky { get; set; }
        public string chucdanh { get; set; }
        public string noinhan { get; set; }
        public string tenfilechuky { get; set; }
    }
}
