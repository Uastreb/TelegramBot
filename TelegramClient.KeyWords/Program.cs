using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TL;
using WTelegram;

namespace TelegramClient.KeyWords
{
    class Program
    {
        private static Settings _settings;
        private static Client _tgClient;


        static void Main(string[] args)
        {
            _settings = XmlSerializationHelper<Settings>.DeserializeFromFile(@"Settings.xml");

            Authorization().ConfigureAwait(false).GetAwaiter().GetResult();

            _tgClient.Update += TgClient_Update;

            Console.WriteLine("Type Cancel to exit the program");
            var text = string.Empty;
            do
            {
                text = Console.ReadLine();
            }
            while (!string.Equals(text, "Cancel", StringComparison.OrdinalIgnoreCase));
        }

        private static readonly Dictionary<long, User> _users = new();
        private static readonly Dictionary<long, ChatBase> _chats = new();


        private static string User(long id)
        {
            if (_users.TryGetValue(id, out var user))
            {
                return user.ToString();
            }

            return $"User {id}";
        }

        private static string Chat(long id)
        {
            if (_chats.TryGetValue(id, out var chat))
            {
                return chat.ToString();
            }

            return $"Chat {id}";
        }

        private static string Username(Peer peer)
        {
            if (_chats.TryGetValue(peer.ID, out var chat) && chat is Channel channel)
            {
                return channel.username;
            }

            if (_users.TryGetValue(peer.ID, out var chatUser) && chatUser is User user)
            {
                return user.ID.ToString();
            }

            return $"{peer}";
        }

        private static string Peer(Peer peer)
        {
            if (peer is PeerUser userId)
            {
                var user = User(userId.user_id);

                return user;
            }

            if (peer is PeerChat or PeerChannel)
            {
                var chat = Chat(peer.ID);

                return chat;
            }

            if (peer is not null)
            {
                return $"Peer {peer.ID}";
            }

            return null;
        }



        private static void TgClient_Update(IObject obj)
        {
            if (obj is not UpdatesBase updates) return;
            updates.CollectUsersChats(_users, _chats);
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage unm: HandleMessage(unm.message); break;
                        //case UpdateEditMessage uem: DisplayMessage(uem.message, true); break;
                        //case UpdateDeleteChannelMessages udcm: Console.WriteLine($"{udcm.messages.Length} message(s) deleted in {Chat(udcm.channel_id)}"); break;
                        //case UpdateDeleteMessages udm: Console.WriteLine($"{udm.messages.Length} message(s) deleted"); break;
                        //case UpdateUserTyping uut: Console.WriteLine($"{User(uut.user_id)} is {uut.action}"); break;
                        //case UpdateChatUserTyping ucut: Console.WriteLine($"{Peer(ucut.from_id)} is {ucut.action} in {Chat(ucut.chat_id)}"); break;
                        //case UpdateChannelUserTyping ucut2: Console.WriteLine($"{Peer(ucut2.from_id)} is {ucut2.action} in {Chat(ucut2.channel_id)}"); break;
                        //case UpdateChatParticipants { participants: ChatParticipants cp }: Console.WriteLine($"{cp.participants.Length} participants in {Chat(cp.chat_id)}"); break;
                        //case UpdateUserStatus uus: Console.WriteLine($"{User(uus.user_id)} is now {uus.status.GetType().Name[10..]}"); break;
                        //case UpdateUserName uun: Console.WriteLine($"{User(uun.user_id)} has changed profile name: @{uun.username} {uun.first_name} {uun.last_name}"); break;
                        //case UpdateUserPhoto uup: Console.WriteLine($"{User(uup.user_id)} has changed profile photo"); break;
                        //default: Console.WriteLine(update.GetType().Name); break; // there are much more update types than the above cases
                }
        }

        private static async Task Authorization()
        {
            _tgClient = new Client(Config);
            Helpers.Log = (l, s) => System.Diagnostics.Debug.WriteLine(s);

            var my = await _tgClient.LoginUserIfNeeded();
            _users[my.id] = my;

            var dialogs = await _tgClient.Messages_GetAllDialogs(); // dialogs = groups/channels/users
            dialogs.CollectUsersChats(_users, _chats);

            Console.WriteLine($"We are logged-in as {my.username ?? my.first_name + " " + my.last_name} (id {my.id})");
        }

        private static string Config(string what)
        {
            switch (what)
            {
                case "api_id": return _settings.ApiId;
                case "api_hash": return _settings.ApiHash;
                case "phone_number": return _settings.PhoneNumber;
                case "verification_code": Console.Write("Code: "); return Console.ReadLine();
                case "first_name": return _settings.FirstName;      // if sign-up is required
                case "last_name": return _settings.LastName;        // if sign-up is required
                case "password": return _settings.Password;     // if user has enabled 2FA
                default: return null;                  // let WTelegramClient decide the default config
            }
        }

        private static void HandleMessage(MessageBase messageBase, bool edit = false)
        {
            switch (messageBase)
            {
                case Message m:
                    {
                        foreach (var keyWord in _settings.KeyWords)
                        {
                            if (m.message.Contains(keyWord))
                            {
                                var messageLinkUriBuilder = new UriBuilder();

                                messageLinkUriBuilder.Scheme = "https";
                                messageLinkUriBuilder.Host = @"t.me";
                                messageLinkUriBuilder.Path = $"{Username(m.peer_id)}/{m.ID}";

                                var messageLink = messageLinkUriBuilder.Uri;

                                _tgClient.SendMessageAsync(_settings.IdChatToPaste.HasValue ? _chats[_settings.IdChatToPaste.Value] : InputPeer.Self, messageLink.AbsoluteUri);
                            }
                        }

                        break;
                    }
                case MessageService ms:
                    {
                        Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]");
                        break;
                    }
            }
        }
    }
}
