using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todo.ViewModels.Todo;

public class ToDoItemViewModel : BaseToDoViewModel
{
    public int Id { get; set; }
    public bool IsDone { get; set; }

}
