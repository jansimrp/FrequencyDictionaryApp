using FluentValidation;

namespace FrequencyDictionaryApp.Validators
{
    public class UserInputValidator : AbstractValidator<string[]>
    {
        public UserInputValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Count() == 2)
                .WithMessage("User should give inputFileLocation and outputFileLocation");

            RuleFor(x => x)
               .Must(x => File.Exists(x.First()) == true)
               .WithMessage("UserInput file doesnot exist");

            RuleFor(x => x)
              .Must(x => Path.GetExtension(x.First()) == ".txt")
              .WithMessage("UserInput file is in invalidformat");
        }
    }

}
