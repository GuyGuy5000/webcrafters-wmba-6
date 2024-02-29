using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace wmbaApp.ViewModels
{
    public class ValidationView
    {
        public bool IsValid { get; set; }

        public ValidationView(bool isValid)
        {
            this.IsValid = isValid;
        }
    }
}
