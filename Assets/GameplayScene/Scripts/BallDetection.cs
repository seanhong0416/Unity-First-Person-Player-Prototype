using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDetection : MonoBehaviour
{
    public float wall_impact_force = 150f;
    int layer_of_wall;
    int layer_of_floor;
    Vector3 ball_reset_position;

    Rigidbody ball;
    GameObject scoreboard;
    // Start is called before the first frame update
    void Start()
    {
        layer_of_wall = LayerMask.NameToLayer("Wall");
        layer_of_floor = LayerMask.NameToLayer("Floor");
        
        ball = gameObject.GetComponent<Rigidbody>();
        ball.useGravity = false;
        ball.velocity = Vector3.zero;
        ball_reset_position = ball.position;

        scoreboard = GameObject.Find("Scoreboard");
    }

    // Update is called once per frame
    void Update()
    {
        if (!ball.useGravity && ball.velocity != Vector3.zero)
            ball.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ball hit wall, bounce back harder
        if (collision.gameObject.layer == layer_of_wall)
        {
            ball.AddForce(collision.contacts[0].normal * wall_impact_force);
            Debug.Log("wall hit");
        }
        //ball touch ground => reset ball and add score
        else if (collision.gameObject.layer == layer_of_floor)
        {
            ball.position = ball_reset_position;
            ball.useGravity = false;
            ball.velocity = Vector3.zero;
            //add score
            if (collision.gameObject.name == "TeamTakinaFloor")
            {
                //Scoring.ScoreOrangeCats.Value += 1;
                scoreboard.GetComponent<TeamScoreManagement>().team_chisato_score.Value += 1;
            }
            else
            {
                //Scoring.ScoreBoys.Value += 1;
                scoreboard.GetComponent<TeamScoreManagement>().team_takina_score.Value += 1;
            }
        }
    }
}
