using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNTPersianUtils.Core;

namespace Hatra.Messenger.Models.TokenAuth
{
    public class LoginRequestModel
    {
        [ValidIranianMobileNumber]
        public string PhoneNumber { get; set; }
    }

    public class LoginModel
    {
        [ValidIranianMobileNumber]
        public string PhoneNumber { get; set; }

        public string Token { get; set; }
    }

    public class LoginResultModel
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public LoginResultModel()
        {
            
        }

        public LoginResultModel(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }
}
