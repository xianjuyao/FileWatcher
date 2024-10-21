namespace LogLib
{
    public class LogItem
    {
        public string Timestamp { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }

        public LogItem(string timestamp, string message, string level)
        {
            Timestamp = timestamp;
            Message = message;
            Level = level;
        }

        public override string ToString()
        {
            return $"[{Timestamp}] [{Level}] {Message}";
        }
    }
}