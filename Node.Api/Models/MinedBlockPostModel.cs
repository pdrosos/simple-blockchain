using System;
using System.ComponentModel.DataAnnotations;

namespace Node.Api.Models
{
    public class MinedBlockPostModel
    {
        [Required]
        public string BlockDataHash { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public ulong Nonce { get; set; }
    }
}
