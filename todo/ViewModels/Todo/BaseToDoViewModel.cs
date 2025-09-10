using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace todo.ViewModels.Todo;

public class BaseToDoViewModel
{
    [Required]
    [StringLength(150, MinimumLength =1)]
    public string Title { get; set; } = string.Empty;

}
