using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace insoden.Model
{
    public class skdn
    {
        public DateTime NgayDL { get; set; }
        public decimal Socif { get; set; }
        public string TaiKhoan { get; set; }
        public string TenKhachHang { get; set; }
        public decimal DuNo { get; set; }
        public decimal LaiSuat { get; set; }   
    
        public string LoaiTien { get; set; }
        public DateTime NgayVay { get; set; }
        public DateTime NgayDenHan { get; set; }

        public string TrangThai { get; set; }
    }
}
