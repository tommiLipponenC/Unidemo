namespace Unidemo.DTO
{
    public class ResponseDto
    {
        public string? Message { get; set; }
        public bool? Success { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
