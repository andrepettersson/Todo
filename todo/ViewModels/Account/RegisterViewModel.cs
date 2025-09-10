using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using todo.ViewModels.Account;


namespace todo.ViewModels;

public class RegisterViewModel : BaseAccountViewModel
{
    [Required]
    public string FirstName { get; set; } =string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
