using Navislamia.Database.Interfaces;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.DbLoaders
{
    public interface IRepositoryLoader
    {
        public int Init() => 0;
    }

    public class RepositoryLoader : IRepositoryLoader
    {
        INotificationService notificationSVC;

        public List<Task<IRepository>> Tasks = new List<Task<IRepository>>();

        public RepositoryLoader(INotificationService notificationService)
        {
            notificationSVC = notificationService;
        }

        public bool Execute()
        {
            Task loadTask = Task.WhenAll(Tasks);

            try
            {
                loadTask.Wait();
            }
            catch (Exception ex) { }

            if (!loadTask.IsCompletedSuccessfully)
            {
                foreach (Task<IRepository> task in Tasks)
                {
                    if (task.IsFaulted)
                    {
                        notificationSVC.WriteError($"A load in the {GetType().Name} has encountered an exception!");
                        notificationSVC.WriteException(task.Exception);
                    }
                }

                return false;
            }

            StringBuilder sb = new StringBuilder($"[yellow]{GetType().Name}[/] [green]load completed successfully![/]\n");

            foreach (Task<IRepository> task in Tasks) // Write success msgs
            {
                IRepository repo = task.Result;

                sb.AppendLine($"\t- " +
                    $"[yellow]{repo.Count}[/] rows loaded from {repo.Name}");
            }

            notificationSVC.WriteMarkup(sb.ToString());

            return true;
        }
    }
}
