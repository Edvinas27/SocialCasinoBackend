using Bogus;
using SocialCasino.Models;

namespace SocialCasino.Services;

public class FakeCreditCardService
{
    public CreditCard GenerateCreditCard(decimal balance)
    {
        var faker = new Faker<CreditCard>()
            .RuleFor(c => c.CardNumber, f => f.Finance.CreditCardNumber())
            .RuleFor(c => c.ExpiryMonth, f => f.Date.Future().Month.ToString("D2"))
            .RuleFor(c => c.ExpiryYear, f => f.Date.Future().Year.ToString())
            .RuleFor(c => c.CVV, f => f.Finance.CreditCardCvv())
            .RuleFor(c => c.Balance, f=> balance); 
        
            return faker.Generate();
    }
}