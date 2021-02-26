namespace Toems_Common.Dto
{
    public class DtoToken
    {
        public string access_token { get; set; }
        public string error_description { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }

        public string username { get; set; }
        public string membership { get; set; }
        public string theme { get; set; }

        public string is_validated { get; set; }
    }
}