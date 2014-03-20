using System.Collections.Generic;
using System.Data.Entity.Validation;
using BitsmackGTAPI.Interfaces;
using BitsmackGTAPI.Models;
using System.Linq;

namespace BitsmackGTAPI.DAL
{
    public class HabitDAL : IHabitDAL
    {
        private readonly IGTRepository<HabitTasks> _taskRepo;
        private readonly IGTRepository<HabitTags> _tagsRepo;
        private readonly IGTRepository<HabitTaskTags> _taskTagsRepo;
        private readonly IGTRepository<HabitTaskHistory> _taskHistoryRepo;

        public HabitDAL(IGTRepository<HabitTasks> taskRepo, IGTRepository<HabitTags> tagsRepo,
                        IGTRepository<HabitTaskTags> taskTagsRepo, IGTRepository<HabitTaskHistory> taskHistoryRepo)
        {
            _tagsRepo = tagsRepo;
            _taskHistoryRepo = taskHistoryRepo;
            _taskRepo = taskRepo;
            _taskTagsRepo = taskTagsRepo;
        }


        public IEnumerable<HabitTasks> GetHabitTasks()
        {
            return _taskRepo.AllForRead();
        }

        public IEnumerable<HabitTaskHistory> GetHabitTaskHistory()
        {
            return _taskHistoryRepo.AllForRead();
        }

        public void Update(HabitTasks existingTask)
        {
            _taskRepo.Update(existingTask);
        }

        public void Save()
        {
            _taskRepo.Save();
            _taskHistoryRepo.Save();

        }

        public void Insert(HabitTasks newTask)
        {
            _taskRepo.Insert(newTask);
        }

        public void Insert(HabitTaskHistory newHist)
        {
            _taskHistoryRepo.Insert(newHist);
        }

        public List<HabitDetailViewModel> TaskHistoryView()
        {
            return _taskRepo.AllForRead().ToList().Join(_taskHistoryRepo.AllForRead().ToList(), task => task.id, hist => hist.taskID,
                                               (task, hist) => new HabitDetailViewModel
                                                   {
                                                       Name = task.text,
                                                       Points = hist.value,
                                                       TranDate = hist.historyDate.ToShortDateString(),
                                                       Type = task.type
                                                   }).OrderByDescending(x=>x.TranDate).ToList();
        }
    }
}