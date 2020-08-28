using System;
using System.Linq;
using LanSensor.PollingMonitor.Domain.Models;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using LanSensor.PollingMonitor.Infrastructure.Models.Slack;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Infrastructure.RestServices.Slack
{
    public class SlackMessageSender : IMessageSender
    {
        private readonly IServiceConfiguration _serviceConfiguration;
        private readonly IHttpCallExecuteService _httpCallExecuteService;

        public SlackMessageSender(
            IServiceConfiguration serviceConfiguration,
            IHttpCallExecuteService httpCallExecuteService
            )
        {
            _serviceConfiguration = serviceConfiguration;
            _httpCallExecuteService = httpCallExecuteService;
        }

        public bool SendMessage(string message)
        {
            var token = new LoginToken
            {
                Token = _serviceConfiguration.ApplicationConfiguration.SlackConfiguration.ApiKey
            };

            var channels = _httpCallExecuteService.HttpGet(token, "conversations.list").GetAwaiter().GetResult();

            var channelList = JsonConvert.DeserializeObject<SlackChannelListResponse>(channels);

            var channel = channelList.Channels.FirstOrDefault(x =>
                x.id == _serviceConfiguration.ApplicationConfiguration.SlackConfiguration.ChannelId);

            if (channel == null)
            {
                throw new Exception("Slack channel Id not valid");
            }

            var body = JsonConvert.SerializeObject(new SlackPostMessage
            {
                Channel = channel.id,
                Text = message
            });

            var postMessageResult = _httpCallExecuteService.HttpPost(token, "chat.postMessage", body).GetAwaiter().GetResult();

            var postResult = JsonConvert.DeserializeObject<SlackPostMessageResponse>(postMessageResult);

            if (!postResult.ok)
            {
                var errorResult = JsonConvert.DeserializeObject<SlackError>(postMessageResult);
                throw new Exception($"Post message fails, with error: {errorResult.error}");
            }

            return true;
        }
    }
}
