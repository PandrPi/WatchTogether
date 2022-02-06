using Newtonsoft.Json;
using System;

namespace WatchTogether.Browser.BrowserCommands
{
    internal class BrowserCommandSerializer
    {
        public struct CommandEntity
        {
            public string FullTypeName { get; set; }
            public object[] Parameters { get; set; }
        }

        /// <summary>
        /// Converts the specified commandText to a new IBrowserCommand instance
        /// </summary>
        /// <param name="commandText">The command string for conversion</param>
        /// <returns></returns>
        public static IBrowserCommand Deserialize(string commandText)
        {
            var commandEntity = JsonConvert.DeserializeObject<CommandEntity>(commandText);

            var commandType = Type.GetType(commandEntity.FullTypeName);
            var commandInstance = (IBrowserCommand)Activator.CreateInstance(commandType);
            commandInstance.Initialize(commandEntity.Parameters);

            return commandInstance;
        }

        /// <summary>
        /// Generates a commandText string from the specified command type and parameters
        /// </summary>
        /// <param name="commandType">The type of the command inherited from IBrowserCommand</param>
        /// <param name="parameters">The array of objects that will be passed to
        /// a command before its execution</param>
        /// <returns></returns>
        public static string Serialize(Type commandType, params object[] parameters)
        {
            if (typeof(IBrowserCommand).IsAssignableFrom(commandType) == false)
                throw new Exception($"ERROR: {commandType} type is not inherited from IBrowserCommand interface!");

            var command = new CommandEntity
            {
                FullTypeName = commandType.FullName,
                Parameters = parameters
            };

            return JsonConvert.SerializeObject(command);
        }
    }
}
