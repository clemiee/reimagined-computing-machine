using System.Threading.Tasks;
using dotenv.net;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace reimagined_computing_machine
{
    public class Program
    {
        public readonly EventId BotEventId = new EventId(42, "Program");
        public DiscordClient Client { get; set; }
        public static void Main(string[] args)
        {
            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            // first, we need to read the .env files and take a specific values.
            var envVars = DotEnv.Read(options: new DotEnvOptions(probeForEnv: true, probeLevelsToSearch: 4, ignoreExceptions: false));
            string APIKey = envVars["BOT_TOKEN"];
            // next, let's load the values from that file
            // to our client's configuration
            var discord = new DiscordConfiguration()
            {
                Token = APIKey,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            };

            // then we want to instantiate our client
            this.Client = new DiscordClient(discord);

            // next, let's hook some events, so we know
            // what's going on
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientErrored += this.Client_ClientError;

            // finally, let's connect and log in
            await this.Client.ConnectAsync();

            // and this is to prevent premature quitting
            await Task.Delay(-1);
        }
        private Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            // let's log the fact that this event occured
            sender.Logger.LogInformation(BotEventId, "Client is ready to process events.");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            sender.Logger.LogInformation(BotEventId, $"Guild available: {e.Guild.Name}");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            // let's log the details of the error that just 
            // occured in our client
            sender.Logger.LogError(BotEventId, e.Exception, "Exception occured");

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }
    }
}