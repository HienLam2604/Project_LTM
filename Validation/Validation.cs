using System.Text.RegularExpressions;

namespace Validation
{
    public class ValidateEmail
    {
        public bool checkEmail(string email)
        {
            return Regex.IsMatch(email, @"^(?=.{1,64}@)[A-Za-z0-9_-]+(\.[A-Za-z0-9_-]+)*@[^-][A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*(\.[A-Za-z]{2,})$");
        }
    }

    public class ValidatePhone
    {
        public bool checkPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^(0[1-9])+([0-9]{8})$");
        }
    }
}