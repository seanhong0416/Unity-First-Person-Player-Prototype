using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TeamScoreManagement : NetworkBehaviour
{
    public NetworkVariable<int> team_chisato_score = new NetworkVariable<int>(0);
    [SerializeField] TMP_Text teamChisatoScoreboard;
    public NetworkVariable<int> team_takina_score = new NetworkVariable<int>(0);
    [SerializeField] TMP_Text teamTakinaScoreboard;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //initialize text
        teamChisatoScoreboard.SetText(team_chisato_score.Value.ToString());

        team_chisato_score.OnValueChanged += (int previous_value, int new_value) =>
        {
            teamChisatoScoreboard.SetText(team_chisato_score.Value.ToString());
        };

        teamTakinaScoreboard.SetText(team_takina_score.Value.ToString());

        team_takina_score.OnValueChanged += (int previous_value, int new_value) =>
        {
            teamTakinaScoreboard.SetText(team_takina_score.Value.ToString());
        };
    }
}
