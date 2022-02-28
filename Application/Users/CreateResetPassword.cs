

using Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class CreateResetPassword

    {

        public class ResetPasswordCommand : IRequest<CommandResult>
        { 
            public string email { get; set; }
        }



        public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, CommandResult>
        {

            private readonly ApplicationDbContext datacontext;
            private readonly UserManager<AppUser> usermanager;

            public ResetPasswordHandler(ApplicationDbContext datacontext,UserManager<AppUser> usermanager)
            {
                this.datacontext = datacontext;
                this.usermanager = usermanager;
            }

            public async Task<CommandResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
            {
                CommandResult resultOp = new CommandResult();

                // Get Identity User details user user manager
                //var user = await usermanager.FindByNameAsync(request.email);

                //   var user = await usermanager.FindByEmailAsync(request.email);

                //var user = await usermanager.FindByNameAsync(request.email);


                var user = await usermanager.Users.FirstOrDefaultAsync(x => x.Email == request.email);



                // Generate password reset token
                var token = await usermanager.GeneratePasswordResetTokenAsync(user);

                //RandomNumberGenerator eng = new RandomNumberGenerator();

                // Generate OTP
                int codeReset = RandomNumberGenerator.Generate(100000, 999999);


               // AppUser currentuser = await usermanager.FindByEmailAsync(request.email);


                var resetPassword = new ResetPassword()
                {
                    Email = request.email,
                    ResetCode = codeReset.ToString(),
                    Token = token,
                    UserId = user.Id,
                    InsertDateTimeUTC = DateTime.UtcNow
                };

                await datacontext.AddAsync(resetPassword);

                var result = await datacontext.SaveChangesAsync() > 0;

             await EmailSender.SendEmailAsync(request.email, "Reset Password Code", "Hello "
             + request.email + "<br><br>Please find the reset password token below<br><br><b>"
             + codeReset + "<b><br><br>Thanks<br>");


                if (result)
                {
                    resultOp.Success = true;
                    resultOp.Message = "Token sent successfully in email";

                }

                else
                {
                    resultOp.Success = false;
                    resultOp.Message = "failed to generate reset code";
                }

                return resultOp;
            }
        }
    }
}

