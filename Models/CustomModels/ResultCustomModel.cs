namespace TreeStore.Models.CustomModels
{
    /// <summary>
    /// Custom dữ liệu trả về
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultCustomModel<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int? Code { get; set; }
    }
}
