using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using DNTPersianUtils.Core;
using Hatra.Messenger.Authorization.Users;

namespace Hatra.Messenger.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserProfileDto
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string AvatarAddress{ get; set; }

        public string Status { get; set; }

        public string PhoneNumber { get; set; }
    }
    [AutoMapFrom(typeof(User))]
    public class UpdateUserProfileDto
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public string AvatarAddress{ get; set; }

        [StringLength(100)]
        public string Status { get; set; }

        [ValidIranianMobileNumber]
        [Required]
        public string PhoneNumber { get; set; }

        public void ApplyCorrectYeKe()
        {
            UserName?.ApplyCorrectYeKe();
            Name?.ApplyCorrectYeKe();
            Surname?.ApplyCorrectYeKe();
            Status?.ApplyCorrectYeKe();
        }
    }
}
