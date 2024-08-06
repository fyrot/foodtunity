using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMudBlazorCACApp1.Components.Services
{
    public class NavigationInfoService
    {
        public string prevPage = "";
        public bool homePageDrawerOpen = false;
        public bool settingVolunteerInfoPageOpen = false;
        public bool enableAnimation = true;
        public bool listingsPageSavedOpen = true;
        public bool listingsPageUserOpen = true;
        // settings page random greeting fix
        Random randomNumGenerator = new Random();
        List<string> logInGreetings = new List<string>
        {
            "Hello, ", "Hey, ", "Salutations, ", "Greetings, "
        };
        public string settingsGreetingText = "";
        public void UpdateSettingsGreetingText()
        {
            settingsGreetingText = logInGreetings[randomNumGenerator.Next(logInGreetings.Count)];
        }
    }
}
