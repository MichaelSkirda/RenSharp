using RenSharpClient.Models;
using System;
using System.Collections.Generic;

namespace RenSharpClient.Controllers
{
    internal class AudioChannelController
    {
        internal readonly Dictionary<string, AudioChannel> Channels = new Dictionary<string, AudioChannel>();

        internal void CreateChannel(string name, AudioChannel channel)
        {
            AssertNotExists(name);
            Channels[name] = channel;
        }

        internal AudioChannel GetChannel(string name)
        {
            AssertExists(name);
            return Channels[name];
        }

        internal bool Exists(string name)
            => Channels.ContainsKey(name);

        private void AssertExists(string name)
        {
            bool exists = Exists(name);
            if (exists == false)
                throw new InvalidOperationException($"Аудиоканал с именем '{name}' - не существует." +
                    $" Вам надо создать его или использовать стандартные music/audio.");
        }

        private void AssertNotExists(string name)
        {
            bool exists = Exists(name);
            if (exists)
                throw new InvalidOperationException($"Аудиоканал с именем '{name}' - уже существует.");
        }
    }
}
