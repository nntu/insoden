using System;

namespace insoden
{
    public class DatainNoCo
    {
        public string cn_motk { get; set; }

        public string cn_giaodich { get; set; }

        public string cifno { get; set; }

        public string tentk { get; set; }

        public string tk { get; set; }

        public string loaitien { get; set; }

        public string loai { get; set; }

        public decimal sotien { get; set; }

        public decimal sodu { get; set; }

        public string ghichu { get; set; }

        public int cngd { get; set; }

        public string truser { get; set; }

        public string auxtrc { get; set; }

        public string chuthich_aux { get; set; }

        public DateTime ngaygiaodich { get; set; }

        public int seq { get; set; }

        public decimal trancd { get; set; }

        public DateTime ngayin { get; set; }
    }

    public class filedd
    {
        public string nguon { get; set; }

        public string ddhist { get; set; }
    }

    public class ListItem_1
    {
        public string id { get; set; }

        public string name { get; set; }

        public string id_name { get; set; }
    }

    public class ThongTinNganHang
    {
        public string tencn_vi { get; set; }

        public string tencn_en { get; set; }

        public string diachi { get; set; }

        public string tinh { get; set; }

        public string fax { get; set; }

        public string dt { get; set; }

        public string tennguoiky { get; set; }

        public string chucdanh { get; set; }

        public string noinhan { get; set; }

        public DateTime ngaycuoiky { get; set; }

        public string cfname1 { get; set; }

        public string cfname2 { get; set; }

        public string addr1 { get; set; }

        public string addr2 { get; set; }

        public string cifno { get; set; }

        public string tenfilechuky { get; set; }
    }

    public class incif
    {
        public string acctno { get; set; }

        public double cbal { get; set; }

        public string currtyp { get; set; }

        public string acctyp { get; set; }

        public string cfindi { get; set; }
    }

    public class Lsgl
    {
        public DateTime? Ngayht { get; set; }

        public DateTime? Ngayhl { get; set; }

        public decimal Ghino { get; set; }

        public decimal Ghico { get; set; }

        public string Nguon { get; set; }

        public string Sottgd { get; set; }

        public string Mota { get; set; }
    }

    public class doichieuDienluc
    {
        public string MaKh { get; set; }
        public string TenKH { get; set; }
        public string TK { get; set; }
        public string DiaChi { get; set; }
        public string KyHoaDon { get; set; }
        public DateTime? NgayTT { get; set; }

        public decimal Tien { get; set; }
        public decimal Thue { get; set; }
        public decimal TongTien { get; set; }
        public string GhiChu { set; get; }
    }

    public class dcdienluc
    {
        public int STT { get; set; }
        public decimal MaGd { get; set; }
        public DateTime NgayGD { get; set; }
        public string LoaiTien { get; set; }
        public decimal PSNo { get; set; }

        public decimal PSCo { get; set; }
        public decimal SoDu { get; set; }

        public string GhiChu { set; get; }
    }

    public class saokeTienVay
    {
        public DateTime Ngay { get; set; }
        public string gio { get; set; }
        public string MaGD { get; set; }
        public string MoTaMaGD { get; set; }
        public DateTime NgayLHGD { get; set; }
        public string MaSP { get; set; }
        public decimal Sotien { get; set; }
        public string LoaiTien { get; set; }
        public string DuNo { get; set; }
        public string MaGDV { get; set; }
        public string DienGiai { get; set; }
        public string CN { get; set; }
    }
}