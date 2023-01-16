using DataAnnotationsExtensions.ClientValidation;
using RCDWeb.App_Start;

[assembly: WebActivator.PreApplicationStartMethod(typeof(RegisterClientValidationExtensions), "Start")]
 
namespace RCDWeb.App_Start {
    public static class RegisterClientValidationExtensions {
        public static void Start() {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}