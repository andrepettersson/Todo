using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todo.ViewModels.Todo;

public class UpdateToDoViewModel : BaseToDoViewModel
{
    public bool IsDone { get; set; }
}
