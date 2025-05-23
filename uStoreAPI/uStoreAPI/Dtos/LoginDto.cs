﻿using System.ComponentModel.DataAnnotations;

namespace uStoreAPI.Dtos
{
    public class LoginDto
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public bool Remember { get; set; }
    }
}
