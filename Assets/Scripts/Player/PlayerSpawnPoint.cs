using UnityEngine;
using Unity.Cinemachine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private PlayerHealthUI healthUI;

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, transform.position, Quaternion.identity);

        cinemachineCamera.Target.TrackingTarget = player.transform;
        healthUI.SetPlayer(player.GetComponent<Player>());
    }
}