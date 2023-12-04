using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navislamia.Game.Entities.Data.Interfaces;
using Navislamia.Notification;

namespace Navislamia.Data.Loaders;

public interface IRepositoryLoader
{
    public List<IEfRepository> Init() => null;

    public List<IEfRepository> Repositories { get; set; }
}

public class RepositoryLoader : IRepositoryLoader
{
    INotificationModule notificationSVC;

    public List<Task<IEfRepository>> Tasks = new List<Task<IEfRepository>>();

    public List<IEfRepository> Repositories { get; set; } = new List<IEfRepository>();

    public RepositoryLoader(INotificationModule notificationModule)
    {
        notificationSVC = notificationModule;
    }

    public bool Execute()
    {
        for (int i = 0; i < Tasks.Count; i++)
        {
            var task = Tasks[i];

            task.Wait();

            if (!task.IsCompletedSuccessfully)
            {
                notificationSVC.WriteError($"An exception occured trying to execute one more Tasks in the {this.GetType().Name}!");
                notificationSVC.WriteException(task.Exception);

                return false;
            }

            IEfRepository repo = task.Result;

            Repositories.Add(repo);

            notificationSVC.WriteMarkup($"[yellow]{GetType().Name}:[/] [orange3]{repo.Count}[/] rows loaded from {repo.Name}");
        }

        Tasks.Clear();

        return true;
    }
}
