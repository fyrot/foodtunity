using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMudBlazorCACApp1.Components.Services
{
    public class ListingDataStorageService
    {
        public List<ExtendedListingData> extendedListingDataSource = new List<ExtendedListingData>();
        public Dictionary<string, ExtendedListingData> savedExtendedListingData = new Dictionary<string, ExtendedListingData>(); 
        
        public List<UserCreatedListingData> userCreatedListingData = new List<UserCreatedListingData>();
        public List<ExtendedListingData> extendedListingData = new List<ExtendedListingData>();
        public Dictionary<string, string> userVolunteerInfo = new Dictionary<string, string> {
            {"name", "" },
            {"age", "0" }
        };
        public bool homeListingContentLoaded = true;
        public ExtendedListingData presentedExtendedListing;
        string userCreatedListingDataPath = Path.Combine(FileSystem.Current.AppDataDirectory, "userCreatedListingData.txt");
        string savedExtendedListingDataPath = Path.Combine(FileSystem.Current.AppDataDirectory, "savedExtendedListings.txt");
        string userVolunteerInfoPath = Path.Combine(FileSystem.Current.AppDataDirectory, "userVolunteerInfo.txt");
        public ListingDataStorageService() {
            
            if (File.Exists(userCreatedListingDataPath))
            {
                userCreatedListingData = JsonSerializer.Deserialize<List<UserCreatedListingData>>(File.ReadAllText(userCreatedListingDataPath));
            }
            if (File.Exists(savedExtendedListingDataPath))
            {
                savedExtendedListingData = JsonSerializer.Deserialize<Dictionary<string, ExtendedListingData>>(File.ReadAllText(savedExtendedListingDataPath));
                List<string> keysToRemove = new List<string>();
                foreach (string k in savedExtendedListingData.Keys)
                {
                    if (DateTimeOffset.Now.ToUnixTimeSeconds() >= savedExtendedListingData[k].endDateTimestamp + savedExtendedListingData[k].endTimeTimestamp)
                    {
                        keysToRemove.Add(k);
                    }
                }
                foreach (string r in  keysToRemove)
                {
                    savedExtendedListingData.Remove(r);
                }
                //File.WriteAllText(savedExtendedListingDataPath, JsonSerializer.Serialize(savedExtendedListingDataPath)); what was this???
            }
            if (File.Exists(userVolunteerInfoPath))
            {
                string[] userVolunteerInfoData = File.ReadAllLines(userVolunteerInfoPath);
                userVolunteerInfo["name"] = userVolunteerInfoData[0];
                userVolunteerInfo["age"] = userVolunteerInfoData[1];
            }
        }
        public void AddToUserCreatedList(UserCreatedListingData toAdd)
        {
            userCreatedListingData.Add(toAdd);
        }
        public void SaveUserCreatedList()
        {
            File.WriteAllText(userCreatedListingDataPath, JsonSerializer.Serialize(userCreatedListingData));
        }
        public void SaveUserVolunteerInfo(string userName, int userAge)
        {
            userVolunteerInfo["name"] = userName;
            userVolunteerInfo["age"] = userAge.ToString();
            string[] userInfo = { userVolunteerInfo["name"], userVolunteerInfo["age"] };
            File.WriteAllLines(userVolunteerInfoPath, userInfo);
        }
        public void SaveSavedExtendedListingList()
        {
            File.WriteAllText(savedExtendedListingDataPath, JsonSerializer.Serialize(savedExtendedListingData));
        }
        public void WipeOfflineListingsOnSignOut()
        {
            File.Delete(savedExtendedListingDataPath);
            File.Delete(userCreatedListingDataPath);
        }
        public async void UpdateExtendedListingData(QuerySnapshot data)
        {
            List<ExtendedListingData> extendedListingDataUpdate = new List<ExtendedListingData>();
            foreach (var d in data.Documents)
            {
                if (d.Id == "0") { continue;  }
                var objectData = d.ToDictionary();
                Dictionary<string, object> nestedTagInformation = (Dictionary<string, object>)objectData["tags"];
                Dictionary<string, string> castedTagInformation = new Dictionary<string, string>();
                foreach (string k in nestedTagInformation.Keys)
                {
                    castedTagInformation.Add(k, (string)nestedTagInformation[k]);
                }
                extendedListingDataUpdate.Add(new ExtendedListingData
                {
                    accountId = (string)objectData["accountId"],
                    accountType = (string)objectData["accountType"],
                    accountVerified = (bool)objectData["accountVerified"],
                    address = (string)objectData["address"],
                    addressLat = (double)objectData["addressLat"],
                    addressLon = (double)objectData["addressLon"],
                    description = (string)objectData["description"],
                    endDateTimestamp = (long)objectData["endDateTimestamp"],
                    endTimeTimestamp = (long)objectData["endTimeTimestamp"],
                    imageUrl = (string)objectData["imageUrl"],
                    listingId = (string)objectData["listingId"], // same as d.Id
                    profileUrl = (string)objectData["profileUrl"],
                    startDateTimestamp = (long)objectData["startDateTimestamp"],
                    startTimeTimestamp = (long)objectData["startTimeTimestamp"],
                    tags = castedTagInformation,
                    accountName = (string)objectData["accountName"],
                    requestsSignUpInfo = (bool)objectData["requestsSignUpInfo"],
                    carouselDisplay = d.GetValue<string[]>("carouselDisplay"),
                    minimumAge = d.GetValue<int>("minimumAge"),
                    signedUpUsers = new Dictionary<string, Dictionary<string, string>>()
                });
            }
            extendedListingData = extendedListingDataUpdate;
            extendedListingDataSource = extendedListingDataUpdate;
        }
        public void ExtendedListingDataFilterByTag(string tag)
        {
            if (tag == null || tag == "") { return; }
            extendedListingData = (from listing in extendedListingDataSource
                       where (listing.tags.ContainsKey(tag) == true)
                        select listing).ToList();
            
        }
        /*private string[] ConvertListObjectToListString(object listToConvert)
        {
            return ((System.Collections.IEnumerable)listToConvert).Cast<object>.Select(i => i.ToString()).ToArray();
            
        }*/
        public string[] ReturnIdsOfSavedListings()
        {
            return (from listing in savedExtendedListingData.Keys.ToArray() select listing).ToList().ToArray();
        }
        public string[] ReturnIdsOfUserListings()
        {

            return (from listing in userCreatedListingData select listing.listingId).ToList().ToArray();
        }
        public void ExtendedListingDataFilterByDistance(double latitude, double longitude, double radius)
        {
            if (radius == 101 || latitude == -360.0 || longitude == -360.0) { return; }
            extendedListingData = (from listing in extendedListingData where (ExtendedListingDataFilterByDistanceMileConverter(latitude, longitude, listing.addressLat, listing.addressLon) <= radius) select listing).ToList();
        }
        // emerald splash's 20 meter radius
        public double ExtendedListingDataFilterByDistanceMileConverter(double lat1, double lon1, double lat2, double lon2)
        {
            double latDiff = Math.Abs(lat2 - lat1);
            double lonDiff = Math.Abs(lon2 - lon1);
            double LONGITUDE_TO_MILES = 55.11761;
            double LATITUDE_TO_MILES = 68.93939;
            return Math.Sqrt((Math.Pow(latDiff * LATITUDE_TO_MILES, 2)) + (Math.Pow(lonDiff * LONGITUDE_TO_MILES, 2)));
        }
        public class UserCreatedListingData
        {
            public string address { get; set; }
            public double addressLat { get; set; }
            public double addressLon { get; set; }
            public string accountId { get; set; }
            public string listingId { get; set; }
            public long startDateTimestamp { get; set; }
            public long startTimeTimestamp { get; set; }
            public long endDateTimestamp { get; set; }
            public long endTimeTimestamp { get; set; }
            public string imageUrl { get; set; }
            public string description { get; set; }
            public bool requestsSignUpInfo { get; set; }
            public string[] carouselDisplay { get; set; }
            public int minimumAge { get; set; }
            public Dictionary<string, string> tags { get; set; }
        }
        public class ExtendedListingData
        {
            public string address { get; set; }
            public double addressLat { get; set; }
            public double addressLon { get; set; }
            public string accountId { get; set; }
            public bool accountVerified { get; set; }
            
            public string accountType { get; set; }
            public string profileUrl { get; set; }

            public string listingId { get; set; }
            public long startDateTimestamp { get; set; }
            public long startTimeTimestamp { get; set; }
            public long endDateTimestamp { get; set; }
            public long endTimeTimestamp { get; set; }
            public string imageUrl { get; set; }
            public string description { get; set; }
            public bool requestsSignUpInfo { get; set; }
            public Dictionary<string, Dictionary<string, string>> signedUpUsers { get; set; } = new Dictionary<string, Dictionary<string, string>>();
            public Dictionary<string, string> tags { get; set; }
            public string[] carouselDisplay { get; set; }
            public int minimumAge { get; set;  }
            public string accountName { get; set; }
        }
    }
}
