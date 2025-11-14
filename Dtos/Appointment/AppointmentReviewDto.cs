using System.ComponentModel.DataAnnotations;

namespace randevuappapi.Dtos.Appointment
{
    public class AppointmentReviewDto
    {
        public Guid AppointmentId { get; set; }
        public string? Comment { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}
