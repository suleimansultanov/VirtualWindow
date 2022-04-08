using System.ComponentModel.DataAnnotations;

namespace NasladdinPlace.Application.Dtos.Feedback
{
    public class FeedbackBodyDto
    {
        [Required]
        [StringLength(3000)]
        public string Content { get; set; }
    }
}