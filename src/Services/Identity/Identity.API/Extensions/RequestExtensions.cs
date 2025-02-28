namespace Identity.API.Extensions
{
    public static class RequestExtensions
    {
        public static bool TryValidate<T>(this T model, out List<string> errors) where T : notnull
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            errors = validationResults.Select(static vr => vr.ErrorMessage ?? string.Empty).ToList();
            return isValid;
        }
    }
}
