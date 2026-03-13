namespace Toems_Common.Dto
{
    public class DtoActionResult
    {
        public DtoActionResult()
        {
            Success = false;
        }

        public string ErrorMessage { get; set; }
        public int Id { get; set; }
        public bool Success { get; set; }
    }
}