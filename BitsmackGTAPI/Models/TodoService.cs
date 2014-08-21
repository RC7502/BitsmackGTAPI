using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using Toodledo.Client;
using Toodledo.Model;
using Toodledo.Model.API;

namespace BitsmackGTAPI.Models
{
    public class TodoService :ITodoService
    {
        private readonly ICommonService _commonService;
        private readonly IDAL _dal;

        public TodoService(ICommonService commonService, IDAL dal)
        {
            _commonService = commonService;
            _dal = dal;
        }

        public void RefreshData(bool overwrite, DateTime startdate, DateTime enddate)
        {
            var key = _commonService.GetAPIKeyByName(APINames.TOODLEDO);
            //if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;
            IEnumerable<Todos> list = GetToodledoData(key, startdate, enddate);
            foreach (var todoDay in list)
            {
                var existingTran =
                    _dal.GetTodoRecords()
                        .FirstOrDefault(x => x.trandate == todoDay.trandate && x.foldername == todoDay.foldername);
                if (existingTran != null && overwrite)
                {
                    Copy(todoDay, existingTran);
                    _dal.Update(todoDay);
                }
                else if (existingTran == null)
                {
                    _dal.Insert(todoDay);
                    //_commonService.LogActivity(todoDay);
                }
            }
            _dal.SaveTodos();

            key.last_update = DateTime.UtcNow;
            _commonService.UpdateAPIKey(key);
        }

        private void Copy(Todos from, Todos to)
        {
            to.numadded = from.numadded;
            to.numcompleted = from.numcompleted;
        }

        private IEnumerable<Todos> GetToodledoData(APIKeys key, DateTime startdate, DateTime enddate)
        {
            var list = new List<Todos>();
            try
            {
                //var restClient = new RestClient();
                //var request = new RestRequest(string.Format("https://api.toodledo.com/3/tasks/get.php?access_token={0}&f=json", key.user_token), Method.GET);
                //var response = restClient.Execute<Task>(request);
                //var data = JsonConvert.DeserializeObject<Task>(response.Content);

                var client = (ITasks)Session.Create("rc@bitsmack.com", "mudpie", "BitsmackGT");
                var general = (IGeneral)Session.Create("rc@bitsmack.com", "mudpie", "BitsmackGT");
                var tasks = client.GetTasks(new TaskQuery
                    {
                        Before = enddate,
                        After = startdate
                    });
                for (DateTime counter = startdate; counter <= enddate; counter = counter.AddDays(1))
                {

                    var tasksAdded = tasks.Tasks.Where(x => x.Added.Date == counter.Date).ToList();
                    var tasksCompleted = tasks.Tasks.Where(x => x.Completed.Date == counter.Date).ToList();
                    //var taskFolders = tasks.Tasks.Select(x => x.Folder).Distinct();
                    var taskFolders = general.GetFolders();

                    foreach (var taskFolder in taskFolders)
                    {
                        if (taskFolder != null)
                        {
                            var newRec = new Todos
                                {
                                    trandate = counter,
                                    foldername = taskFolder.Name,
                                    numadded = tasksAdded.Count(x => x.Folder.Id == taskFolder.Id),
                                    numcompleted = tasksCompleted.Count(x => x.Folder.Id == taskFolder.Id)
                                };
                            if(newRec.numadded > 0 || newRec.numcompleted > 0)
                                list.Add(newRec);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //_commonService
                _commonService.WriteLog(EventLogSeverity.Error, MethodBase.GetCurrentMethod().Name, ex.Message);

                //Update Last Modified Date
                if (key != null)
                {
                    key.last_update = DateTime.UtcNow;
                    _commonService.UpdateAPIKey(key);
                }
            } 

            return list;
        }
    }
}