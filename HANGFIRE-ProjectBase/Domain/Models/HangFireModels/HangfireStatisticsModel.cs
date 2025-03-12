namespace Domain.Models.HangFireModels
{
    public class HangfireStatisticsModel
    {
        public long? Succeeded { get; set; }
        public long? Failed { get; set; }
        public long? Processing { get; set; }
        public long? Scheduled { get; set; }
        public long? Enqueued { get; set; }
        public long? Deleted { get; set; }
        public long? Retries { get; set; }
        public long? Servers { get; set; }
    }
}
