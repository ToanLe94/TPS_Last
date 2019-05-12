using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController GC;

    private UserInput player { get { return FindObjectOfType<UserInput>(); } set { player = value; } }

    private PlayerUI playerUI { get { return FindObjectOfType<PlayerUI>(); } set { playerUI = value; } }

    private WeaponHandler wp { get { return player.GetComponent<WeaponHandler>(); } set { wp = value; } }

    private CharacterStats charStats { get { return player.GetComponent<CharacterStats>(); } set { charStats = value; } }


    //[SerializeField] string version = "v0.0.1";
    //[SerializeField] string roomName = "zombie-fps-test";
    //[SerializeField] string playerName = "Player";
    ////[SerializeField] List<GameObject> players = new List<GameObject>();
    //public GameObject playerPrefab;
    //public Transform spawnPoint;
    //public GameObject enemySpawner;
    //public GameObject lobbyCam;
    //public GameObject CameraRig;

    //public GameObject lobbyUI;
    //public GameObject inGameUI;
    //public Text statusText;

    ////public List<GameObject> Players
    ////{
    ////    get
    ////    {
    ////        return players;
    ////    }
    ////}

    //// void Start() {
    //// 	PhotonNetwork.autoJoinLobby = true;
    //// 	PhotonNetwork.ConnectUsingSettings(version);
    //// }

    //// public virtual void OnJoinedLobby() {
    //// 	RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 4 };
    //// 	PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    //// }

    //// public virtual void OnJoinedRoom() {
    //void Start()
    //{
    //    lobbyCam.SetActive(false);
    //    lobbyUI.SetActive(false);
    //    CameraRig.SetActive(true);
    //    GameObject playerObj = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    //    // GameObject player = PhotonNetwork.Instantiate(playerName, spawnPoint.position, spawnPoint.rotation, 0);
    //    // players.Add(player);

    //    inGameUI.SetActive(true);
    //    enemySpawner.SetActive(true);
    //    enemySpawner.GetComponent<EnemySpawner>().target = playerObj;
    //}

    //// void Update() {
    //// 	statusText.text = PhotonNetwork.connectionStateDetailed.ToString();
    //// }
    void Awake()
    {
        if (GC == null)
        {
            GC = this;
        }
        else
        {
            if (GC != this)
            {
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {

        //Debug.Log("Vao update UIPLayer local player");
        if (playerUI)
        {
            if (wp)
            {
                if (playerUI.ammoText)
                {
                    if (wp.currentWeapon == null)
                    {
                        playerUI.ammoText.text = "Unarmed.";
                    }
                    else
                    {
                        playerUI.ammoText.text = wp.currentWeapon.ammo.clipAmmo + "//" + wp.currentWeapon.ammo.carryingAmmo;
                    }
                }
            }
            if (playerUI.healthBar && playerUI.healthText)
            {
                //Debug.Log("healthbar value :" + playerUI.healthBar.value + "health cua player: " + charStats.health);
                playerUI.healthBar.value = charStats.health;

                playerUI.healthText.text = Mathf.Round(playerUI.healthBar.value).ToString();
            }
        }

    }
}
