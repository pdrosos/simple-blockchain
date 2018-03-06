using System;
using System.ComponentModel.DataAnnotations;

namespace Node.Api.Models
{
    public class MinedBlock
    {
        [Required]
        public string BlockDataHash { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public int Nonce { get; set; }

        [Required]
        public string BlockHash { get; set; }
    }
}
