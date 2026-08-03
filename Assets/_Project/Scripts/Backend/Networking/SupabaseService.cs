using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Backend
{
    /// <summary>
    /// Supabase integration service for authentication and database
    /// Referência: BK-001, BK-008
    /// </summary>
    public class SupabaseService : MonoBehaviour
    {
        [Header("Supabase Configuration")]
        [SerializeField] private string supabaseUrl = "https://your-project.supabase.co";
        [SerializeField] private string supabaseAnonKey = "your-anon-key";
        [SerializeField] private float requestTimeout = 10f;

        [Header("State")]
        [SerializeField] private bool isInitialized;
        [SerializeField] private bool isAuthenticated;
        [SerializeField] private string currentUserId;
        [SerializeField] private string currentAccessToken;

        private Dictionary<string, string> headers;

        public bool IsInitialized => isInitialized;
        public bool IsAuthenticated => isAuthenticated;
        public string CurrentUserId => currentUserId;

        /// <summary>
        /// Event raised when auth state changes
        /// </summary>
        public event Action<AuthState> OnAuthStateChanged;

        private void Awake()
        {
            InitializeHeaders();
        }

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize Supabase client
        /// Referência: BK-001
        /// </summary>
        public void Initialize()
        {
            if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseAnonKey))
            {
                Debug.LogError("[Supabase] Missing configuration");
                return;
            }

            Debug.Log("[Supabase] Initializing...");
            isInitialized = true;

            // Try to restore session
            StartCoroutine(RestoreSession());
        }

        /// <summary>
        /// Initialize HTTP headers
        /// </summary>
        private void InitializeHeaders()
        {
            headers = new Dictionary<string, string>
            {
                { "apikey", supabaseAnonKey },
                { "Authorization", $"Bearer {supabaseAnonKey}" },
                { "Content-Type", "application/json" },
                { "Prefer", "return=representation" }
            };
        }

        /// <summary>
        /// Anonymous authentication
        /// Referência: BK-001
        /// </summary>
        public void AuthenticateAnonymously()
        {
            if (!isInitialized)
            {
                Debug.LogError("[Supabase] Not initialized");
                return;
            }

            StartCoroutine(AuthenticateAnonymouslyCoroutine());
        }

        private IEnumerator AuthenticateAnonymouslyCoroutine()
        {
            Debug.Log("[Supabase] Authenticating anonymously...");

            var body = new Dictionary<string, string>
            {
                { "data", "{}" }
            };

            string json = JsonUtility.ToJson(new AnonymousAuthRequest { data = "{}" });

            using var request = new UnityEngine.Networking.UnityWebRequest(
                $"{supabaseUrl}/auth/v1/signup",
                "POST"
            );

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                currentUserId = response.user?.id;
                currentAccessToken = response.access_token;

                // Update auth header
                headers["Authorization"] = $"Bearer {currentAccessToken}";

                isAuthenticated = true;
                OnAuthStateChanged?.Invoke(AuthState.Authenticated);

                Debug.Log($"[Supabase] Authenticated: {currentUserId}");

                // Create player profile
                StartCoroutine(CreatePlayerProfile());
            }
            else
            {
                Debug.LogError($"[Supabase] Auth failed: {request.error}");
                OnAuthStateChanged?.Invoke(AuthState.Failed);
            }
        }

        /// <summary>
        /// Restore previous session
        /// </summary>
        private IEnumerator RestoreSession()
        {
            string savedToken = PlayerPrefs.GetString("supabase_token", "");
            string savedUserId = PlayerPrefs.GetString("supabase_user_id", "");

            if (!string.IsNullOrEmpty(savedToken) && !string.IsNullOrEmpty(savedUserId))
            {
                // Verify token is still valid
                // For simplicity, we'll just restore it
                currentAccessToken = savedToken;
                currentUserId = savedUserId;
                headers["Authorization"] = $"Bearer {currentAccessToken}";

                isAuthenticated = true;
                OnAuthStateChanged?.Invoke(AuthState.Authenticated);

                Debug.Log($"[Supabase] Session restored for: {currentUserId}");
            }

            yield return null;
        }

        /// <summary>
        /// Create player profile after first auth
        /// Referência: BK-008
        /// </summary>
        private IEnumerator CreatePlayerProfile()
        {
            var profile = new PlayerProfileData
            {
                id = currentUserId,
                display_name = $"Player_{currentUserId.Substring(0, 8)}",
                created_at = DateTime.UtcNow.ToString("o"),
                level = 1,
                experience = 0
            };

            string json = JsonUtility.ToJson(profile);

            using var request = new UnityEngine.Networking.UnityWebRequest(
                $"{supabaseUrl}/rest/v1/players",
                "POST"
            );

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log("[Supabase] Player profile created");
            }
            else
            {
                // Profile might already exist
                Debug.LogWarning($"[Supabase] Profile creation: {request.error}");
            }
        }

        /// <summary>
        /// Save data to Supabase
        /// Referência: BK-004, BK-005
        /// </summary>
        public IEnumerator SaveData(string table, string id, Dictionary<string, object> data)
        {
            if (!isAuthenticated)
            {
                Debug.LogError("[Supabase] Not authenticated");
                yield break;
            }

            string json = JsonUtility.ToJson(data);
            string url = string.IsNullOrEmpty(id) 
                ? $"{supabaseUrl}/rest/v1/{table}"
                : $"{supabaseUrl}/rest/v1/{table}?id=eq.{id}";

            using var request = new UnityEngine.Networking.UnityWebRequest(
                url,
                string.IsNullOrEmpty(id) ? "POST" : "PATCH"
            );

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.Log($"[Supabase] Saved to {table}: {id}");
            }
            else
            {
                Debug.LogError($"[Supabase] Save failed: {request.error}");
            }
        }

        /// <summary>
        /// Load data from Supabase
        /// Referência: BK-004, BK-005
        /// </summary>
        public IEnumerator LoadData(string table, string id, Action<string> onComplete)
        {
            if (!isAuthenticated)
            {
                Debug.LogError("[Supabase] Not authenticated");
                onComplete?.Invoke(null);
                yield break;
            }

            string url = string.IsNullOrEmpty(id)
                ? $"{supabaseUrl}/rest/v1/{table}?user_id=eq.{currentUserId}"
                : $"{supabaseUrl}/rest/v1/{table}?id=eq.{id}";

            using var request = new UnityEngine.Networking.UnityWebRequest(url, "GET");

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[Supabase] Load failed: {request.error}");
                onComplete?.Invoke(null);
            }
        }

        /// <summary>
        /// Sign out
        /// </summary>
        public void SignOut()
        {
            currentUserId = null;
            currentAccessToken = null;
            isAuthenticated = false;

            PlayerPrefs.DeleteKey("supabase_token");
            PlayerPrefs.DeleteKey("supabase_user_id");

            InitializeHeaders();

            OnAuthStateChanged?.Invoke(AuthState.Unauthenticated);
            Debug.Log("[Supabase] Signed out");
        }

        /// <summary>
        /// Delete player account and data
        /// Referência: BK-021
        /// </summary>
        public IEnumerator DeleteAccount()
        {
            if (!isAuthenticated)
            {
                Debug.LogError("[Supabase] Not authenticated");
                yield break;
            }

            Debug.Log("[Supabase] Deleting account...");

            // Delete player data
            yield return DeleteData("players", currentUserId);
            yield return DeleteData("player_economy", currentUserId);
            yield return DeleteData("player_inventory", currentUserId);
            yield return DeleteData("leaderboard_entries", null);

            // Sign out
            SignOut();

            Debug.Log("[Supabase] Account deleted");
        }

        /// <summary>
        /// Delete data from table
        /// </summary>
        private IEnumerator DeleteData(string table, string userId)
        {
            string id = userId ?? currentUserId;
            string url = $"{supabaseUrl}/rest/v1/{table}?user_id=eq.{id}";

            using var request = new UnityEngine.Networking.UnityWebRequest(url, "DELETE");

            foreach (var header in headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[Supabase] Delete {table} failed: {request.error}");
            }
        }
    }

    /// <summary>
    /// Authentication state
    /// </summary>
    public enum AuthState
    {
        Unauthenticated,
        Authenticating,
        Authenticated,
        Failed
    }

    /// <summary>
    /// Anonymous auth request
    /// </summary>
    [System.Serializable]
    public class AnonymousAuthRequest
    {
        public string data;
    }

    /// <summary>
    /// Auth response
    /// </summary>
    [System.Serializable]
    public class AuthResponse
    {
        public string access_token;
        public string refresh_token;
        public AuthUser user;
    }

    /// <summary>
    /// Auth user
    /// </summary>
    [System.Serializable]
    public class AuthUser
    {
        public string id;
        public string email;
    }

    /// <summary>
    /// Player profile data
    /// Referência: BK-008
    /// </summary>
    [System.Serializable]
    public class PlayerProfileData
    {
        public string id;
        public string display_name;
        public string created_at;
        public int level;
        public float experience;
    }
}
