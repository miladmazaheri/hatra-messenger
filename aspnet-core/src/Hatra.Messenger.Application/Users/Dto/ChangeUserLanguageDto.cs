using System.ComponentModel.DataAnnotations;

namespace Hatra.Messenger.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}