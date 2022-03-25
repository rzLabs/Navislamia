using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Security;
using System.IO;
using System.Threading.Tasks;

using Navislamia.Configuration;

using Serilog;

namespace DevConsole
{
    public enum CommandFlag
    {
        Engine = 0,
        Account = 1,
        Character = 2
    }

    public class Command
    {
        public string Key;

        public int[] Targets = Array.Empty<int>();

        public dynamic[] Arguments;

        public CommandFlag[] Flags;

        public bool HasTargets
        {
            get => Targets?.Length > 0;
        }

        public bool HasArguments
        {
            get => Arguments?.Length > 0;
        }

        public bool HasFlags
        {
            get => Flags?.Length > 0;
        }
    }

    public class CommandUtility
    {
        ConfigurationManager confMgr = ConfigurationManager.Instance;

        delegate void CommandAction(Command command);

        Dictionary<string, CommandAction> actions = new Dictionary<string, CommandAction>();

        public CommandUtility() => init();

        public CommandUtility(string commandText)
        {
            init();
            Parse(commandText);
        }

        void init()
        {
            #region Server
            actions.Add("about", about);
            actions.Add("clear", conClear);
            actions.Add("start", netStart); //TODO: Not implemented yet (maybe ever)
            actions.Add("stop", netStop);
            actions.Add("get", getVar);
            actions.Add("list", listVar);
            actions.Add("shutdown", shutdown);
            #endregion

            #region Client
            actions.Add("disconnect", user_Disconnect);
            actions.Add("whois", user_whoIs);
            #endregion

            Log.Information("Command Utility started!\n\t- {count} commands registered", actions.Count);
        }

        //TODO: Somehow catch whitespace in arguments block
        public Command Parse(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                Log.Error("commandText cannot be null!");
                return null;
            }

            Command cmd;

            string[] cmdBlks = commandText.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

            if (cmdBlks.Length == 0)
            {
                Log.Error("Command arguments length is invalid!");
                return null;
            }

            cmd = new Command();

            if (cmdBlks.Length >= 1)
                cmd.Key = cmdBlks[0];

            if (cmdBlks.Length == 2)
                cmd.Arguments = arguments(cmdBlks[1]);
            else if (cmdBlks.Length == 3)
            {
                cmd.Flags = flags(cmdBlks[1]);
                cmd.Arguments = arguments(cmdBlks[2]);
            }
            else if (cmdBlks.Length == 4)
            {
                cmd.Flags = flags(cmdBlks[1]);
                cmd.Targets = targets(cmdBlks[2]);
                cmd.Arguments = arguments(cmdBlks[3]);
            }

            return cmd;
        }

        public async void Wait(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                message = "Waiting for input...";

            Log.Information(message, ConsoleColor.Green);

            string i = Console.ReadLine();

            if (string.IsNullOrEmpty(i))
            {
                Log.Error("Failed to parse input!");
                return;
            }

            Command cmd = Parse(i);

            if (cmd == null)
            {
                Log.Error("Invalid command!");
                return;
            }

            await execute(cmd).ConfigureAwait(true);

            Console.ReadLine();
        }

        #region Command Methods

        private void about(Command command)
        {
            string aboutStr = "\n\nNavislamia v0.0.1\nDevelopers:\n\t- iSmokeDrow\n\t- Aodai\n\t- Glandu2\n\t- Sandro";

            Log.Information(aboutStr);
        }

        private void conClear(Command command) => Console.Clear();

        private void getVar(Command command)
        {
            if (!command.HasArguments)
                return;

            foreach (string arg in command.Arguments)
            {
                string v = confMgr?[arg].ToString();

                Log.Information($"{arg} : {v}");
            }
        }

        private void listVar(Command command)
        {
            foreach (var config in confMgr.Settings)
                Log.Information("{Name} : {Value}", config.Name, config.Value.ToString());
        }
        private void netStart(Command command)
        {
        }

        private void netStop(Command command)
        {
            Console.Write("Attempting to stop the network instance...");

        }

        private void user_Disconnect(Command command)
        {
            if (!command.HasArguments)
                return;

            int id = ArgToInt(command);

            if (id != -1)
            {
                //TODO: disconnect user
            }
        }

        private void user_whoIs(Command command)
        {


        }

        private void shutdown(Command command)
        {
            int defaultTime = (int)confMgr?["shutdown.timer"];
            int shutdownTime = (command.HasArguments) ? ArgToInt(command) : defaultTime;

            DevConsole.Program.Shutdown(shutdownTime * 1000);
        }

        #endregion

        async Task execute(Command cmd)
        {
            if (!actions.ContainsKey(cmd.Key))
                Log.Warning("No action associated with command: {key}", cmd.Key);
            else
            {
                try
                {
                    actions[cmd.Key.ToLower()]?.Invoke(cmd);
                }
                catch (Exception ex)
                {
                    Log.Error("An Exception has occured executing the action:\nKey: {key}\nMessage: {exMessage}\nStack Trace: {exStack}", cmd.Key, ex.Message, ex.StackTrace);
                }
            }

            Wait("Waiting for input...");
        }

        string[] arguments(string block)
        {
            if (string.IsNullOrEmpty(block))
                throw new ArgumentNullException("block", "block is null!");

            string[] blks = block.Split(',');

            if (blks.Length == 0)
                return new string[] { block };
            else
                return blks;
        }

        CommandFlag[] flags(string block)
        {
            if (string.IsNullOrEmpty(block))
                throw new ArgumentNullException("block", "block is null");

            string[] blks = block.Split(',');

            if (blks.Length == 0)
                blks = new string[1] { block };

            CommandFlag[] f = new CommandFlag[blks.Length];

            for (int i = 0; i < blks.Length; i++)
            {
                string blk = blks?[i];

                switch (blk.ToLower())
                {
                    case "-a": //account
                        f[i] = CommandFlag.Account;
                        break;

                    case "-c": // Character
                        f[i] = CommandFlag.Character;
                        break;

                    case "-s": // Server
                        f[i] = CommandFlag.Engine;
                        break;
                }
            }

            if (f?.Length == 0)
                throw new ArgumentException("FlagList [f] is null and that shouldn't be possible!", "l (CommandFlag[])");

            return f;
        }

        int[] targets(string block)
        {
            if (string.IsNullOrEmpty(block))
                throw new ArgumentNullException("block", "block is null");

            string[] blks = block.Split(',');

            if (blks.Length == 0)
                blks = new string[1] { block };

            int[] t = new int[blks.Length];

            for (int blkIdx = 0; blkIdx < blks.Length; blkIdx++)
            {
                string id = blks[blkIdx];
                int i = 0;

                if (!int.TryParse(id, out i))
                    throw new InvalidCastException($"Failed to parse value: {id} in blks");

                t[blkIdx] = i;
            }

            if (t.Length == 0)
                throw new ArgumentException("Target list [t] is null and that shouldn't be possible!", "t (int[])");

            return t;
        }

        int ArgToInt(Command command, int index = 0)
        {
            int id = -1;
            if (!int.TryParse(command.Arguments[index], out id))
                Log.Error("Failed to parse argument at index: {index}", index);

            return id;
        }
    }
}
