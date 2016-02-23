using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookSDK.Models
{
    public class AccessTokenData
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Issued { get; set; }
        public string State { get; set; }
        public string AppId { get; set; }
        public string FacebookId { get; set; }
        public string[] CurrentPermissions { get; set; }

        public AccessTokenData()
        {
        }

        /// <summary>
        /// Xóa hết dữ liệu của AccessTokenData về trạng thái ban đầu
        /// </summary>
        public void Reset()
        {
            AccessToken = string.Empty;
            Expires = DateTime.MinValue;
            Issued = DateTime.MinValue;
            State = string.Empty;
            AppId = string.Empty;
            FacebookId = string.Empty;
            CurrentPermissions = new string[0];
        }

    }
}
