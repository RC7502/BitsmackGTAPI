using System.Collections.Generic;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Interfaces
{
    public interface IHabitDAL
    {
        IEnumerable<HabitTasks> GetHabitTasks();
        IEnumerable<HabitTaskHistory> GetHabitTaskHistory();
        void Update(HabitTasks existingTask);
        void Save();
        void Insert(HabitTasks newTask);
        void Insert(HabitTaskHistory newHist);
        List<HabitDetailViewModel> TaskHistoryView();
    }
}