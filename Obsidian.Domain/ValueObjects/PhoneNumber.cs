using PhoneNumbers;

namespace Obsidian.Domain.ValueObjects
{
    public sealed record PhoneNumber
    {
        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                    "Phone number is required.",
                    nameof(value));

            var phoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                var parsedNumber = phoneUtil.Parse(value, null);

                if (!phoneUtil.IsValidNumber(parsedNumber))
                    throw new ArgumentException(
                        "Phone number is invalid.",
                        nameof(value));

                var formattedNumber = phoneUtil.Format(
                    parsedNumber,
                    PhoneNumberFormat.E164);

                var normalizedNumber = formattedNumber.TrimStart('+');
                return new PhoneNumber(normalizedNumber);
            }
            catch (NumberParseException)
            {
                throw new ArgumentException(
                    "Phone number is invalid.",
                    nameof(value));
            }
        }

        public override string ToString() => Value;
    }
}
