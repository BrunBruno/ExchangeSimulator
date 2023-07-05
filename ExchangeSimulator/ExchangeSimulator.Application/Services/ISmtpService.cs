namespace ExchangeSimulator.Application.Services;

/// <summary>
/// 
/// </summary>
public interface ISmtpService {
   /// <summary>
   /// 
   /// </summary>
   /// <param name="email"></param>
   /// <param name="subject"></param>
   /// <param name="message"></param>
   /// <returns></returns>
    Task SendMessage(string email, string subject, string message);
}