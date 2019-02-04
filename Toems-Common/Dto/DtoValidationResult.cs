namespace Toems_Common.Dto
{
    public class DtoValidationResult
    {
        public DtoValidationResult()
        {
            Success = false;
        }

        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
    }
}