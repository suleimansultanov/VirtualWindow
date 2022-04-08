using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Application.Dtos.Feedback
{
    public class FeedbackDto
    {
        [Required]
        public SenderInfoDto SenderInfo { get; set; }

        [Required]
        public FeedbackBodyDto Body { get; set; }

        [Required]
        public AppInfoDto AppInfo { get; set; }
    }
}