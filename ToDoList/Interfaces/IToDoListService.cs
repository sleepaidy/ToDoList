using ToDoList.Models.Entities;
using ToDoList.Models.Home;

namespace ToDoList.Interfaces
{
    public interface IToDoListService
    {
        ToDoItem CreateTusk(ToDoTuskViewModel viewModel);
    }
}
