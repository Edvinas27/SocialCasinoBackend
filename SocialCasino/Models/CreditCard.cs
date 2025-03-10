namespace SocialCasino.Models;

public class CreditCard
{
    public int Id { get; set; }
    public string CardNumber { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
    public string CVV { get; set; }
    public decimal Balance { get; set; }
}