using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] float turnDuration = 5f;
    [SerializeField] PlayerMovement player1;
    [SerializeField] PlayerMovement player2;
    private float turnNum;
    [SerializeField] GameObject Platform1;
    [SerializeField] GameObject Platform2;
    [SerializeField] GameObject Platform3;

    float timer;
    PlayerMovement active;

    void Start()
    {
        turnNum = 1;
        SetActivePlayer(player1);
        timer = turnDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SwapTurn();
            timer = turnDuration;
        }

    }

    void SwapTurn()
    {
        
        SetActivePlayer(active == player1 ? player2 : player1);

        var shooter = active.GetComponent<Shooter2D>();
        if (shooter) shooter.ResetForTurn();
        turnNum++;
        if (turnNum == 1)
        {
            Platform1.SetActive(true);
            Platform2.SetActive(false);
            Platform3.SetActive(false);
        }
        if (turnNum == 2)
        {
            Platform1.SetActive(false);
            Platform2.SetActive(true);
            Platform3.SetActive(false);
        }
        if( turnNum == 3)
        {
            Platform1.SetActive(false);
            Platform2.SetActive(false);
            Platform3.SetActive(true);
            turnNum = 0;
        }

    }

    void SetActivePlayer(PlayerMovement next)
    {
        active = next;

        player1.SetActiveState(active == player1);
        player2.SetActiveState(active == player2);

        var a1 = player1.GetComponentInChildren<AimArrowController>(true);
        var a2 = player2.GetComponentInChildren<AimArrowController>(true);
        if (a1) a1.SetAimEnabled(active == player1);
        if (a2) a2.SetAimEnabled(active == player2);


        var s1 = player1.GetComponent<Shooter2D>();
        var s2 = player2.GetComponent<Shooter2D>();
        if (s1) s1.enabled = (active == player1);
        if (s2) s2.enabled = (active == player2);


        var shooter = active.GetComponent<Shooter2D>();
        if (shooter) shooter.ResetForTurn();


        var aa = active.GetComponentInChildren<AimArrowController>(true);
        if (aa) aa.ResetAngle(0f);
    }
}
