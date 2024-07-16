using Microsoft.Extensions.Logging;
using Postman.PubSub.Business;
using Postman.PubSub.Business.Dto;

namespace Postman.Pub
{
    public class Hello(
        IManager<UserConfirmDto> userManager,
        IManager<FooDto> fooManager,
        ILogger<Hello> logger)
        : IHello
    {
        public async Task Say()
        {
            Console.WriteLine("Welcome! We're going to begin work!");
            var i = 1;
            while (i <= 10)
            {
                await Work();
                i += 1;
            }
        }

        async Task Work()
        {
            Console.WriteLine($"press 1 for user or 2 for foo");
            var c = Console.ReadLine();
            int.TryParse(c, out var choice);

            if (choice == 1)
            {
                Console.WriteLine($"What is the user id?");
                var id = Console.ReadLine();
                int.TryParse(id, out var userId);
                Console.WriteLine("What is the email?");
                var email = Console.ReadLine();
                var dto = new UserConfirmDto { Id = userId, Email = email ?? string.Empty };
                await userManager.Process(dto);
            }
            else if (choice == 2)
            {
                Console.WriteLine("What is your foo id?");
                var id = Console.ReadLine();
                int.TryParse(id, out var fooId);
                Console.WriteLine("What is the name?");
                var name = Console.ReadLine();
                var dto = new FooDto() { Id = fooId, Name = name ?? string.Empty };
                await fooManager.Process(dto);
            }
        }
    }
}