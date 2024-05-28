using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS.Controllers
{
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class Data
    {
        public List<KeyValue> membershipCard = new List<KeyValue>();
        public List<KeyValue> customerResources = new List<KeyValue>();
        public List<KeyValue> career = new List<KeyValue>();
        public List<KeyValue> paymentMethods = new List<KeyValue>();
        public List<KeyValue> accountingAccountCode = new List<KeyValue>();
        public List <KeyValue> paramfunction = new List<KeyValue>();
        public void InitializeData()
        {
            membershipCard = new List<KeyValue>
            {
                new KeyValue { Key = "dong", Value = "Đồng" },
                new KeyValue { Key = "bac", Value = "Bạc" },
                new KeyValue { Key = "vang", Value = "Vàng" },
                new KeyValue { Key = "bachkim", Value = "Bạch Kim" },
                new KeyValue { Key = "kimcuong", Value = "Kim Cương" }
            };
            customerResources = new List<KeyValue>
            {
                new KeyValue { Key = "SKH01", Value = "Website" },
                new KeyValue { Key = "SKH02", Value = "Mạng Xã Hội" },
                new KeyValue { Key = "SKH03", Value = "Internet" },
                new KeyValue { Key = "SKH04", Value = "Báo Chí" },
                new KeyValue { Key = "SKH05", Value = "Hội Thảo & Offline" },
                new KeyValue { Key = "SKH06", Value = "Bạn Bè" }
            };
            career = new List<KeyValue>
            {
                new KeyValue { Key = "CNPM", Value = "Công Nghệ Phần Mềm" },
                new KeyValue { Key = "DTVT", Value = "Điện Tử Viễn Thông" },
                new KeyValue { Key = "KT", Value = "Kế Toán" },
                new KeyValue { Key = "KiemToan", Value = "Kiểm Toán" },
                new KeyValue { Key = "MKT", Value = "Marketing" },
                new KeyValue { Key = "NH", Value = "Ngân Hàng" },
                new KeyValue { Key = "QTDN", Value = "Quản Trị Doanh Nghiệp" },
                new KeyValue { Key = "QTKD", Value = "Quản Trị Kinh Doanh" },
                new KeyValue { Key = "QTNL", Value = "Quản Trị Nhân Lực" },
                new KeyValue { Key = "TCDN", Value = "Tài Chính Doanh Nghiệp" },
                new KeyValue { Key = "TTUD", Value = "Toán - Tin ứng Dụng" },
                new KeyValue { Key = "K", Value = "Khác" }
            };
            paymentMethods = new List<KeyValue>
            {
                new KeyValue { Key = "TM", Value = "Tiền Mặt" },
                new KeyValue { Key = "CK", Value = "Chuyển Khoản" },
                new KeyValue { Key = "T", Value = "Thẻ" },
                new KeyValue { Key = "K", Value = "Khác" }
            };
            accountingAccountCode = new List<KeyValue>
            {
                new KeyValue { Key = "131", Value = "Phải Thu Của Khách Hàng" },
                new KeyValue { Key = "136", Value = "Phải Thu Nội Bộ" },
                new KeyValue { Key = "1361", Value = "Vốn kinh doanh ở các đơn vị trực thuộc" },
                new KeyValue { Key = "1368", Value = "Phải thu nội bộ khác" },
                new KeyValue { Key = "141", Value = "Tạm ứng" },
                new KeyValue { Key = "334", Value = "Phải trả người lao động" },
                new KeyValue { Key = "3341", Value = "Phải trả công nhân viên" },
                new KeyValue { Key = "3348", Value = "Phải trả người lao động khác" },
                new KeyValue { Key = "335", Value = "Chi phí phải trả" },
                new KeyValue { Key = "338", Value = "Phải trả, phải nộp khác" },
                new KeyValue { Key = "3382", Value = "Kinh phí công đoàn" },
                new KeyValue { Key = "3383", Value = "Bảo hiểm xã hội" },
                new KeyValue { Key = "3384", Value = "Bảo hiểm y tế" },
                new KeyValue { Key = "3385", Value = "Phải trả về cổ phần hóa" },
                new KeyValue { Key = "3386", Value = "Nhận ký quỹ, ký cược ngắn hạn" },
                new KeyValue { Key = "3387", Value = "Doanh thu chưa thực hiện" },
                new KeyValue { Key = "3388", Value = "Phải trả, phải nộp khác" },
                new KeyValue { Key = "3389", Value = "Bảo hiểm thất nghiệp" },
                new KeyValue { Key = "341", Value = "Vay và nợ thuê tài chính" },
                 new KeyValue { Key = "K", Value = "Khác" }
            };
            paramfunction = new List<KeyValue>
            {
                new KeyValue {Key ="string", Value = "string"},
                new KeyValue {Key="int", Value = "int"},
                new KeyValue {Key="float",Value = "float"},
                new KeyValue {Key="double",Value = "double"},
                new KeyValue {Key="datetime",Value = "datetime"},
                new KeyValue {Key="bool",Value = "bool"},
            };
        }
    }
}