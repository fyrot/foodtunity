using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace BlazorMudBlazorCACApp1.Components.Services
{
    public class FirebaseAuthService
    {
        public FirebaseAuthConfig config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCc3GwW5cePpeAf_rl3a81OIxTCFYcDRJg ",
            AuthDomain = "foodconnectcac.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new GoogleProvider().AddScopes("email"),
                new EmailProvider()
                // ...
            }
        };
        
        public FirebaseAuthClient client;
        public FirestoreDb firestore = FirestoreDb.Create("foodconnectcac");
        public Firebase.Auth.UserCredential userCredentialStored;
        public FirebaseAuthService() { client = new FirebaseAuthClient(config); }
        
        
        // foodconnectcac-firebase-adminsdk-36xr9-2cc9e19e63

        // Add methods for authentication (e.g., SignIn, SignOut, etc.)
    }
}
