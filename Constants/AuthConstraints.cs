namespace Pizza_API.Constants
{
    public static class AuthConstraints
    {
        public const int NameMinLength = 2;
        public const int NameMaxLength = 50;

        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 100;

        public const int RefreshTokenMaxLength = 200;
    }
}