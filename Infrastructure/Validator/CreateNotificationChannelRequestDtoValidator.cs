using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Validator
{
    using FluentValidation;
    using Application.Dto;

    public class CreateNotificationChannelRequestDtoValidator : AbstractValidator<CreateNotificationChannelRequestDto>
    {
        public CreateNotificationChannelRequestDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId geçerli bir kullanıcı ID'si olmalı!");

            RuleFor(x => x.WatchedAddress)
                .NotEmpty().WithMessage("WatchedAddress boş olamaz!")
                .Must(addr => addr.StartsWith("0x") && addr.Length == 42)
                .WithMessage("WatchedAddress geçerli bir Ethereum adresi formatında olmalı!");

            RuleFor(x => x.Target)
                .NotEmpty().WithMessage("Target boş olamaz!")
                .MaximumLength(200).WithMessage("Target çok uzun olamaz!");
        }
    }

}
