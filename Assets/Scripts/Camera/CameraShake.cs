using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private CinemachineVirtualCamera cineVirtualCamera;
    private float ShakeTimer;
    
    
    private void Awake()
    {
        Instance = this;
        cineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    
    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinePerlinCamera = 
            cineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    
        cinePerlinCamera.m_AmplitudeGain = intensity;
        ShakeTimer = time;
    }
    
    private void Update()
    {
        if (ShakeTimer > 0)
        {
            ShakeTimer -= Time.deltaTime;
            if (ShakeTimer <= 0)
            {
                CinemachineBasicMultiChannelPerlin cinePerlinCamera =
                    cineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    
                cinePerlinCamera.m_AmplitudeGain = 0f;
            }
    
        }
    }
}
