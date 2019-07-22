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
        public DateTime NgayDuyet { get; set; }       

        public string TrangThai { get; set; }

        public decimal chovay { get; set; }
        public decimal thuno { get; set; }

        public decimal thoigian { get; set; }
        public DateTime KyHanGoc { get; set; }
        public decimal phaitra { get; set; }
        public string quanhe { get; set; }

        public DateTime? denngay { get; set; }

        public string LoaiSaoKe { get; set; }
    }
}
