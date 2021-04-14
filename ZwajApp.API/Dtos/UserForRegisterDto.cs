using System.ComponentModel.DataAnnotations;

namespace ZwajApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string  UserName { get; set; }
         [StringLength(8,MinimumLength=4,ErrorMessage="يجب أن لا تقل كلمة السر عن أربعة أحرف ولا تزيد عن ثمانية")]
        public string  Password { get; set; }
        
    }
}