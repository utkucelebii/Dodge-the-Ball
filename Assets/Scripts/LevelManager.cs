using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Game Settings")]
    [SerializeField] private int playerCount = 10;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    public float moveSpeed;
    public bool hardness;
    public List<GameObject> gamers = new List<GameObject>();
    public int sizeX = 50;
    public int sizeY = 50;

    [Space(5f)]
    [Header("Menu Settings")]
    [SerializeField] private Animator fadeAnim;
    [SerializeField] private Image fade;

    public CameraController cinemachineController;

    private void Start()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int y = sizeY / 10; y < sizeY / 2; y+=y)
        {
            for (int x = sizeX / 10; x < sizeX / 2; x+=x)
            {
                positions.AddRange(new List<Vector3>{
                    new Vector3(x, 1, y),
                    new Vector3(x, 1, -y),
                    new Vector3(-x, 1, y),
                    new Vector3(-x, 1, -y)
                });
            }
        }
        GameObject p = Instantiate(player);
        p.transform.parent = transform;
        gamers.Add(p);

        cinemachineController.player = p.transform;

        for (int i = 0; i < playerCount - 1; i++)
        {
            Vector3 pos = positions[Random.Range(0, positions.Count)];
            positions.Remove(pos);

            GameObject e = Instantiate(enemy);
            e.transform.position = pos;
            e.transform.parent = transform;
            gamers.Add(e);
        }
    }

    public void Respawn()
    {
        StartCoroutine("ReloadScene");
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(3f);
        fadeAnim.SetBool("Fade", true);
        yield return new WaitUntil(() => fade.color.a == 1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
