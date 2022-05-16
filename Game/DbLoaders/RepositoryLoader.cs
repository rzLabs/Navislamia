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

        public List<IRepository> Repositories { get; set; }
    }

    public class RepositoryLoader : IRepositoryLoader
    {
        INotificationService notificationSVC;

        public List<Task<IRepository>> Tasks = new List<Task<IRepository>>();

        public List<IRepository> Repositories { get; set; } = new List<IRepository>();

        public RepositoryLoader(INotificationService notificationService)
        {
            notificationSVC = notificationService;
        }

        public async Task<bool> Execute()
        {
            try
            {
                StringBuilder sb = new StringBuilder($"[yellow]{GetType().Name}[/] [green]load completed successfully![/]\n");

                while (Tasks.Any())
                {
                    var task = await Task.WhenAny(Tasks);

                    IRepository repo = task.Result;

                    Repositories.Add(repo);

                    sb.AppendLine($"\t- " +
                        $"[yellow]{repo.Count}[/] rows loaded from {repo.Name}");

                    notificationSVC.WriteMarkup(sb.ToString());

                    Tasks.Remove(task);
                }
            }
            catch (Exception ex) { }

            return true;
        }
    }
}
