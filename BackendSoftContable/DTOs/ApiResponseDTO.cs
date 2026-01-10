namespace BackendSoftContable.DTOs
{
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        // 🔹 Respuesta exitosa con data
        public static ApiResponseDTO<T> SuccessResponse(T data, string message = "")
        {
            return new ApiResponseDTO<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        // 🔹 Respuesta exitosa sin data
        public static ApiResponseDTO<T> SuccessResponse(string message)
        {
            return new ApiResponseDTO<T>
            {
                Success = true,
                Message = message
            };
        }

        // 🔹 Respuesta de error
        public static ApiResponseDTO<T> Fail(string message)
        {
            return new ApiResponseDTO<T>
            {
                Success = false,
                Message = message
            };
        }
    }
}
