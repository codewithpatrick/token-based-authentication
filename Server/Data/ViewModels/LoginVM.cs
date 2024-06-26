﻿using System.ComponentModel.DataAnnotations;

namespace Server.Data.ViewModels;

public class LoginVm
{
    [Required] 
    public string EmailAddress { get; set; }

    [Required] 
    public string Password { get; set; }
}