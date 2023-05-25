using MultiplayerServer.Utilities;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerConfig", menuName = "ScriptableObjects/ServerConfig")]
public class ServerConfigurationScriptable : ScriptableObject
{
    #region Singleton
    private static ServerConfigurationScriptable _instance;
    public static ServerConfigurationScriptable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ServerConfigurationScriptable>("ServerConfig");
            }
            return _instance;
        }
    }
    #endregion

    [SerializeField] ServerConfig.Environment environment;
    public EnvironmentConfig LoadSelectedConfig()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("ServerConfig");
        if (jsonFile != null)
        {
            ServerConfig serverConfig = JsonConvert.DeserializeObject<ServerConfig>(jsonFile.text);
            var environmentConfig = ServerConfig.GetEnvironmentConfig(serverConfig, environment);
            return environmentConfig;
        }
        else
        {
            Debug.LogError("ServerConfig.json file not found in Resources folder.");
            return null;
        }
    }
}
