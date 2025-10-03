using GCG;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GCG
{
    public static class UserDataManager
    {
        public delegate void OnSavedValueChangedDelegate(string name, string value);
        public delegate void OnAmountChangedDelegate(string currency, long oldAmount, long newAmount);

        public static int CLEAR_VERSION = 5; // Increasing this will clear users data

        private static UserData userData;
        private const string prefKey = "s_userdata";
        private const string backupPrefKey = "s_userdata_backup";

        public static bool PausedCallbacks
        {
            get;
            set;
        }

        public static OnAmountChangedDelegate OnCurrencyChanged
        {
            get; set;
        }

        public static OnSavedValueChangedDelegate OnSavedValueChanged
        {
            get; set;
        }

        public static UserData UserData => userData;

        public static bool Initialized
        {
            get; private set;
        } = false;

        public static void Init()
        {
            if (Initialized)
                return;

            Load();

            foreach (KeyValuePair<string, string> entry in UserData.savedValues)
            {
                OnSavedValueChanged?.Invoke(entry.Key, entry.Value);
            }

            Initialized = true;
        }

        public static void Load()
        {
            string logText = "Loading userdata. ";
            string stringUserData = PlayerPrefs.GetString(prefKey, "");

            if (string.IsNullOrEmpty(stringUserData))
            {
                userData = new UserData();
                stringUserData = JsonConvert.SerializeObject(userData);
                PlayerPrefs.SetString(prefKey, stringUserData);
                PlayerPrefs.Save();
            } else
            {
                var savedUserData = JsonConvert.DeserializeObject<UserData>(stringUserData);

                if (savedUserData == null)
                {
                    logText += "\nSaved UserData couldnt be deserialized: " + stringUserData + ".";
                    string stringUserDataBackup = PlayerPrefs.GetString(backupPrefKey, "");
                    logText += "\nTrying to recover backup UserData: " + stringUserDataBackup + ".";

                    var savedBackupUserData = JsonConvert.DeserializeObject<UserData>(stringUserDataBackup);

                    if (savedBackupUserData == null)
                    {
                        logText += "\nFailed to recover backup UserData. Creating new UserData.";
                        savedUserData = new UserData();
                    } else
                    {
                        logText += "\nSuccessfully recovered backup UserData.";
                        savedUserData = savedBackupUserData;
                    }
                }

                userData = new UserData(savedUserData);
            }

            GCGUtil.Log(logText);
        }

        public static void Save()
        {
            if (userData == null)
                return;

            var prevData = PlayerPrefs.GetString(prefKey, "");

            if (!string.IsNullOrEmpty(prevData))
            {
                PlayerPrefs.SetString(backupPrefKey, prevData);
            }

            string stringUserData = JsonConvert.SerializeObject(userData);
            PlayerPrefs.SetString(prefKey, stringUserData);
            PlayerPrefs.Save();
            string logText = "Saved userdata. ";

#if UNITY_EDITOR
            logText += stringUserData;
#endif

            GCGUtil.Log(logText);
        }

        public static void ResetData()
        {
            PlayerPrefs.SetString(prefKey, "");
            PlayerPrefs.SetString(backupPrefKey, "");
            PlayerPrefs.Save();
            Load();
        }

        public static string UserID
        {
            get
            {
                return userData.id;
            }
        }

        public static bool SeenVersion()
        {
            return GetSavedValue("SeenVersion_" + Application.version, "0").ToBool();
        }

        public static bool FirstTimePlaying()
        {
            return GetSavedValue("FirstTimePlaying", "1").ToBool();
        }

        public static string GetSavedValue(string name, string defaultValue)
        {
            if (!UserData.savedValues.ContainsKey(name))
            {
                UserData.savedValues.Add(name, defaultValue);
            }

            return UserData.savedValues[name];
        }

        public static void SetSavedValue(string name, object value)
        {
            if (!UserData.savedValues.ContainsKey(name))
            {
                UserData.savedValues.Add(name, value.ToString());
            }

            UserData.savedValues[name] = value.ToString();

            if (!PausedCallbacks)
                OnSavedValueChanged?.Invoke(name, value.ToString());
        }

    }

    [Serializable]
    public class UserData
    {
        public string id;
        public Dictionary<string, string> savedValues;

        public long created;

        public string bindings;

        public UserData()
        {
            id = Guid.NewGuid().ToString();
            savedValues = new Dictionary<string, string>();
            bindings = string.Empty;
            created = DateTime.Now.ToUnixTimestamp();
        }

        public UserData(UserData userData)
        {
            if (userData == null)
            {
                string message = "UserData is null; ";

                if (userData != null)
                {
                    message += JsonConvert.SerializeObject(userData);
                }

                GCGUtil.LogError(message);

                userData = new UserData();
            }

            id = userData.id;
            savedValues = userData.savedValues;
            bindings = userData.bindings;
        }
    }
}

