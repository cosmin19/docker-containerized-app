using Enviroself.Areas.User.Features.Account.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enviroself.Areas.Media.Features.Entity
{
    [Table("MediaFile")]
    public class MediaFile
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public long Size { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public DateTime CreatedOnUtc { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
    }
}
