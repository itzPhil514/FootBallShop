namespace FootBallShop2.Service
{
    public interface ISMSSenderService
    {
        Task SendSmsAsync(string number, string message);
    }
}
