using Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class CreateNewPassword
    {
        public class Command : IRequest<CommandResult>
        {
            public string Email { get; set; }
            public string Code { get; set; }

            public string NewPassword { get; set; }
        }


        public class Handler : IRequestHandler<Command, CommandResult>
        {
            private readonly ApplicationDbContext datacontext;
            private readonly UserManager<AppUser> userManager;

            public Handler(ApplicationDbContext _datacontext,UserManager<AppUser> userManager)
            {
                datacontext = _datacontext;
                this.userManager = userManager;
            }

            public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
            {

                CommandResult commandRes = new CommandResult();

                //var user = await userManager.FindByEmailAsync(request.Email);

                var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == request.Email);


                // getting token from otp
                var resetPasswordDetails = await datacontext.ResetPasswords
                    .Where(rp => rp.ResetCode == request.Code && rp.UserId == user.Id)
                    .OrderByDescending(rp => rp.InsertDateTimeUTC)
                    .FirstOrDefaultAsync();

                // Verify if token is older than 15 minutes
                var expirationDateTimeUtc = resetPasswordDetails.InsertDateTimeUTC.AddMinutes(15);

                if (expirationDateTimeUtc < DateTime.UtcNow)
                {
                    commandRes.Success = false;
                    commandRes.Message = "reset code is expired, please generate a new one !";

                
                }

                var res = await userManager.ResetPasswordAsync(user, resetPasswordDetails.Token, request.NewPassword);


                if (res.Succeeded)
                {
                    commandRes.Success = true;
                    commandRes.Message = "Password Changed ";
                }


                return commandRes;
            }
        }
    }
}
