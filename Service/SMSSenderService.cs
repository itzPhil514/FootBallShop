using FootBallShop.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FootBallShop.Service
{
    public class SMSSenderService : ISMSSenderService
    {
        private readonly TwilioSettings _twilioSettings; 
        
       
        public SMSSenderService(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value; 
        }
        public async Task SendSmsAsync(string number, string message)
        {
            TwilioClient.Init(_twilioSettings.AccountSId, _twilioSettings.AuthToken);

            await MessageResource.CreateAsync(
            to: number, 
            from: _twilioSettings.FromPhoneNumber, 

            body: message 
            );
        }
    } 
}
