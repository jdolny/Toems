namespace Toems_Common.Dto
{
    public class DtoApiObjectResponse
    {
        public DtoApiObjectResponse()
        {
            Success = false;
        }

        public string ErrorMessage { get; set; }
        public int Id { get; set; }
        public string ObjectJson { get; set; }
        public bool Success { get; set; }
    }
}