namespace NewsAppExample.Helper
{
    public interface IPasswordHashService
    {
        public string IterateHash(string input, string salt);

        public string ComputeHashWithSalt_SHA512(string input, string salt);
    }
}
