using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    public List<Enemie> Enemies;
    public GameObject Lose;
    public GameObject Win;

    [SerializeField] private LevelConfig Config;

    public GameObject HPText;
    public GameObject WaveText;
    public GameObject EnemyHPText;

    private int currWave = 0;

    public Image CooldownImage;
    public GameObject InactiveImage;
    private float superAttackCooldownTime = 2.0f;
    private float superAttackCooldownTimer = 0.0f;

    public GameObject SmallGoblin;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HPText.GetComponent<Text>().text = "Player HP: " + Player.Hp;
        WaveText.GetComponent<Text>().text = "Total waves: " + Config.Waves.Length + "\nCurrent wave: " + (currWave + 1);
        InactiveImage.gameObject.SetActive(true);
        CooldownImage.fillAmount = 0.0f;
        SpawnWave();
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.D))
        {
            //Move the player in the given direction
            Player.gameObject.transform.position += Vector3.right * Player.Speed * Time.deltaTime;

            //Rotate the player to face the direction they're moving in
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            Player.gameObject.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Player.gameObject.transform.position += Vector3.left * Player.Speed * Time.deltaTime;

            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            Player.gameObject.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Player.gameObject.transform.position += Vector3.forward * Player.Speed * Time.deltaTime;

            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            Player.gameObject.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Player.gameObject.transform.position += Vector3.back * Player.Speed * Time.deltaTime;

            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            Player.gameObject.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }

        ShowPlayerHP();
        ShowEnemyHP();

        if (Player.hasEnemiesNearby)
            InactiveImage.gameObject.SetActive(false);
        else
            InactiveImage.gameObject.SetActive(true);

        if (superAttackCooldownTimer > 0f)
            Cooldown();
    }

    public void ShowPlayerHP()
    {
        HPText.GetComponent<Text>().text = "Player HP: " + Player.Hp;
    }

    public void AddPlayerHP(int hp)
    {
        Player.AddHP(hp);
        HPText.GetComponent<Text>().text = "Player HP: " + Player.Hp;
    }

    public void ShowEnemyHP()
    {
        EnemyHPText.GetComponent<Text>().text = "Enemy HP: ";
        for (int i = 0; i < Enemies.Count; i++)
        {
            EnemyHPText.GetComponent<Text>().text += "\n" + Enemies[i].Hp;
        }
    }

    public void AddEnemie(Enemie enemie)
    {
        Enemies.Add(enemie);
    }

    public void RemoveEnemie(Enemie enemie)
    {
        Enemies.Remove(enemie);
        if (enemie.gameObject.name.Contains("GoblinSplitter"))
        {
            Vector3 position1 = enemie.gameObject.transform.position + transform.right * 2;
            Vector3 position2 = enemie.gameObject.transform.position - transform.right * 2;
            Instantiate(SmallGoblin, position1, Quaternion.identity);
            Instantiate(SmallGoblin, position2, Quaternion.identity);
        }
        if (Enemies.Count == 0)
        {
            //SpawnWave();
            Invoke(nameof(SpawnWave), 0.25f);  //Delay the new wave in case new enemies will spawn
        }
    }

    public void GameOver()
    {
        Lose.SetActive(true);
    }

    private void SpawnWave()
    {
        if (currWave >= Config.Waves.Length && Enemies.Count == 0)
        {
            Win.SetActive(true);
            return;
        }
        else if (Enemies.Count > 0)
            return;

        WaveText.GetComponent<Text>().text = "Total waves: " + Config.Waves.Length + "\nCurrent wave: " + (currWave + 1);
        var wave = Config.Waves[currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(character, pos, Quaternion.identity);
        }
        currWave++;

    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    
    public void Attack()
    {
        Player.Attack();
    }

    public void SuperAttack()
    {
        //If user clicked the button while it's inactive
        if (InactiveImage.gameObject.activeSelf)
            return;
        //If user clicked the button while cooldown hasn't passed
        if (superAttackCooldownTimer > 0.0f)
            return;
        Player.SuperAttack();
        superAttackCooldownTimer = superAttackCooldownTime;
    }

    private void Cooldown()
    {
        superAttackCooldownTimer -= Time.deltaTime;
        if (superAttackCooldownTimer < 0.0f)
        {
            CooldownImage.fillAmount = 0.0f;
            superAttackCooldownTimer = 0f;
        }
        else
        {
            CooldownImage.fillAmount = superAttackCooldownTimer / superAttackCooldownTime;
        }
    }
}
